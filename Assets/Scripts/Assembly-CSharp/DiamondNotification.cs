using UnityEngine;
using UnityEngine.UI;

public class DiamondNotification : MonoBehaviour
{
	public const string ANIM_NAME_IN = "EndScreenNotificationBox";

	public Text notificationText;

	public Animator animator;

	public void Notify(string textToShow, bool hide = false)
	{
		if (!(GameStats.Instance == null))
		{
			notificationText.text = textToShow;
			animator.Play("EndScreenNotificationBox");
		}
	}

	public void NotifyLocale(string localeID)
	{
		LocalizationLoader.Instance.SetText(notificationText, localeID);
		Notify(notificationText.text);
	}
}
