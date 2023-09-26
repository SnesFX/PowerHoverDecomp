using UnityEngine;

public class SceneEffect : MonoBehaviour
{
	public EffectType Effect;

	public bool effectEnabled;

	public bool ManualDisable;

	private void Start()
	{
		EffectType effectType = ((SceneLoader.Instance.Current != null) ? SceneLoader.Instance.Current.EffectType : ((SceneLoader.Instance.GetSceneDetails(Application.loadedLevelName) != null) ? SceneLoader.Instance.GetSceneDetails(Application.loadedLevelName).EffectType : EffectType.None));
		if (!DeviceSettings.Instance.EnableScreenParticles || ManualDisable || effectType != Effect)
		{
			base.gameObject.SetActive(false);
		}
	}

	private void FixedUpdate()
	{
		if (DeviceSettings.Instance.EnableScreenParticles)
		{
			if (effectEnabled && Effect == EffectType.SlowMotion)
			{
				base.gameObject.SetActive(true);
			}
			else if (Effect == EffectType.SlowMotion)
			{
				base.gameObject.SetActive(false);
			}
		}
	}
}
