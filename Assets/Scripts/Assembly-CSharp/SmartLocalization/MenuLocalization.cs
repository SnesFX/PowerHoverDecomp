using System;
using UnityEngine;
using UnityEngine.UI;

namespace SmartLocalization
{
	public class MenuLocalization : LanguageManager
	{
		public string StartsWithString;

		private void Awake()
		{
			LanguageManager languageManager = LanguageManager.Instance;
			languageManager.OnChangeLanguage = (ChangeLanguageEventHandler)Delegate.Combine(languageManager.OnChangeLanguage, new ChangeLanguageEventHandler(OnLanguageChanged));
		}

		private void Start()
		{
			UpdateTexts();
		}

		private void OnDestroy()
		{
			if (LanguageManager.HasInstance)
			{
				LanguageManager languageManager = LanguageManager.Instance;
				languageManager.OnChangeLanguage = (ChangeLanguageEventHandler)Delegate.Remove(languageManager.OnChangeLanguage, new ChangeLanguageEventHandler(OnLanguageChanged));
			}
		}

		private void UpdateTexts()
		{
			Text[] componentsInChildren = GetComponentsInChildren<Text>(true);
			foreach (Text text in componentsInChildren)
			{
				if (text.gameObject.name.StartsWith(StartsWithString) || text.gameObject.name.StartsWith("MainMenu"))
				{
					LocalizationLoader.Instance.SetText(text, text.gameObject.name);
				}
			}
			TextMesh[] componentsInChildren2 = GetComponentsInChildren<TextMesh>(true);
			foreach (TextMesh textMesh in componentsInChildren2)
			{
				if (textMesh.gameObject.name.StartsWith(StartsWithString) || textMesh.gameObject.name.StartsWith("MainMenu"))
				{
					LocalizationLoader.Instance.SetText(textMesh, textMesh.gameObject.name);
				}
				else
				{
					LocalizationLoader.Instance.SetTextFont(textMesh);
				}
			}
		}

		private void OnLanguageChanged(LanguageManager languageManager)
		{
			UpdateTexts();
		}
	}
}
