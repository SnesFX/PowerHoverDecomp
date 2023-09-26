using System;
using System.Collections.Generic;
using SA.Common.Pattern;
using SmartLocalization;
using UnityEngine;

public class OptionsPanel : MenuPanel
{
	public List<Setting> GameSettings;

	public GameObject UnlockLevels;

	private LanguageManager languageManager;

	private bool fbExists;

	private bool twitterExists;

	private const string fbpackage = "com.facebook.katana";

	private const string twitterPackage = "com.twitter.android";

	private new void Awake()
	{
		base.Awake();
		string identifier = string.Format("{0}{1}", "GameSetti_", 2);
		languageManager = LanguageManager.Instance;
		if (!GameDataController.Exists(identifier))
		{
			int num = 1;
			List<SmartCultureInfo> supportedLanguages = languageManager.GetSupportedLanguages();
			SmartCultureInfo systemLanguage = languageManager.GetSupportedSystemLanguage();
			num = ((systemLanguage == null) ? supportedLanguages.FindIndex((SmartCultureInfo x) => x.languageCode.Equals("en")) : supportedLanguages.FindIndex((SmartCultureInfo x) => x.languageCode.Equals(systemLanguage.languageCode)));
			if (num == -1)
			{
				num = 1;
			}
			languageManager.ChangeLanguage(supportedLanguages[num]);
			GameDataController.Save((float)num, identifier);
		}
	}

	private void Start()
	{
		if (GameSettings != null)
		{
			for (int i = 0; i < GameSettings.Count; i++)
			{
				string identifier = string.Format("{0}{1}", "GameSetti_", GameSettings[i].id);
				GameSettings[i].value = ((!GameDataController.Exists(identifier)) ? GameSettings[i].defaultValue : GameDataController.Load<float>(identifier));
				GameSettings[i].UpdateUI();
				if (GameSettings[i].type == Setting.SettingType.Sfx)
				{
					GameSettings[i].SettingChanged(GameSettings[i].value);
				}
			}
		}
		CheckSocialApps();
		LanguageManager instance = LanguageManager.Instance;
		instance.OnChangeLanguage = (ChangeLanguageEventHandler)Delegate.Combine(instance.OnChangeLanguage, new ChangeLanguageEventHandler(OnLanguageChanged));
	}

	private void OnDestroy()
	{
		if (LanguageManager.HasInstance)
		{
			LanguageManager instance = LanguageManager.Instance;
			instance.OnChangeLanguage = (ChangeLanguageEventHandler)Delegate.Remove(instance.OnChangeLanguage, new ChangeLanguageEventHandler(OnLanguageChanged));
		}
		AndroidNativeUtility.OnPackageCheckResult -= OnPackageCheck;
	}

	public override void Activate(bool enable)
	{
		base.Activate(enable);
		if (UnlockLevels != null && StuckButtonVisibility.Instance != null && !StuckButtonVisibility.Instance.Unlocked)
		{
			UnlockLevels.SetActive(true);
		}
		else if (UnlockLevels != null)
		{
			UnlockLevels.SetActive(false);
		}
	}

	public void HideUnlockButton()
	{
		if (UnlockLevels != null)
		{
			UnlockLevels.SetActive(false);
		}
	}

	private void OnLanguageChanged(LanguageManager languageManager)
	{
		if (GameSettings == null)
		{
			return;
		}
		for (int i = 0; i < GameSettings.Count; i++)
		{
			if (GameSettings[i].type != Setting.SettingType.Language)
			{
				GameSettings[i].UpdateUI();
			}
		}
	}

	public void SettingChanged(int setting)
	{
		Setting setting2 = GameSettings.Find((Setting x) => x.id == setting);
		if (setting2 != null)
		{
			if (setting2.type == Setting.SettingType.Language)
			{
				setting2.SettingChanged(setting2.value + 1f);
			}
			else
			{
				setting2.SettingChanged();
			}
		}
	}

	public void SettingChangedScroll(int setting)
	{
		Setting setting2 = GameSettings.Find((Setting x) => x.id == setting);
		if (setting2 != null)
		{
			setting2.SettingChanged(setting2.uiScroll.value);
		}
	}

	public void StartCredits()
	{
		SceneLoader.Instance.LoadCredits();
	}

	public void OpenURL(bool facebook)
	{
		if (facebook)
		{
			if (fbExists)
			{
				Debug.Log("opening fb app");
				Application.OpenURL("fb://page/523308337824661");
			}
			else
			{
				Debug.Log("opening fb in browser");
				Application.OpenURL("http://www.facebook.com/oddrokoddrok");
			}
		}
		else if (twitterExists)
		{
			Debug.Log("opening twitter app");
			Application.OpenURL("twitter://user?screen_name=Oddrok");
		}
		else
		{
			Debug.Log("opening twitter in browser");
			Application.OpenURL("https://twitter.com/Oddrok");
		}
	}

	public void OpenSavesUI()
	{
		Singleton<GooglePlaySavedGamesManager>.Instance.ShowSavedGamesUI("Saves", 1);
	}

	private void CheckSocialApps()
	{
		AndroidNativeUtility.OnPackageCheckResult += OnPackageCheck;
		Singleton<AndroidNativeUtility>.Instance.CheckIsPackageInstalled("com.facebook.katana");
		Singleton<AndroidNativeUtility>.Instance.CheckIsPackageInstalled("com.twitter.android");
	}

	private void OnPackageCheck(AN_PackageCheckResult res)
	{
		if (res.IsSucceeded)
		{
			if (res.packageName.Equals("com.facebook.katana"))
			{
				fbExists = true;
			}
			else if (res.packageName.Equals("com.twitter.android"))
			{
				twitterExists = true;
			}
		}
	}
}
