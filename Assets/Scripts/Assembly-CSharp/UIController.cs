using System;
using System.Collections;
using System.Collections.Generic;
using SmartLocalization;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UIController : MonoBehaviour
{
	public enum IngameMenuState
	{
		UnSet = 0,
		None = 1,
		Normal = 2,
		Paused = 3,
		Killed = 4,
		End = 5,
		PopUp = 6,
		GameOver = 7
	}

	private class FriendBeatenNotification
	{
		public float ScoreToBeat;

		public string FriendName;

		public FriendBeatenNotification(float score, string name)
		{
			ScoreToBeat = score;
			FriendName = name;
		}

		public bool ShowNotification(float score)
		{
			if (score > ScoreToBeat)
			{
				return true;
			}
			return false;
		}
	}

	public const int MAX_DESC_CHARS = 60;

	public GameObject NormalDialog;

	public GameObject PauseDialog;

	public GameObject EndDialog;

	public GameObject EndDialogEndless;

	public GameObject EndDialogChallenge;

	public GameObject PopUpDialog;

	public GameObject GameOverDialog;

	public GameObject ChallengeStartInfo;

	public Text LevelName;

	public Text LevelNameShadow;

	public NotificationController notificationController;

	public IngamePopUp popUpController;

	public IngameMenuButtonController buttonController;

	public LifeUI lifeUI;

	public AudioMixerSnapshot normalSnapshot;

	public AudioMixerSnapshot pausedSnapshot;

	public AudioMixerSnapshot killedSnapshot;

	public AudioMixerSnapshot reverseSnapshot;

	public bool leftPressed;

	public bool rightPressed;

	public bool upPressed;

	public bool flipControls;

	public Text timeText;

	public Text totalDistance;

	public Text bestDistance;

	public Image life;

	public ChallengeHitLifes challengeHits;

	public AchievementNotification achievementNotification;

	public Animator trickItemAnimator;

	public Text collectedText;

	public Text collectedBestText;

	public Text tricksText;

	public Text tricksChange;

	public Text tricksMultiplier;

	public Text trickName;

	public GameObject retryButton;

	public Text comboMultiplierText;

	public AudioClip lifeSaverClip;

	public InfoController KillInfo;

	public Text hiddenText;

	public GameObject AdApparatus;

	private int AdForRetriesUsage;

	public MeshRenderer batteryIcon;

	public GameObject boltIcon;

	private CheckPointContoller checkpointController;

	private HoverController hoverController;

	private float dieTimer;

	private float autoRewindTimer;

	private float lastAxisValue;

	private List<FriendBeatenNotification> beatNotifications;

	private int KillCounterOnLevel;

	private int lastScore;

	private bool bestNotified;

	private float endTimer;

	private bool AdRunning;

	public static UIController Instance { get; private set; }

	public int AdForLifeUsage { get; private set; }

	public IngameMenuState menuState { get; private set; }

	private void Awake()
	{
		Instance = this;
		SwitchIngameMenu(Main.Instance ? IngameMenuState.None : IngameMenuState.Normal);
		timeText.transform.parent.gameObject.SetActive(false);
		if (DeviceSettings.Instance != null && DeviceSettings.Instance.RunningOnTV)
		{
			NormalDialog.GetComponent<RectTransform>().localScale = NormalDialog.GetComponent<RectTransform>().localScale * 0.95f;
		}
		retryButton.gameObject.SetActive(false);
		totalDistance.gameObject.SetActive(false);
		checkpointController = UnityEngine.Object.FindObjectOfType<CheckPointContoller>();
		hoverController = UnityEngine.Object.FindObjectOfType<HoverController>();
	}

	private void OnEnable()
	{
		if (Application.isEditor && !Application.isPlaying)
		{
			base.transform.position = new Vector3(0f, 0f, 1000f);
		}
		else
		{
			base.transform.position = Vector3.zero;
		}
	}

	private void SwitchIngameMenu(IngameMenuState newState)
	{
		if (newState == menuState)
		{
			return;
		}
		menuState = newState;
		buttonController.SetMenuButtons(newState);
		switch (newState)
		{
		case IngameMenuState.None:
			NormalDialog.SetActive(false);
			EndDialog.SetActive(false);
			EndDialogEndless.SetActive(false);
			if (EndDialogChallenge != null)
			{
				EndDialogChallenge.SetActive(false);
			}
			GameOverDialog.SetActive(false);
			PopUpDialog.SetActive(false);
			PauseDialog.SetActive(false);
			break;
		case IngameMenuState.PopUp:
			normalSnapshot.TransitionTo(0.1f);
			NormalDialog.SetActive(true);
			PopUpDialog.SetActive(true);
			PauseDialog.SetActive(false);
			EndDialog.SetActive(false);
			EndDialogEndless.SetActive(false);
			if (EndDialogChallenge != null)
			{
				EndDialogChallenge.SetActive(false);
			}
			break;
		case IngameMenuState.End:
			if (SceneLoader.Instance.Current.IsEndless || SceneLoader.Instance.Current.IsChallenge)
			{
				killedSnapshot.TransitionTo(0.75f);
				KillCounterOnLevel++;
				if (UnityAdsIngetration.Instance.IsInitialized)
				{
					if (KillCounterOnLevel > 4 && UnityAdsIngetration.Instance.IsInterstisialsAdReady && UnityEngine.Random.Range(0f, 1f) > Mathf.Min(0.6f, 0.7f - (float)KillCounterOnLevel * 0.012f))
					{
						KillCounterOnLevel = -UnityEngine.Random.Range(-1, 3);
						UnityAdsIngetration.Instance.ShowAdMobAd();
					}
					else
					{
						UnityAdsIngetration.Instance.BannerShow();
					}
				}
			}
			NormalDialog.SetActive(false);
			EndDialog.SetActive(!SceneLoader.Instance.Current.IsEndless && !SceneLoader.Instance.Current.IsChallenge);
			EndDialogEndless.SetActive(SceneLoader.Instance.Current.IsEndless && !SceneLoader.Instance.Current.IsChallenge);
			if (EndDialogChallenge != null)
			{
				EndDialogChallenge.SetActive(SceneLoader.Instance.Current.IsChallenge);
			}
			if (SceneLoader.Instance.Current.IsChallenge && EndDialogChallenge.GetComponent<MenuPanel>() != null && EndDialogChallenge != null)
			{
				EndDialogChallenge.GetComponent<MenuPanel>().Activate(true);
			}
			else if (SceneLoader.Instance.Current.IsEndless && EndDialogEndless.GetComponent<MenuPanel>() != null)
			{
				EndDialogEndless.GetComponent<MenuPanel>().Activate(true);
			}
			else if (!SceneLoader.Instance.Current.IsEndless && EndDialog.GetComponent<MenuPanel>() != null)
			{
				EndDialog.GetComponent<MenuPanel>().Activate(true);
			}
			PauseDialog.SetActive(false);
			PopUpDialog.SetActive(false);
			break;
		case IngameMenuState.Killed:
			killedSnapshot.TransitionTo(0.75f);
			NormalDialog.SetActive(true);
			KillCounterOnLevel++;
			if (!Main.Instance.TutorialLevel && UnityAdsIngetration.Instance.IsInitialized)
			{
				if (KillCounterOnLevel > 1 && UnityAdsIngetration.Instance.IsInterstisialsAdReady && UnityEngine.Random.Range(0f, 1f) > Mathf.Min(0.5f, 0.6f - (float)KillCounterOnLevel * 0.1f))
				{
					KillCounterOnLevel = -UnityEngine.Random.Range(3, 7);
					UnityAdsIngetration.Instance.ShowAdMobAd();
				}
				else
				{
					UnityAdsIngetration.Instance.BannerShow();
				}
			}
			if (!Main.Instance.TutorialLevel)
			{
				KillInfo.ShowInfos();
			}
			break;
		case IngameMenuState.Normal:
			if (UnityAdsIngetration.Instance != null && UnityAdsIngetration.Instance.IsInitialized)
			{
				UnityAdsIngetration.Instance.BannerHide();
			}
			endTimer = 0f;
			normalSnapshot.TransitionTo(0.1f);
			NormalDialog.SetActive(true);
			GameOverDialog.SetActive(false);
			PauseDialog.SetActive(false);
			EndDialog.SetActive(false);
			EndDialogEndless.SetActive(false);
			if (EndDialogChallenge != null)
			{
				EndDialogChallenge.SetActive(false);
			}
			PopUpDialog.SetActive(false);
			if (SceneLoader.Instance != null && SceneLoader.Instance.Current != null && Main.Instance != null)
			{
				lifeUI.gameObject.SetActive(!SceneLoader.Instance.Current.IsEndless && !SceneLoader.Instance.Current.IsChallenge && !Main.Instance.TutorialLevel);
				if (SceneLoader.Instance.Current.IsChallenge && GameController.Instance.State == GameController.GameState.Start)
				{
					ChallengeStartInfo.SetActive(true);
				}
				else if (SceneLoader.Instance.Current.IsChallenge)
				{
					ChallengeStartInfo.SetActive(false);
				}
			}
			break;
		case IngameMenuState.Paused:
		{
			NormalDialog.SetActive(true);
			PauseDialog.SetActive(true);
			if (UnityAdsIngetration.Instance.IsInitialized && Main.Instance != null && !Main.Instance.TutorialLevel)
			{
				UnityAdsIngetration.Instance.BannerShow();
			}
			if (PauseDialog.GetComponent<MenuPanel>() != null)
			{
				MenuPanel[] components = PauseDialog.GetComponents<MenuPanel>();
				for (int i = 0; i < components.Length; i++)
				{
					if ((SceneLoader.Instance.Current.IsEndless || SceneLoader.Instance.Current.IsChallenge) && components[i].Inputs.InMenu == CustomInputController.InputMenu.PauseEndless)
					{
						components[i].Activate(true);
					}
					else if (!SceneLoader.Instance.Current.IsEndless && components[i].Inputs.InMenu == CustomInputController.InputMenu.Pause)
					{
						components[i].Activate(true);
					}
				}
			}
			Text levelName = LevelName;
			string visibleName = SceneLoader.Instance.Current.VisibleName;
			LevelNameShadow.text = visibleName;
			levelName.text = visibleName;
			EndDialog.SetActive(false);
			EndDialogEndless.SetActive(false);
			if (EndDialogChallenge != null)
			{
				EndDialogChallenge.SetActive(false);
			}
			PopUpDialog.SetActive(false);
			retryButton.gameObject.SetActive(false);
			if (ChallengeStartInfo != null)
			{
				ChallengeStartInfo.SetActive(false);
			}
			dieTimer = 1f;
			pausedSnapshot.TransitionTo(0.05f);
			break;
		}
		case IngameMenuState.GameOver:
			if (UnityAdsIngetration.Instance.IsInitialized)
			{
				UnityAdsIngetration.Instance.BannerHide();
			}
			GameOverDialog.SetActive(true);
			if (GameOverDialog.GetComponent<MenuPanel>() != null)
			{
				GameOverDialog.GetComponent<MenuPanel>().Activate(true);
			}
			NormalDialog.SetActive(false);
			EndDialog.SetActive(false);
			EndDialogEndless.SetActive(false);
			if (EndDialogChallenge != null)
			{
				EndDialogChallenge.SetActive(false);
			}
			PauseDialog.SetActive(false);
			PopUpDialog.SetActive(false);
			break;
		}
	}

	private void Update()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (Input.GetMouseButton(0) && PauseDialog != null && !PauseDialog.activeSelf)
		{
			Vector3 vector = Camera.main.ScreenToViewportPoint(Input.mousePosition);
			if (vector.y < 0.85f)
			{
				if (vector.x > 0.5f)
				{
					if (flipControls)
					{
						leftPressed = true;
						rightPressed = false;
					}
					else
					{
						rightPressed = true;
						leftPressed = false;
					}
				}
				else if (flipControls)
				{
					rightPressed = true;
					leftPressed = false;
				}
				else
				{
					leftPressed = true;
					rightPressed = false;
				}
				return;
			}
		}
		rightPressed = (leftPressed = false);
		if (Input.GetButtonUp("Pause"))
		{
			if (Main.Instance.CreditsLevel)
			{
				MenuSwitching();
			}
			else if (PauseDialog != null && PauseDialog.activeSelf)
			{
				UIButtonResume();
			}
			else
			{
				UIButtonPause();
			}
			return;
		}
		if (Input.GetKeyDown(KeyCode.R))
		{
			Reset();
		}
		if (GameController.Instance.State == GameController.GameState.Ending && retryButton.activeSelf && (Input.GetMouseButton(0) || Input.GetButtonDown("Fire") || Input.GetButtonUp("Submit")))
		{
			leftPressed = true;
		}
		else
		{
			if (menuState == IngameMenuState.PopUp || Main.Instance.CreditsLevel || !DeviceSettings.Instance.EnableInputDevices)
			{
				return;
			}
			float axis = Input.GetAxis("Horizontal");
			if (axis < 0f && (axis == -1f || axis < lastAxisValue))
			{
				if (!flipControls)
				{
					leftPressed = true;
				}
				else
				{
					rightPressed = true;
				}
			}
			else if (!flipControls)
			{
				leftPressed = false;
			}
			else
			{
				rightPressed = false;
			}
			if (axis > 0f && (axis == 1f || axis > lastAxisValue))
			{
				if (!flipControls)
				{
					rightPressed = true;
				}
				else
				{
					leftPressed = true;
				}
			}
			else if (!flipControls)
			{
				rightPressed = false;
			}
			else
			{
				leftPressed = false;
			}
			lastAxisValue = axis;
		}
	}

	private void FixedUpdate()
	{
		if (totalDistance.gameObject.activeSelf)
		{
			totalDistance.text = hoverController.walker.TotalDistance.ToString("0000000");
			if (hoverController.walker.TotalDistance > SceneLoader.Instance.Current.Storage.BestDistance)
			{
				if (!bestNotified && SceneLoader.Instance.Current.Storage.BestDistance > 1f)
				{
					bestNotified = true;
					LocalizationLoader.Instance.SetText(notificationController.notificationText, "IngameUI.NewHighScore");
					notificationController.Notify(LanguageManager.Instance.GetTextValue("IngameUI.NewHighScore").ToUpper());
				}
				else if ((bestNotified || SceneLoader.Instance.Current.Storage.BestDistance <= 0f) && beatNotifications != null)
				{
					for (int i = 0; i < beatNotifications.Count; i++)
					{
						if (beatNotifications[i].ShowNotification(hoverController.walker.TotalDistance))
						{
							notificationController.Notify(string.Format("{0} {1}m", beatNotifications[i].FriendName, beatNotifications[i].ScoreToBeat));
							beatNotifications.RemoveAt(i);
							break;
						}
					}
				}
				bestDistance.text = string.Format("{0}", hoverController.walker.TotalDistance.ToString("0000000"));
			}
		}
		else if (SceneLoader.Instance.Current.IsChallenge)
		{
			collectedText.text = string.Format("{0}/{1}", LevelStats.Instance.CollectebleCollectCount, SceneLoader.Instance.GetChallengeLevelLimit(SceneLoader.Instance.Current));
			if (LevelStats.Instance.HighScore() > 0)
			{
				collectedBestText.text = string.Format("{0}", LevelStats.Instance.HighScore());
			}
			else
			{
				collectedBestText.text = string.Empty;
			}
		}
		else
		{
			collectedText.text = string.Format("{0}/{1}", LevelStats.Instance.CollectebleCollectCount, LevelStats.Instance.CollectablesMax());
			if (LevelStats.Instance.HighScore() > 0)
			{
				collectedBestText.text = string.Format("{0}/{1}", LevelStats.Instance.HighScore(), LevelStats.Instance.CollectablesMax());
			}
			else
			{
				collectedBestText.text = string.Empty;
			}
		}
		switch (GameController.Instance.State)
		{
		case GameController.GameState.Ending:
			endTimer += Time.fixedDeltaTime;
			if (endTimer > 1f && !retryButton.activeSelf)
			{
				retryButton.SetActive(true);
			}
			else if (endTimer > 3f)
			{
				GameController.Instance.SetState(GameController.GameState.End);
			}
			break;
		case GameController.GameState.End:
			if (menuState != IngameMenuState.End)
			{
				SwitchIngameMenu(IngameMenuState.End);
			}
			retryButton.gameObject.SetActive(false);
			break;
		case GameController.GameState.Reverse:
			if (PauseDialog.activeSelf)
			{
				Pause();
			}
			retryButton.gameObject.SetActive(false);
			if (dieTimer != 1f)
			{
				reverseSnapshot.TransitionTo(0.25f);
				dieTimer = 1f;
				SwitchIngameMenu(IngameMenuState.Normal);
			}
			break;
		case GameController.GameState.Kill:
			if (dieTimer < 2f)
			{
				if (dieTimer <= 0f)
				{
					SwitchIngameMenu(IngameMenuState.Killed);
					autoRewindTimer = 0f;
				}
				dieTimer += Time.deltaTime;
			}
			else if (dieTimer > 0.5f && LifeController.Instance.LifeCount < 1)
			{
				SwitchIngameMenu(IngameMenuState.GameOver);
			}
			else if (dieTimer > 2f && LifeController.Instance.LifeCount > 0)
			{
				checkpointController.Rewind(false);
			}
			break;
		case GameController.GameState.Start:
			if (SceneLoader.Instance != null && SceneLoader.Instance.Current != null && (SceneLoader.Instance.Current.IsEndless || SceneLoader.Instance.Current.IsChallenge))
			{
				normalSnapshot.TransitionTo(0.05f);
			}
			break;
		case GameController.GameState.Resume:
		case GameController.GameState.Running:
			if (PauseDialog.activeSelf)
			{
				Pause();
			}
			if (ChallengeStartInfo != null && ChallengeStartInfo.activeSelf)
			{
				ChallengeStartInfo.SetActive(false);
			}
			if (!LevelStats.Instance.UpdateProgress && hoverController.walker.Speed > 0f)
			{
				normalSnapshot.TransitionTo(0.05f);
			}
			if (dieTimer > 0f)
			{
				dieTimer -= Time.deltaTime * 0.3f;
			}
			LevelStats.Instance.UpdateProgress = hoverController.walker.Speed > 0f;
			break;
		case GameController.GameState.Paused:
			break;
		}
	}

	public void CollectableAdded()
	{
		if ((!(Main.Instance != null) || !Main.Instance.CreditsLevel) && trickItemAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.1f)
		{
			trickItemAnimator.Play("ScoreItemAdd", -1, 0f);
		}
	}

	public void StartLevel()
	{
		if (UnityAdsIngetration.Instance != null && UnityAdsIngetration.Instance.IsInitialized)
		{
			UnityAdsIngetration.Instance.BannerHide();
		}
		AdForLifeUsage = 0;
		AdForRetriesUsage = 0;
		KillCounterOnLevel = ((SceneLoader.Instance.Current.IsEndless || SceneLoader.Instance.Current.IsChallenge) ? UnityEngine.Random.Range(-2, 4) : ((UnityEngine.Random.value > 0.65f) ? 2 : 0));
		SwitchIngameMenu(IngameMenuState.Normal);
		if (!Main.Instance.TutorialLevel && !Main.Instance.CreditsLevel)
		{
			if (Main.Instance.CurrentScene.Equals("Hover29"))
			{
				Camera.main.gameObject.AddComponent<FakeDollyZoom>().StartZoom(5f, 1000f, 50f);
			}
			else
			{
				Camera.main.gameObject.AddComponent<FakeDollyZoom>().StartZoom();
			}
		}
		if (!Main.Instance.TutorialLevel && !Main.Instance.CreditsLevel)
		{
			hoverController.walker.SetZooming();
		}
		GameObject gameObject = GameObject.Find(Main.Instance.CurrentScene);
		if (gameObject != null)
		{
			Light[] componentsInChildren = gameObject.GetComponentsInChildren<Light>();
			foreach (Light light in componentsInChildren)
			{
				light.enabled = true;
			}
			gameObject.GetComponentInChildren<SceneRenderSettings>().LoadSettings();
			FogTrigger[] componentsInChildren2 = gameObject.GetComponentsInChildren<FogTrigger>();
			foreach (FogTrigger fogTrigger in componentsInChildren2)
			{
				fogTrigger.SetStartFogValues();
			}
		}
		if (!Main.Instance.CreditsLevel)
		{
			totalDistance.gameObject.SetActive(SceneLoader.Instance.Current.IsEndless);
			bestDistance.enabled = SceneLoader.Instance.Current.IsEndless;
			collectedText.transform.parent.gameObject.SetActive(!SceneLoader.Instance.Current.IsEndless);
			if (batteryIcon != null)
			{
				batteryIcon.enabled = !SceneLoader.Instance.Current.IsChallenge || (SceneLoader.Instance.Current.IsChallenge && SceneLoader.Instance.Current.Group != 2);
			}
			if (boltIcon != null)
			{
				boltIcon.SetActive(SceneLoader.Instance.Current.IsChallenge && SceneLoader.Instance.Current.Group == 2);
			}
		}
		SetBestResult();
		GameStats.Instance.GamesPlayed++;
		if (SceneLoader.Instance.Current.IsChallenge)
		{
			GameStats.Instance.ChallengeTries++;
			challengeHits.SetHitLifes(TrickController.Instance.ChallengeCharacter.Hits);
		}
		if (!SceneLoader.Instance.Current.IsEndless || SceneLoader.Instance.Current.IsChallenge || !GameCenter.Instance.LoggedIn)
		{
			return;
		}
		GameCenter.Leaderboard leaderboard = GameCenter.Instance.GetLeaderboard(SceneLoader.Instance.Current.SceneName);
		if (leaderboard == null || leaderboard.MyRank <= 0 || leaderboard.Scores.Count <= 1)
		{
			return;
		}
		beatNotifications = new List<FriendBeatenNotification>();
		foreach (GameCenter.LeaderboardScore score in leaderboard.Scores)
		{
			if (score.Rank < leaderboard.MyRank)
			{
				beatNotifications.Add(new FriendBeatenNotification(score.Score, score.UserID));
			}
		}
	}

	private void SetBestResult()
	{
		if (SceneLoader.Instance.Current.IsEndless)
		{
			bestDistance.text = string.Format("{0}", (!(SceneLoader.Instance.Current.Storage.BestDistance > 0f)) ? "-" : SceneLoader.Instance.Current.Storage.BestDistance.ToString("0000000"));
		}
	}

	public void Life(bool enable)
	{
		life.enabled = enable;
	}

	public void DistanceBoost(int boost)
	{
		if (boost > 0)
		{
			totalDistance.color = ((boost <= 0) ? Color.white : Color.yellow);
		}
		else
		{
			totalDistance.color = Color.white;
		}
	}

	public void Pause()
	{
		if (!(PauseDialog == null))
		{
			if (PauseDialog.activeSelf)
			{
				SwitchIngameMenu(IngameMenuState.Normal);
			}
			else
			{
				SwitchIngameMenu(IngameMenuState.Paused);
			}
		}
	}

	public void UIButtonPopUpButton(int id)
	{
		PlayClick();
		if (popUpController.Type == IngamePopUp.PopUpType.BreakableQuestion)
		{
			SetBrekables(id == 0);
			Main.Instance.ResumeGame();
		}
		SwitchIngameMenu(IngameMenuState.Normal);
	}

	private void SetBrekables(bool enable)
	{
		Monolith[] componentsInChildren = GameController.Instance.transform.GetComponentsInChildren<Monolith>(true);
		foreach (Monolith monolith in componentsInChildren)
		{
			monolith.gameObject.SetActive(enable);
		}
		Collectable[] componentsInChildren2 = GameController.Instance.transform.GetComponentsInChildren<Collectable>(true);
		foreach (Collectable collectable in componentsInChildren2)
		{
			collectable.gameObject.SetActive(!enable);
		}
	}

	public void UIButtonDebugGhost()
	{
		popUpController.ShowGhostQuestion();
		SwitchIngameMenu(IngameMenuState.PopUp);
	}

	public void UIButtonNext()
	{
		PlayClick();
		SceneDetails nextLevel = SceneLoader.Instance.GetNextLevel(Main.Instance.CurrentScene, SceneLoader.Instance.Current.Group);
		if (nextLevel != null && nextLevel.Storage.IsOpen)
		{
			SceneLoader.Instance.LoadLevel(nextLevel.SceneName);
			return;
		}
		Main.Instance.UnlockLevel = true;
		MenuSwitching();
	}

	public void UIButtonToMainMenu()
	{
		PlayClick();
		MenuSwitching();
	}

	private void MenuSwitching()
	{
		if (GameStats.Instance != null)
		{
			GameStats.Instance.SaveStats();
		}
		GameController.GameState state = GameController.Instance.State;
		AudioController.Instance.SwitchMusic((SceneLoader.Instance.Current.Group == 1) ? 3 : 0);
		GameController.Instance.SetState(GameController.GameState.Paused);
		SwitchIngameMenu(IngameMenuState.None);
		LevelStats.Instance.ResetProgress();
		LifeController.Instance.Reset();
		Main.Instance.FromChallengeLevel = SceneLoader.Instance.Current.IsChallenge;
		Main.Instance.SwitchMenu(MenuType.Main);
		Main.Instance.ResumeGame();
		Main.Instance.ClearPopUps();
		bool flag = true;
		if (!Main.Instance.TutorialLevel && !Main.Instance.CreditsLevel && UnityAdsIngetration.Instance.IsInitialized)
		{
			SceneDetails nextLevel = SceneLoader.Instance.GetNextLevel(Main.Instance.CurrentScene, SceneLoader.Instance.Current.Group);
			if (nextLevel != null && UnityEngine.Random.Range(0f, 1f) < ((!nextLevel.Storage.IsOpen) ? 0.25f : 0.75f))
			{
				UnityAdsIngetration.Instance.ShowAdMobAd();
				flag = false;
			}
		}
		if (UnityAdsIngetration.Instance != null && UnityAdsIngetration.Instance.IsInitialized)
		{
			if (flag)
			{
				UnityAdsIngetration.Instance.BannerShow();
			}
			else
			{
				UnityAdsIngetration.Instance.BannerHide();
			}
		}
	}

	public void UIButtonResume()
	{
		PlayClick();
		Main.Instance.ResumeGame();
		SwitchIngameMenu(IngameMenuState.Normal);
	}

	public void UIButtonPause()
	{
		PlayClick();
		Main.Instance.PauseGame();
	}

	public void UIButtonLifeSaver()
	{
		if (LifeController.Instance.LifeSavers <= 0)
		{
			return;
		}
		if (UnityAdsIngetration.Instance.IsAdsActivated)
		{
			if (AdRunning)
			{
				return;
			}
			if (AdApparatus.activeSelf)
			{
				bool flag = false;
				if (UnityAdsIngetration.Instance.IsReady())
				{
					AdRunning = true;
					StartCoroutine(WaitAndPlayAd());
					flag = true;
				}
				else if (UnityAdsIngetration.Instance.IsInterstisialsAdReady)
				{
					UnityAdsIngetration.Instance.ShowAdMobAd();
					GameStats.Instance.GameOvers++;
					LifeController.Instance.UseLifeSaver(true, true);
					Rewind();
					flag = true;
				}
				else
				{
					LocalizationLoader.Instance.SetText(notificationController.notificationText, "MainMenu.ConnectionLost");
					notificationController.Notify(LanguageManager.Instance.GetTextValue("MainMenu.ConnectionLost"));
				}
				if (flag)
				{
					KillCounterOnLevel = -2;
					if (UnityAnalyticsIntegration.Instance != null)
					{
						UnityAnalyticsIntegration.Instance.MakeEvent("Ad_LifeSaver", new Dictionary<string, object> { 
						{
							"scene",
							Main.Instance.CurrentScene
						} });
					}
				}
			}
			else
			{
				AdApparatus.SetActive(true);
			}
		}
		else
		{
			GetComponent<AudioSource>().PlayOneShot(lifeSaverClip);
			GameStats.Instance.GameOvers++;
			LifeController.Instance.UseLifeSaver(true, false);
			Rewind();
		}
	}

	private IEnumerator WaitAndPlayAd()
	{
		yield return new WaitForSeconds(2f);
		UnityAdsIngetration.Instance.ShowSuperSonicAd(AdShown);
	}

	public void AdShownMoreTries(IronSourcePlacement placement)
	{
		IronSourceEvents.onRewardedVideoAdRewardedEvent -= AdShownMoreTries;
		AdRunning = false;
		if (SceneLoader.Instance.Current.IsChallenge)
		{
			SceneLoader.Instance.TriesLeft += SceneLoader.MAX_LIFES;
			string identifier = string.Format("{0}_{1}", DateTime.Today.Year, DateTime.Today.DayOfYear);
			GameDataController.Save(SceneLoader.Instance.TriesLeft, identifier);
		}
		else
		{
			SceneLoader.Instance.Current.Storage.BestTime = SceneLoader.MAX_RETRIES_ENDLESS;
			SceneLoader.Instance.SetAdBlockTimer(SceneLoader.Instance.Current);
		}
		buttonController.AdForRetries.Show(false);
		Rewind();
	}

	public void AdShown(IronSourcePlacement placement)
	{
		IronSourceEvents.onRewardedVideoAdRewardedEvent -= AdShown;
		AdRunning = false;
		GameStats.Instance.GameOvers++;
		LifeController.Instance.UseLifeSaver(true, true);
		Rewind();
	}

	public void UIButtonBoltsForRetries()
	{
		if (GameStats.Instance.ChallengeMoney >= 1000)
		{
			GameStats.Instance.ChallengeMoney -= 1000;
			GameStats.Instance.ChallengeMoneySpent += 1000;
			SceneLoader.Instance.TriesLeft += SceneLoader.MAX_LIFES;
			string identifier = string.Format("{0}_{1}", DateTime.Today.Year, DateTime.Today.DayOfYear);
			GameDataController.Save(SceneLoader.Instance.TriesLeft, identifier);
			buttonController.AdForRetries.Show(false);
			buttonController.BoltsForRetries.Show(false);
			Rewind();
		}
	}

	public void UIButtonAdForLife()
	{
		PlayClick();
		if (UnityAdsIngetration.Instance.IsAdsActivated && UnityAdsIngetration.Instance.IsReady())
		{
			AdForLifeUsage++;
			UnityAdsIngetration.Instance.ShowSuperSonicAd(AdForLifeShown);
			KillCounterOnLevel = -3;
			if (UnityAnalyticsIntegration.Instance != null)
			{
				UnityAnalyticsIntegration.Instance.MakeEvent("Ad_ForLife", new Dictionary<string, object>
				{
					{
						"scene",
						Main.Instance.CurrentScene
					},
					{ "AdForLifeUsage", AdForLifeUsage }
				});
			}
		}
	}

	public void AdForLifeShown(IronSourcePlacement placement)
	{
		LifeController.Instance.ChangeLifes(true);
		lifeUI.UpdateLifeCount();
		IronSourceEvents.onRewardedVideoAdRewardedEvent -= AdForLifeShown;
	}

	public void UIButtonLifeSaverAd()
	{
		if (UnityAdsIngetration.Instance.IsReady())
		{
			UnityAdsIngetration.Instance.ShowSuperSonicAd(AdShown);
			KillCounterOnLevel = -3;
		}
	}

	public void UIButtonRetriesForAd()
	{
		PlayClick();
		if (UnityAdsIngetration.Instance.IsAdsActivated && UnityAdsIngetration.Instance.IsReady())
		{
			AdForRetriesUsage++;
			UnityAdsIngetration.Instance.ShowSuperSonicAd(AdShownMoreTries);
			KillCounterOnLevel = -3;
			if (UnityAnalyticsIntegration.Instance != null)
			{
				UnityAnalyticsIntegration.Instance.MakeEvent("Ad_ForRetries", new Dictionary<string, object>
				{
					{
						"scene",
						Main.Instance.CurrentScene
					},
					{ "AdForRetriesUsage", AdForRetriesUsage }
				});
			}
		}
	}

	public void Reset()
	{
		if (UnityAdsIngetration.Instance != null && UnityAdsIngetration.Instance.IsAdsActivated && SceneLoader.Instance.Current != null && SceneLoader.Instance.Current.IsEndless)
		{
			SceneLoader.Instance.Current.Storage.BestTime -= 1f;
			if (SceneLoader.Instance.Current.Storage.BestTime <= 0f && SceneLoader.Instance.UnlockTimeLeft(SceneLoader.Instance.Current) > 28800.0)
			{
				SceneLoader.Instance.SetAdBlockTimer(SceneLoader.Instance.Current);
			}
		}
		hoverController.animationController.DeleteTrail();
		if (PauseDialog.activeSelf)
		{
			Main.Instance.ResumeGame();
			Pause();
		}
		else
		{
			SwitchIngameMenu(IngameMenuState.Normal);
		}
		SetBestResult();
		bestNotified = false;
		LevelStats.Instance.ResetProgress();
		checkpointController.Reset();
		LifeController.Instance.Reset();
		GameStats.Instance.GamesPlayed++;
		if (SceneLoader.Instance.Current.IsChallenge)
		{
			GameStats.Instance.ChallengeTries++;
			challengeHits.SetHitLifes(TrickController.Instance.ChallengeCharacter.Hits);
		}
	}

	public void Rewind()
	{
		Main.Instance.ResumeGame();
		Pause();
		hoverController.animationController.DeleteTrail();
		checkpointController.Rewind(true);
	}

	private void PlayClick()
	{
		GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
	}

	public void PressLeft(bool enable)
	{
	}

	public void PressRight(bool enable)
	{
	}

	public void PressJump(bool enable)
	{
		if (upPressed != enable)
		{
			upPressed = enable;
		}
	}
}
