using System;
using SA.Common.Pattern;
using SA.IOSNative.StoreKit;
using UnityEngine;

public class SK_CloudService : Singleton<SK_CloudService>
{
	public static int AuthorizationStatus
	{
		get
		{
			return BillingNativeBridge.CloudService_AuthorizationStatus();
		}
	}

	public static event Action<SK_AuthorizationResult> OnAuthorizationFinished;

	public static event Action<SK_RequestCapabilitieResult> OnCapabilitiesRequestFinished;

	public static event Action<SK_RequestStorefrontIdentifierResult> OnStorefrontIdentifierRequestFinished;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void RequestAuthorization()
	{
		BillingNativeBridge.CloudService_RequestAuthorization();
	}

	public void RequestCapabilities()
	{
		BillingNativeBridge.CloudService_RequestCapabilities();
	}

	public void RequestStorefrontIdentifier()
	{
		BillingNativeBridge.CloudService_RequestStorefrontIdentifier();
	}

	private void Event_AuthorizationFinished(string data)
	{
		int status = Convert.ToInt32(data);
		SK_AuthorizationResult obj = new SK_AuthorizationResult((SK_CloudServiceAuthorizationStatus)status);
		SK_CloudService.OnAuthorizationFinished(obj);
	}

	private void Event_RequestCapabilitieSsuccess(string data)
	{
		int capability = Convert.ToInt32(data);
		SK_RequestCapabilitieResult obj = new SK_RequestCapabilitieResult((SK_CloudServiceCapability)capability);
		SK_CloudService.OnCapabilitiesRequestFinished(obj);
	}

	private void Event_RequestCapabilitiesFailed(string errorData)
	{
		SK_RequestCapabilitieResult obj = new SK_RequestCapabilitieResult(errorData);
		SK_CloudService.OnCapabilitiesRequestFinished(obj);
	}

	private void Event_RequestStorefrontIdentifierSsuccess(string storefrontIdentifier)
	{
		SK_RequestStorefrontIdentifierResult sK_RequestStorefrontIdentifierResult = new SK_RequestStorefrontIdentifierResult();
		sK_RequestStorefrontIdentifierResult.StorefrontIdentifier = storefrontIdentifier;
		SK_CloudService.OnStorefrontIdentifierRequestFinished(sK_RequestStorefrontIdentifierResult);
	}

	private void Event_RequestStorefrontIdentifierFailed(string errorData)
	{
		SK_RequestStorefrontIdentifierResult obj = new SK_RequestStorefrontIdentifierResult(errorData);
		SK_CloudService.OnStorefrontIdentifierRequestFinished(obj);
	}

	static SK_CloudService()
	{
		SK_CloudService.OnAuthorizationFinished = delegate
		{
		};
		SK_CloudService.OnCapabilitiesRequestFinished = delegate
		{
		};
		SK_CloudService.OnStorefrontIdentifierRequestFinished = delegate
		{
		};
	}
}
