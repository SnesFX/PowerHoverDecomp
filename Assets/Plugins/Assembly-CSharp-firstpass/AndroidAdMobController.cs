using System;
using System.Collections.Generic;
using SA.Common.Pattern;
using UnityEngine;

public class AndroidAdMobController : Singleton<AndroidAdMobController>, GoogleMobileAdInterface
{
	private bool _IsInited;

	private Dictionary<int, AndroidADBanner> _banners;

	private bool _IsEditorTestingEnabled = true;

	private int _EditorFillRate = 100;

	private string _BannersUunitId;

	private string _InterstisialUnitId;

	private string _RewardedVideoAdUnitId;

	private const string DEVICES_SEPARATOR = ",";

	private bool _InterstitialShowOnLoad;

	private bool _RewardedVideoShowOnLoad;

	public List<GoogleMobileAdBanner> banners
	{
		get
		{
			List<GoogleMobileAdBanner> list = new List<GoogleMobileAdBanner>();
			if (_banners == null)
			{
				return list;
			}
			foreach (KeyValuePair<int, AndroidADBanner> banner in _banners)
			{
				list.Add(banner.Value);
			}
			return list;
		}
	}

	public bool IsInited
	{
		get
		{
			return _IsInited;
		}
	}

	public string BannersUunitId
	{
		get
		{
			return _BannersUunitId;
		}
	}

	public string InterstisialUnitId
	{
		get
		{
			return _InterstisialUnitId;
		}
	}

	public string RewardedVideoAdUnitId
	{
		get
		{
			return _RewardedVideoAdUnitId;
		}
	}

	public bool IsEditorTestingEnabled
	{
		get
		{
			return SA_EditorTesting.IsInsideEditor && _IsEditorTestingEnabled;
		}
	}

	public event Action<string, int> OnRewarded = delegate
	{
	};

	public event Action OnRewardedVideoAdClosed = delegate
	{
	};

	public event Action<int> OnRewardedVideoAdFailedToLoad = delegate
	{
	};

	public event Action OnRewardedVideoAdLeftApplication = delegate
	{
	};

	public event Action OnRewardedVideoLoaded = delegate
	{
	};

	public event Action OnRewardedVideoAdOpened = delegate
	{
	};

	public event Action OnRewardedVideoStarted = delegate
	{
	};

	public event Action OnInterstitialLoaded = delegate
	{
	};

	public event Action<int> OnInterstitialFailedLoading = delegate
	{
	};

	public event Action OnInterstitialOpened = delegate
	{
	};

	public event Action OnInterstitialClosed = delegate
	{
	};

	public event Action OnInterstitialLeftApplication = delegate
	{
	};

