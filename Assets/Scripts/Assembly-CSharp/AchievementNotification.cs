using UnityEngine;
using UnityEngine.UI;

public class AchievementNotification : MonoBehaviour
{
	public const string ANIM_NAME_IN = "CollectableNotificationFadeIn";

	public const string ANIM_NAME_OUT = "CollectableNotificationFadeOut";

	public Text notificationText;

	public Animator animator;

	public AudioSource audioS;

	private float showTimer;

	private bool dontHide;

	private void Update()
	{
		if (showTimer > 0f && !dontHide)
		{
			showTimer -= Time.deltaTime;
			if (showTimer <= 0f && animator != null)
			{
				animator.Play("CollectableNotificationFadeOut");
			}
		}
	}

	public void Notify(bool hide = false)
	{
		if (!(GameStats.Instance == null))
		{
			if (animator != null)
			{
				animator.Play("CollectableNotificationFadeIn");
			}
			if (audioS != null)
			{
				audioS.Play();
			}
			showTimer = 4f;
			dontHide = hide;
		}
	}
}
