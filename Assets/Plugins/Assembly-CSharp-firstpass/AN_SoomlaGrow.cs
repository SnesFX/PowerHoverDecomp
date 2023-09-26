using System;
using SA.Common.Pattern;
using UnityEngine;

public class AN_SoomlaGrow : Singleton<AN_SoomlaGrow>
{
	private static bool _IsInitialized = false;

	public static event Action ActionInitialized;

	public static event Action ActionConnected;

	public static event Action ActionDisconnected;

	public void CreateListner()
	{
	}

	public static void Init()
	{
		if (!_IsInitialized && AndroidNativeSettings.Instance.EnableSoomla)
		{
			Singleton<AN_SoomlaGrow>.Instance.CreateListner();
			AN_SoomlaProxy.Initalize(AndroidNativeSettings.Instance.SoomlaGameKey, AndroidNativeSettings.Instance.SoomlaEnvKey);
			Singleton<AndroidTwitterManager>.Instance.OnTwitterLoginStarted += OnTwitterLoginStarted;
			Singleton<AndroidTwitterManager>.Instance.OnTwitterLogOut += OnTwitterLogOut;
			Singleton<AndroidTwitterManager>.Instance.OnAuthCompleteAction += HandleOnAuthCompleteAction;
			Singleton<AndroidTwitterManager>.Instance.OnUserDataRequestCompleteAction += HandleOnUserDataRequestCompleteAction;
			Singleton<AndroidTwitterManager>.Instance.OnTwitterPostStarted += TW_PostStarted;
			Singleton<AndroidTwitterManager>.Instance.OnPostingCompleteAction += TW_PostCompleted;
			SPFacebook.OnLoginStarted += FB_OnLoginStarted;
			SPFacebook.OnLogOut += FB_OnLogOut;
			SPFacebook.OnAuthCompleteAction += FB_OnAuthCompleteAction;
			SPFacebook.OnPostStarted += FB_PostStarted;
			SPFacebook.OnPostingCompleteAction += FB_PostCompleted;
			SPFacebook.OnFriendsDataRequestCompleteAction += FB_HandleOnFriendsDataRequestCompleteAction;
			SPFacebook.OnFriendsRequestStarted += FB_OnFriendsRequestStarted;
			_IsInitialized = true;
		}
	}

	public static void PurchaseStarted(string prodcutId)
	{
		if (CheckState())
		{
			AN_SoomlaProxy.OnMarketPurchaseStarted(prodcutId);
		}
	}

	public static void PurchaseFinished(string prodcutId, long priceInMicros, string currency)
	{
		if (CheckState())
		{
			AN_SoomlaProxy.OnMarketPurchaseFinished(prodcutId, priceInMicros, currency);
		}
	}

	public static void PurchaseCanceled(string prodcutId)
	{
		if (CheckState())
		{
			AN_SoomlaProxy.OnMarketPurchaseCancelled(prodcutId);
		}
	}

	public static void SetPurhsesSupportedState(bool isSupported)
	{
		if (CheckState())
		{
			AN_SoomlaProxy.SetBillingState(isSupported);
		}
	}

	public static void PurchaseError()
	{
		if (CheckState())
		{
			AN_SoomlaProxy.OnMarketPurchaseFailed();
		}
	}

	private static void FriendsRequest(AN_SoomlaEventType eventType, AN_SoomlaSocialProvider provider)
	{
		if (CheckState())
		{
			AN_SoomlaProxy.OnFriendsRequest((int)eventType, (int)provider);
		}
	}

	public static void SocialLogin(AN_SoomlaEventType eventType, AN_SoomlaSocialProvider provider)
	{
		if (CheckState())
		{
			AN_SoomlaProxy.OnSocialLogin((int)eventType, (int)provider);
		}
	}

	public static void SocialLoginFinished(AN_SoomlaSocialProvider provider, string ProfileId)
	{
		if (CheckState())
		{
			AN_SoomlaProxy.OnSocialLoginFinished((int)provider, ProfileId);
		}
	}

	public static void SocialLogOut(AN_SoomlaEventType eventType, AN_SoomlaSocialProvider provider)
	{
		if (CheckState())
		{
			AN_SoomlaProxy.OnSocialLogout((int)eventType, (int)provider);
		}
	}

	public static void SocialShare(AN_SoomlaEventType eventType, AN_SoomlaSocialProvider provider)
	{
		if (CheckState())
		{
			AN_SoomlaProxy.OnSocialShare((int)eventType, (int)provider);
		}
	}

	private static void FB_OnFriendsRequestStarted()
	{
		FriendsRequest(AN_SoomlaEventType.SOOMLA_EVENT_STARTED, AN_SoomlaSocialProvider.FACEBOOK);
	}

	private static void FB_HandleOnFriendsDataRequestCompleteAction(FB_Result res)
	{
		if (res.IsSucceeded)
		{
			FriendsRequest(AN_SoomlaEventType.SOOMLA_EVENT_FINISHED, AN_SoomlaSocialProvider.FACEBOOK);
		}
		else
		{
			FriendsRequest(AN_SoomlaEventType.SOOMLA_EVENT_FAILED, AN_SoomlaSocialProvider.FACEBOOK);
		}
	}

