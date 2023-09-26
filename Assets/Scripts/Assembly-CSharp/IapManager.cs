using System.Collections;
using System.Collections.Generic;
using SA.Common.Pattern;
using UnityEngine;

public class IapManager : MonoBehaviour
{
	private const bool DEBUG = false;

	public GameObject AdsIAPBox;

	public IapButton[] AdsButtons;

	public GameObject BoltBox;

	public ChallengeIapButton[] PrizeItems;

	public IAPBoltsGainedPopUp BoltsGainedPopUp;

	private static bool IsInited;

	private const string base64key = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqfNHUj5dvvT+VdYMOEhLFFO/khga7UItjP/eOEVy9fwk6YNeTUC6zpszJw2DIf2zvFCl1TlVPJuLpJ8/Yzt3oKeB0ec6tmH51pLAjxTtShEZm++n4XxgkSkwvDtez/GHyTgNSjGaPAclcJ8GQKKhrFUSzb3gd8nM9q0/16ydcfxOlWuO9Sdv9j2MqQ9Sfhl05LCjr9ZW8TR+0L1DIEqOVk9FRXNpFkikqyQmGo5qCl0pBceUa7z1/UOGt0ySYa1iimUERQv+oCakoYg7GVHMCaK2ShVR2TGM0l+5iB5FPpbi188tDKPnU5pI52QNg4nffNo/+jmWnYTeDOy9QFeSFQIDAQAB";

	private const string IAP_ID = "no_adds";

	private const string IAP_ID_2 = "no_locks";

	private const string IAP_BOLTS_1 = "bolts1";

	private const string IAP_BOLTS_2 = "bolts2";

	private const string IAP_BOLTS_3 = "bolts3";

	private const string IAP_LEVEL_UNLOCK = "level_unlock";

	private const string IAP_CHAPTER_2 = "chapter_2";

	private string devId;

	private bool AdStarted;

	private string localizedPrice;

	private string NoAdsPrizeText;

	public static IapManager Instance { get; private set; }

	public bool IsPopUpActive
	{
		get
		{
			return AdsIAPBox.activeSelf;
		}
	}

	public bool Unlocked { get; private set; }

	private void Awake()
	{
		devId = "_dum";
	}

	private void Start()
	{
		Instance = this;
		ShowButtons(false);
		ShowPopUp(false);
		AndroidNativeUtility.OnAndroidIdLoaded += OnAndroidIdLoaded;
		Singleton<AndroidNativeUtility>.Instance.LoadAndroidId();
		AndroidInAppPurchaseManager.Client.AddProduct("no_adds");
		AndroidInAppPurchaseManager.Client.AddProduct("no_locks");
		AndroidInAppPurchaseManager.Client.AddProduct("bolts1");
		AndroidInAppPurchaseManager.Client.AddProduct("bolts2");
		AndroidInAppPurchaseManager.Client.AddProduct("bolts3");
		AndroidInAppPurchaseManager.Client.AddProduct("level_unlock");
		AndroidInAppPurchaseManager.Client.AddProduct("chapter_2");
		AndroidInAppPurchaseManager.ActionProductPurchased += OnProductPurchased;
		AndroidInAppPurchaseManager.ActionProductConsumed += OnProductConsumed;
		AndroidInAppPurchaseManager.ActionBillingSetupFinished += OnBillingConnected;
		AndroidInAppPurchaseManager.Client.Connect("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqfNHUj5dvvT+VdYMOEhLFFO/khga7UItjP/eOEVy9fwk6YNeTUC6zpszJw2DIf2zvFCl1TlVPJuLpJ8/Yzt3oKeB0ec6tmH51pLAjxTtShEZm++n4XxgkSkwvDtez/GHyTgNSjGaPAclcJ8GQKKhrFUSzb3gd8nM9q0/16ydcfxOlWuO9Sdv9j2MqQ9Sfhl05LCjr9ZW8TR+0L1DIEqOVk9FRXNpFkikqyQmGo5qCl0pBceUa7z1/UOGt0ySYa1iimUERQv+oCakoYg7GVHMCaK2ShVR2TGM0l+5iB5FPpbi188tDKPnU5pI52QNg4nffNo/+jmWnYTeDOy9QFeSFQIDAQAB");
	}

