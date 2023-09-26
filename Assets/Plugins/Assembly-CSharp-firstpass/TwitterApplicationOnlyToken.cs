using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ANMiniJSON;
using SA.Common.Pattern;
using UnityEngine;

public class TwitterApplicationOnlyToken : Singleton<TwitterApplicationOnlyToken>
{
	private string _currentToken;

	private const string TWITTER_CONSUMER_KEY = "wEvDyAUr2QabVAsWPDiGwg";

	private const string TWITTER_CONSUMER_SECRET = "igRxZbOrkLQPNLSvibNC3mdNJ5tOlVOPH3HNNKDY0";

	private const string BEARER_TOKEN_KEY = "bearer_token_key";

	private Dictionary<string, string> Headers = new Dictionary<string, string>();

	public string currentToken
	{
		get
		{
			if (_currentToken == null && PlayerPrefs.HasKey("bearer_token_key"))
			{
				_currentToken = PlayerPrefs.GetString("bearer_token_key");
			}
			return _currentToken;
		}
	}

	public event Action ActionComplete = delegate
	{
	};

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void RetrieveToken()
	{
		StartCoroutine(Load());
	}

	private IEnumerator Load()
	{
		string url = "https://api.twitter.com/oauth2/token";
		byte[] plainTextBytes = Encoding.UTF8.GetBytes(SocialPlatfromSettings.Instance.TWITTER_CONSUMER_KEY + ":" + SocialPlatfromSettings.Instance.TWITTER_CONSUMER_SECRET);
		string encodedAccessToken = Convert.ToBase64String(plainTextBytes);
		Headers.Clear();
		Headers.Add("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
		Headers.Add("Authorization", "Basic " + encodedAccessToken);
		WWWForm form = new WWWForm();
		form.AddField("grant_type", "client_credentials");
		WWW www = new WWW(url, form.data, Headers);
		yield return www;
		if (www.error == null)
		{
			Dictionary<string, object> dictionary = Json.Deserialize(www.text) as Dictionary<string, object>;
			_currentToken = dictionary["access_token"] as string;
			PlayerPrefs.SetString("bearer_token_key", _currentToken);
		}
		this.ActionComplete();
	}
}
