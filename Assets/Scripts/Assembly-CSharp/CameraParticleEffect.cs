using UnityEngine;

public class CameraParticleEffect : MonoBehaviour
{
	public CameraParticleEffectType EffectType;

	public ParticleSystem particleSystem;

	public void UpdateEffect(bool enable)
	{
		particleSystem.enableEmission = enable;
	}
}
