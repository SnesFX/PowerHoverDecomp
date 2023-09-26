using UnityEngine;

public class AnimationTriggerAnimationSwapper : ResetObject
{
	public AnimationTrigger TargetTrigger;

	public string SwapToAnimation;

	public string BlendingAnimation;

	public float BlendingSpeedOnMultiplier;

	public float BlendingSpeedOffMultiplier;

	private string DefaultAnimation;

	private bool swapped;

	private float blendTarget;

	private float blending;

	private void Start()
	{
		if (BlendingAnimation != null && BlendingAnimation.Length > 0)
		{
			blending = (blendTarget = 0f);
			DefaultAnimation = TargetTrigger.AnimationToPlay;
			TargetTrigger.animatorObj.Play(BlendingAnimation, -1, 0f);
			TargetTrigger.animatorObj.SetFloat("Blend", 0f);
		}
	}

	private void Update()
	{
		if (blendTarget != blending)
		{
			blending += Time.deltaTime * ((!swapped) ? (0f - BlendingSpeedOffMultiplier) : BlendingSpeedOnMultiplier);
			if ((swapped && blending > blendTarget) || (!swapped && blending < blendTarget))
			{
				blending = blendTarget;
			}
			TargetTrigger.animatorObj.SetFloat("Blend", blending);
		}
	}

	public void Swap()
	{
		TargetTrigger.ActivateTrigger();
		if (BlendingAnimation != null && BlendingAnimation.Length > 0)
		{
			TargetTrigger.animatorObj.Play(BlendingAnimation);
			swapped = !swapped;
			blendTarget = ((blendTarget != 1f) ? 1 : 0);
		}
		else
		{
			TargetTrigger.animatorObj.Play(TargetTrigger.AnimationToPlay);
		}
		string animationToPlay = TargetTrigger.AnimationToPlay;
		TargetTrigger.AnimationToPlay = SwapToAnimation;
		SwapToAnimation = animationToPlay;
	}

	public override void Reset(bool isRewind)
	{
		if (DefaultAnimation != null && DefaultAnimation.Length > 0)
		{
			TargetTrigger.AnimationToPlay = DefaultAnimation;
			DefaultAnimation = string.Empty;
		}
	}
}