	private void OnAndroidIdLoaded(string id)
	{
		if (UnityAdsIngetration.Instance.UniqueID == null)
		{
			UnityAdsIngetration.Instance.UniqueID = id;
		}
		AndroidNativeUtility.OnAndroidIdLoaded -= OnAndroidIdLoaded;
		devId = string.Format("{0}_{1}_{2}", devId, (!GameDataController.Exists("nonsense")) ? "why" : GameDataController.Load<string>("nonsense"), id);
	}

	private void LoadStored()
	{
		if (GameDataController.Exists("nonsense"))
		{
			string identifier = string.Format("{0}{1}", GameDataController.Load<string>("nonsense"), "no_adds");
			if (GameDataController.Exists(identifier) && GameDataController.Load<string>(identifier).Equals(string.Format("{0}{1}", "no_adds", devId)))
			{
				UnityAdsIngetration.Instance.SetAdsActive(false);
				ShowButtons(false);
				Unlocked = true;
			}
		}
	}

	private void ShowButtons(bool show)
	{
		if (AdsButtons == null)
		{
			return;
		}
		for (int i = 0; i < AdsButtons.Length; i++)
		{
			if (AdsButtons[i] != null)
			{
				AdsButtons[i].ShowButton(show);
			}
		}
	}

	public void ShowPopUp(bool show)
	{
		ShowPopUp(show, 0.0);
	}

	public void ShowPopUp(bool show, double waitTime)
	{
		if (AdsIAPBox == null)
		{
			return;
		}
		if (!show && AdsIAPBox.activeSelf)
		{
			StartCoroutine(PopUpDisable(AdsIAPBox, 0.75f));
			return;
		}
		AdsIAPBox.GetComponent<IAPBox>().SetTime(waitTime);
		AdsIAPBox.SetActive(show);
		if (show)
		{
			AdsIAPBox.GetComponent<MenuPanel>().Activate(true);
		}
	}

	public void ShowBoltPopUp(bool show)
	{
		if (!show && BoltBox.activeSelf)
		{
			StartCoroutine(PopUpDisable(BoltBox, 0.5f));
			return;
		}
		BoltBox.SetActive(show);
		if (show)
		{
			BoltBox.GetComponent<MenuPanel>().Activate(true);
		}
	}

	private IEnumerator WaitAndBuy()
	{
		yield return new WaitForSeconds(0.2f);
		AndroidInAppPurchaseManager.Client.Purchase("no_adds", devId);
		if (UnityAnalyticsIntegration.Instance != null)
		{
			UnityAnalyticsIntegration.Instance.MakeEvent("Purchase", new Dictionary<string, object>
			{
				{
					"stage",
					Main.Instance.CurrentScene
				},
				{
					"firstLocked",
					string.Empty
				}
			});
		}
	}

	private IEnumerator WaitAndConsume(string item)
	{
		yield return new WaitForSeconds(0.2f);
		AndroidInAppPurchaseManager.Client.Purchase(item, devId);
		if (UnityAnalyticsIntegration.Instance != null)
		{
			UnityAnalyticsIntegration.Instance.MakeEvent("Consume", new Dictionary<string, object> { { "item", item } });
		}
	}

	private IEnumerator PopUpDisable(GameObject box, float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		box.SetActive(false);
	}

	public string GetPrice()
	{
		return localizedPrice;
	}

	public void WatchAdd()
	{
		if (!AdStarted)
		{
			AdStarted = true;
			StartCoroutine(WaitAndPlayAd());
		}
	}

	public void WatchAdBolts()
	{
		if (!AdStarted)
		{
			AdStarted = true;
			StartCoroutine(WaitAndPlayBoltAd());
		}
	}

	private IEnumerator WaitAndPlayBoltAd()
	{
		yield return new WaitForSeconds(1f);
		if (UnityAdsIngetration.Instance.IsAdsActivated)
		{
			if (!UnityAdsIngetration.Instance.ShowSuperSonicAd(AdShownBolts) && UnityAdsIngetration.Instance.IsInterstisialsAdReady)
			{
				UnityAdsIngetration.Instance.ShowAdMobAd(true);
			}
			if (UnityAnalyticsIntegration.Instance != null)
			{
				UnityAnalyticsIntegration.Instance.MakeEvent("Ad_Bolts", new Dictionary<string, object>
				{
					{
						"ChallengeMoney",
						GameStats.Instance.ChallengeMoney
					},
					{
						"ChallengeMoneySpent",
						GameStats.Instance.ChallengeMoneySpent
					}
				});
			}
		}
	}