	private static void FB_OnAuthCompleteAction(FB_Result res)
	{
		if (res.IsSucceeded)
		{
			SocialLoginFinished(AN_SoomlaSocialProvider.FACEBOOK, Singleton<SPFacebook>.Instance.UserId);
		}
		else
		{
			SocialLogin(AN_SoomlaEventType.SOOMLA_EVENT_FAILED, AN_SoomlaSocialProvider.FACEBOOK);
		}
	}

	private static void FB_OnLoginStarted()
	{
		SocialLogin(AN_SoomlaEventType.SOOMLA_EVENT_STARTED, AN_SoomlaSocialProvider.FACEBOOK);
	}

	private static void FB_OnLogOut()
	{
		SocialLogOut(AN_SoomlaEventType.SOOMLA_EVENT_STARTED, AN_SoomlaSocialProvider.FACEBOOK);
		SocialLogOut(AN_SoomlaEventType.SOOMLA_EVENT_FINISHED, AN_SoomlaSocialProvider.FACEBOOK);
	}

	private static void FB_PostStarted()
	{
		SocialShare(AN_SoomlaEventType.SOOMLA_EVENT_STARTED, AN_SoomlaSocialProvider.FACEBOOK);
	}

	private static void FB_PostCompleted(FB_PostResult res)
	{
		Debug.Log("FB_PostCompleted");
		if (res.IsSucceeded)
		{
			Debug.Log("SOOMLA_EVENT_FINISHED");
			SocialShare(AN_SoomlaEventType.SOOMLA_EVENT_FINISHED, AN_SoomlaSocialProvider.FACEBOOK);
		}
		else
		{
			Debug.Log("SOOMLA_EVENT_CNACELED");
			SocialShare(AN_SoomlaEventType.SOOMLA_EVENT_CNACELED, AN_SoomlaSocialProvider.FACEBOOK);
		}
	}

	private static void HandleOnAuthCompleteAction(TWResult res)
	{
		if (!res.IsSucceeded)
		{
			SocialLogin(AN_SoomlaEventType.SOOMLA_EVENT_CNACELED, AN_SoomlaSocialProvider.TWITTER);
		}
		else
		{
			Singleton<AndroidTwitterManager>.Instance.LoadUserData();
		}
	}

	private static void HandleOnUserDataRequestCompleteAction(TWResult res)
	{
		if (res.IsSucceeded)
		{
			SocialLoginFinished(AN_SoomlaSocialProvider.TWITTER, Singleton<AndroidTwitterManager>.Instance.userInfo.id);
		}
		else
		{
			SocialLogin(AN_SoomlaEventType.SOOMLA_EVENT_FAILED, AN_SoomlaSocialProvider.TWITTER);
		}
	}

	private static void OnTwitterLoginStarted()
	{
		SocialLogin(AN_SoomlaEventType.SOOMLA_EVENT_STARTED, AN_SoomlaSocialProvider.TWITTER);
	}

	private static void OnTwitterLogOut()
	{
		SocialLogOut(AN_SoomlaEventType.SOOMLA_EVENT_STARTED, AN_SoomlaSocialProvider.TWITTER);
		SocialLogOut(AN_SoomlaEventType.SOOMLA_EVENT_FINISHED, AN_SoomlaSocialProvider.TWITTER);
	}

	private static void TW_PostStarted()
	{
		SocialShare(AN_SoomlaEventType.SOOMLA_EVENT_STARTED, AN_SoomlaSocialProvider.TWITTER);
	}

	private static void TW_PostCompleted(TWResult res)
	{
		if (res.IsSucceeded)
		{
			SocialShare(AN_SoomlaEventType.SOOMLA_EVENT_FINISHED, AN_SoomlaSocialProvider.TWITTER);
		}
		else
		{
			SocialShare(AN_SoomlaEventType.SOOMLA_EVENT_FAILED, AN_SoomlaSocialProvider.TWITTER);
		}
	}

	private static bool CheckState()
	{
		if (AndroidNativeSettings.Instance.EnableSoomla)
		{
			Init();
		}
		return AndroidNativeSettings.Instance.EnableSoomla;
	}

	private void OnInitialized()
	{
		Debug.Log("AN_SOOMAL OnInitialized");
		AN_SoomlaGrow.ActionInitialized();
	}

	private void OnConnected()
	{
		AN_SoomlaGrow.ActionConnected();
	}

	private void OnDisconnected()
	{
		AN_SoomlaGrow.ActionDisconnected();
	}

	static AN_SoomlaGrow()
	{
		AN_SoomlaGrow.ActionInitialized = delegate
		{
		};
		AN_SoomlaGrow.ActionConnected = delegate
		{
		};
		AN_SoomlaGrow.ActionDisconnected = delegate
		{
		};
	}
}
