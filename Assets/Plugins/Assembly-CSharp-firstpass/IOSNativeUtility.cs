using System;
using SA.Common.Pattern;
using UnityEngine;

public class IOSNativeUtility : Singleton<IOSNativeUtility>
{
	public bool IsGuidedAccessEnabled
	{
		get
		{
			return false;
		}
	}

	public static bool IsRunningTestFlightBeta
	{
		get
		{
			return true;
		}
	}

	public static event Action<ISN_Locale> OnLocaleLoaded;

	public static event Action<bool> GuidedAccessSessionRequestResult;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void GetLocale()
	{
	}

	public static void CopyToClipboard(string text)
	{
	}

	public static void RedirectToAppStoreRatingPage()
	{
		RedirectToAppStoreRatingPage(IOSNativeSettings.Instance.AppleId);
	}

	public static void RedirectToAppStoreRatingPage(string appleId)
	{
	}

	public static void SetApplicationBagesNumber(int count)
	{
	}

	public static void ShowPreloader()
	{
	}

	public static void HidePreloader()
	{
	}

	public void RequestGuidedAccessSession(bool enabled)
	{
	}

	private void OnGuidedAccessSessionRequestResult(string data)
	{
		bool obj = Convert.ToBoolean(data);
		IOSNativeUtility.GuidedAccessSessionRequestResult(obj);
	}

	private void OnLocaleLoadedHandler(string data)
	{
		string[] array = data.Split('|');
		string countryCode = array[0];
		string contryName = array[1];
		string languageCode = array[2];
		string languageName = array[3];
		ISN_Locale obj = new ISN_Locale(countryCode, contryName, languageCode, languageName);
		IOSNativeUtility.OnLocaleLoaded(obj);
	}

	static IOSNativeUtility()
	{
		IOSNativeUtility.OnLocaleLoaded = delegate
		{
		};
		IOSNativeUtility.GuidedAccessSessionRequestResult = delegate
		{
		};
	}
}
