using UnityEngine;

[RequireComponent(typeof(Wave))]
public class WaveModifier : ObjectActivator
{
	public float targetScale;

	public float AnimationSpeed = 1f;

	private float animationLerp;

	private Wave wave;

	public override void Start()
	{
		base.Start();
		wave = GetComponent<Wave>();
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		if (base.IsActive && animationLerp < 1f)
		{
			animationLerp += Time.fixedDeltaTime * AnimationSpeed;
			wave.scale = Mathf.Lerp(wave.scale, targetScale, animationLerp);
		}
		else
		{
			animationLerp = 0f;
		}
	}
}
