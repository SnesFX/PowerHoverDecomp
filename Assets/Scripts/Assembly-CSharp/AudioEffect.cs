using UnityEngine;

public class AudioEffect : MonoBehaviour
{
	public AudioController.Effect Effect;

	public AudioEffectPlayRules Rules;

	private float blockPlayTimer;

	public void PlayEffect()
	{
		blockPlayTimer = 0.3f;
		AudioController.Instance.Play(Effect);
	}

	private void Update()
	{
		if (blockPlayTimer > 0f)
		{
			blockPlayTimer -= Time.deltaTime;
		}
	}

	private void OnStart()
	{
		if (Rules != null && Rules.Type == AudioEffectPlayRules.EventType.OnStart)
		{
			PlayEffect();
		}
	}

	private void OnDestroy()
	{
		if (Rules != null && Rules.Type == AudioEffectPlayRules.EventType.OnDestroy)
		{
			PlayEffect();
		}
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		PlayOnTrigger(collider.tag, AudioEffectPlayRules.EventType.OnTriggerEnter);
	}

	private void OnTriggerExit2D(Collider2D collider)
	{
		PlayOnTrigger(collider.tag, AudioEffectPlayRules.EventType.OnTriggerExit);
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (!(blockPlayTimer > 0f))
		{
			PlayOnTrigger(collider.tag, AudioEffectPlayRules.EventType.OnTriggerEnter);
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (!(blockPlayTimer > 0f))
		{
			PlayOnTrigger(collider.tag, AudioEffectPlayRules.EventType.OnTriggerExit);
		}
	}

	private void PlayOnTrigger(string tag, AudioEffectPlayRules.EventType type)
	{
		if (Rules == null || Rules.Type != type)
		{
			return;
		}
		if (Rules.Tags != null)
		{
			if (Rules.Tags.Contains(tag))
			{
				PlayEffect();
			}
		}
		else
		{
			PlayEffect();
		}
	}
}
