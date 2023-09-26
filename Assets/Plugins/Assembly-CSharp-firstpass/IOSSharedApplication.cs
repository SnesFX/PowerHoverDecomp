using System;
using SA.Common.Models;
using SA.Common.Pattern;
using UnityEngine;

public class IOSSharedApplication : Singleton<IOSSharedApplication>
{
	public const string URL_SCHEME_EXISTS = "url_scheme_exists";

	public const string URL_SCHEME_NOT_FOUND = "url_scheme_not_found";

	public static event Action<ISN_CheckUrlResult> OnUrlCheckResultAction;

	public static event Action<string> OnAdvertisingIdentifierLoadedAction;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void CheckUrl(string url)
	{
	}

	public void OpenUrl(string url)
	{
	}

	public void GetAdvertisingIdentifier()
	{
	}

	private void UrlCheckSuccess(string url)
	{
		IOSSharedApplication.OnUrlCheckResultAction(new ISN_CheckUrlResult(url));
	}

	private void UrlCheckFailed(string url)
	{
		IOSSharedApplication.OnUrlCheckResultAction(new ISN_CheckUrlResult(url, new Error()));
	}

	private void OnAdvertisingIdentifierLoaded(string Identifier)
	{
		IOSSharedApplication.OnAdvertisingIdentifierLoadedAction(Identifier);
	}

	static IOSSharedApplication()
	{
		IOSSharedApplication.OnUrlCheckResultAction = delegate
		{
		};
		IOSSharedApplication.OnAdvertisingIdentifierLoadedAction = delegate
		{
		};
	}
}
