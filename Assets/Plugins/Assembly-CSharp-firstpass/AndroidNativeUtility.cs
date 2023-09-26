using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ANMiniJSON;
using SA.Common.Models;
using SA.Common.Pattern;
using UnityEngine;

public class AndroidNativeUtility : Singleton<AndroidNativeUtility>
{
	private string _redirectUrl = string.Empty;

	private string _clientId = string.Empty;

	private string _clientSecret = string.Empty;

	public static int SDKLevel
	{
		get
		{
			IntPtr clazz = AndroidJNI.FindClass("android.os.Build$VERSION");
			IntPtr staticFieldID = AndroidJNI.GetStaticFieldID(clazz, "SDK_INT", "I");
			return AndroidJNI.GetStaticIntField(clazz, staticFieldID);
		}
	}

	public static event Action<AN_PackageCheckResult> OnPackageCheckResult;

	public static event Action<string> OnAndroidIdLoaded;

	public static event Action<string> InternalStoragePathLoaded;

	public static event Action<string> ExternalStoragePathLoaded;

	public static event Action<AN_Locale> LocaleInfoLoaded;

	public static event Action<string[]> ActionDevicePackagesListLoaded;

	public static event Action<AN_NetworkInfo> ActionNetworkInfoLoaded;

	public static event Action<AN_RefreshTokenResult> OnOAuthRefreshTokenLoaded;

	public static event Action<AN_AccessTokenResult> OnOAuthAccessTokenLoaded;

	public static event Action<AN_DeviceCodeResult> OnDeviceCodeLoaded;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void GenerateRefreshToken(string redirectUrl, string clientId, string clientSecret)
	{
		_redirectUrl = redirectUrl;
		_clientId = clientId;
		_clientSecret = clientSecret;
		AndroidNative.GenerateRefreshToken(_redirectUrl, _clientId);
	}

	public void RefreshOAuthToken(string refreshToken, string clientId, string clientSecret)
	{
		StartCoroutine(RefreshOAuthTokenRequest(clientId, clientSecret, refreshToken));
	}

	public void ObtainUserDeviceCode(string clientId)
	{
		StartCoroutine(ObtainUserDeviceCodeRequest(clientId));
	}

	public void CheckIsPackageInstalled(string packageName)
	{
		AndroidNative.isPackageInstalled(packageName);
	}

	public void StartApplication(string bundle)
	{
		AndroidNative.runPackage(bundle);
	}

