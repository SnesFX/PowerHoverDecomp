using UnityEngine;

public class CollectEffect : MonoBehaviour
{
	public Animator effectAnimator;

	private static int effectState = Animator.StringToHash("CoinCollect");

	private float playTimer;

	public void Show(Vector3 startPosition, Quaternion startRotation)
	{
		base.transform.localPosition = startPosition;
		base.transform.rotation = startRotation;
		base.gameObject.SetActive(true);
		effectAnimator.enabled = true;
		effectAnimator.Play(effectState, -1, 0f);
		playTimer = 2f;
	}

	private void FixedUpdate()
	{
		if (playTimer > 0f)
		{
			playTimer -= Time.fixedDeltaTime;
			if (playTimer <= 0f)
			{
				base.gameObject.SetActive(false);
			}
		}
	}
}
