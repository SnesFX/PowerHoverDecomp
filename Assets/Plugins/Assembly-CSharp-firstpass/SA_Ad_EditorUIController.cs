using System;
using UnityEngine;
using UnityEngine.UI;

public class SA_Ad_EditorUIController : MonoBehaviour
{
	public GameObject VideoPanel;

	public GameObject InterstitialPanel;

	public Image[] AppIcons;

	public Text[] AppNames;

	public event Action<bool> OnCloseVideo = delegate
	{
	};

	public event Action OnVideoLeftApplication = delegate
	{
	};

	public event Action<bool> OnCloseInterstitial = delegate
	{
	};

	public event Action OnInterstitialLeftApplication = delegate
	{
	};

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		SA_EditorTesting.CheckForEventSystem();
		Canvas component = GetComponent<Canvas>();
		component.sortingOrder = 10001;
	}

	private void Start()
	{
	}

	public void InterstitialClick()
	{
		this.OnInterstitialLeftApplication();
	}

	public void VideoClick()
	{
		this.OnVideoLeftApplication();
	}

	public void ShowInterstitialAd()
	{
		base.gameObject.SetActive(true);
		InterstitialPanel.SetActive(true);
	}

	public void ShowVideoAd()
	{
		base.gameObject.SetActive(true);
		VideoPanel.SetActive(true);
	}

	public void CloseInterstitial()
	{
		base.gameObject.SetActive(false);
		InterstitialPanel.SetActive(false);
		this.OnCloseInterstitial(true);
	}

	public void CloseVideo()
	{
		base.gameObject.SetActive(false);
		VideoPanel.SetActive(false);
		this.OnCloseVideo(true);
	}
}