	public void StartApplication(string packageName, Dictionary<string, string> extras)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<string, string> extra in extras)
		{
			stringBuilder.AppendFormat("{0}{1}{2}", extra.Key, "|", extra.Value);
			stringBuilder.Append("`");
		}
		stringBuilder.Append("endofline");
		Debug.Log("[StartApplication] with Extras " + stringBuilder.ToString());
		AndroidNative.LaunchApplication(packageName, stringBuilder.ToString());
	}

	public void LoadAndroidId()
	{
		AndroidNative.LoadAndroidId();
	}

	public void GetInternalStoragePath()
	{
		AndroidNative.GetInternalStoragePath();
	}

	public void GetExternalStoragePath()
	{
		AndroidNative.GetExternalStoragePath();
	}

	public string GetExternalStoragePublicDirectory(AN_ExternalStorageType type)
	{
		return AndroidNative.GetExternalStoragePublicDirectory(type.ToString());
	}

	public void LoadLocaleInfo()
	{
		AndroidNative.LoadLocaleInfo();
	}

	public void LoadPackagesList()
	{
		AndroidNative.LoadPackagesList();
	}

	public void LoadNetworkInfo()
	{
		AndroidNative.LoadNetworkInfo();
	}

	public static void OpenSettingsPage(string action)
	{
		AndroidNative.OpenSettingsPage(action);
	}

	public static void ShowPreloader(string title, string message)
	{
		AN_PoupsProxy.ShowPreloader(title, message, AndroidNativeSettings.Instance.DialogTheme);
	}

	public static void ShowPreloader(string title, string message, AndroidDialogTheme theme)
	{
		AN_PoupsProxy.ShowPreloader(title, message, AndroidNativeSettings.Instance.DialogTheme);
	}

	public static void HidePreloader()
	{
		AN_PoupsProxy.HidePreloader();
	}

	public static void OpenAppRatingPage(string url)
	{
		AN_PoupsProxy.OpenAppRatePage(url);
	}

	public static void RedirectToGooglePlayRatingPage(string url)
	{
		OpenAppRatingPage(url);
	}

	public static void HideCurrentPopup()
	{
		AN_PoupsProxy.HideCurrentPopup();
	}

	public static void InvitePlusFriends()
	{
		AndroidNative.InvitePlusFriends();
	}

	private void RefreshTokenCodeReceived(string data)
	{
		Debug.Log(data);
		string[] array = data.Split(new string[1] { "|" }, StringSplitOptions.None);
		int num = int.Parse(array[0]);
		if (num == 1)
		{
			StartCoroutine(GenerateRefreshTokenRequest(array[1], _clientId, _clientSecret, _redirectUrl));
			return;
		}
		AN_RefreshTokenResult obj = new AN_RefreshTokenResult("Request Authorization Code error");
		AndroidNativeUtility.OnOAuthRefreshTokenLoaded(obj);
	}

	private IEnumerator GenerateRefreshTokenRequest(string code, string clientId, string clientSecret, string redirectUrl)
	{
		WWWForm requestForm = new WWWForm();
		requestForm.AddField("grant_type", "authorization_code");
		requestForm.AddField("code", code);
		requestForm.AddField("client_id", clientId);
		requestForm.AddField("client_secret", clientSecret);
		requestForm.AddField("redirect_uri", redirectUrl);
		WWW response = new WWW("https://accounts.google.com/o/oauth2/token", requestForm);
		yield return response;
		if (string.IsNullOrEmpty(response.error))
		{
			Dictionary<string, object> dictionary = Json.Deserialize(response.text) as Dictionary<string, object>;
			string accessToken = ((!dictionary.ContainsKey("access_token")) ? string.Empty : dictionary["access_token"].ToString());
			string refreshToken = ((!dictionary.ContainsKey("refresh_token")) ? string.Empty : dictionary["refresh_token"].ToString());
			string tokenType = ((!dictionary.ContainsKey("token_type")) ? string.Empty : dictionary["token_type"].ToString());
			long expiresIn = ((!dictionary.ContainsKey("expires_in")) ? 0 : ((long)dictionary["expires_in"]));
			AN_RefreshTokenResult obj = new AN_RefreshTokenResult(accessToken, refreshToken, tokenType, expiresIn);
			AndroidNativeUtility.OnOAuthRefreshTokenLoaded(obj);
		}
		else
		{
			AN_RefreshTokenResult obj2 = new AN_RefreshTokenResult(response.error);
			AndroidNativeUtility.OnOAuthRefreshTokenLoaded(obj2);
		}
	}

	private IEnumerator RefreshOAuthTokenRequest(string clientId, string clientSecret, string refreshToken)
	{
		WWWForm requestForm = new WWWForm();
		requestForm.AddField("grant_type", "refresh_token");
		requestForm.AddField("client_id", clientId);
		requestForm.AddField("client_secret", clientSecret);
		requestForm.AddField("refresh_token", refreshToken);
		WWW response = new WWW("https://accounts.google.com/o/oauth2/token", requestForm);
		yield return response;
		if (string.IsNullOrEmpty(response.error))
		{
			Dictionary<string, object> dictionary = Json.Deserialize(response.text) as Dictionary<string, object>;
			string accessToken = ((!dictionary.ContainsKey("access_token")) ? string.Empty : dictionary["access_token"].ToString());
			string tokenType = ((!dictionary.ContainsKey("token_type")) ? string.Empty : dictionary["token_type"].ToString());
			long expiresIn = ((!dictionary.ContainsKey("expires_in")) ? 0 : ((long)dictionary["expires_in"]));
			AN_AccessTokenResult obj = new AN_AccessTokenResult(accessToken, tokenType, expiresIn);
			AndroidNativeUtility.OnOAuthAccessTokenLoaded(obj);
		}
		else
		{
			AN_AccessTokenResult obj2 = new AN_AccessTokenResult(response.error);
			AndroidNativeUtility.OnOAuthAccessTokenLoaded(obj2);
		}
	}

	private IEnumerator ObtainUserDeviceCodeRequest(string clientId)
	{
		WWWForm requestForm = new WWWForm();
		requestForm.AddField("client_id", clientId);
		requestForm.AddField("scope", "email profile");
		WWW response = new WWW("https://accounts.google.com/o/oauth2/device/code", requestForm);
		yield return response;
		Debug.Log(response.text);
		if (string.IsNullOrEmpty(response.error))
		{
			Dictionary<string, object> dictionary = Json.Deserialize(response.text) as Dictionary<string, object>;
			string deviceCode = ((!dictionary.ContainsKey("device_code")) ? string.Empty : dictionary["device_code"].ToString());
			string userCode = ((!dictionary.ContainsKey("user_code")) ? string.Empty : dictionary["user_code"].ToString());
			string verificationUrl = ((!dictionary.ContainsKey("verification_url")) ? string.Empty : dictionary["verification_url"].ToString());
			long expiresIn = ((!dictionary.ContainsKey("expires_in")) ? 0 : ((long)dictionary["expires_in"]));
			long interval = ((!dictionary.ContainsKey("interval")) ? 0 : ((long)dictionary["interval"]));
			AN_DeviceCodeResult obj = new AN_DeviceCodeResult(deviceCode, userCode, verificationUrl, expiresIn, interval);
			AndroidNativeUtility.OnDeviceCodeLoaded(obj);
		}
		else
		{
			AN_DeviceCodeResult obj2 = new AN_DeviceCodeResult(response.error);
			AndroidNativeUtility.OnDeviceCodeLoaded(obj2);
		}
	}

	private void OnAndroidIdLoadedEvent(string id)
	{
		AndroidNativeUtility.OnAndroidIdLoaded(id);
	}

	private void OnPacakgeFound(string packageName)
	{
		AN_PackageCheckResult obj = new AN_PackageCheckResult(packageName);
		AndroidNativeUtility.OnPackageCheckResult(obj);
	}

	private void OnPacakgeNotFound(string packageName)
	{
		AN_PackageCheckResult obj = new AN_PackageCheckResult(packageName, new Error(0, "Pacakge not Found"));
		AndroidNativeUtility.OnPackageCheckResult(obj);
	}

	private void OnExternalStoragePathLoaded(string path)
	{
		AndroidNativeUtility.ExternalStoragePathLoaded(path);
	}

	private void OnInternalStoragePathLoaded(string path)
	{
		AndroidNativeUtility.InternalStoragePathLoaded(path);
	}

	private void OnLocaleInfoLoaded(string data)
	{
		string[] array = data.Split("|"[0]);
		AN_Locale aN_Locale = new AN_Locale();
		aN_Locale.CountryCode = array[0];
		aN_Locale.DisplayCountry = array[1];
		aN_Locale.LanguageCode = array[2];
		aN_Locale.DisplayLanguage = array[3];
		AndroidNativeUtility.LocaleInfoLoaded(aN_Locale);
	}

	private void OnPackagesListLoaded(string data)
	{
		string[] obj = data.Split("|"[0]);
		AndroidNativeUtility.ActionDevicePackagesListLoaded(obj);
	}

	private void OnNetworkInfoLoaded(string data)
	{
		string[] array = data.Split("|"[0]);
		AN_NetworkInfo aN_NetworkInfo = new AN_NetworkInfo();
		aN_NetworkInfo.SubnetMask = array[0];
		aN_NetworkInfo.IpAddress = array[1];
		aN_NetworkInfo.MacAddress = array[2];
		aN_NetworkInfo.SSID = array[3];
		aN_NetworkInfo.BSSID = array[4];
		aN_NetworkInfo.LinkSpeed = Convert.ToInt32(array[5]);
		aN_NetworkInfo.NetworkId = Convert.ToInt32(array[6]);
		AndroidNativeUtility.ActionNetworkInfoLoaded(aN_NetworkInfo);
	}

	static AndroidNativeUtility()
	{
		AndroidNativeUtility.OnPackageCheckResult = delegate
		{
		};
		AndroidNativeUtility.OnAndroidIdLoaded = delegate
		{
		};
		AndroidNativeUtility.InternalStoragePathLoaded = delegate
		{
		};
		AndroidNativeUtility.ExternalStoragePathLoaded = delegate
		{
		};
		AndroidNativeUtility.LocaleInfoLoaded = delegate
		{
		};
		AndroidNativeUtility.ActionDevicePackagesListLoaded = delegate
		{
		};
		AndroidNativeUtility.ActionNetworkInfoLoaded = delegate
		{
		};
		AndroidNativeUtility.OnOAuthRefreshTokenLoaded = delegate
		{
		};
		AndroidNativeUtility.OnOAuthAccessTokenLoaded = delegate
		{
		};
		AndroidNativeUtility.OnDeviceCodeLoaded = delegate
		{
		};
	}
}
