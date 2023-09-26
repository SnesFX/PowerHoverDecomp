using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;
using UnityEngine.Audio;

public class Main : MonoBehaviour
{
	public static string SplashScreenName = "Splash";

	public AudioMixerSnapshot normalSnapshot;

	public AudioMixerSnapshot loadingSnapshot;

	private GrayscaleEffect effect;

	private bool IngameActive;

	private MenuType MenuActive;

	private MenuType MenuNext;

	private MenuType MenuAfterLoading;

	private bool makeServiceConnection;

	private Stack<MenuType> PopUpStack;

	private string[] menuScenes = new string[3] { "MenuMain", "MenuPopUp", "MenuLogo" };

	private Dictionary<MenuType, ControllerBase> menuBases;

	private float fakeLoadingTimer;

	private float menuSwitchOffTimer;

	public static Main Instance { get; private set; }

	public bool IsPaused { get; private set; }

	public bool IsGenuine { get; set; }

	public string CurrentScene { get; set; }

	public bool UnlockLevel { get; set; }

	public bool InMenu
	{
		get
		{
			return MenuActive != MenuType.Ingame;
		}
	}

	public bool InCutScene
	{
		get
		{
			return MenuNext == MenuType.CutScene || MenuNext == MenuType.CutSceneEnd;
		}
	}

	public bool FromChallengeLevel { get; set; }

	public bool TestingLevel { get; set; }

	public bool TutorialLevel { get; set; }

	public bool CreditsLevel { get; set; }

	public bool MainMenuLoaded
	{
		get
		{
			return menuBases != null && menuBases.ContainsKey(MenuType.Logo);
		}
	}

	private void Awake()
	{
		Instance = this;
		HOTween.Init(true, true, true);
		GameDataController.Save("winteriscoming", "nonsense");
		PopUpStack = new Stack<MenuType>();
		effect = GetComponent<GrayscaleEffect>();
		IsGenuine = false;
		menuBases = new Dictionary<MenuType, ControllerBase>(menuScenes.Length);
		TutorialLevel = Application.loadedLevelName.Equals("Tutorial");
		CreditsLevel = Application.loadedLevelName.Equals("Credits");
		if (Application.loadedLevelName.Equals("MenuMain"))
		{
			TestingLevel = false;
			IngameActive = false;
			menuBases.Add(MenuType.Main, Object.FindObjectOfType<MainController>());
		}
		else if (!Application.loadedLevelName.Equals(SplashScreenName))
		{
			TestingLevel = true;
			IngameActive = true;
		}
		for (int i = 0; i < menuScenes.Length; i++)
		{
			if (IngameActive || !(menuScenes[i] == "MenuMain"))
			{
				Application.LoadLevelAdditive(menuScenes[i]);
			}
		}
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
		makeServiceConnection = GameDataController.Exists(GameCenter.STORAGE_CONNECT);
	}

	private void Update()
	{
		if (MenuActive != MenuType.Ingame && !makeServiceConnection)
		{
			GameCenter.Instance.Authenticate();
			makeServiceConnection = true;
		}
		if (menuSwitchOffTimer > 0f)
		{
			menuSwitchOffTimer -= Time.deltaTime;
			if (menuSwitchOffTimer <= 0f)
			{
				effect.StartFadeOut();
				if (MenuActive == MenuType.Ingame)
				{
					SceneLoader.Instance.RemoveCurrentLevel();
				}
				else if (MenuActive != MenuType.CutScene)
				{
					menuBases[MenuActive].gameObject.SetActive(false);
				}
				if (MenuNext == MenuType.Ingame)
				{
					UIController.Instance.StartLevel();
				}
				else if (MenuNext != MenuType.CutScene && MenuNext != MenuType.CutSceneEnd)
				{
					menuBases[MenuNext].gameObject.SetActive(true);
				}
				MenuActive = MenuNext;
			}
		}
		else if (fakeLoadingTimer > 0f)
		{
			fakeLoadingTimer -= Time.deltaTime;
			if (fakeLoadingTimer <= 0f)
			{
				SwitchMenu(MenuAfterLoading);
			}
		}
	}

	public void AddMenuBase(ControllerBase menu)
	{
		menuBases.Add(menu.type, menu);
	}

	public ControllerBase ShowPopUp(MenuType menu)
	{
		if (menuBases.ContainsKey(menu))
		{
			if (PopUpStack.Count > 0 && PopUpStack.Peek() == menu)
			{
				return menuBases[menu];
			}
			PopUpStack.Push(menu);
			menuBases[menu].gameObject.SetActive(true);
			return menuBases[menu];
		}
		return null;
	}

