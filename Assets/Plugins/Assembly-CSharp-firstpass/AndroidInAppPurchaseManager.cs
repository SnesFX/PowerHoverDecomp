using System;
using UnityEngine;

public static class AndroidInAppPurchaseManager
{
	public static AN_InAppClient _Client;

	public static AN_InAppClient Client
	{
		get
		{
			if (_Client == null)
			{
				GameObject gameObject = new GameObject("AndroidInAppPurchaseManager");
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
				if (Application.isEditor && AndroidNativeSettings.Instance.Is_InApps_EditorTestingEnabled)
				{
					_Client = gameObject.AddComponent<AN_InApp_EditorClient>();
				}
				if (_Client == null)
				{
					_Client = gameObject.AddComponent<AN_InAppAndroidClient>();
				}
				_Client.ActionBillingSetupFinished += HandleActionBillingSetupFinished;
				_Client.ActionProductConsumed += HandleActionProductConsumed;
				_Client.ActionProductPurchased += HandleActionProductPurchased;
				_Client.ActionRetrieveProducsFinished += HandleActionRetrieveProducsFinished;
			}
			return _Client;
		}
	}

	[Obsolete("Instance is deprectaed, please use Client instead")]
	public static AN_InAppClient Instance
	{
		get
		{
			return Client;
		}
	}

	public static event Action<BillingResult> ActionProductPurchased;

	public static event Action<BillingResult> ActionProductConsumed;

	public static event Action<BillingResult> ActionBillingSetupFinished;

	public static event Action<BillingResult> ActionRetrieveProducsFinished;

	private static void HandleActionRetrieveProducsFinished(BillingResult res)
	{
		AndroidInAppPurchaseManager.ActionRetrieveProducsFinished(res);
	}

	private static void HandleActionProductPurchased(BillingResult res)
	{
		AndroidInAppPurchaseManager.ActionProductPurchased(res);
	}

	private static void HandleActionProductConsumed(BillingResult res)
	{
		AndroidInAppPurchaseManager.ActionProductConsumed(res);
	}

	private static void HandleActionBillingSetupFinished(BillingResult res)
	{
		AndroidInAppPurchaseManager.ActionBillingSetupFinished(res);
	}

	static AndroidInAppPurchaseManager()
	{
		AndroidInAppPurchaseManager.ActionProductPurchased = delegate
		{
		};
		AndroidInAppPurchaseManager.ActionProductConsumed = delegate
		{
		};
		AndroidInAppPurchaseManager.ActionBillingSetupFinished = delegate
		{
		};
		AndroidInAppPurchaseManager.ActionRetrieveProducsFinished = delegate
		{
		};
		_Client = null;
	}
}
