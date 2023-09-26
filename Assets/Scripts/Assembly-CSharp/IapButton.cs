using UnityEngine;
using UnityEngine.UI;

public class IapButton : MonoBehaviour
{
	public Text textDesc;

	public Text textPrize;

	private void Start()
	{
	}

	public void SetTexts(string name, string prize)
	{
		if (textDesc != null)
		{
			textDesc.text = name;
		}
		if (textPrize != null)
		{
			textPrize.text = prize;
		}
	}

	public void ShowButton(bool show)
	{
		base.gameObject.SetActive(show);
		Button[] componentsInChildren = GetComponentsInChildren<Button>();
		foreach (Button button in componentsInChildren)
		{
			button.interactable = show;
		}
	}

	private void Update()
	{
		if (IapManager.Instance != null && base.gameObject.activeSelf && IapManager.Instance.Unlocked)
		{
			base.gameObject.SetActive(false);
		}
	}
}
