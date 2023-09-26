using UnityEngine;

public class LegacyAnimationSpeed : MonoBehaviour
{
	public float AnimationPlayBackSpeed = 1f;

	public Animation anim;

	private void Start()
	{
		anim = GetComponent<Animation>();
		foreach (AnimationState item in anim)
		{
			item.speed = AnimationPlayBackSpeed;
		}
	}
}
