using UnityEngine;
using UnityEngine.UI;

public class Chapter2Text : MonoBehaviour
{
	public Text buttonText;

	private void Start()
	{
		if (DeviceSettings.Instance != null && DeviceSettings.Instance.RunningOnTV)
		{
			Vector3 localPosition = GetComponent<RectTransform>().localPosition;
			localPosition.x += 40f;
			localPosition.y -= 25f;
			GetComponent<RectTransform>().localPosition = localPosition;
		}
	}

	public void SetChapterText(bool chapter2)
	{
		LocalizationLoader.Instance.SetText(buttonText, "MainMenu.Chapter");
		buttonText.text = string.Format("{0} {1}", buttonText.text, (!chapter2) ? 1 : 2);
	}

	public void SetVisible(bool visible)
	{
		buttonText.enabled = visible;
	}
}
