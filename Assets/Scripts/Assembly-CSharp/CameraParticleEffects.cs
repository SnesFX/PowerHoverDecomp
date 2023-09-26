using System.Collections.Generic;
using UnityEngine;

public class CameraParticleEffects : MonoBehaviour
{
	public List<CameraParticleEffect> ParticleEffects;

	private void Start()
	{
		for (int i = 0; i < ParticleEffects.Count; i++)
		{
			ParticleEffects[i].UpdateEffect(false);
			ParticleEffects[i].gameObject.SetActive(true);
		}
	}

	public void DisableAllEffects()
	{
		for (int i = 0; i < ParticleEffects.Count; i++)
		{
			ParticleEffects[i].UpdateEffect(false);
		}
	}

	public void UpdateEffects(List<CameraParticleEffectType> enabled, bool enable)
	{
		List<CameraParticleEffect> list = new List<CameraParticleEffect>(ParticleEffects);
		for (int i = 0; i < enabled.Count; i++)
		{
			if (enabled[i] != 0)
			{
				CameraParticleEffect cameraParticleEffect = ParticleEffects.Find((CameraParticleEffect x) => x.EffectType == enabled[i]);
				cameraParticleEffect.UpdateEffect(enable);
				list.Remove(cameraParticleEffect);
			}
		}
		if (enable)
		{
			for (int j = 0; j < list.Count; j++)
			{
				list[j].UpdateEffect(false);
			}
		}
	}
}