	private IEnumerator WaitAndPlayAd()
	{
		yield return new WaitForSeconds(2f);
		ShowPopUp(false);
		bool noAds = !UnityAdsIngetration.Instance.ShowSuperSonicAd();
		if (noAds && UnityAdsIngetration.Instance.IsInterstisialsAdReady)
		{
			UnityAdsIngetration.Instance.ShowAdMobAd(true);
			noAds = false;
		}
		if (noAds)
		{
			UnityAdsIngetration.Instance.RewardForVideo();
			if (UnityAnalyticsIntegration.Instance != null)
			{
				UnityAnalyticsIntegration.Instance.MakeEvent("Ad_NoAddUnlock", new Dictionary<string, object>
				{
					{
						"locked",
						string.Empty
					},
					{
						"firstLocked",
						string.Empty
					}
				});
			}
		}
		else if (UnityAnalyticsIntegration.Instance != null)
		{
			UnityAnalyticsIntegration.Instance.MakeEvent("Ad_UnlockLevel", new Dictionary<string, object>
			{
				{
					"locked",
					string.Empty
				},
				{
					"firstLocked",
					string.Empty
				}
			});
		}
		AdStarted = false;
	}

	public void AdShownBolts(IronSourcePlacement placement)
	{
		IronSourceEvents.onRewardedVideoAdRewardedEvent -= AdShownBolts;
		BoltBox.GetComponent<IAPBuyBolts>().ClosePopUp();
		AdStarted = false;
		int num = Random.Range(500, 2000);
		BoltsGainedPopUp.OpenPopUp(false, num);
		GameStats.Instance.ChallengeMoney += num;
	}

	public void BuyLevelUnlocks()
	{
		StartCoroutine(WaitAndConsume("level_unlock"));
	}

	public void BuyChapter2()
	{
		StartCoroutine(WaitAndConsume("chapter_2"));
	}

	public void BuyAdsOff()
	{
		StartCoroutine(WaitAndBuy());
	}

	public void BuyBolts1()
	{
		StartCoroutine(WaitAndConsume("bolts1"));
	}

	public void BuyBolts2()
	{
		StartCoroutine(WaitAndConsume("bolts2"));
	}

	public void BuyBolts3()
	{
		StartCoroutine(WaitAndConsume("bolts3"));
	}

	private void OnBillingConnected(BillingResult result)
	{
		AndroidInAppPurchaseManager.ActionBillingSetupFinished -= OnBillingConnected;
		if (result.isSuccess)
		{
			AndroidInAppPurchaseManager.Client.RetrieveProducDetails();
			AndroidInAppPurchaseManager.ActionRetrieveProducsFinished += OnRetrieveProductsFinised;
			return;
		}
		UnityAdsIngetration.Instance.SetAdsActive(true);
		Unlocked = false;
		ShowButtons(true);
		LoadStored();
	}

