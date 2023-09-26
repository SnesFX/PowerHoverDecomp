using UnityEngine;

public class LogoController : ControllerBase
{
	public GameObject LogoCamera;

	public Animator LogoAnimator;

	public AudioSource LogoAudio;

	public Animator fader;

	private float logoTimer;

	private bool logoOn;

	private int step;

	public MenuType currentMenu { get; private set; }

	public override void Awake()
	{
		type = MenuType.Logo;
		currentMenu = MenuType.Logo;
		LogoCamera.SetActive(false);
		base.Awake();
		fader.transform.GetComponent<MeshRenderer>().enabled = true;
		if (Application.loadedLevelName.Equals("MenuLogo"))
		{
			if ((bool)debugEventSystem)
			{
				debugEventSystem.SetActive(true);
			}
			base.gameObject.SetActive(true);
			GetComponent<AudioListener>().enabled = true;
		}
		else if ((bool)debugEventSystem)
		{
			debugEventSystem.SetActive(false);
		}
	}

	public override void OnEnable()
	{
		LogoCamera.SetActive(true);
		base.OnEnable();
		logoOn = true;
		step = 3;
		logoTimer = 1f;
	}

	private void Update()
	{
		if (!logoOn)
		{
			return;
		}
		logoTimer -= Time.deltaTime;
		if (logoTimer <= 0f)
		{
			if (step == 3)
			{
				fader.Play("oddrokfadein", -1, 0f);
				logoTimer = 0.9f;
				step = 0;
			}
			else if (step == 0)
			{
				logoTimer = 4.4f;
				step = 1;
				LogoAnimator.Play("PHLogoNEw", -1, 0f);
				LogoAudio.Play();
			}
			else if (step == 1)
			{
				step = 2;
				logoTimer = 1f;
				fader.Play("oddrokfadeout", -1, 0f);
			}
			else if (step == 2)
			{
				LogoCompleted();
			}
		}
		else if (step == 1 && logoTimer < 3f && (Input.GetMouseButton(0) || Input.GetButtonDown("Fire")))
		{
			logoTimer = 0f;
			step = 1;
		}
	}

	private void LogoCompleted()
	{
		logoOn = false;
		step = 0;
		logoTimer = 1f;
		LogoAudio.Stop();
		if (!GameDataController.Exists("LastScene"))
		{
			SceneLoader.Instance.LoadTutorial();
		}
		else
		{
			Main.Instance.SwapMenu(MenuType.Main);
		}
	}
}
