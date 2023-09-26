using UnityEngine;

public class BoostEffect : MonoBehaviour
{
	private const string COLOR_PREFIX = "_TintColor";

	public float fadeSpeed;

	public Material fadingMaterial;

	private Material fadeMaterialClone;

	public GameObject trail1;

	private float fadingStartAmount;

	private float fadingAmount;

	private Color fadeColor;

	private bool startFade;

	public ParticleSystem fireParticles1;

	public ParticleSystem fireParticles2;

	public AudioSource audioSource;

	private void Awake()
	{
		fadeMaterialClone = Object.Instantiate(fadingMaterial);
		if (fadingMaterial != null)
		{
			fadeColor = fadeMaterialClone.GetColor("_TintColor");
			fadingAmount = fadeColor.a;
		}
		trail1.GetComponent<Renderer>().material = fadeMaterialClone;
		audioSource.volume = 0f;
	}

	public void StartFade()
	{
		startFade = true;
		ParticleSystem particleSystem = fireParticles1;
		float emissionRate = 0f;
		fireParticles2.emissionRate = emissionRate;
		particleSystem.emissionRate = emissionRate;
	}

	private void Update()
	{
		if (startFade)
		{
			FadeColor();
			if (audioSource.volume > 0f)
			{
				audioSource.volume -= Time.fixedDeltaTime * 0.5f;
			}
		}
		else if (audioSource.volume < 0.5f)
		{
			audioSource.volume += Time.fixedDeltaTime * 0.5f;
		}
	}

	private void FadeColor()
	{
		fadeColor.a = fadingAmount;
		fadeMaterialClone.SetColor("_TintColor", fadeColor);
		if (fadingAmount > 0f)
		{
			fadingAmount -= Time.deltaTime * fadeSpeed;
		}
		else
		{
			Reset();
		}
	}

	public void Reset()
	{
		Object.Destroy(base.gameObject, 2f);
	}
}