	private void OnRetrieveProductsFinised(BillingResult result)
	{
		AndroidInAppPurchaseManager.ActionRetrieveProducsFinished -= OnRetrieveProductsFinised;
		if (result.isSuccess)
		{
			IsInited = true;
			if (AndroidInAppPurchaseManager.Client.Inventory.GetProductDetails("bolts1") != null)
			{
				PrizeItems[0].SetTexts(string.Empty, AndroidInAppPurchaseManager.Client.Inventory.GetProductDetails("bolts1").LocalizedPrice);
			}
			if (AndroidInAppPurchaseManager.Client.Inventory.GetProductDetails("bolts2") != null)
			{
				PrizeItems[1].SetTexts(string.Empty, AndroidInAppPurchaseManager.Client.Inventory.GetProductDetails("bolts2").LocalizedPrice);
			}
			if (AndroidInAppPurchaseManager.Client.Inventory.GetProductDetails("bolts3") != null)
			{
				PrizeItems[2].SetTexts(string.Empty, AndroidInAppPurchaseManager.Client.Inventory.GetProductDetails("bolts3").LocalizedPrice);
			}
			if (AndroidInAppPurchaseManager.Client.Inventory.GetProductDetails("level_unlock") != null)
			{
				PrizeItems[3].SetTexts(string.Empty, AndroidInAppPurchaseManager.Client.Inventory.GetProductDetails("level_unlock").LocalizedPrice);
			}
			if (AndroidInAppPurchaseManager.Client.Inventory.GetProductDetails("chapter_2") != null)
			{
				PrizeItems[4].SetTexts(string.Empty, AndroidInAppPurchaseManager.Client.Inventory.GetProductDetails("chapter_2").LocalizedPrice);
			}
			if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("level_unlock"))
			{
				UnlockLevels();
			}
			if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("no_adds") || AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("no_locks"))
			{
				if (CheckPlayload(AndroidInAppPurchaseManager.Client.Inventory.GetPurchaseDetails("no_adds")) || CheckPlayload(AndroidInAppPurchaseManager.Client.Inventory.GetPurchaseDetails("no_locks")))
				{
					UnityAdsIngetration.Instance.SetAdsActive(false);
					ShowButtons(false);
					Unlocked = true;
					if (GameDataController.Exists("nonsense"))
					{
						string identifier = string.Format("{0}{1}", GameDataController.Load<string>("nonsense"), "no_adds");
						GameDataController.Save(string.Format("{0}{1}", "no_adds", devId), identifier);
					}
				}
				else if (GameDataController.Exists("overrideId"))
				{
					UnityAdsIngetration.Instance.SetAdsActive(false);
					Unlocked = true;
					ShowButtons(false);
				}
				else
				{
					UnityAdsIngetration.Instance.SetAdsActive(true);
					Unlocked = false;
					ShowButtons(true);
				}
				return;
			}
			localizedPrice = AndroidInAppPurchaseManager.Client.Inventory.GetProductDetails("no_adds").LocalizedPrice;
			if (AndroidInAppPurchaseManager.Client.Inventory.GetProductDetails("no_adds") != null)
			{
				string text = AndroidInAppPurchaseManager.Client.Inventory.GetProductDetails("no_adds").LocalizedPrice;
				if (AdsButtons != null)
				{
					for (int i = 0; i < AdsButtons.Length; i++)
					{
						AdsButtons[i].SetTexts(string.Empty, text);
					}
				}
				NoAdsPrizeText = text;
			}
			UnityAdsIngetration.Instance.SetAdsActive(true);
			Unlocked = false;
			ShowButtons(true);
		}
		else
		{
			UnityAdsIngetration.Instance.SetAdsActive(true);
			Unlocked = false;
			ShowButtons(true);
			LoadStored();
		}
	}

	public string GetNoAdsPrize()
	{
		return NoAdsPrizeText;
	}

	private bool CheckPlayload(GooglePurchaseTemplate load)
	{
		if (load != null && load.DeveloperPayload != null && load.DeveloperPayload.Length > 1 && (load.DeveloperPayload.Equals(devId) || load.DeveloperPayload.Contains(string.Format("{0}_{1}", "_dum", (!GameDataController.Exists("nonsense")) ? "winteriscoming" : GameDataController.Load<string>("nonsense")))))
		{
			return true;
		}
		return false;
	}

	private void UnlockLevels()
	{
		SceneLoader.Instance.UnlockAllLevels();
		LevelSelectionPathBuilder levelSelectionPathBuilder = Object.FindObjectOfType<LevelSelectionPathBuilder>();
		if (levelSelectionPathBuilder != null)
		{
			levelSelectionPathBuilder.Refresh(true);
		}
		IapPanel iapPanel = Object.FindObjectOfType<IapPanel>();
		if (iapPanel != null && iapPanel.IsActive)
		{
			iapPanel.ClosePopUp();
		}
		if ((bool)StuckButtonVisibility.Instance)
		{
			StuckButtonVisibility.Instance.SetUnlocked();
		}
		if ((bool)GameStats.Instance)
		{
			GameStats.Instance.ChallengeMoney += 20000;
		}
		if (UnityAnalyticsIntegration.Instance != null)
		{
			UnityAnalyticsIntegration.Instance.MakeEvent("PurchaseUnlock", new Dictionary<string, object>
			{
				{
					"stage",
					Main.Instance.CurrentScene
				},
				{
					"firstLocked",
					string.Empty
				}
			});
		}
	}

	private void OnProductConsumed(BillingResult result)
	{
		if (result.IsSuccess)
		{
			OnProcessingConsumeProduct(result.Purchase);
		}
	}

	private void OnProcessingConsumeProduct(GooglePurchaseTemplate purchase)
	{
		if (purchase.OrderId != null && purchase.OrderId.Length >= 1 && purchase.State == GooglePurchaseState.PURCHASED)
		{
			int num = 0;
			if (purchase.SKU.Equals("bolts1"))
			{
				num = 5000;
			}
			else if (purchase.SKU.Equals("bolts2"))
			{
				num = 20000;
			}
			else if (purchase.SKU.Equals("bolts3"))
			{
				num = 100000;
			}
			BoltBox.GetComponent<IAPBuyBolts>().ClosePopUp();
			BoltsGainedPopUp.OpenPopUp(false, num);
			GameStats.Instance.ChallengeMoney += num;
			GameStats.Instance.SaveStats();
		}
	}

	private void OnProductPurchased(BillingResult result)
	{
		if (result.IsSuccess)
		{
			OnProcessingPurchasedProduct(result.Purchase);
		}
		if (result.Response != 7)
		{
			return;
		}
		if (result.Purchase.SKU.StartsWith("no_"))
		{
			GameDataController.Save("ydaerlawenwo", "overrideId");
			UnityAdsIngetration.Instance.SetAdsActive(false);
			Unlocked = true;
			ShowButtons(false);
			ShowPopUp(false);
			if (DeviceSettings.Instance.RunningOnTV)
			{
				CustomInputController.Instance.PopPanel();
			}
		}
		else if (result.Purchase.SKU.EndsWith("level_unlock"))
		{
			UnlockLevels();
		}
		else if (result.Purchase.SKU.EndsWith("chapter_2"))
		{
			SceneLoader.Instance.UnlockLevel(SceneLoader.Instance.GetFirstLevel(1));
			ClosePopUps();
			MainController mainController = Object.FindObjectOfType<MainController>();
			if (mainController != null)
			{
				mainController.SwitchMap();
				mainController.SetOnMain();
			}
		}
	}

	private void OnProcessingPurchasedProduct(GooglePurchaseTemplate purchase)
	{
		if (AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased(purchase.SKU) && purchase.DeveloperPayload.Equals(devId))
		{
			if (purchase.SKU.StartsWith("bolt"))
			{
				AndroidInAppPurchaseManager.Client.Consume(purchase.SKU);
			}
			else
			{
				VerifyPurhcase(purchase);
			}
		}
	}

	private void VerifyPurhcase(GooglePurchaseTemplate purchase)
	{
		if (purchase.OrderId == null || purchase.OrderId.Length < 1)
		{
			return;
		}
		if (purchase.SKU.StartsWith("no_"))
		{
			UnityAdsIngetration.Instance.SetAdsActive(false);
			Unlocked = true;
			ShowButtons(false);
			ShowPopUp(false);
			if (GameDataController.Exists("nonsense"))
			{
				string identifier = string.Format("{0}{1}", GameDataController.Load<string>("nonsense"), "no_adds");
				GameDataController.Save(string.Format("{0}{1}", "no_adds", devId), identifier);
			}
		}
		else if (purchase.SKU.Contains("chapter"))
		{
			SceneLoader.Instance.UnlockLevel(SceneLoader.Instance.GetFirstLevel(1));
			ClosePopUps();
			MainController mainController = Object.FindObjectOfType<MainController>();
			if (mainController != null)
			{
				mainController.SwitchMap();
				mainController.SetOnMain();
			}
			if (UnityAnalyticsIntegration.Instance != null)
			{
				UnityAnalyticsIntegration.Instance.MakeEvent("PurchaseChapter2", new Dictionary<string, object>
				{
					{
						"stage",
						Main.Instance.CurrentScene
					},
					{
						"firstLocked",
						string.Empty
					}
				});
			}
		}
		else
		{
			UnlockLevels();
		}
	}

	private void ClosePopUps()
	{
		IapPanel[] array = Object.FindObjectsOfType<IapPanel>();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null && array[i].IsActive)
			{
				array[i].ClosePopUp();
			}
		}
	}
}
