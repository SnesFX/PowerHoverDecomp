using UnityEngine;

public class DropRock : ObjectActivator
{
	public GameObject BpositionObject;

	public GameObject ShadowObject;

	public float AnimationSpeed = 1f;

	private Vector3 Bposition;

	private float animationLerp;

	private Vector3 move;

	private Vector3 startShadowScale;

	private bool shadow;

	public override void Start()
	{
		Bposition = BpositionObject.transform.position;
		startShadowScale = ShadowObject.transform.localScale;
		shadow = true;
		ShadowObject.SetActive(false);
		base.Start();
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		if (base.IsActive && animationLerp < 1f)
		{
			if (shadow)
			{
				childObject.SetActive(false);
			}
			if (shadow && animationLerp < 1f)
			{
				ShadowObject.transform.localScale = startShadowScale * (0.1f + 4f * animationLerp);
				ShadowObject.SetActive(true);
				animationLerp += Time.fixedDeltaTime;
				if (animationLerp >= 1f)
				{
					shadow = false;
					animationLerp = 0f;
				}
			}
			else
			{
				animationLerp += Time.fixedDeltaTime * AnimationSpeed;
				base.transform.position = Vector3.Lerp(base.transform.position, Bposition, animationLerp);
				if (!childObject.activeSelf && Vector3.Distance(base.transform.position, Bposition) < 1f)
				{
					childObject.SetActive(true);
				}
			}
		}
		else if (!base.IsActive)
		{
			animationLerp = 0f;
			shadow = true;
			ShadowObject.SetActive(false);
		}
	}
}
