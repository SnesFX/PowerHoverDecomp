using UnityEngine;

public class GrayscaleEffect : MonoBehaviour
{
	public enum FadeState
	{
		Off = 0,
		FadeIn = 1,
		FadeOut = 2
	}

	private const float EFFECT_SPEED = 0.3f;

	public Animator Curtainanimator;

	public MeshRenderer BlackFill;

	private float fadeTimer;

	private Camera cam;

	public FadeState fadeState { get; private set; }

	private void Awake()
	{
		cam = GetComponent<Camera>();
		cam.enabled = false;
	}

	private void FixedUpdate()
	{
		switch (fadeState)
		{
		case FadeState.Off:
			fadeTimer = 0f;
			Curtainanimator.Play("CurtainIdle");
			cam.enabled = false;
			break;
		case FadeState.FadeIn:
			Curtainanimator.Play("CurtainDown");
			fadeTimer += Time.fixedDeltaTime * 0.3f;
			if (fadeTimer > 10f)
			{
				fadeState = FadeState.Off;
			}
			break;
		case FadeState.FadeOut:
			Curtainanimator.Play("CurtainUp");
			fadeTimer += Time.fixedDeltaTime * 0.3f;
			if (fadeTimer > 1.4f)
			{
				fadeState = FadeState.Off;
			}
			break;
		}
	}

	public void ShowBlack()
	{
		BlackFill.enabled = true;
		cam.enabled = true;
	}

	public void HideBlack()
	{
		BlackFill.enabled = false;
		cam.enabled = false;
	}

	public void StartFadeIn()
	{
		fadeTimer = 0f;
		cam.enabled = true;
		fadeState = FadeState.FadeIn;
	}

	public void StartFadeOut()
	{
		cam.enabled = true;
		fadeState = FadeState.FadeOut;
	}
}
