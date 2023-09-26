using System.Collections.Generic;
using SmartLocalization;
using UnityEngine;

public class RateUsButton : MonoBehaviour
{
	public bool IsInSettings;

	public GameObject RateObject;

	private const string RatePressed = "RatePressed";

	private bool VisibilitySet;

	private bool Used;

	private void Start()
	{
		if (GameDataController.Exists("RatePressed") || GameStats.Instance == null || GameStats.Instance.LevelCompetions < 10)
		{
			RateObject.SetActive(false);
			Used = true;
		}
		else if (IsInSettings)
		{
			RateObject.SetActive(true);
		}
		else
		{
			RateObject.SetActive(false);
		}
		string languageCode = LanguageManager.Instance.CurrentlyLoadedCulture.languageCode;
		if (languageCode != null && languageCode.Equals("ja"))
		{
			RateObject.SetActive(false);
			Used = true;
		}
	}

	private void Update()
	{
		if (!Used && (!(DeviceSettings.Instance != null) || !DeviceSettings.Instance.RunningOnTV) && (!(SceneLoader.Instance != null) || SceneLoader.Instance.Current == null || !SceneLoader.Instance.Current.IsChallenge) && !IsInSettings)
		{
			if (!VisibilitySet && GameController.Instance != null && GameController.Instance.State == GameController.GameState.End)
			{
				VisibilitySet = true;
				RateObject.SetActive(Random.value < 0.1f + Mathf.Min(0.4f, (float)GameStats.Instance.LevelCompetions / 100f));
			}
			else if (VisibilitySet && GameController.Instance != null && GameController.Instance.State == GameController.GameState.Start)
			{
				VisibilitySet = false;
				RateObject.SetActive(false);
			}
		}
	}

	public void RateButtonPressed()
	{
		AndroidRateUsPopUp androidRateUsPopUp = AndroidRateUsPopUp.Create(LanguageManager.Instance.GetTextValue("MainMenu.RateUsHeader"), LanguageManager.Instance.GetTextValue("MainMenu.RateUsText"), "market://details?id=com.oddrok.powerhover", LanguageManager.Instance.GetTextValue("MainMenu.RateUsRate"), LanguageManager.Instance.GetTextValue("MainMenu.RateUsAskLater"), LanguageManager.Instance.GetTextValue("MainMenu.RateUsNoThanks"));
		androidRateUsPopUp.ActionComplete += onRatePopUpClose;
	}

	private void onRatePopUpClose(AndroidDialogResult result)
	{
		switch (result)
		{
		case AndroidDialogResult.RATED:
			GameDataController.Save(true, "RatePressed");
			if (UnityAnalyticsIntegration.Instance != null)
			{
				UnityAnalyticsIntegration.Instance.MakeEvent("RateUs", new Dictionary<string, object>
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
			break;
		case AndroidDialogResult.DECLINED:
			GameDataController.Save(true, "RatePressed");
			break;
		}
	}
}
