using UnityEngine;
using UnityEngine.UI;

public class MenuNotificationController : MonoBehaviour
{
	public const string ANIM_NAME = "NotificationFadeIn";

	public Text notificationText;

	private Animator animator;

	public static MenuNotificationController Instance { get; private set; }

	private void Start()
	{
		Instance = this;
		animator = GetComponent<Animator>();
	}

	public void Notify(string text)
	{
		notificationText.text = text;
		animator.Play("NotificationFadeIn", -1, 0f);
	}
}