	public void ClearPopUps()
	{
		for (int i = 0; i < PopUpStack.Count; i++)
		{
			MenuType menuType = PopUpStack.Pop();
			if (menuType == MenuType.PopUp)
			{
				(menuBases[menuType] as PopUpController).Close();
			}
		}
	}

	public void ClosePopUp()
	{
		if (PopUpStack.Count > 0)
		{
			MenuType menuType = PopUpStack.Pop();
			if (menuType != MenuType.PopUp)
			{
				menuBases[menuType].gameObject.SetActive(false);
			}
		}
	}

	public void HideMenu(MenuType menu)
	{
		if (menuBases.ContainsKey(menu))
		{
			menuBases[menu].gameObject.SetActive(false);
		}
	}

	public void SwapMenu(MenuType menu, bool fromSplash = false)
	{
		if (menuBases.ContainsKey(menu) && menu != MenuType.Ingame)
		{
			effect.ShowBlack();
			IngameActive = false;
			MenuNext = menu;
			if (!fromSplash && MenuActive != MenuType.CutScene)
			{
				menuBases[MenuActive].gameObject.SetActive(false);
			}
			if (MenuNext != MenuType.CutScene && MenuNext != MenuType.CutSceneEnd)
			{
				menuBases[MenuNext].gameObject.SetActive(true);
			}
			MenuActive = MenuNext;
			effect.HideBlack();
		}
	}

	public void SwitchMenu(MenuType menu, bool doLoading = false)
	{
		effect.StartFadeIn();
		menuSwitchOffTimer = 1.8f;
		if (menuBases.ContainsKey(menu))
		{
			IngameActive = false;
			if (doLoading)
			{
				loadingSnapshot.TransitionTo(0.1f);
				fakeLoadingTimer = 2.5f;
				MenuNext = MenuType.Loading;
				MenuAfterLoading = menu;
			}
			else
			{
				if (menu == MenuType.Loading)
				{
					loadingSnapshot.TransitionTo(0.1f);
				}
				else
				{
					normalSnapshot.TransitionTo(0.2f);
				}
				MenuNext = menu;
			}
		}
		else
		{
			switch (menu)
			{
			case MenuType.Ingame:
				IngameActive = true;
				MenuNext = menu;
				normalSnapshot.TransitionTo(0.2f);
				break;
			case MenuType.CutScene:
			case MenuType.CutSceneEnd:
				MenuNext = menu;
				normalSnapshot.TransitionTo(0.2f);
				break;
			}
		}
	}

	public void StartLevel()
	{
		IngameActive = true;
		MenuNext = MenuType.Ingame;
		normalSnapshot.TransitionTo(0.4f);
		menuSwitchOffTimer = 0.1f;
	}

	public void IngameOff()
	{
		IngameActive = false;
	}

	public void FakeFadeIn()
	{
		loadingSnapshot.TransitionTo(0.1f);
		effect.StartFadeIn();
	}

	public void FakeFadeOut()
	{
		normalSnapshot.TransitionTo(0.2f);
		effect.StartFadeOut();
	}

	public void FakeFadeing(float waitTime = 0.3f)
	{
		StartCoroutine(FadeInOut(waitTime));
	}

	private IEnumerator FadeInOut(float waitTime = 0.3f)
	{
		effect.StartFadeIn();
		yield return new WaitForSeconds(waitTime);
		effect.StartFadeOut();
		StopCoroutine("FadeInOut");
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			PauseGame();
		}
		if (AudioController.Instance != null)
		{
			AudioController.Instance.SwitchState(pause);
		}
	}

	public void PauseGame()
	{
		if (!IsPaused && IngameActive && (!(GameController.Instance != null) || GameController.Instance.State == GameController.GameState.Running || GameController.Instance.State == GameController.GameState.Start))
		{
			GameController.Instance.SetState(GameController.GameState.Paused);
			IsPaused = true;
			Time.timeScale = 0f;
			UIController.Instance.Pause();
		}
	}

	public void ResumeGame(bool fromMenu = false)
	{
		if (IsPaused)
		{
			if (fromMenu)
			{
				menuBases[MenuActive].gameObject.SetActive(false);
			}
			IsPaused = false;
			Time.timeScale = 1f;
			GameController.Instance.ResumeLastState();
		}
	}
}
