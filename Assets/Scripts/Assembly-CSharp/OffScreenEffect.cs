using UnityEngine;

public class OffScreenEffect : MonoBehaviour
{
	public GameObject alertObject;

	public InfoController offScreenInfo;

	private AudioSource audioSource;

	private float targetVolumeStart;

	private float effectTimer;

	private float targetVolume;

	private HoverController hoverController;

	private bool offScreen;

	private SpriteRenderer spriteRenderer;

	private float fadingAmountStart;

	private float fadingAmount;

	private Color fadeColor;

	private void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		fadeColor = spriteRenderer.color;
		fadingAmountStart = fadeColor.a;
		fadeColor.a = 0f;
		spriteRenderer.color = fadeColor;
		hoverController = Object.FindObjectOfType<HoverController>();
		audioSource = GetComponent<AudioSource>();
		targetVolumeStart = audioSource.volume;
		audioSource.volume = 0f;
		audioSource.pitch = 1f;
		audioSource.Stop();
		spriteRenderer.enabled = false;
		alertObject.SetActive(false);
	}

	private void FadeIn()
	{
		audioSource.volume = 0f;
		audioSource.Play();
		spriteRenderer.enabled = true;
		effectTimer = 0f;
		targetVolume = targetVolumeStart;
		fadingAmount = fadingAmountStart;
		alertObject.SetActive(true);
	}

	private void FadeOut()
	{
		targetVolume = 0f;
		effectTimer = 0f;
		fadingAmount = 0f;
		alertObject.SetActive(false);
	}

	private void Update()
	{
		if (SceneLoader.Instance == null || SceneLoader.Instance.Current == null || GameController.Instance.State != GameController.GameState.Running || SceneLoader.Instance.Current.IsEndless || Main.Instance.CreditsLevel || SceneLoader.Instance.Current.IsChallenge)
		{
			return;
		}
		if (offScreen != hoverController.OffLimits)
		{
			offScreen = hoverController.OffLimits;
			if (offScreen)
			{
				FadeIn();
			}
			else
			{
				FadeOut();
			}
		}
		if (effectTimer < 1f)
		{
			audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, effectTimer += 0.5f * Time.deltaTime);
			fadeColor.a = Mathf.Lerp(fadeColor.a, fadingAmount, effectTimer);
			spriteRenderer.color = fadeColor;
		}
		else if (fadingAmount == 0f && spriteRenderer.enabled)
		{
			audioSource.Stop();
			spriteRenderer.enabled = false;
		}
		else if (fadingAmount == fadingAmountStart && spriteRenderer.enabled)
		{
			effectTimer += Time.deltaTime;
			if (effectTimer > 0.1f && offScreenInfo != null)
			{
				offScreenInfo.ShowInfos(0f);
			}
			if (effectTimer > 1.2f)
			{
				FadeOut();
				audioSource.Stop();
				spriteRenderer.enabled = false;
				hoverController.checkpointController.PlayerDie();
			}
		}
	}
}
