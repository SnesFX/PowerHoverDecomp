using UnityEngine;

public class startAnimationAtRandomFrame : MonoBehaviour
{
	private float waitTime = 0.1f;

	private Animator anim;

	private void OnEnable()
	{
		anim = GetComponent<Animator>();
		anim.speed = Random.Range(0, 2000);
		waitTime = 0.1f;
	}

	private void Update()
	{
		if (waitTime > 0f)
		{
			waitTime -= Time.deltaTime;
		}
		else
		{
			anim.speed = 1f;
		}
	}
}
