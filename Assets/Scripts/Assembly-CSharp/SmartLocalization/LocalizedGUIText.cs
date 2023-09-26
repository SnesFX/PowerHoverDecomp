using System;
using UnityEngine;
using UnityEngine.UI;

namespace SmartLocalization
{
	public class LocalizedGUIText : MonoBehaviour
	{
		public string localizedKey = "INSERT_KEY_HERE";

		private void Start()
		{
			LanguageManager instance = LanguageManager.Instance;
			instance.OnChangeLanguage = (ChangeLanguageEventHandler)Delegate.Combine(instance.OnChangeLanguage, new ChangeLanguageEventHandler(OnChangeLanguage));
			OnChangeLanguage(instance);
		}

		private void OnDestroy()
		{
			if (LanguageManager.HasInstance)
			{
				LanguageManager instance = LanguageManager.Instance;
				instance.OnChangeLanguage = (ChangeLanguageEventHandler)Delegate.Remove(instance.OnChangeLanguage, new ChangeLanguageEventHandler(OnChangeLanguage));
			}
		}

		private void OnChangeLanguage(LanguageManager languageManager)
		{
			GetComponent<Text>().text = LanguageManager.Instance.GetTextValue(localizedKey);
		}
	}
}
