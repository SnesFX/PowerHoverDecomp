using System;
using SA.Common.Pattern;
using UnityEngine;

public class UnityAdsIngetration : MonoBehaviour
{
	private const bool DEBUG = false;

	private GoogleMobileAdBanner banner;

	private int bannerShowCounter;

	private Map map;

	private bool firstCall;

	private bool openNewLevel;

	public static UnityAdsIngetration Instance { get; private set; }

	public bool IsInitialized
	{
		get
		{
			return IsAdsActivated && (IsReady() || IsInterstisialsAdReady);
		}
	}

	public bool IsAdsActivated { get; private set; }

	public bool IsInterstisialsAdReady { get; private set; }

	public string UniqueID { get; set; }

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		map = UnityEngine.Object.FindObjectOfType<Map>();
		GooglePlayUtils.ActionAdvertisingIdLoaded = (Action<GP_AdvertisingIdLoadResult>)Delegate.Combine(GooglePlayUtils.ActionAdvertisingIdLoaded, new Action<GP_AdvertisingIdLoadResult>(ActionAdvertisingIdLoaded));
		Singleton<GooglePlayUtils>.Instance.GetAdvertisingId();
	}

	private void ActionAdvertisingIdLoaded(GP_AdvertisingIdLoadResult res)
	{
		if (res.IsSucceeded)
		{
			UniqueID = res.id;
		}
	}

	private void OnDestroy()
	{
		if (Singleton<AndroidAdMobController>.Instance != null)
		{
			Singleton<AndroidAdMobController>.Instance.OnInterstitialLoaded -= OnInterstisialsLoaded;
			Singleton<AndroidAdMobController>.Instance.OnInterstitialOpened -= OnInterstisialsOpen;
			Singleton<AndroidAdMobController>.Instance.OnInterstitialClosed -= OnInterstisialsClosed;
		}
		if (!IsAdsActivated)
		{
		}
	}

	private void OnApplicationPause(bool isPaused)
	{
		if (firstCall)
		{
			IronSource.Agent.onApplicationPause(isPaused);
		}
	}

	private void InitAdMob()
	{
		Singleton<AndroidAdMobController>.Instance.Init("ca-app-pub-7838208249918515/2280487180", "ca-app-pub-7838208249918515/9022027184");
		Singleton<AndroidAdMobController>.Instance.OnInterstitialLoaded += OnInterstisialsLoaded;
		Singleton<AndroidAdMobController>.Instance.OnInterstitialOpened += OnInterstisialsOpen;
		Singleton<AndroidAdMobController>.Instance.OnInterstitialClosed += OnInterstisialsClosed;
		Singleton<AndroidAdMobController>.Instance.AddTestDevice("AB425CD3103CC62882D489EEE79B3127");
		Singleton<AndroidAdMobController>.Instance.LoadInterstitialAd();
		banner = Singleton<AndroidAdMobController>.Instance.CreateAdBanner(TextAnchor.UpperCenter, GADBannerSize.BANNER);
		banner.ShowOnLoad = false;
		GoogleMobileAdBanner googleMobileAdBanner = banner;
		googleMobileAdBanner.OnLoadedAction = (Action<GoogleMobileAdBanner>)Delegate.Combine(googleMobileAdBanner.OnLoadedAction, new Action<GoogleMobileAdBanner>(BannerLoaded));
	}

	private void InitSuperSonic()
	{
		IronSourceEvents.onInterstitialAdReadyEvent += InterstitialReadyEvent;
		IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialLoadFailedEvent;
		IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialShowSuccessEvent;
		IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialShowFailEvent;
		IronSource.Agent.init("482600dd", IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL);
		IronSource.Agent.shouldTrackNetworkState(true);
		IronSource.Agent.loadInterstitial();
	}

	private void InterstitialShowSuccessEvent()
	{
		IronSource.Agent.loadInterstitial();
	}

	private void InterstitialReadyEvent()
	{
	}

	private void InterstitialLoadFailedEvent(IronSourceError error)
	{
	}

	private void InterstitialShowFailEvent(IronSourceError error)
	{
		IronSource.Agent.loadInterstitial();
	}

	private void RewardedVideoAdRewardedEvent(IronSourcePlacement placement)
	{
		RewardForVideo();
		IronSourceEvents.onRewardedVideoAdRewardedEvent -= RewardedVideoAdRewardedEvent;
	}

	public void SetAdsActive(bool enable)
	{
		IsAdsActivated = enable;
		if (enable)
		{
			Initialize();
		}
		else if (banner != null)
		{
			Singleton<AndroidAdMobController>.Instance.DestroyBanner(banner.id);
		}
	}

	public void Initialize()
	{
		if (!firstCall || !IsInitialized)
		{
			firstCall = true;
			InitSuperSonic();
			InitAdMob();
		}
	}

	public bool BannerReady()
	{
		return banner.IsLoaded;
	}

	public void BannerHide()
	{
		if (banner != null)
		{
			banner.Hide();
			if (bannerShowCounter > 10)
			{
				banner.Refresh();
				bannerShowCounter = 0;
			}
		}
	}

	public void BannerShow()
	{
		if (BannerReady())
		{
			banner.Show();
			bannerShowCounter++;
		}
	}

	private void BannerLoaded(GoogleMobileAdBanner banner)
	{
	}

	public bool IsReady()
	{
		return IronSource.Agent.isRewardedVideoAvailable();
	}

	public void ShowAdMobAd(bool openLevel = false)
	{
		if ((IsInterstisialsAdReady || IronSource.Agent.isInterstitialReady()) && IsAdsActivated)
		{
			openNewLevel = openLevel;
			if (!ShowIronInterstitial())
			{
				Singleton<AndroidAdMobController>.Instance.ShowInterstitialAd();
			}
		}
	}

	private bool ShowIronInterstitial()
	{
		int num = (GameDataController.Exists("ACOUNT") ? GameDataController.Load<int>("ACOUNT") : 0);
		num++;
		if (num == 0 && IsInterstisialsAdReady)
		{
			GameDataController.Save(num, "ACOUNT");
			return false;
		}
		if (IronSource.Agent.isInterstitialReady())
		{
			num = ((num <= 0) ? num : (-2));
			IronSource.Agent.showInterstitial();
			GameDataController.Save(num, "ACOUNT");
			return true;
		}
		GameDataController.Save(num, "ACOUNT");
		return false;
	}

	public bool ShowSuperSonicAd(Action<IronSourcePlacement> AdResult = null)
	{
		if (!IsReady())
		{
			return false;
		}
		if (AdResult == null)
		{
			IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
		}
		else
		{
			IronSourceEvents.onRewardedVideoAdRewardedEvent += AdResult;
		}
		IronSource.Agent.showRewardedVideo();
		return true;
	}

	public void RewardForVideo()
	{
		if (map == null)
		{
			map = UnityEngine.Object.FindObjectOfType<Map>();
		}
		if (map != null)
		{
			MapLevelSelector lastSelectedLevel = map.GetLastSelectedLevel();
			if (lastSelectedLevel != null)
			{
				lastSelectedLevel.StartScene();
			}
		}
	}

	private void OnInterstisialsLoaded()
	{
		IsInterstisialsAdReady = true;
	}

	private void OnInterstisialsOpen()
	{
		IsInterstisialsAdReady = false;
		Singleton<AndroidAdMobController>.Instance.LoadInterstitialAd();
	}

	private void OnInterstisialsClosed()
	{
		if (openNewLevel)
		{
			openNewLevel = false;
			RewardForVideo();
		}
	}

	public void GiveReward()
	{
		OnInterstisialsClosed();
	}
}
