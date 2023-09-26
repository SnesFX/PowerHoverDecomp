using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PopUpController : ControllerBase
{
	public TypeWriter popUpText;

	public Text hiddenCopy;

	public AudioMixerSnapshot normalSnapshot;

	public AudioMixerSnapshot pausedSnapshot;

	public RectTransform content;

	private AudioSource fadeInAudio;

	private Animator fadeInAnimator;

	private float showTimer;

	private int showState;

	public override void Awake()
	{
		type = MenuType.PopUp;
		base.Awake();
		fadeInAudio = GetComponent<AudioSource>();
		fadeInAnimator = GetComponent<Animator>();
		if (Application.loadedLevelName.Equals("MenuPopUp"))
		{
			if ((bool)debugEventSystem)
			{
				debugEventSystem.SetActive(true);
			}
		}
		else if ((bool)debugEventSystem)
		{
			debugEventSystem.SetActive(false);
		}
		base.gameObject.SetActive(true);
	}

	private void Start()
	{
		if (DeviceSettings.Instance != null && DeviceSettings.Instance.RunningOnTV)
		{
			Vector3 position = content.position;
			position.y -= 20f;
			content.position = position;
		}
	}

	private void FixedUpdate()
	{
		if (!(showTimer > 0f) || !popUpText.WritingCompleted)
		{
			return;
		}
		showTimer -= Time.fixedDeltaTime;
		if (!(showTimer <= 0f))
		{
			return;
		}
		if (showState == 0)
		{
			if (fadeInAnimator != null)
			{
				fadeInAnimator.Play("MenuPopUpFadeOut", -1, 0f);
			}
			showTimer = 1f;
			showState = 1;
		}
		else
		{
			Close();
		}
	}

	private void StartShow()
	{
		showTimer = 3.5f;
		showState = 0;
		if (fadeInAnimator != null)
		{
			fadeInAnimator.Play("MenuPopUpFadeIn", -1, 0f);
		}
		if (fadeInAudio != null)
		{
			fadeInAudio.Play();
		}
		if (UnityAdsIngetration.Instance.IsInitialized)
		{
			UnityAdsIngetration.Instance.BannerHide();
		}
	}

	public void SetText(InfoBox.InfoBoxDetails details)
	{
		LocalizationLoader.Instance.SetText(hiddenCopy, "MainMenu.Active");
		hiddenCopy.text = details.text;
		LocalizationLoader.Instance.SetText(popUpText.GetComponent<Text>(), "MainMenu.Active");
		popUpText.StartWriting(details.text, (details.page <= 1) ? 1f : 0.35f);
		StartShow();
	}

	public void Close()
	{
		Main.Instance.ClosePopUp();
		popUpText.StopWriting();
		if (fadeInAnimator != null)
		{
			fadeInAnimator.Play("MenuPopUpDefault", -1, 0f);
		}
		if (!Main.Instance.InMenu)
		{
			UIController.Instance.buttonController.PauseButton.SetActive(true);
		}
	}
}
