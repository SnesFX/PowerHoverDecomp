using System;
using UnityEngine;

public class AN_DeepLinkingManager : MonoBehaviour
{
	public static event Action<string> OnDeepLinkReceived;

	public static void GetLaunchDeepLinkId()
	{
		AN_SocialSharingProxy.GetLaunchDeepLinkId();
	}

	private void DeepLinkReceived(string linkId)
	{
		AN_DeepLinkingManager.OnDeepLinkReceived(linkId);
	}
}