	public event Action<string> OnAdInAppRequest = delegate
	{
	};

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			foreach (KeyValuePair<int, AndroidADBanner> banner in _banners)
			{
				banner.Value.Pause();
			}
			return;
		}
		foreach (KeyValuePair<int, AndroidADBanner> banner2 in _banners)
		{
			banner2.Value.Resume();
		}
	}

	public void Init(string ad_unit_id)
	{
		if (_IsInited)
		{
			Debug.LogWarning("Init shoudl be called only once. Call ignored");
			return;
		}
		_IsInited = true;
		_BannersUunitId = ad_unit_id;
		_InterstisialUnitId = ad_unit_id;
		_RewardedVideoAdUnitId = ad_unit_id;
		_banners = new Dictionary<int, AndroidADBanner>();
		if (IsEditorTestingEnabled)
		{
			Debug.Log("Initialized with Editor Testing Profile");
			Singleton<SA_EditorAd>.Instance.SetFillRate(_EditorFillRate);
		}
		else
		{
			AN_GoogleAdProxy.InitMobileAd(ad_unit_id);
		}
	}

	public void Init(string banners_unit_id, string interstisial_unit_id)
	{
		if (_IsInited)
		{
			Debug.LogWarning("Init shoudl be called only once. Call ignored");
			return;
		}
		Init(banners_unit_id);
		SetInterstisialsUnitID(interstisial_unit_id);
	}

	public void InitEditorTesting(bool isTestingEnabled, int editorFillRate)
	{
		_IsEditorTestingEnabled = isTestingEnabled;
		_EditorFillRate = editorFillRate;
	}

	public void SetBannersUnitID(string ad_unit_id)
	{
		_BannersUunitId = ad_unit_id;
		AN_GoogleAdProxy.ChangeBannersUnitID(ad_unit_id);
	}

	public void SetInterstisialsUnitID(string ad_unit_id)
	{
		_InterstisialUnitId = ad_unit_id;
		AN_GoogleAdProxy.ChangeInterstisialsUnitID(ad_unit_id);
	}

	public void SetRewardedVideoAdUnitID(string id)
	{
		_RewardedVideoAdUnitId = id;
		AN_GoogleAdProxy.ChangeRewardedVideoUnitID(_RewardedVideoAdUnitId);
	}

	public void AddKeyword(string keyword)
	{
		if (!_IsInited)
		{
			Debug.LogWarning("AddKeyword shoudl be called only after Init function. Call ignored");
		}
		else
		{
			AN_GoogleAdProxy.AddKeyword(keyword);
		}
	}

	public void SetBirthday(int year, AndroidMonth month, int day)
	{
		if (!_IsInited)
		{
			Debug.LogWarning("SetBirthday shoudl be called only after Init function. Call ignored");
		}
		else
		{
			AN_GoogleAdProxy.SetBirthday(year, (int)month, day);
		}
	}

	public void TagForChildDirectedTreatment(bool tagForChildDirectedTreatment)
	{
		if (!_IsInited)
		{
			Debug.LogWarning("TagForChildDirectedTreatment shoudl be called only after Init function. Call ignored");
		}
		else
		{
			AN_GoogleAdProxy.TagForChildDirectedTreatment(tagForChildDirectedTreatment);
		}
	}

	public void AddTestDevice(string deviceId)
	{
		if (!_IsInited)
		{
			Debug.LogWarning("AddTestDevice shoudl be called only after Init function. Call ignored");
		}
		else
		{
			AN_GoogleAdProxy.AddTestDevice(deviceId);
		}
	}

	public void AddTestDevices(params string[] ids)
	{
		if (!_IsInited)
		{
			Debug.LogWarning("AddTestDevice shoudl be called only after Init function. Call ignored");
		}
		else if (ids.Length != 0)
		{
			AN_GoogleAdProxy.AddTestDevices(string.Join(",", ids));
		}
	}

	public void SetGender(GoogleGender gender)
	{
		if (!_IsInited)
		{
			Debug.LogWarning("SetGender shoudl be called only after Init function. Call ignored");
		}
		else
		{
			AN_GoogleAdProxy.SetGender((int)gender);
		}
	}

	public GoogleMobileAdBanner CreateAdBanner(TextAnchor anchor, GADBannerSize size)
	{
		if (!_IsInited)
		{
			Debug.LogWarning("CreateBannerAd shoudl be called only after Init function. Call ignored");
			return null;
		}
		AndroidADBanner androidADBanner = new AndroidADBanner(anchor, size, GADBannerIdFactory.nextId);
		_banners.Add(androidADBanner.id, androidADBanner);
		return androidADBanner;
	}

	public GoogleMobileAdBanner CreateAdBanner(int x, int y, GADBannerSize size)
	{
		if (!_IsInited)
		{
			Debug.LogWarning("CreateBannerAd shoudl be called only after Init function. Call ignored");
			return null;
		}
		AndroidADBanner androidADBanner = new AndroidADBanner(x, y, size, GADBannerIdFactory.nextId);
		_banners.Add(androidADBanner.id, androidADBanner);
		return androidADBanner;
	}

	public void DestroyBanner(int id)
	{
		if (_banners != null && _banners.ContainsKey(id))
		{
			AndroidADBanner androidADBanner = _banners[id];
			if (androidADBanner.IsLoaded)
			{
				_banners.Remove(id);
				AN_GoogleAdProxy.DestroyBanner(id);
			}
			else
			{
				androidADBanner.DestroyAfterLoad();
			}
		}
	}

	public void StartInterstitialAd()
	{
		if (!_IsInited)
		{
			Debug.LogWarning("StartInterstitialAd shoudl be called only after Init function. Call ignored");
		}
		else if (IsEditorTestingEnabled)
		{
			_InterstitialShowOnLoad = true;
			SA_EditorAd.OnInterstitialLoadComplete += HandleOnInterstitialLoadComplete_Editor;
			Singleton<SA_EditorAd>.Instance.LoadInterstitial();
		}
		else
		{
			AN_GoogleAdProxy.StartInterstitialAd();
		}
	}

	public void LoadInterstitialAd()
	{
		if (!_IsInited)
		{
			Debug.LogWarning("LoadInterstitialAd shoudl be called only after Init function. Call ignored");
		}
		else if (IsEditorTestingEnabled)
		{
			SA_EditorAd.OnInterstitialLoadComplete += HandleOnInterstitialLoadComplete_Editor;
			Singleton<SA_EditorAd>.Instance.LoadInterstitial();
		}
		else
		{
			AN_GoogleAdProxy.LoadInterstitialAd();
		}
	}

	private void HandleOnInterstitialLoadComplete_Editor(bool success)
	{
		SA_EditorAd.OnInterstitialLoadComplete -= HandleOnInterstitialLoadComplete_Editor;
		if (success)
		{
			this.OnInterstitialLoaded();
			if (_InterstitialShowOnLoad)
			{
				_InterstitialShowOnLoad = false;
				ShowInterstitialAd();
			}
		}
		else
		{
			this.OnInterstitialFailedLoading(0);
		}
	}

	public void ShowInterstitialAd()
	{
		if (!_IsInited)
		{
			Debug.LogWarning("ShowInterstitialAd shoudl be called only after Init function. Call ignored");
		}
		else if (IsEditorTestingEnabled)
		{
			SA_EditorAd.OnInterstitialLeftApplication += HandleOnInterstitialLeftApplication_Editor;
			SA_EditorAd.OnInterstitialFinished += HandleOnInterstitialFinished_Editor;
			Singleton<SA_EditorAd>.Instance.ShowInterstitial();
			this.OnInterstitialOpened();
		}
		else
		{
			AN_GoogleAdProxy.ShowInterstitialAd();
		}
	}

	private void HandleOnInterstitialFinished_Editor(bool isRewarded)
	{
		SA_EditorAd.OnInterstitialLeftApplication -= HandleOnInterstitialLeftApplication_Editor;
		SA_EditorAd.OnInterstitialFinished -= HandleOnInterstitialFinished_Editor;
		this.OnInterstitialClosed();
	}

	private void HandleOnInterstitialLeftApplication_Editor()
	{
		this.OnInterstitialLeftApplication();
	}

	public void StartRewardedVideo()
	{
		if (!_IsInited)
		{
			Debug.LogWarning("StartRewardedVideo shoudl be called only after Init function. Call ignored");
			return;
		}
		_RewardedVideoShowOnLoad = true;
		LoadRewardedVideo();
	}

	public void LoadRewardedVideo()
	{
		if (!_IsInited)
		{
			Debug.LogWarning("ShowRewardedVideo shoudl be called only after Init function. Call ignored");
		}
		else if (IsEditorTestingEnabled)
		{
			SA_EditorAd.OnVideoLoadComplete += HandleOnVideoLoadComplete_Editor;
			Singleton<SA_EditorAd>.Instance.LoadVideo();
		}
		else
		{
			AN_GoogleAdProxy.LoadRewardedVideo();
		}
	}

	private void HandleOnVideoLoadComplete_Editor(bool success)
	{
		SA_EditorAd.OnVideoLoadComplete -= HandleOnVideoLoadComplete_Editor;
		if (success)
		{
			this.OnRewardedVideoLoaded();
			if (_RewardedVideoShowOnLoad)
			{
				_RewardedVideoShowOnLoad = false;
				ShowRewardedVideo();
			}
		}
		else
		{
			this.OnRewardedVideoAdFailedToLoad(-1);
		}
	}

	public void ShowRewardedVideo()
	{
		if (!_IsInited)
		{
			Debug.LogWarning("ShowRewardedVideo shoudl be called only after Init function. Call ignored");
		}
		else if (IsEditorTestingEnabled)
		{
			SA_EditorAd.OnVideoLeftApplication += HandleOnVideoLeftApplication_Editor;
			SA_EditorAd.OnVideoFinished += HandleOnVideoFinished_Editor;
			Singleton<SA_EditorAd>.Instance.ShowVideo();
			this.OnRewardedVideoAdOpened();
		}
		else
		{
			AN_GoogleAdProxy.ShowRewardedVideo();
		}
	}

	private void HandleOnVideoFinished_Editor(bool isRewarded)
	{
		SA_EditorAd.OnVideoLeftApplication -= HandleOnVideoLeftApplication_Editor;
		SA_EditorAd.OnVideoFinished -= HandleOnVideoFinished_Editor;
		this.OnRewardedVideoAdClosed();
	}

	private void HandleOnVideoLeftApplication_Editor()
	{
		this.OnRewardedVideoAdLeftApplication();
	}

	public void RecordInAppResolution(GADInAppResolution resolution)
	{
		AN_GoogleAdProxy.RecordInAppResolution((int)resolution);
	}

	public GoogleMobileAdBanner GetBanner(int id)
	{
		if (_banners.ContainsKey(id))
		{
			return _banners[id];
		}
		Debug.LogWarning("Banner id: " + id + " not found");
		return null;
	}

	private void OnBannerAdLoaded(string data)
	{
		string[] array = data.Split("|"[0]);
		int id = Convert.ToInt32(array[0]);
		int w = Convert.ToInt32(array[1]);
		int h = Convert.ToInt32(array[2]);
		AndroidADBanner androidADBanner = GetBanner(id) as AndroidADBanner;
		if (androidADBanner != null)
		{
			androidADBanner.SetDimentions(w, h);
			androidADBanner.OnBannerAdLoaded();
		}
	}

	private void OnBannerAdFailedToLoad(string bannerID)
	{
		int id = Convert.ToInt32(bannerID);
		AndroidADBanner androidADBanner = GetBanner(id) as AndroidADBanner;
		if (androidADBanner != null)
		{
			androidADBanner.OnBannerAdFailedToLoad();
		}
	}

	private void OnBannerAdOpened(string bannerID)
	{
		int id = Convert.ToInt32(bannerID);
		AndroidADBanner androidADBanner = GetBanner(id) as AndroidADBanner;
		if (androidADBanner != null)
		{
			androidADBanner.OnBannerAdOpened();
		}
	}

	private void OnBannerAdClosed(string bannerID)
	{
		int id = Convert.ToInt32(bannerID);
		AndroidADBanner androidADBanner = GetBanner(id) as AndroidADBanner;
		if (androidADBanner != null)
		{
			androidADBanner.OnBannerAdClosed();
		}
	}

	private void OnBannerAdLeftApplication(string bannerID)
	{
		int id = Convert.ToInt32(bannerID);
		AndroidADBanner androidADBanner = GetBanner(id) as AndroidADBanner;
		if (androidADBanner != null)
		{
			androidADBanner.OnBannerAdLeftApplication();
		}
	}

	private void OnInterstitialAdLoaded()
	{
		this.OnInterstitialLoaded();
	}

	private void OnInterstitialAdFailedToLoad(string errorCode)
	{
		this.OnInterstitialFailedLoading(int.Parse(errorCode));
	}

	private void OnInterstitialAdOpened()
	{
		this.OnInterstitialOpened();
	}

	private void OnInterstitialAdClosed()
	{
		this.OnInterstitialClosed();
	}

	private void OnInterstitialAdLeftApplication()
	{
		this.OnInterstitialLeftApplication();
	}

	private void RewardedCallback(string data)
	{
		string[] array = data.Split(new string[1] { "|" }, StringSplitOptions.None);
		this.OnRewarded(array[0], int.Parse(array[1]));
	}

	private void RewardedVideoAdClosed()
	{
		this.OnRewardedVideoAdClosed();
	}

	private void RewardedVideoAdFailedToLoad(string errorCode)
	{
		this.OnRewardedVideoAdFailedToLoad(int.Parse(errorCode));
	}

	private void RewardedVideoAdLeftApplication()
	{
		this.OnRewardedVideoAdLeftApplication();
	}

	private void RewardedVideoLoaded()
	{
		this.OnRewardedVideoLoaded();
	}

	private void RewardedVideoAdOpened()
	{
		this.OnRewardedVideoAdOpened();
	}

	private void RewardedVideoStarted()
	{
		this.OnRewardedVideoStarted();
	}

	private void OnInAppPurchaseRequested(string productId)
	{
		this.OnAdInAppRequest(productId);
	}
}
