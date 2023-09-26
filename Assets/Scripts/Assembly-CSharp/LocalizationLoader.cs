using System.Collections.Generic;
using SmartLocalization;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationLoader : MonoBehaviour
{
	private const int FontCount = 2;

	private Dictionary<string, Font> fontCache;

	private LanguageManager lam;

	public static LocalizationLoader Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
		lam = LanguageManager.Instance;
		fontCache = new Dictionary<string, Font>(2);
	}

	public void SetText(Text t, string localeID)
	{
		string textValue = lam.GetTextValue(localeID);
		if (textValue != null)
		{
			t.text = lam.GetTextValue(localeID).ToUpper();
			t.font = GetFont();
		}
	}

	public void SetText(TextMesh t, string localeID)
	{
		string textValue = lam.GetTextValue(localeID);
		if (textValue != null)
		{
			t.text = lam.GetTextValue(localeID).ToUpper();
			SetTextFont(t);
		}
	}

	public void SetTextFont(TextMesh t)
	{
		t.font = GetFont();
		t.fontSize = 92;
		t.gameObject.GetComponent<MeshRenderer>().material.mainTexture = t.font.material.mainTexture;
	}

	private Font GetFont()
	{
		string fontResourcePath = lam.GetFontResourcePath("LocaleFont");
		if (!fontCache.ContainsKey(fontResourcePath))
		{
			fontCache[fontResourcePath] = lam.GetFont("LocaleFont");
		}
		return fontCache[fontResourcePath];
	}
}
