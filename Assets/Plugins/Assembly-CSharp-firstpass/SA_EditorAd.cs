using System;
using SA.Common.Pattern;
using UnityEngine;

public class SA_EditorAd : Singleton<SA_EditorAd>
{
	public const float MIN_LOAD_TIME = 1f;

	public const float MAX_LOAD_TIME = 3f;

	private bool _IsInterstitialLoading;

	private bool _IsVideoLoading;

	private bool _IsInterstitialReady;

	private bool _IsVideoReady;

	private int _FillRate = 100;

	private SA_Ad_EditorUIController _EditorUI;

	public bool IsVideoReady
	{
		get
		{
			return _IsVideoReady;
		}
	}

	public bool IsVideoLoading
	{
		get
		{
			return _IsVideoLoading;
		}
	}

	public bool IsInterstitialReady
	{
		get
		{
			return _IsInterstitialReady;
		}
	}

	public bool IsInterstitialLoading
	{
		get
		{
			return _IsInterstitialLoading;
		}
	}

	public bool HasFill
	{
		get
		{
			int num = UnityEngine.Random.Range(1, 100);
			if (num <= _FillRate)
			{
				return true;
			}
			return false;
		}
	}

	public int FillRate
	{
		get
		{
			return _FillRate;
		}
	}

	private SA_Ad_EditorUIController EditorUI
	{
		get
		{
			return _EditorUI;
		}
	}

	public static event Action<bool> OnInterstitialFinished;

	public static event Action<bool> OnInterstitialLoadComplete;

	public static event Action OnInterstitialLeftApplication;

	public static event Action<bool> OnVideoFinished;

	public static event Action<bool> OnVideoLoadComplete;

	public static event Action OnVideoLeftApplication;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void SetFillRate(int fillRate)
	{
		_FillRate = fillRate;
	}

	public void LoadInterstitial()
	{
		if (!_IsInterstitialLoading && !IsInterstitialReady)
		{
			_IsInterstitialLoading = true;
			float time = UnityEngine.Random.Range(1f, 3f);
			Invoke("OnInterstitialRequestComplete", time);
		}
	}

	public void ShowInterstitial()
	{
		if (!IsInterstitialReady)
		{
		}
	}

	public void LoadVideo()
	{
		if (!_IsVideoLoading && !IsVideoReady)
		{
			_IsVideoLoading = true;
			float time = UnityEngine.Random.Range(1f, 3f);
			Invoke("OnVideoRequestComplete", time);
		}
	}

	public void ShowVideo()
	{
		if (!IsVideoReady)
		{
		}
	}

	private void OnVideoRequestComplete()
	{
		_IsVideoLoading = false;
		_IsVideoReady = HasFill;
		SA_EditorAd.OnVideoLoadComplete(_IsVideoReady);
	}

	private void OnInterstitialRequestComplete()
	{
		_IsInterstitialLoading = false;
		_IsInterstitialReady = HasFill;
		SA_EditorAd.OnInterstitialLoadComplete(_IsInterstitialReady);
	}

	private void OnInterstitialFinished_UIEvent(bool IsRewarded)
	{
		_IsInterstitialReady = false;
		SA_EditorAd.OnInterstitialFinished(IsRewarded);
	}

	private void OnVideoFinished_UIEvent(bool IsRewarded)
	{
		_IsVideoReady = false;
		SA_EditorAd.OnVideoFinished(IsRewarded);
	}

	private void OnInterstitialLeftApplication_UIEvent()
	{
		SA_EditorAd.OnInterstitialLeftApplication();
	}

	private void OnVideoLeftApplication_UIEvent()
	{
		SA_EditorAd.OnVideoLeftApplication();
	}

	static SA_EditorAd()
	{
		SA_EditorAd.OnInterstitialFinished = delegate
		{
		};
		SA_EditorAd.OnInterstitialLoadComplete = delegate
		{
		};
		SA_EditorAd.OnInterstitialLeftApplication = delegate
		{
		};
		SA_EditorAd.OnVideoFinished = delegate
		{
		};
		SA_EditorAd.OnVideoLoadComplete = delegate
		{
		};
		SA_EditorAd.OnVideoLeftApplication = delegate
		{
		};
	}
}
