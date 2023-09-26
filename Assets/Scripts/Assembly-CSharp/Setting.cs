using System;
using System.Collections.Generic;
using SmartLocalization;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Setting
{
	public enum SettingType
	{
		Music = 0,
		Sfx = 1,
		Language = 2,
		iCloud = 3
	}

	public const string STORAGE_ID_PREFIX = "GameSetti_";

	public SettingType type;

	public int id;

	public Text uiText;

	public Scrollbar uiScroll;

	public float defaultValue;

	[HideInInspector]
	public float value;

	public void UpdateUI()
	{
		SettingType settingType = type;
		if (settingType == SettingType.Language)
		{
			List<SmartCultureInfo> supportedLanguages = LanguageManager.Instance.GetSupportedLanguages();
			int num = Mathf.CeilToInt(value);
			if (num >= supportedLanguages.Count)
			{
				num = 0;
			}
			value = num;
			LanguageManager.Instance.ChangeLanguage(supportedLanguages[num]);
			LocalizationLoader.Instance.SetText(uiText, (value != 1f) ? "MainMenu.Off" : "MainMenu.On");
			uiText.text = supportedLanguages[num].nativeName.ToUpper();
		}
		else
		{
			LocalizationLoader.Instance.SetText(uiText, (value != 1f) ? "MainMenu.Off" : "MainMenu.On");
			if (value > 0f && value < 1f)
			{
				uiText.text = string.Format("{0}%", Mathf.CeilToInt(value * 100f));
			}
			if (uiScroll != null)
			{
				uiScroll.value = value;
			}
		}
	}

	public void SettingChanged(float setValue = -1f)
	{
		value = ((setValue == -1f) ? ((float)((value != 1f) ? 1 : 0)) : setValue);
		UpdateUI();
		GameDataController.Save(value, string.Format("{0}{1}", "GameSetti_", id));
		if (AudioController.Instance != null)
		{
			if (type == SettingType.Music)
			{
				AudioController.Instance.SetMusicVolume(value);
			}
			else if (type == SettingType.Sfx)
			{
				AudioController.Instance.SetSFXVolume(value);
			}
		}
	}
}
