using System;
using System.Collections.Generic;
using MiniJSON;
using SA.Common.Pattern;
using UnityEngine;

public class GPSSaveGame : MonoBehaviour
{
	private const bool DEBUG = false;

	private static string SCENE_DATA = "SceneData";

	private static string MAP_OBJECTS = "MapObjects";

	private static string GAME_STATS = "GameStats";

	private static string LAST_SCENE = "LastScene";

	private static string CHALLENGE_CHARACTERS = "ChallengeCharacters";

	public Texture2D defaultImage;

	private MapObject[] mapObjects;

	private Dictionary<string, object> mapData;

	private Dictionary<string, object> characterData;

	private bool cloudInitialized;

	private GPSSavedGamesButton saveButton;

	private bool autoLoad;

	private bool autoSave;

	public static GPSSaveGame Instance { get; private set; }

	public bool SaveFound { get; private set; }

	public string GetSaveData()
	{
		string empty = string.Empty;
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
		foreach (SceneDetails scene in SceneLoader.Instance.Scenes)
		{
			dictionary2.Add(scene.SceneName, scene.Storage.ToJson());
		}
		foreach (SceneDetails challenge in SceneLoader.Instance.Challenges)
		{
			dictionary2.Add(challenge.SceneName, challenge.Storage.ToJson());
		}
		dictionary.Add(SCENE_DATA, dictionary2);
		Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
		if (mapObjects == null)
		{
			mapObjects = UnityEngine.Object.FindObjectsOfType<MapObject>();
		}
		MapObject[] array = mapObjects;
		foreach (MapObject mapObject in array)
		{
			dictionary3.Add(mapObject.ObjectIdentifier, mapObject.Unlocked);
		}
		dictionary.Add(MAP_OBJECTS, dictionary3);
		dictionary.Add(GAME_STATS, GameStats.Instance.ToJson());
		dictionary.Add(LAST_SCENE, (!GameDataController.Exists("LastScene")) ? "saved" : GameDataController.Load<string>("LastScene"));
		ChallengeCharacterPanel instance = ChallengeCharacterPanel.Instance;
		Dictionary<string, object> dictionary4 = new Dictionary<string, object>();
		if (instance != null)
		{
			CharacterMenuItem[] characters = instance.Characters;
			foreach (CharacterMenuItem characterMenuItem in characters)
			{
				dictionary4.Add(characterMenuItem.Character.CharacterName, characterMenuItem.Character.IsLocked);
			}
		}
		dictionary.Add(CHALLENGE_CHARACTERS, dictionary4);
		empty = Json.Serialize(dictionary);
		dictionary.Clear();
		dictionary3.Clear();
		dictionary2.Clear();
		dictionary4.Clear();
		return empty;
	}

	private void ReadSaveData(string saveData)
	{
		Dictionary<string, object> dictionary = Json.Deserialize(saveData) as Dictionary<string, object>;
		Dictionary<string, object> dictionary2 = dictionary[SCENE_DATA] as Dictionary<string, object>;
		foreach (SceneDetails scene in SceneLoader.Instance.Scenes)
		{
			if (dictionary2.ContainsKey(scene.SceneName))
			{
				scene.Storage.FromJson(dictionary2[scene.SceneName] as Dictionary<string, object>);
				SceneLoader.Instance.SaveSceneStorage(scene.SceneName, scene.Storage);
			}
		}
		foreach (SceneDetails challenge in SceneLoader.Instance.Challenges)
		{
			if (dictionary2.ContainsKey(challenge.SceneName))
			{
				challenge.Storage.FromJson(dictionary2[challenge.SceneName] as Dictionary<string, object>);
				SceneLoader.Instance.SaveChallenge(challenge.SceneName, challenge.Storage);
			}
		}
		if (UnityEngine.Object.FindObjectOfType<LevelSelectionPathBuilder>() != null)
		{
			UnityEngine.Object.FindObjectOfType<LevelSelectionPathBuilder>().UpdateBossLimits();
		}
		if (dictionary.ContainsKey(CHALLENGE_CHARACTERS))
		{
			characterData = dictionary[CHALLENGE_CHARACTERS] as Dictionary<string, object>;
		}
		else
		{
			characterData = new Dictionary<string, object>();
		}
		mapData = dictionary[MAP_OBJECTS] as Dictionary<string, object>;
		if (mapData.Count > 0)
		{
			MainController mainController = UnityEngine.Object.FindObjectOfType<MainController>();
			if (mainController != null)
			{
				mapObjects = mainController.map.GetMapObjects();
				if (mapObjects != null)
				{
					SyncMapData(mapObjects);
				}
			}
		}
		if (dictionary.ContainsKey(GAME_STATS) && GameStats.Instance != null)
		{
			GameStats.Instance.FromJson(dictionary[GAME_STATS] as Dictionary<string, object>);
		}
		if (dictionary.ContainsKey(LAST_SCENE))
		{
			GameDataController.Save(dictionary[LAST_SCENE] as string, LAST_SCENE);
		}
	}

