using System;
using System.Collections.Generic;
using SmartLocalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class Map : MonoBehaviour
{
	private const float DEFAULT_ZOOM = 5f;

	public Camera roadCamera;

	public Camera roadUICamera;

	public LayerMask MapObjectLayer;

	public CurvySpline roadSpline;

	public MapPopUp mapPopUp;

	public CharacterPanel characterPanel;

	public Text townTotalEnergy;

	public MapCameraSplineWalker cameraSpline;

	public InfoBox energyOver200InfoBox;

	public InfoBox notEnoughEnergy;

	public InfoBox firstEnergyUnlock;

	public int totalUnlockedEnergy;

	public GameObject mapStatBlocker;

	public InteractiveObjects ActiveObjects;

	private float orthographicTarget;

	private bool onMap;

	private MapObject[] mapObjects;

	private int unlockedCounter;

	private MapEnergyIcon selectedItem;

	private MapLevelSelector lvlSelectedItem;

	private MapLevelSelector lvlLastSelectedItem;

	private MapCutSceneCasette cutSelected;

	public LevelSelectionPathBuilder levelPath;

	private Vector3 defCamPosition;

	private Vector3 camTarget;

	private float camMoveTimer;

	private float selectionClearWaitTimer;

	private void Start()
	{
		defCamPosition = roadCamera.transform.localPosition;
		mapObjects = GetComponentsInChildren<MapObject>();
		int num = 0;
		unlockedCounter = 0;
		for (int i = 0; i < mapObjects.Length; i++)
		{
			num += mapObjects[i].defaultCost;
			if (mapObjects[i].Unlocked)
			{
				totalUnlockedEnergy += mapObjects[i].defaultCost;
				unlockedCounter++;
			}
			else
			{
				totalUnlockedEnergy += mapObjects[i].defaultCost - mapObjects[i].BatteryCost;
			}
		}
		CheckPowerUpCompletion();
		LocalizationLoader.Instance.SetText(townTotalEnergy, "MainMenu.Map.TotalEnergyRestored");
		townTotalEnergy.text = string.Format("{0} {1}", townTotalEnergy.text, totalUnlockedEnergy);
		roadCamera.GetComponent<BloomOptimized>().enabled = DeviceSettings.Instance.EnableBloom;
		roadUICamera.enabled = false;
		LanguageManager instance = LanguageManager.Instance;
		instance.OnChangeLanguage = (ChangeLanguageEventHandler)Delegate.Combine(instance.OnChangeLanguage, new ChangeLanguageEventHandler(OnChangeLanguage));
	}

	private void OnDestroy()
	{
		if (LanguageManager.HasInstance)
		{
			LanguageManager instance = LanguageManager.Instance;
			instance.OnChangeLanguage = (ChangeLanguageEventHandler)Delegate.Remove(instance.OnChangeLanguage, new ChangeLanguageEventHandler(OnChangeLanguage));
		}
	}

	private void OnChangeLanguage(LanguageManager languageManager)
	{
		if (levelPath != null)
		{
			levelPath.UpdateLevels();
		}
		LocalizationLoader.Instance.SetText(townTotalEnergy, "MainMenu.Map.TotalEnergyRestored");
		townTotalEnergy.text = string.Format("{0} {1}", townTotalEnergy.text, totalUnlockedEnergy);
	}

	private void CamTarget(Vector3 point)
	{
		Vector3 vector = roadCamera.WorldToViewportPoint(point);
		vector.y = (vector.y - 0.5f) * 42f;
		camTarget = defCamPosition;
		camTarget.y += vector.y;
	}

	private void Update()
	{
		if (!roadUICamera.enabled || !Input.GetMouseButtonDown(0) || OnButton() || mapPopUp.IsActive || mapStatBlocker.activeSelf)
		{
			return;
		}
		bool flag = false;
		RaycastHit hitInfo;
		if (Physics.Raycast(roadUICamera.ScreenPointToRay(Input.mousePosition), out hitInfo, 200f, MapObjectLayer.value))
		{
			MapCutSceneCasette componentInParent = hitInfo.transform.GetComponentInParent<MapCutSceneCasette>();
			MapEnergyIcon component = hitInfo.transform.GetComponent<MapEnergyIcon>();
			MapLevelSelector componentInParent2 = hitInfo.transform.GetComponentInParent<MapLevelSelector>();
			if (component != null || componentInParent2 != null)
			{
				if (component != null)
				{
					SelectMapEnergyObject(component);
				}
				else
				{
					if (hitInfo.transform.gameObject.name.Equals("GameCenter"))
					{
						componentInParent2.GameCenterOpen();
						return;
					}
					InfoBox component2 = hitInfo.transform.GetComponent<InfoBox>();
					if (component2 != null && component2.enabled)
					{
						ShowLockedInfo(componentInParent2, component2);
						StuckButtonVisibility.Instance.SetVisible();
						return;
					}
					if (lvlSelectedItem != null && componentInParent2 != lvlSelectedItem)
					{
						ClearSelectedLevel();
					}
					else if (componentInParent2 == lvlSelectedItem)
					{
						levelPath.SetLastLevelOpened(componentInParent2);
					}
					componentInParent2.SetSelected(true);
					lvlSelectedItem = componentInParent2;
					lvlLastSelectedItem = componentInParent2;
					ClearSelectedMapItem();
					ClearCutSceneSelector();
				}
			}
			else if (componentInParent != null)
			{
				SelectCutsenePlayer(componentInParent);
			}
			else if (hitInfo.collider.CompareTag("ExtraLife"))
			{
				hitInfo.collider.transform.GetComponentInParent<MapLifeGenerator>().CollectLife();
			}
			else
			{
				if (hitInfo.transform.GetComponent<InfoBox>() != null && hitInfo.transform.GetComponent<InfoBox>().enabled)
				{
					hitInfo.transform.GetComponent<InfoBox>().ShowInfo();
					return;
				}
				flag = true;
			}
		}
		else
		{
			flag = true;
		}
		if (flag)
		{
			ClearSelectedLevel();
			ClearCutSceneSelector();
		}
	}

	public MapObject[] GetMapObjects()
	{
		return mapObjects;
	}

	private void FixedUpdate()
	{
		if (selectedItem != null)
		{
			roadCamera.transform.localPosition = Vector3.Lerp(roadCamera.transform.localPosition, camTarget, Time.fixedDeltaTime * 3f);
		}
		else if (selectionClearWaitTimer > 0f)
		{
			selectionClearWaitTimer -= Time.fixedDeltaTime;
		}
		else if (camMoveTimer > 0f)
		{
			camMoveTimer -= Time.fixedDeltaTime;
			if (camMoveTimer <= 0f)
			{
				roadCamera.transform.localPosition = defCamPosition;
			}
			else
			{
				roadCamera.transform.localPosition = Vector3.Lerp(roadCamera.transform.localPosition, defCamPosition, Time.fixedDeltaTime * 3f);
			}
		}
	}

	public void ClearSelectedMapItem()
	{
		if (selectedItem != null && selectedItem.IsActivated)
		{
			selectedItem.SelectFromMap(false);
			mapPopUp.Show(false);
			selectedItem = null;
			camMoveTimer = 1f;
		}
	}

	public MapLevelSelector GetLastSelectedLevel()
	{
		return lvlLastSelectedItem;
	}

	public void ShowLockedInfo(MapLevelSelector mls, InfoBox lockedInfo)
	{
		SceneDetails previousLevel = SceneLoader.Instance.GetPreviousLevel(mls.sceneDetails.SceneName, levelPath.group);
		if (mls.sceneDetails.IsEndless && mls.BossEnergyDiff > 0)
		{
			lockedInfo.LocalizationID = "MainMenu.Notification.CollectMoreDiamonds";
		}
		else if (previousLevel != null && previousLevel.IsEndless && ((previousLevel.SceneName.Equals("Endless9") && previousLevel.Storage.BestDistance < 1000f) || (!previousLevel.SceneName.Equals("Endless9") && previousLevel.Storage.BestDistance < 1500f)))
		{
			lockedInfo.LocalizationID = "MainMenu.Notification.PassPreviousBoss";
		}
		else
		{
			lockedInfo.LocalizationID = "MainMenu.Notification.PassPreviousStageFirst";
		}
		lockedInfo.ShowInfo();
	}

	public void SelectOnMap(string sceneName)
	{
		MapLevelSelector levelSelector = levelPath.GetLevelSelector(sceneName);
		if (levelSelector != null)
		{
			levelPath.SetLastLevelOpened(levelSelector);
			if (lvlSelectedItem != null)
			{
				lvlSelectedItem.SetSelected(false);
			}
			lvlSelectedItem = levelSelector;
			lvlLastSelectedItem = levelSelector;
			lvlSelectedItem.SetSelected(true);
			cameraSpline.SetOnMap(levelPath.lastOpenLevelPosition);
		}
	}

	public MapLevelSelector GetMapLevelSelector(string sceneName)
	{
		return levelPath.GetLevelSelector(sceneName);
	}

	public void ClearSelectedLevel()
	{
		if (lvlSelectedItem != null)
		{
			lvlSelectedItem.SetSelected(false);
			lvlSelectedItem = null;
		}
	}

	public void SelectMapEnergyObject(MapEnergyIcon mei)
	{
		if (!mei.IsActivated)
		{
			if (selectedItem != null && selectedItem.IsActivated && selectedItem != mei)
			{
				selectedItem.SelectFromMap(false);
			}
			selectedItem = mei;
			CamTarget(mei.transform.position);
			mei.SelectFromMap(true);
			mapPopUp.Show(true, mei.mapObject);
		}
		ClearCutSceneSelector();
		ClearSelectedLevel();
	}

	public void SelectCutsenePlayer(MapCutSceneCasette mcsc)
	{
		mcsc.StartCutScene();
		cutSelected = mcsc;
		ClearSelectedMapItem();
		ClearSelectedLevel();
	}

	public void ClearCutSceneSelector()
	{
		if (cutSelected != null)
		{
			cutSelected.Deselect();
		}
	}

	private bool OnButton()
	{
		EventSystem current = EventSystem.current;
		if (!current.IsPointerOverGameObject())
		{
			return false;
		}
		if (!current.currentSelectedGameObject)
		{
			return false;
		}
		return true;
	}

	public void SwitchAudioListener(bool enable)
	{
		if (cameraSpline != null)
		{
			cameraSpline.listener.enabled = enable;
			if (AudioController.Instance != null)
			{
				AudioController.Instance.SetListener(!enable);
			}
		}
	}

	public void BackToMenu()
	{
		ClearSelectedLevel();
		ClearSelectedMapItem();
		cameraSpline.ScrollToStart(0f);
	}

	public void SetOnMap(bool enable, bool moveCamera)
	{
		roadUICamera.enabled = enable;
		Time.timeScale = 1f;
		if (levelPath != null)
		{
			levelPath.UpdateLevels();
			CheckUnlocks();
		}
		if (enable && moveCamera && levelPath != null)
		{
			if (levelPath.lastOpenLevel == null)
			{
				MapLevelSelector levelSelector = levelPath.GetLevelSelector(SceneLoader.Instance.GetFirstLevelSceneName(levelPath.group));
				levelPath.SetLastLevelOpened(levelSelector);
			}
			MapLevelSelector levelSelector2 = levelPath.GetLevelSelector(levelPath.lastOpenLevel.SceneName);
			if (levelSelector2 != null)
			{
				lvlSelectedItem = levelSelector2;
				lvlLastSelectedItem = levelSelector2;
				lvlSelectedItem.SetSelected(true);
			}
			cameraSpline.SetOnMap(levelPath.lastOpenLevelPosition);
		}
		if (Main.Instance != null && Main.Instance.CurrentScene != null && lvlSelectedItem != null)
		{
			lvlSelectedItem.ShowGainedScore();
		}
		LocalizationLoader.Instance.SetText(townTotalEnergy, "MainMenu.Map.TotalEnergyRestored");
		townTotalEnergy.text = string.Format("{0} {1}", townTotalEnergy.text, totalUnlockedEnergy);
	}

	private void CheckUnlocks()
	{
		SceneDetails sceneDetails = SceneLoader.Instance.Scenes[0];
		List<SceneDetails> groupScenes = SceneLoader.Instance.GetGroupScenes(levelPath.group);
		for (int i = 0; i < groupScenes.Count; i++)
		{
			SceneDetails sceneDetails2 = groupScenes[i];
			if (sceneDetails2.Storage.IsOpen)
			{
				sceneDetails = sceneDetails2;
			}
		}
		SceneDetails nextLevel = SceneLoader.Instance.GetNextLevel(sceneDetails.SceneName, sceneDetails.Group);
		if (nextLevel != null && IsLevelPassed(sceneDetails))
		{
			if (levelPath != null)
			{
				levelPath.UpdateBossLimits();
			}
			Unlock(sceneDetails);
		}
		if (mapObjects != null)
		{
			CheckPowerUpCompletion();
		}
	}

	private bool IsLevelPassed(SceneDetails sd)
	{
		if (sd.IsEndless)
		{
			return (!sd.SceneName.Equals("Endless9")) ? (sd.Storage.BestDistance / 4500f > 0.3333f) : (sd.Storage.BestDistance >= 1000f);
		}
		return sd.Storage.HighScore / (float)sd.Storage.TrickCount >= 0.4f;
	}

	public void PowerButton()
	{
		PowerButtonCheck();
	}

	public bool PowerButtonCheck()
	{
		bool result = false;
		if (selectedItem != null)
		{
			if (GameStats.Instance.TotalBattery >= selectedItem.mapObject.BatteryCost)
			{
				if (!PlayerPrefs.HasKey("firstEnergyInfo") && selectedItem.mapObject.BatteryCost < 100)
				{
					firstEnergyUnlock.ShowInfo();
					PlayerPrefs.SetInt("firstEnergyInfo", 1);
					PlayerPrefs.Save();
				}
				if (!PlayerPrefs.HasKey("200Info") && totalUnlockedEnergy + selectedItem.mapObject.BatteryCost > 500)
				{
					energyOver200InfoBox.ShowInfo();
					PlayerPrefs.SetInt("200Info", 1);
					PlayerPrefs.Save();
				}
				GameStats.Instance.TotalBattery -= selectedItem.mapObject.BatteryCost;
				GameStats.Instance.UnlockedBattery += selectedItem.mapObject.BatteryCost;
				totalUnlockedEnergy += selectedItem.mapObject.BatteryCost;
				selectedItem.mapObject.BatteryCost = 0;
				selectedItem.mapObject.Unlock();
				CheckPowerUpCompletion();
				if (selectedItem.mapObject.StatToGet == PlayerStatType.Character)
				{
					characterPanel.Unlock();
				}
				selectionClearWaitTimer = 1.5f;
				ClearSelectedMapItem();
				result = true;
			}
			else
			{
				notEnoughEnergy.ShowInfo();
			}
			levelPath.UpdateLevels();
			LocalizationLoader.Instance.SetText(townTotalEnergy, "MainMenu.Map.TotalEnergyRestored");
			townTotalEnergy.text = string.Format("{0} {1}", townTotalEnergy.text, totalUnlockedEnergy);
		}
		return result;
	}

	private void CheckPowerUpCompletion()
	{
		int num = -1;
		int num2 = 0;
		for (int i = 0; i < mapObjects.Length; i++)
		{
			if (mapObjects[i].Unlocked)
			{
				num2++;
			}
			else
			{
				mapObjects[i].energyIcon.gameObject.SetActive(true);
				if (mapObjects[i].GetComponentInChildren<MapHelperArrow>(true) != null)
				{
					mapObjects[i].GetComponentInChildren<MapHelperArrow>(true).gameObject.SetActive(totalUnlockedEnergy < 5);
				}
			}
			if (mapObjects[i].BatteryCost == 5)
			{
				num = i;
			}
		}
		if (num2 == 0)
		{
			for (int j = 0; j < mapObjects.Length; j++)
			{
				if (j == num)
				{
					continue;
				}
				if (GameStats.Instance.TotalBattery < 200)
				{
					mapObjects[j].energyIcon.gameObject.SetActive(false);
					if (mapObjects[j].GetComponentInChildren<MapHelperArrow>(true) != null)
					{
						mapObjects[j].GetComponentInChildren<MapHelperArrow>(true).gameObject.SetActive(false);
					}
				}
				else if (GameStats.Instance.TotalBattery > 5)
				{
					mapObjects[j].energyIcon.gameObject.SetActive(true);
					if (mapObjects[j].GetComponentInChildren<MapHelperArrow>(true) != null)
					{
						mapObjects[j].GetComponentInChildren<MapHelperArrow>(true).gameObject.SetActive(GameStats.Instance.TotalBattery > 1000);
					}
				}
			}
		}
		GameStats.Instance.PoweredUp = (float)num2 / (float)mapObjects.Length * 100f;
	}

	private void OnEnable()
	{
		if (Main.Instance != null && Main.Instance.UnlockLevel)
		{
			Main.Instance.UnlockLevel = false;
		}
	}

	private void OnDisable()
	{
		SwitchAudioListener(false);
	}

	private void Unlock(SceneDetails current)
	{
		SceneDetails nextLevel = SceneLoader.Instance.GetNextLevel(current.SceneName, current.Group);
		if (nextLevel == null || nextLevel.IsEndless)
		{
			return;
		}
		SceneLoader.Instance.UnlockNextLevel(current.SceneName, current.Group);
		if (levelPath != null)
		{
			levelPath.lastOpenLevel = nextLevel;
			levelPath.UpdateLevels();
			if (levelPath.UnlockLevel(levelPath.lastOpenLevel))
			{
				cameraSpline.SetOnMap(levelPath.lastOpenLevelPosition);
			}
		}
	}

	public void Unlock(bool isDebug = false)
	{
		SceneDetails firstLockedLevel = SceneLoader.Instance.GetFirstLockedLevel(levelPath.group);
		if (firstLockedLevel != null)
		{
			if (isDebug)
			{
				firstLockedLevel.Storage.HighScore = 15f;
				firstLockedLevel.Storage.BestDistance = 1500f;
			}
			SceneLoader.Instance.UnlockLevel(firstLockedLevel);
			levelPath.lastOpenLevel = firstLockedLevel;
			levelPath.UpdateLevels();
			if (levelPath.UnlockLevel(levelPath.lastOpenLevel))
			{
				cameraSpline.SetOnMap(levelPath.lastOpenLevelPosition);
				levelPath.UpdateBossLimits();
			}
		}
	}

	public void ResetProgress()
	{
		GameDataController.Delete("LastScene");
		SceneLoader.Instance.ResetStorages();
		levelPath.lastOpenLevel = SceneLoader.Instance.GetFirstLevel(levelPath.group);
		SceneLoader.Instance.SaveSceneStorage(levelPath.lastOpenLevel.SceneName, levelPath.lastOpenLevel.Storage);
		UnityEngine.Object.FindObjectOfType<ChallengePanel>().Reset();
		if (GameDataController.Exists(StuckButtonVisibility.STUCK_UNLOCKED_PREFIX))
		{
			GameDataController.Delete(StuckButtonVisibility.STUCK_UNLOCKED_PREFIX);
		}
		if ((bool)StuckButtonVisibility.Instance)
		{
			StuckButtonVisibility.Instance.ClearStuckCounter();
		}
		PlayerPrefs.DeleteAll();
	}

	public void AddBattery()
	{
		GameStats.Instance.TotalBattery += 1000;
		GameStats.Instance.ChallengeMoney += 50000;
	}

	public void Reset()
	{
		unlockedCounter = 0;
		for (int i = 0; i < mapObjects.Length; i++)
		{
			mapObjects[i].Reset();
		}
		GameStats.Instance.TotalBattery = 5;
		LifeController.Instance.HardReset();
		PlayerStats.Instance.ResetStats();
		CheckPowerUpCompletion();
	}
}
