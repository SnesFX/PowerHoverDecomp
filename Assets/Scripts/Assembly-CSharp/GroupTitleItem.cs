using System;
using SmartLocalization;
using UnityEngine;
using UnityEngine.UI;

public class GroupTitleItem : MonoBehaviour
{
	public Text visibleName;

	public string groupName = string.Empty;

	private void Start()
	{
		LanguageManager instance = LanguageManager.Instance;
		instance.OnChangeLanguage = (ChangeLanguageEventHandler)Delegate.Combine(instance.OnChangeLanguage, new ChangeLanguageEventHandler(OnLanguageChanged));
	}

	private void OnDestroy()
	{
		LanguageManager instance = LanguageManager.Instance;
		instance.OnChangeLanguage = (ChangeLanguageEventHandler)Delegate.Remove(instance.OnChangeLanguage, new ChangeLanguageEventHandler(OnLanguageChanged));
	}

	private void OnLanguageChanged(LanguageManager languageManager)
	{
		if (!groupName.Equals(string.Empty))
		{
			visibleName.text = string.Format("{0}", LanguageManager.Instance.GetTextValue(string.Format("MainMenu.{0}", groupName)));
		}
	}
}
