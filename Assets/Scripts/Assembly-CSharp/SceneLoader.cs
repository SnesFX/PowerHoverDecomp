using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
	private enum LoadingState
	{
		Off = 0,
		Starting = 1,
		Loading = 2,
		FakeLoading = 3
	}

	public static int MAX_RETRIES_ENDLESS = 25;

	public static int MAX_LIFES = 25;

	public const string TUTORIAL_SCENE = "Tutorial";

	public const string CREDITS_SCENE = "Credits";

	public const string STORAGE_PREFIX = "SCOREZ_";

	private const float MIN_LOADING_TIME = 1.6f;

	public List<string> Groups = new List<string>();

	public List<SceneDetails> Scenes = new List<SceneDetails>();

	public List<SceneDetails> Challenges = new List<SceneDetails>();

	private LoadingState loadingState;

	private float fakeLoadingTimer;

	private SceneDetails Tutorial;

	private SceneDetails Credits;

	public static SceneLoader Instance { get; private set; }

	public SceneDetails Current { get; private set; }

	public bool IsLoading
	{
		get
		{
			return loadingState != LoadingState.Off;
		}
	}

	public bool ManualStartTutorial { get; private set; }

	public int TriesLeft { get; set; }

	private void Awake()
	{
		Instance = this;
		LoadSceneStorages();
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
		Tutorial = new SceneDetails();
		Tutorial.SceneName = (Tutorial.VisibleName = "Tutorial");
		Tutorial.Group = -1;
		Tutorial.GhostFile = string.Empty;
		Tutorial.Storage = GetDefaultStorage(Tutorial, true);
		Credits = new SceneDetails();
		Credits.SceneName = (Credits.VisibleName = "Credits");
		Credits.Group = -1;
		Credits.SceneMusic = AudioController.MusicType.Slow;
		Credits.GhostFile = string.Empty;
		Credits.Storage = GetDefaultStorage(Credits, true);
		if (!Application.loadedLevelName.Equals("MenuMain"))
		{
			SceneDetails sceneDetails = (Main.Instance.TutorialLevel ? Tutorial : (Main.Instance.CreditsLevel ? Credits : ((Scenes.Find((SceneDetails x) => x.SceneName == Application.loadedLevelName) != null) ? Scenes.Find((SceneDetails x) => x.SceneName == Application.loadedLevelName) : GetDailyChallenge(Application.loadedLevelName))));
			if (sceneDetails != null)
			{
				Main.Instance.CurrentScene = sceneDetails.SceneName;
				Current = sceneDetails;
			}
		}
		CheckBoss5000();
	}

	private void Update()
	{
		switch (loadingState)
		{
		case LoadingState.Off:
			break;
		case LoadingState.Starting:
			fakeLoadingTimer += Time.deltaTime;
			if (fakeLoadingTimer > 1.1f)
			{
				Main.Instance.HideMenu(MenuType.Main);
				loadingState = LoadingState.Loading;
				fakeLoadingTimer = 0f;
				LevelStats.Instance.StartProgress(Current.Storage);
				if (Current.IsEndless && UnlockTimeLeft(Current) > 28800.0)
				{
					Current.Storage.BestTime = MAX_RETRIES_ENDLESS;
				}
				Application.LoadLevelAdditive(Current.SceneName);
				AudioController.Instance.SwitchMusic((int)Current.SceneMusic);
			}
			break;
		case LoadingState.FakeLoading:
			fakeLoadingTimer -= Time.deltaTime;
			if (fakeLoadingTimer <= 0f)
			{
				loadingState = LoadingState.Off;
				Main.Instance.StartLevel();
			}
			break;
		case LoadingState.Loading:
		{
			fakeLoadingTimer += Time.deltaTime;
			GameObject gameObject = GameObject.Find(Current.SceneName);
			if (!(gameObject != null))
			{
				break;
			}
			Light[] componentsInChildren = gameObject.GetComponentsInChildren<Light>();
			foreach (Light light in componentsInChildren)
			{
				light.enabled = false;
			}
			Camera[] componentsInChildren2 = gameObject.transform.GetComponentsInChildren<Camera>(true);
			foreach (Camera camera in componentsInChildren2)
			{
				camera.cullingMask &= ~(1 << LayerMask.NameToLayer("PopUp"));
				if (camera.gameObject.layer != LayerMask.NameToLayer("UI"))
				{
					camera.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
				}
			}
			AudioSource[] componentsInChildren3 = gameObject.transform.GetComponentsInChildren<AudioSource>(true);
			foreach (AudioSource audioSource in componentsInChildren3)
			{
				audioSource.outputAudioMixerGroup = AudioController.Instance.SFXMixerGroup;
			}
			Monolith[] componentsInChildren4 = gameObject.transform.GetComponentsInChildren<Monolith>(true);
			foreach (Monolith monolith in componentsInChildren4)
			{
				monolith.gameObject.SetActive(false);
			}
			if (Current.GhostFile.Equals(string.Empty))
			{
				int num = 0;
				Collectable[] componentsInChildren5 = gameObject.transform.GetComponentsInChildren<Collectable>(true);
				foreach (Collectable collectable in componentsInChildren5)
				{
					if (collectable.Type == Collectable.CollectableType.Normal)
					{
						num++;
					}
				}
				LevelStats.Instance.SetItemCount(num);
			}
			fakeLoadingTimer = 1.6f - fakeLoadingTimer;
			if (UnityAnalyticsIntegration.Instance != null)
			{
				UnityAnalyticsIntegration.Instance.MakeEvent("LevelStarted", new Dictionary<string, object>
				{
					{
						"stage",
						Main.Instance.CurrentScene
					},
					{
						"score",
						Current.Storage.HighScore
					}
				});
			}
			loadingState = LoadingState.FakeLoading;
			break;
		}
		}
	}

	public void LoadCredits()
	{
		LoadLevel("Credits");
	}

	public void LoadTutorial(bool manualStart = false)
	{
		ManualStartTutorial = manualStart;
		Main.Instance.TutorialLevel = true;
		LoadLevel("Tutorial");
	}

	public void LoadLevel(string sceneName)
	{
		Main.Instance.TutorialLevel = sceneName.Equals("Tutorial");
		Main.Instance.CreditsLevel = sceneName.Equals("Credits");
		SceneDetails sceneDetails = (Main.Instance.TutorialLevel ? Tutorial : ((!Main.Instance.CreditsLevel) ? Scenes.Find((SceneDetails x) => x.SceneName == sceneName) : Credits));
		if (sceneDetails != null)
		{
			LoadLevel(sceneDetails);
		}
	}

	public void LoadChallenge(string challege)
	{
		Main.Instance.TutorialLevel = false;
		Main.Instance.CreditsLevel = false;
		SceneDetails dailyChallenge = GetDailyChallenge(challege);
		if (dailyChallenge == null)
		{
			Debug.Log("cannot load challenge " + challege);
		}
		else
		{
			LoadLevel(dailyChallenge);
		}
	}

	private void LoadLevel(SceneDetails level)
	{
		Main.Instance.ClearPopUps();
		Main.Instance.FakeFadeIn();
		RemoveCurrentLevel();
		Current = level;
		Main.Instance.CurrentScene = level.SceneName;
		if (!Main.Instance.TutorialLevel && !Main.Instance.CreditsLevel)
		{
			GameDataController.Save(Current.SceneName, "LastScene");
		}
		if (level.SceneName.Equals(GetFirstLevelSceneName(0)))
		{
			GameDataController.Save(true, "FirstLevelStarted");
		}
		loadingState = LoadingState.Starting;
		fakeLoadingTimer = 0f;
	}

	public void RemoveCurrentLevel()
	{
		if (Current != null)
		{
			GameObject gameObject = GameObject.Find(Current.SceneName);
			if (gameObject != null)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}
	}

	public bool IsChapter2Unlocked()
	{
		List<SceneDetails> groupScenes = GetGroupScenes(0);
		List<SceneDetails> groupScenes2 = GetGroupScenes(1);
		return groupScenes[groupScenes.Count - 1].Storage.BestDistance > 1000f || groupScenes2[0].Storage.IsOpen;
	}

	public List<SceneDetails> GetGroupScenes(int group)
	{
		return Scenes.FindAll((SceneDetails x) => x.Group == group);
	}

	public int GetChallengeLevelLimit(SceneDetails sd)
	{
		switch ((ChallengeType)sd.Group)
		{
		case ChallengeType.DontFall:
			return 250 + sd.Storage.CasetteState * 250;
		case ChallengeType.Distance:
			return 750 + sd.Storage.CasetteState * Mathf.Max(100, 500 - Mathf.CeilToInt((float)sd.Storage.CasetteState * 0.25f) * 50);
		case ChallengeType.Collect:
			return 20 + sd.Storage.CasetteState * 5;
		case ChallengeType.DontMiss:
			return 10 + sd.Storage.CasetteState * 5;
		default:
			return 0;
		}
	}

	public int GetChallengeLevelLimit(string sceneName)
	{
		SceneDetails dailyChallenge = GetDailyChallenge(sceneName);
		if (dailyChallenge == null)
		{
			return 0;
		}
		return GetChallengeLevelLimit(dailyChallenge);
	}

	public SceneDetails UnlockNextLevel(string currentSceneName, int group)
	{
		SceneDetails nextLevel = GetNextLevel(currentSceneName, group);
		if (nextLevel != null && !nextLevel.Storage.IsOpen)
		{
			UnlockLevel(nextLevel);
			return nextLevel;
		}
		return null;
	}

	public void UnlockAllLevels()
	{
		foreach (SceneDetails scene in Scenes)
		{
			UnlockLevel(scene);
		}
	}

	public void UnlockLevel(SceneDetails sd)
	{
		sd.Storage.IsOpen = true;
		SetAdBlockTimer(sd);
		SaveSceneStorage(sd.SceneName, sd.Storage);
	}

	public double UnlockTimeLeft(SceneDetails sceneDetails)
	{
		string identifier = string.Format("t_{0}", sceneDetails.SceneName);
		if (GameDataController.Exists(identifier))
		{
			long dateData = Convert.ToInt64(GameDataController.Load<string>(identifier));
			DateTime dateTime = DateTime.FromBinary(dateData);
			DateTime now = DateTime.Now;
			return (now - dateTime).TotalSeconds;
		}
		return 0.0;
	}

	public void SetAdBlockTimer(SceneDetails sd)
	{
		if (sd.AdBlock || sd.IsEndless)
		{
			GameDataController.Save(DateTime.Now.ToBinary().ToString(), string.Format("t_{0}", sd.SceneName));
		}
	}

	public SceneDetails GetNextLevel(string current, int group)
	{
		List<SceneDetails> groupScenes = GetGroupScenes(group);
		int num = groupScenes.FindIndex((SceneDetails x) => x.SceneName == current);
		if (num > -1 && num < groupScenes.Count - 1)
		{
			return groupScenes[num + 1];
		}
		return null;
	}

	public SceneDetails GetFirstLevel(int group)
	{
		List<SceneDetails> groupScenes = GetGroupScenes(group);
		return (groupScenes == null) ? null : groupScenes[0];
	}

	public string GetFirstLevelSceneName(int group)
	{
		List<SceneDetails> groupScenes = GetGroupScenes(group);
		return (groupScenes == null) ? string.Empty : groupScenes[0].SceneName;
	}

	public SceneDetails GetFirstLockedLevel(int group)
	{
		List<SceneDetails> groupScenes = GetGroupScenes(group);
		foreach (SceneDetails item in groupScenes)
		{
			if (!item.Storage.IsOpen)
			{
				return item;
			}
		}
		return Scenes[1];
	}

	public SceneDetails GetPreviousLevel(string current, int group)
	{
		List<SceneDetails> groupScenes = GetGroupScenes(group);
		int num = groupScenes.FindIndex((SceneDetails x) => x.SceneName == current);
		if (num > 0)
		{
			return groupScenes[num - 1];
		}
		return null;
	}

	public bool IsPreviousCompleted(string current)
	{
		int num = Scenes.FindIndex((SceneDetails x) => x.SceneName == current);
		if (num > 0)
		{
			return Scenes[num - 1].Storage.IsOpen && Scenes[num - 1].Storage.HighScore > 0f;
		}
		return false;
	}

	public void SaveSceneStorage(string sceneName, SceneStorage storage)
	{
		SceneDetails sceneDetails = Scenes.Find((SceneDetails x) => x.SceneName == sceneName);
		if (sceneDetails == null)
		{
		}
		if (sceneDetails.IsEndless && !sceneDetails.IsChallenge)
		{
			CheckBoss5000();
		}
		sceneDetails.Storage = storage;
		string identifier = string.Format("{0}{1}", "SCOREZ_", sceneName);
		GameDataController.Save(sceneDetails.Storage, identifier);
	}

	public void SaveChallenge(string sceneName, SceneStorage storage)
	{
		SceneDetails sceneDetails = Challenges.Find((SceneDetails x) => x.SceneName == sceneName);
		if (sceneDetails == null)
		{
		}
		sceneDetails.Storage = storage;
		string identifier = string.Format("{0}{1}", "SCOREZ_", sceneName);
		GameDataController.Save(sceneDetails.Storage, identifier);
		string identifier2 = string.Format("{0}_{1}", DateTime.Today.Year, DateTime.Today.DayOfYear);
		GameDataController.Save(TriesLeft, identifier2);
	}

	private void CheckBoss5000()
	{
		int num = 0;
		foreach (SceneDetails scene in Scenes)
		{
			if (!scene.IsChallenge && scene.IsEndless && scene.Storage.BestDistance >= 5000f)
			{
				num++;
			}
		}
		GameStats.Instance.Bosses5000 = num;
	}

	public int LoadTriesLeft()
	{
		string identifier = string.Format("{0}_{1}", DateTime.Today.Year, DateTime.Today.DayOfYear);
		if (GameDataController.Exists(identifier))
		{
			TriesLeft = GameDataController.Load<int>(identifier);
		}
		else
		{
			identifier = string.Format("{0}_{1}", DateTime.Today.Year, DateTime.Today.DayOfYear - 1);
			int a = 0;
			if (GameDataController.Exists(identifier))
			{
				a = GameDataController.Load<int>(identifier);
			}
			TriesLeft = Mathf.Max(a, MAX_LIFES);
		}
		return TriesLeft;
	}

	private SceneStorage GetDefaultStorage(SceneDetails sd, bool firstLevel)
	{
		SceneStorage sceneStorage = new SceneStorage();
		sceneStorage.IsOpen = firstLevel;
		sceneStorage.CollectableLetters = new List<int> { 0, 0, 0, 0, 0 };
		sceneStorage.CasetteState = (sd.HasCasette ? 1 : 0);
		sceneStorage.KillCount = -1;
		sceneStorage.BestTime = MAX_RETRIES_ENDLESS;
		return sceneStorage;
	}

	private void LoadSceneStorages()
	{
		bool firstLevel = true;
		bool flag = false;
		if (!GameDataController.Exists("eTimesSet"))
		{
			flag = true;
			GameDataController.Save(1, "eTimesSet");
		}
		foreach (SceneDetails scene in Scenes)
		{
			string identifier = string.Format("{0}{1}", "SCOREZ_", scene.SceneName);
			if (GameDataController.Exists(identifier))
			{
				scene.Storage = GameDataController.Load<SceneStorage>(identifier);
				if (flag)
				{
					scene.Storage.BestTime = MAX_RETRIES_ENDLESS;
				}
			}
			else
			{
				scene.Storage = GetDefaultStorage(scene, firstLevel);
			}
			firstLevel = false;
		}
		firstLevel = true;
		foreach (SceneDetails challenge in Challenges)
		{
			string identifier2 = string.Format("{0}{1}", "SCOREZ_", challenge.SceneName);
			if (GameDataController.Exists(identifier2))
			{
				challenge.Storage = GameDataController.Load<SceneStorage>(identifier2);
			}
			else
			{
				challenge.Storage = GetDefaultStorage(challenge, firstLevel);
				challenge.Storage.KillCount = 0;
			}
			firstLevel = false;
			challenge.IsChallenge = true;
			challenge.IsEndless = challenge.Group == 1 || challenge.Group == 3;
			challenge.VisibleName = challenge.SceneName;
		}
		LoadTriesLeft();
	}

	public void ResetStorages()
	{
		bool firstLevel = true;
		foreach (SceneDetails scene in Scenes)
		{
			string identifier = string.Format("{0}{1}", "SCOREZ_", scene.SceneName);
			if (GameDataController.Exists(identifier))
			{
				GameDataController.Delete(identifier);
				scene.Storage = GetDefaultStorage(scene, firstLevel);
			}
			firstLevel = false;
		}
		firstLevel = true;
		foreach (SceneDetails challenge in Challenges)
		{
			string identifier2 = string.Format("{0}{1}", "SCOREZ_", challenge.SceneName);
			if (GameDataController.Exists(identifier2))
			{
				GameDataController.Delete(identifier2);
				challenge.Storage = GetDefaultStorage(challenge, firstLevel);
				challenge.Storage.KillCount = 0;
			}
			challenge.IsChallenge = true;
			challenge.IsEndless = challenge.Group == 1;
			challenge.VisibleName = challenge.SceneName;
			firstLevel = false;
		}
	}

	public List<Mission> GetMissions(string SceneName)
	{
		SceneDetails sceneDetails = Scenes.Find((SceneDetails r) => r.SceneName == SceneName);
		if (sceneDetails != null)
		{
			return sceneDetails.Missions;
		}
		return null;
	}

	public SceneDetails GetSceneDetails(string sceneName)
	{
		SceneDetails sceneDetails = Scenes.Find((SceneDetails x) => x.SceneName == sceneName);
		if (sceneDetails == null)
		{
			sceneDetails = GetDailyChallenge(sceneName);
			if (sceneDetails != null)
			{
				return sceneDetails;
			}
			return null;
		}
		return sceneDetails;
	}

	public SceneDetails GetDailyChallenge(string sceneName)
	{
		SceneDetails sceneDetails = Challenges.Find((SceneDetails x) => x.SceneName == sceneName);
		if (sceneDetails == null)
		{
			return null;
		}
		return sceneDetails;
	}

	public SceneDetails GetNextDiamondLockScene()
	{
		SceneDetails sceneDetails = null;
		foreach (SceneDetails item in Scenes.FindAll((SceneDetails x) => !x.Storage.IsOpen && x.StarLockCount > 0))
		{
			if (sceneDetails == null)
			{
				sceneDetails = item;
			}
			else if (item.StarLockCount < sceneDetails.StarLockCount)
			{
				sceneDetails = item;
			}
		}
		return sceneDetails;
	}
}
