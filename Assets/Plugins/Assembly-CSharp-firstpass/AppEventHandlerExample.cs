using SA.IOSNative.Core;
using SA.IOSNative.Models;
using UnityEngine;

public class AppEventHandlerExample : MonoBehaviour
{
	private void Awake()
	{
		AppController.Subscribe();
		AppController.OnApplicationDidReceiveMemoryWarning += OnApplicationDidReceiveMemoryWarning;
		AppController.OnApplicationDidBecomeActive += HandleOnApplicationDidBecomeActive;
		AppController.OnOpenURL += OnOpenURL;
		AppController.OnContinueUserActivity += OnContinueUserActivity;
		Invoke("TryDetectURL", 2f);
	}

	private void TryDetectURL()
	{
		LaunchUrl launchUrl = AppController.LaunchUrl;
		if (!launchUrl.IsEmpty)
		{
			IOSMessage.Create("Open URL Detecetd", launchUrl.AbsoluteUrl);
		}
		UniversalLink launchUniversalLink = AppController.LaunchUniversalLink;
		if (!launchUniversalLink.IsEmpty)
		{
			IOSMessage.Create("Launch Universal Link Detecetd", launchUniversalLink.AbsoluteUrl);
		}
	}

	private void OnContinueUserActivity(UniversalLink link)
	{
		IOSMessage.Create("Universal Link Detecetd", link.AbsoluteUrl);
	}

	private void OnDestroy()
	{
		AppController.OnApplicationDidReceiveMemoryWarning -= OnApplicationDidReceiveMemoryWarning;
		AppController.OnApplicationDidBecomeActive -= HandleOnApplicationDidBecomeActive;
		AppController.OnOpenURL -= OnOpenURL;
	}

	private void OnOpenURL(LaunchUrl url)
	{
		IOSMessage.Create("Open URL Detecetd", url.AbsoluteUrl);
	}

	private void HandleOnApplicationDidBecomeActive()
	{
		ISN_Logger.Log("Caught OnApplicationDidBecomeActive event");
	}

	private void OnApplicationDidReceiveMemoryWarning()
	{
		ISN_Logger.Log("Caught OnApplicationDidReceiveMemoryWarning event");
	}
}
