using System.Collections;
using SmartLocalization;
using UnityEngine;
using UnityEngine.UI;

public class MenuFreeToTryPopUp : MonoBehaviour
{
	public Text priceText;

	public Animator anim;

	private bool disabling;

	private void OnEnable()
	{
		if (IapManager.Instance != null)
		{
			string price = IapManager.Instance.GetPrice();
			if (price != null && price.Length > 1)
			{
				SetPrice(price);
			}
		}
		disabling = false;
	}

	public void Show()
	{
		if (!base.gameObject.activeSelf && (!(IapManager.Instance != null) || !IapManager.Instance.Unlocked) && LanguageManager.Instance.LoadedLanguage.Equals("en"))
		{
			base.gameObject.SetActive(true);
		}
	}

	public void ClosePopUp()
	{
		if (!disabling)
		{
			disabling = true;
			anim.Play("AdPopUpDisAppear", -1, 0f);
			StartCoroutine(PopUpDisable(0.8f));
		}
	}

	private IEnumerator PopUpDisable(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		base.gameObject.SetActive(false);
	}

	private void SetPrice(string text)
	{
		priceText.text = string.Format("Unlock rest of the game for {0}", text);
	}
}