	private GPSSavedGamesButton GetButton()
	{
		if (saveButton == null)
		{
			saveButton = UnityEngine.Object.FindObjectOfType<GPSSavedGamesButton>();
		}
		return saveButton;
	}

	public void SyncMapData(MapObject[] objects)
	{
		mapObjects = objects;
		if (characterData != null && characterData.Count > 0)
		{
			ChallengeCharacterPanel instance = ChallengeCharacterPanel.Instance;
			if (instance != null)
			{
				CharacterMenuItem[] characters = instance.Characters;
				foreach (CharacterMenuItem characterMenuItem in characters)
				{
					if (characterData.ContainsKey(characterMenuItem.Character.CharacterName) && !(bool)characterData[characterMenuItem.Character.CharacterName])
					{
						characterMenuItem.Character.IsLocked = false;
					}
				}
				characterData.Clear();
			}
		}
		if (mapObjects.Length == 0 || mapData == null || mapData.Count <= 0)
		{
			return;
		}
		int num = 0;
		MapObject[] array = mapObjects;
		foreach (MapObject mapObject in array)
		{
			foreach (string key in mapData.Keys)
			{
				if (mapObject.IsObject(key) && (bool)mapData[key])
				{
					mapObject.Unlock(false, true);
					if (mapObject.StatToGet == PlayerStatType.Character)
					{
						num++;
					}
				}
			}
		}
		if (num > 0)
		{
			CharacterPanel characterPanel = UnityEngine.Object.FindObjectOfType<CharacterPanel>();
			if (characterPanel != null)
			{
				for (int k = 0; k < num; k++)
				{
					characterPanel.Unlock();
				}
			}
		}
		mapData.Clear();
	}

	private void Awake()
	{
		Instance = this;
		GooglePlaySavedGamesManager.ActionNewGameSaveRequest += ActionNewGameSaveRequest;
		GooglePlaySavedGamesManager.ActionGameSaveLoaded += ActionGameSaveLoaded;
		GooglePlaySavedGamesManager.ActionConflict += ActionConflict;
	}

	private void OnDestroy()
	{
		GooglePlaySavedGamesManager.ActionNewGameSaveRequest -= ActionNewGameSaveRequest;
		GooglePlaySavedGamesManager.ActionGameSaveLoaded -= ActionGameSaveLoaded;
		GooglePlaySavedGamesManager.ActionConflict -= ActionConflict;
	}

	private bool iCloudDisabled()
	{
		string identifier = string.Format("{0}{1}", "GameSetti_", "3");
		if (!GameDataController.Exists(identifier) || (GameDataController.Exists(identifier) && GameDataController.Load<float>(identifier) == 1f))
		{
			return false;
		}
		return true;
	}

	public void LoadSavedGames()
	{
		if (mapObjects == null)
		{
			mapObjects = UnityEngine.Object.FindObjectsOfType<MapObject>();
		}
		autoLoad = true;
		GooglePlaySavedGamesManager.ActionAvailableGameSavesLoaded += ActionAvailableGameSavesLoaded;
		Singleton<GooglePlaySavedGamesManager>.Instance.LoadAvailableSavedGames();
	}

