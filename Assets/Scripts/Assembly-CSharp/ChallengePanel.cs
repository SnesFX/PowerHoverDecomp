using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengePanel : MenuPanel
{
	private const string FMT = "0 000";

	public Text StarAmount;

	public Text TriesTotal;

	public Text DistanceTotal;

	public Text TriesLeft;

	public GameObject TriesLeftBox;

	public IAPTriesPopUp TriesLeftPopUp;

	public IAPBoltsGainedPopUp BoltsGainedPopUp;

	private ChallengeMenuItem challenge;

	private float timeUpdater;

	private bool AdRunning;

	private int AdForRetriesUsage;

	private void Start()
	{
		challenge = GetComponentInChildren<ChallengeMenuItem>();
	}

	private void Update()
	{
		if (!IsActive)
		{
			return;
		}
		timeUpdater += Time.deltaTime;
		if (timeUpdater > 1f)
		{
			timeUpdater = 0f;
			double totalSeconds = (DateTime.Today.AddDays(1.0) - DateTime.Now).TotalSeconds;
			if (totalSeconds <= 1.0)
			{
				SceneLoader.Instance.TriesLeft = SceneLoader.MAX_LIFES;
			}
			TriesLeft.text = string.Format("{0}", Mathf.Max(0, SceneLoader.Instance.TriesLeft));
		}
		if (!StarAmount.text.Equals(GameStats.Instance.ChallengeMoney.ToString("0 000")))
		{
			StarAmount.text = GameStats.Instance.ChallengeMoney.ToString("0 000");
		}
	}

	private void OnEnable()
	{
		timeUpdater = 1f;
		if (challenge != null)
		{
			challenge.gameObject.SetActive(true);
			challenge.SetUnlock();
		}
		if (TriesLeftBox != null && IapManager.Instance != null)
		{
			TriesLeftBox.SetActive(!IapManager.Instance.Unlocked);
		}
		if (GameStats.Instance != null)
		{
			TriesLeft.text = string.Format("{0}", Mathf.Max(0, SceneLoader.Instance.LoadTriesLeft()));
			StarAmount.text = GameStats.Instance.ChallengeMoney.ToString("0 000");
			TriesTotal.text = GameStats.Instance.ChallengeTries.ToString("0 000");
			DistanceTotal.text = GameStats.Instance.ChallengeDistance.ToString("0 000");
		}
	}

	public void UIButtonBoltsForRetries()
	{
		if (GameStats.Instance.ChallengeMoney >= 1000)
		{
			GameStats.Instance.ChallengeMoney -= 1000;
			GameStats.Instance.ChallengeMoneySpent += 1000;
			TriesLeftPopUp.ClosePopUp();
			SceneLoader.Instance.TriesLeft += SceneLoader.MAX_LIFES;
			string identifier = string.Format("{0}_{1}", DateTime.Today.Year, DateTime.Today.DayOfYear);
			GameDataController.Save(SceneLoader.Instance.TriesLeft, identifier);
			BoltsGainedPopUp.OpenPopUp(true);
		}
	}

	public void UIButtonRetriesForAd()
	{
		if (!AdRunning && UnityAdsIngetration.Instance.IsReady())
		{
			AdRunning = true;
			AdForRetriesUsage++;
			UnityAdsIngetration.Instance.ShowSuperSonicAd(AdShownMoreTries);
			if (UnityAnalyticsIntegration.Instance != null)
			{
				UnityAnalyticsIntegration.Instance.MakeEvent("Ad_ForRetries", new Dictionary<string, object>
				{
					{
						"scene",
						Main.Instance.CurrentScene
					},
					{ "AdForRetriesUsage", AdForRetriesUsage }
				});
			}
		}
		else if (UnityAdsIngetration.Instance.IsInterstisialsAdReady)
		{
			UnityAdsIngetration.Instance.ShowAdMobAd();
			TriesLeftPopUp.ClosePopUp();
			SceneLoader.Instance.TriesLeft += SceneLoader.MAX_LIFES;
			string identifier = string.Format("{0}_{1}", DateTime.Today.Year, DateTime.Today.DayOfYear);
			GameDataController.Save(SceneLoader.Instance.TriesLeft, identifier);
			BoltsGainedPopUp.OpenPopUp(true);
		}
	}

	public void AdShownMoreTries(IronSourcePlacement placement)
	{
		IronSourceEvents.onRewardedVideoAdRewardedEvent -= AdShownMoreTries;
		AdRunning = false;
		TriesLeftPopUp.ClosePopUp();
		SceneLoader.Instance.TriesLeft += SceneLoader.MAX_LIFES;
		string identifier = string.Format("{0}_{1}", DateTime.Today.Year, DateTime.Today.DayOfYear);
		GameDataController.Save(SceneLoader.Instance.TriesLeft, identifier);
		BoltsGainedPopUp.OpenPopUp(true);
	}

	public override void Activate(bool activate)
	{
		base.Activate(activate);
		if (activate)
		{
			base.transform.GetComponentInChildren<ChallengeCharacterPanel>().SetupButtons();
			OnEnable();
		}
	}

	public void Reset()
	{
		GameStats instance = GameStats.Instance;
		int num = 0;
		GameStats.Instance.ChallengeMoneySpent = num;
		instance.ChallengeMoney = num;
		base.transform.GetComponentInChildren<ChallengeCharacterPanel>().Reset();
	}

	public void BuyBolts()
	{
	}
}
