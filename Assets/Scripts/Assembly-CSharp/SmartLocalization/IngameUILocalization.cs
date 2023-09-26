using UnityEngine.UI;

namespace SmartLocalization
{
	public class IngameUILocalization : LanguageManager
	{
		private void Awake()
		{
			Text[] componentsInChildren = GetComponentsInChildren<Text>(true);
			foreach (Text text in componentsInChildren)
			{
				if ((text.gameObject.name.StartsWith("IngameUI") || text.gameObject.name.StartsWith("MainMenu")) && LocalizationLoader.Instance != null)
				{
					LocalizationLoader.Instance.SetText(text, text.gameObject.name);
				}
			}
		}
	}
}