	private void ActionAvailableGameSavesLoaded(GooglePlayResult res)
	{
		GooglePlaySavedGamesManager.ActionAvailableGameSavesLoaded -= ActionAvailableGameSavesLoaded;
		if (res.IsSucceeded && Singleton<GooglePlaySavedGamesManager>.Instance.AvailableGameSaves.Count > 0)
		{
			SaveFound = true;
			GP_SnapshotMeta gP_SnapshotMeta = Singleton<GooglePlaySavedGamesManager>.Instance.AvailableGameSaves[0];
			Singleton<GooglePlaySavedGamesManager>.Instance.LoadSpanshotByName(gP_SnapshotMeta.Title);
		}
	}

	public void SaveGame()
	{
		autoSave = true;
		ActionNewGameSaveRequest();
	}

	public void ActionNewGameSaveRequest()
	{
		long playedTime = GameStats.Instance.GetPlayedTime();
		string arg = "Save";
		if (SceneLoader.Instance != null)
		{
			SceneDetails firstLockedLevel = SceneLoader.Instance.GetFirstLockedLevel(SceneLoader.Instance.IsChapter2Unlocked() ? 1 : 0);
			if (firstLockedLevel != null)
			{
				firstLockedLevel = SceneLoader.Instance.GetPreviousLevel(firstLockedLevel.SceneName, firstLockedLevel.Group);
				if (firstLockedLevel != null)
				{
					arg = firstLockedLevel.VisibleName;
				}
			}
		}
		string text = "Saved-Game";
		string description = string.Format("{0} - {1}", arg, DateTime.Now.ToString("MM/dd/yyyy"));
		string saveData = GetSaveData();
		Texture2D coverImage = defaultImage;
		GooglePlaySavedGamesManager.ActionGameSaveResult += ActionGameSaveResult;
		Singleton<GooglePlaySavedGamesManager>.Instance.CreateNewSnapshot(text, description, coverImage, saveData, playedTime);
	}

	private void ActionGameSaveResult(GP_SpanshotLoadResult result)
	{
		GooglePlaySavedGamesManager.ActionGameSaveResult -= ActionGameSaveResult;
		if (result.IsSucceeded)
		{
		}
		autoSave = false;
	}

	private void ActionGameSaveLoaded(GP_SpanshotLoadResult result)
	{
		if (result.IsSucceeded)
		{
			ReadSaveData(result.Snapshot.stringData);
			if (!autoLoad)
			{
				GetButton().ShowInfoLoaded();
			}
			if (!autoLoad)
			{
				LevelSelectionPathBuilder levelSelectionPathBuilder = UnityEngine.Object.FindObjectOfType<LevelSelectionPathBuilder>();
				if (levelSelectionPathBuilder != null)
				{
					MapLevelSelector[] componentsInChildren = levelSelectionPathBuilder.GetComponentsInChildren<MapLevelSelector>();
					foreach (MapLevelSelector mapLevelSelector in componentsInChildren)
					{
						mapLevelSelector.UpdateState();
					}
				}
			}
		}
		autoLoad = false;
	}

	private void ActionConflict(GP_SnapshotConflict result)
	{
		GP_Snapshot snapshot = result.Snapshot;
		GP_Snapshot conflictingSnapshot = result.ConflictingSnapshot;
		GP_Snapshot snapshot2 = snapshot;
		if (snapshot.meta.TotalPlayedTime < conflictingSnapshot.meta.TotalPlayedTime)
		{
			snapshot2 = conflictingSnapshot;
		}
		result.Resolve(snapshot2);
	}

	private void makeTests(string returnData)
	{
		Dictionary<string, object> dictionary = Json.Deserialize(returnData) as Dictionary<string, object>;
		Debug.Log("test1 : " + dictionary[SCENE_DATA]);
		Debug.Log("test2 : " + dictionary[MAP_OBJECTS]);
		Debug.Log("test3 : " + dictionary[GAME_STATS]);
		Debug.Log("test4 : " + dictionary[CHALLENGE_CHARACTERS]);
	}
}
