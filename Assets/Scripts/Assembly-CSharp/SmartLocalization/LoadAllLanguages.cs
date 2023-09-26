using System;
using System.Collections.Generic;
using UnityEngine;

namespace SmartLocalization
{
	public class LoadAllLanguages : MonoBehaviour
	{
		private Dictionary<string, string> currentLanguageValues;

		private List<SmartCultureInfo> availableLanguages;

		private LanguageManager languageManager;

		private Vector2 valuesScrollPosition = Vector2.zero;

		private Vector2 languagesScrollPosition = Vector2.zero;

		private void Awake()
		{
			languageManager = LanguageManager.Instance;
			SmartCultureInfo supportedSystemLanguage = languageManager.GetSupportedSystemLanguage();
			if (supportedSystemLanguage != null)
			{
				languageManager.ChangeLanguage(supportedSystemLanguage);
			}
			if (languageManager.NumberOfSupportedLanguages > 0)
			{
				currentLanguageValues = languageManager.RawTextDatabase;
				availableLanguages = languageManager.GetSupportedLanguages();
			}
			else
			{
				Debug.LogError("No languages are created!, Open the Smart Localization plugin at Window->Smart Localization and create your language!");
			}
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
		}

		private void OnLanguageChanged(LanguageManager languageManager)
		{
			currentLanguageValues = languageManager.RawTextDatabase;
		}

		private void OnGUI()
		{
			if (languageManager.NumberOfSupportedLanguages <= 0)
			{
				return;
			}
			if (languageManager.CurrentlyLoadedCulture != null)
			{
				GUILayout.Label("Current Language:" + languageManager.CurrentlyLoadedCulture.ToString());
			}
			GUILayout.BeginHorizontal();
			GUILayout.Label("Keys:", GUILayout.Width(460f));
			GUILayout.Label("Values:", GUILayout.Width(460f));
			GUILayout.EndHorizontal();
			valuesScrollPosition = GUILayout.BeginScrollView(valuesScrollPosition);
			foreach (KeyValuePair<string, string> currentLanguageValue in currentLanguageValues)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(currentLanguageValue.Key, GUILayout.Width(460f));
				GUILayout.Label(currentLanguageValue.Value, GUILayout.Width(460f));
				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();
			languagesScrollPosition = GUILayout.BeginScrollView(languagesScrollPosition);
			foreach (SmartCultureInfo availableLanguage in availableLanguages)
			{
				if (GUILayout.Button(availableLanguage.nativeName, GUILayout.Width(960f)))
				{
					languageManager.ChangeLanguage(availableLanguage);
				}
			}
			GUILayout.EndScrollView();
		}
	}
}
