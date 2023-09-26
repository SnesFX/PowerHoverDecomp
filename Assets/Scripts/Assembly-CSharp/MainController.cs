using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainController : ControllerBase
{
	private const string ANIM_IN = "MenuPanelFadeIn";

	private const string ANIM_OUT = "MenuPanelFadeOut";

	private const string MapAnimIn = "MainMenuSwitchChapterIn";

	private const string MapAnimOut = "MainMenuSwitchChapterOut";

	public Animator mainPanel;

	public Animator optionsPanel;

	public Animator mapPanel;

	public Animator achievementsPanel;

	public Animator challengePanel;

	public Animator emptyPanel;

	public Map map;

	public Map map2;

	public IapPanel chapter2LockPanel;

	private Map currentMap;

	public Text versionText;

	public Text playButtonText;

	public Chapter2Button chapter2Button;

	public Chapter2Text chapter2Text;

	private AudioSource audioS;

	private CustomInputController customInput;

	private bool firstRun;

	private float waitTimer;

	public MenuType currentMenu { get; private set; }

	private void Start()
	{
		if (DeviceSettings.Instance != null && DeviceSettings.Instance.RunningOnTV)
		{
			mainPanel.GetComponent<RectTransform>().localScale = mainPanel.GetComponent<RectTransform>().localScale * 0.9f;
			optionsPanel.GetComponent<RectTransform>().localScale = optionsPanel.GetComponent<RectTransform>().localScale * 0.9f;
			achievementsPanel.GetComponent<RectTransform>().localScale = optionsPanel.GetComponent<RectTransform>().localScale * 0.9f;
			challengePanel.GetComponent<RectTransform>().localScale = optionsPanel.GetComponent<RectTransform>().localScale * 0.9f;
			emptyPanel.GetComponent<RectTransform>().localScale = optionsPanel.GetComponent<RectTransform>().localScale * 0.9f;
			mapPanel.GetComponent<RectTransform>().offsetMin = new Vector2(30f, -470f);
		}
		ActivatePanel(mainPanel);
		waitTimer = 0f;
	}

	public override void Awake()
	{
		firstRun = true;
		type = MenuType.Main;
		currentMenu = MenuType.Main;
		customInput = Object.FindObjectOfType<CustomInputController>();
		base.Awake();
		audioS = GetComponent<AudioSource>();
		if (Application.loadedLevelName.Equals("MenuMain"))
		{
			if ((bool)debugEventSystem)
			{
				debugEventSystem.SetActive(true);
			}
			base.gameObject.SetActive(true);
		}
		else if ((bool)debugEventSystem)
		{
			debugEventSystem.SetActive(false);
		}
	}

	public override void OnEnable()
	{
		EnablePanel(mainPanel.gameObject);
		if (UnityAdsIngetration.Instance != null && UnityAdsIngetration.Instance.IsInitialized)
		{
			StartCoroutine(HideBanner());
		}
		if (GameDataController.Exists("LastMap"))
		{
			SwitchMap(GameDataController.Load<int>("LastMap") == 0, false);
		}
		else
		{
			SwitchMap(true, false);
		}
		base.OnEnable();
		if (CustomInputController.Instance != null && CustomInputController.Instance.GetCurrentMenu() == CustomInputController.InputMenu.Cutscene)
		{
			map.SetOnMap(true, false);
			mapPanel.GetComponent<MenuPanel>().Activate(true);
		}
		Button[] componentsInChildren = mainPanel.gameObject.GetComponentsInChildren<Button>(true);
		if (componentsInChildren != null)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].gameObject.SetActive(true);
			}
		}
		if (firstRun)
		{
			versionText.text = string.Format("ver {0}", CurrentBundleVersion.version);
			firstRun = false;
		}
	}

	private IEnumerator HideBanner()
	{
		yield return new WaitForSeconds(0.5f);
		UnityAdsIngetration.Instance.BannerHide();
	}

	public void SwitchMap()
	{
		PlayClick();
		if (SceneLoader.Instance.IsChapter2Unlocked())
		{
			GameStats.Instance.Chapter2 = true;
			SceneLoader.Instance.UnlockLevel(SceneLoader.Instance.GetFirstLevel(1));
			AudioController.Instance.SwitchMusic(map.gameObject.activeSelf ? 3 : 0);
			bool activeSelf = map2.gameObject.activeSelf;
			SwitchMap(activeSelf, true);
			GameDataController.Save((!activeSelf) ? 1 : 0, "LastMap");
		}
		else
		{
			chapter2LockPanel.OpenPopUp();
		}
	}

	private void SwitchMap(bool toMap1, bool animateInOut)
	{
		chapter2Button.SetChapterText(!toMap1);
		chapter2Text.SetChapterText(!toMap1);
		if (toMap1)
		{
			StartCoroutine(SwitchMapAnimation(map2, map, animateInOut));
		}
		else
		{
			StartCoroutine(SwitchMapAnimation(map, map2, animateInOut));
		}
	}

	private IEnumerator SwitchMapAnimation(Map mapToClose, Map mapToOpen, bool animateInOut)
	{
		mapToClose.SwitchAudioListener(false);
		if (animateInOut)
		{
			mapToClose.GetComponent<Animator>().Play("MainMenuSwitchChapterOut", -1, 0f);
		}
		yield return new WaitForSeconds(0.2f);
		mapToOpen.gameObject.SetActive(true);
		if (animateInOut)
		{
			mapToOpen.GetComponent<Animator>().Play("MainMenuSwitchChapterIn", -1, 0f);
		}
		mapToClose.gameObject.SetActive(false);
		mapToOpen.SwitchAudioListener(true);
		currentMap = mapToOpen;
		customInput.SetMap(mapToOpen);
		currentMap.SetOnMap(true, false);
		if (Main.Instance != null && Main.Instance.FromChallengeLevel)
		{
			currentMap.SetOnMap(false, false);
			challengePanel.GetComponent<MenuPanel>().Activate(true);
			Main.Instance.FromChallengeLevel = false;
		}
		else if ((Main.Instance != null && Main.Instance.CreditsLevel) || (SceneLoader.Instance != null && SceneLoader.Instance.Current != null && SceneLoader.Instance.Current.IsChallenge))
		{
			currentMap.SetOnMap(false, false);
			if (currentMenu == MenuType.Map)
			{
				SetOnMap();
			}
		}
		else if (currentMenu == MenuType.Map || currentMenu == MenuType.Main || (Main.Instance != null && Main.Instance.TutorialLevel))
		{
			currentMap.SetOnMap(true, false);
			if (Main.Instance != null && currentMenu == MenuType.Map && !Main.Instance.TutorialLevel)
			{
				mapPanel.GetComponent<MenuPanel>().Activate(true);
			}
			else
			{
				mainPanel.GetComponent<MenuPanel>().Activate(true);
			}
		}
	}

	public void StartTutorial()
	{
		if (SceneLoader.Instance != null && !SceneLoader.Instance.IsLoading)
		{
			PlayClick();
			currentMap.cameraSpline.ScrollToStart();
			SceneLoader.Instance.LoadTutorial(true);
		}
	}

	public void MenuItemClicked(int m)
	{
		PlayClick();
		switch (m)
		{
		case 0:
			currentMenu = MenuType.Main;
			if (Main.Instance != null)
			{
				Main.Instance.ResumeGame(true);
			}
			else
			{
				base.gameObject.SetActive(false);
				if ((bool)debugEventSystem)
				{
					debugEventSystem.SetActive(false);
				}
			}
			chapter2Text.SetVisible(true);
			DisablingPanel(mainPanel);
			break;
		case 1:
			if (currentMap == null)
			{
				return;
			}
			mainPanel.enabled = true;
			mainPanel.Play("MenuPanelFadeOut", -1, 0f);
			currentMenu = MenuType.Achievements;
			currentMap.cameraSpline.ScrollToStart();
			DisablingPanel(mainPanel);
			ActivatePanel(achievementsPanel);
			StuckButtonVisibility.Instance.Hide();
			chapter2Text.SetVisible(false);
			break;
		case 12:
		{
			if (currentMap == null)
			{
				return;
			}
			mainPanel.enabled = true;
			mainPanel.Play("MenuPanelFadeOut", -1, 0f);
			currentMenu = MenuType.Challenges;
			currentMap.cameraSpline.ScrollToStart();
			DisablingPanel(mainPanel);
			ActivatePanel(challengePanel);
			StuckButtonVisibility.Instance.Hide();
			IapButton componentInChildren = optionsPanel.GetComponentInChildren<IapButton>();
			if (componentInChildren != null)
			{
				componentInChildren.ShowButton(!IapManager.Instance.Unlocked);
			}
			chapter2Text.SetVisible(false);
			break;
		}
		case 3:
		{
			if (currentMap == null)
			{
				return;
			}
			mainPanel.enabled = true;
			mainPanel.Play("MenuPanelFadeOut", -1, 0f);
			currentMap.cameraSpline.ScrollToStart();
			currentMenu = MenuType.Options;
			DisablingPanel(mainPanel);
			ActivatePanel(optionsPanel);
			StuckButtonVisibility.Instance.Hide();
			IapButton componentInChildren = optionsPanel.GetComponentInChildren<IapButton>();
			if (componentInChildren != null)
			{
				componentInChildren.ShowButton(!IapManager.Instance.Unlocked);
			}
			chapter2Text.SetVisible(false);
			break;
		}
		case 4:
			if (currentMap == null)
			{
				return;
			}
			mainPanel.enabled = true;
			currentMap.SetOnMap(true, true);
			currentMenu = MenuType.Map;
			mainPanel.Play("MenuPanelFadeOut", -1, 0f);
			EnablePanel(mapPanel.gameObject);
			SetOnMap();
			chapter2Text.SetVisible(true);
			break;
		}
		CheckBanner();
	}

	private void CheckBanner()
	{
		if (!UnityAdsIngetration.Instance.IsInitialized)
		{
		}
	}

	public void SetOnMain()
	{
		currentMenu = MenuType.Main;
		mapPanel.GetComponent<MenuPanel>().Activate(false);
		mainPanel.GetComponent<MenuPanel>().Activate(true);
		CheckBanner();
	}

	public void SetOnMap()
	{
		currentMenu = MenuType.Map;
		mainPanel.GetComponent<MenuPanel>().Activate(false);
		mapPanel.GetComponent<MenuPanel>().Activate(true);
		EnablePanel(mapPanel.gameObject);
		CheckBanner();
	}

	public void MenuClose()
	{
		PlayClick();
		EnablePanel(mainPanel.gameObject);
		switch (currentMenu)
		{
		case MenuType.Main:
			mainPanel.Play("MenuPanelFadeOut");
			break;
		case MenuType.Achievements:
			achievementsPanel.Play("MenuPanelFadeOut");
			DisablePanel(achievementsPanel.gameObject);
			mainPanel.Play("MenuPanelFadeIn");
			mainPanel.GetComponent<MenuPanel>().Activate(true);
			break;
		case MenuType.Options:
			optionsPanel.Play("MenuPanelFadeOut");
			DisablePanel(optionsPanel.gameObject);
			mainPanel.Play("MenuPanelFadeIn");
			mainPanel.GetComponent<MenuPanel>().Activate(true);
			break;
		case MenuType.Challenges:
			challengePanel.Play("MenuPanelFadeOut");
			DisablePanel(challengePanel.gameObject);
			mainPanel.Play("MenuPanelFadeIn");
			mainPanel.GetComponent<MenuPanel>().Activate(true);
			break;
		case MenuType.Map:
			currentMap.SetOnMap(false, false);
			DisablePanel(mapPanel.gameObject);
			break;
		}
		currentMenu = MenuType.Main;
		chapter2Button.SetChapterText(currentMap == map2);
		chapter2Text.SetChapterText(currentMap == map2);
		chapter2Text.SetVisible(true);
		CheckBanner();
	}

	private void DisablingPanel(Animator panel)
	{
		DisablePanel(panel.gameObject);
	}

	private void ActivatePanel(Animator panel)
	{
		EnablePanel(panel.gameObject);
		panel.GetComponent<MenuPanel>().Activate(true);
		panel.Play("MenuPanelFadeIn");
	}

	public void PlayClick()
	{
		audioS.PlayOneShot(audioS.clip);
	}

	public void Unlock(bool isDebug = false)
	{
		currentMap.Unlock(isDebug);
	}

	public void ResetProgress()
	{
		GameDataController.Delete("LastMap");
		currentMap.ResetProgress();
		currentMap.cameraSpline.ScrollToStart();
		if (currentMap == map2)
		{
			SwitchMap(true, false);
		}
	}

	public void AddBattery()
	{
		currentMap.AddBattery();
	}

	public void Reset()
	{
		currentMap.Reset();
	}
}
