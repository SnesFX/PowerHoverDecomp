using UnityEngine;
using UnityEngine.UI;

public class NotificationController : MonoBehaviour
{
	public const string ANIM_NAME = "NotificationFadeIn";

	public Text notificationText;

	public AudioSource audioS;

	private Animator animator;

	private void Start()
	{
		animator = GetComponent<Animator>();
	}

	public void Notify(string text)
	{
		notificationText.text = text;
		animator.Play("NotificationFadeIn", -1, 0f);
		if (audioS != null)
		{
			audioS.Play();
		}
	}
}
