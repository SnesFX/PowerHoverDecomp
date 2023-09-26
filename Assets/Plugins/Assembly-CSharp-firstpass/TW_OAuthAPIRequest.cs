using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using SA.Common.Pattern;
using UnityEngine;

public class TW_OAuthAPIRequest : MonoBehaviour
{
	private bool IsFirst = true;

	private string GetParams = string.Empty;

	private string requestUrl;

	private Dictionary<string, string> Headers = new Dictionary<string, string>();

	private SortedDictionary<string, string> requestParams = new SortedDictionary<string, string>();

	public event Action<TW_APIRequstResult> OnResult = delegate
	{
	};

	public static TW_OAuthAPIRequest Create()
	{
		return new GameObject("TW_OAuthAPIRequest").AddComponent<TW_OAuthAPIRequest>();
	}

	public void Send(string url)
	{
		requestUrl = url;
		StartCoroutine(Request());
	}

	public void AddParam(string name, int value)
	{
		AddParam(name, value.ToString());
	}

	public void AddParam(string name, string value)
	{
		if (!IsFirst)
		{
			GetParams += "&";
		}
		else
		{
			GetParams += "?";
		}
		GetParams = GetParams + name + "=" + value;
		IsFirst = false;
		requestParams.Add(name, value);
	}

	protected void SetUrl(string url)
	{
		requestUrl = url;
	}

	private IEnumerator Request()
	{
		TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
		string oauth_consumer_key = SocialPlatfromSettings.Instance.TWITTER_CONSUMER_KEY;
		string oauth_token = string.Empty;
		oauth_token = Singleton<AndroidTwitterManager>.Instance.AccessToken;
		string oauth_signature_method = "HMAC-SHA1";
		string oauth_timestamp = Convert.ToInt64(ts.TotalSeconds).ToString();
		string oauth_nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
		string oauth_version = "1.0";
		requestParams.Add("oauth_version", oauth_version);
		requestParams.Add("oauth_consumer_key", oauth_consumer_key);
		requestParams.Add("oauth_nonce", oauth_nonce);
		requestParams.Add("oauth_signature_method", oauth_signature_method);
		requestParams.Add("oauth_timestamp", oauth_timestamp);
		requestParams.Add("oauth_token", oauth_token);
		string baseString4 = string.Empty;
		baseString4 += "GET&";
		baseString4 = baseString4 + Uri.EscapeDataString(requestUrl) + "&";
		foreach (KeyValuePair<string, string> requestParam in requestParams)
		{
			baseString4 += Uri.EscapeDataString(requestParam.Key + "=" + requestParam.Value + "&");
		}
		baseString4 = baseString4.Substring(0, baseString4.Length - 3);
		string consumerSecret = SocialPlatfromSettings.Instance.TWITTER_CONSUMER_SECRET;
		string oauth_token_secret = string.Empty;
		string signingKey = string.Concat(str2: Uri.EscapeDataString(Singleton<AndroidTwitterManager>.Instance.AccessTokenSecret), str0: Uri.EscapeDataString(consumerSecret), str1: "&");
		HMACSHA1 hasher = new HMACSHA1(new ASCIIEncoding().GetBytes(signingKey));
		string signatureString = Convert.ToBase64String(hasher.ComputeHash(new ASCIIEncoding().GetBytes(baseString4)));
		string authorizationHeaderParams9 = string.Empty;
		authorizationHeaderParams9 += "OAuth ";
		authorizationHeaderParams9 = authorizationHeaderParams9 + "oauth_nonce=\"" + Uri.EscapeDataString(oauth_nonce) + "\",";
		authorizationHeaderParams9 = authorizationHeaderParams9 + "oauth_signature_method=\"" + Uri.EscapeDataString(oauth_signature_method) + "\",";
		authorizationHeaderParams9 = authorizationHeaderParams9 + "oauth_timestamp=\"" + Uri.EscapeDataString(oauth_timestamp) + "\",";
		authorizationHeaderParams9 = authorizationHeaderParams9 + "oauth_consumer_key=\"" + Uri.EscapeDataString(oauth_consumer_key) + "\",";
		authorizationHeaderParams9 = authorizationHeaderParams9 + "oauth_token=\"" + Uri.EscapeDataString(oauth_token) + "\",";
		authorizationHeaderParams9 = authorizationHeaderParams9 + "oauth_signature=\"" + Uri.EscapeDataString(signatureString) + "\",";
		authorizationHeaderParams9 = authorizationHeaderParams9 + "oauth_version=\"" + Uri.EscapeDataString(oauth_version) + "\"";
		requestUrl += GetParams;
		Headers.Add("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
		Headers.Add("Authorization", authorizationHeaderParams9);
		WWW www = new WWW(requestUrl, null, Headers);
		yield return www;
		TW_APIRequstResult result = ((www.error != null) ? new TW_APIRequstResult(false, www.error) : new TW_APIRequstResult(true, www.text));
		this.OnResult(result);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
