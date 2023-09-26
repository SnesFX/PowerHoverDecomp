using System;
using System.Collections;
using System.Collections.Generic;
using SmartLocalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomInputController : MonoBehaviour
{
	[Serializable]
	public enum InputMenu
	{
		Main = 0,
		Achievements = 1,
		Settings = 2,
		Pause = 3,
		PauseEndless = 4,
		EndScreenNormal = 5,
		EndScreenEndless = 6,
		GameOver = 7,
		Map = 8,
		AdPopUp = 9,
		AdPopUpIngame = 10,
		Stats = 11,
		Cutscene = 12,
		FTTPopUp = 13,
		Challenges = 14,
		BuyBolts = 15
	}

	[Serializable]
	public class InputButtons
	{
		public InputMenu InMenu;

		public GameObject[] InObjects;

		public Button CloseAction;

		public MenuPanel Panel { get; set; }
	}

	public AudioSource buttonAudio;

	private InputButtons currentButtons;

	private EventSystem eventSystem;

	private bool released;

	private int selected;

	private Stack<MenuPanel> panelStack;

	private Map map;

	private bool firstSelectionAtStart;

	private float gestureMove;

	private int swipe;

	private int lastSelectionOnMainMenu;

	private bool buttonsSet;

	private bool backBlocked;

	public static CustomInputController Instance { get; private set; }

	public InputMenu GetCurrentMenu()
	{
		return (currentButtons != null) ? currentButtons.InMenu : InputMenu.Main;
	}

	private void Awake()
	{
		Instance = this;
		map = UnityEngine.Object.FindObjectOfType<Map>();
		panelStack = new Stack<MenuPanel>();
	}

	public void SetCurrentButtons(InputButtons activeButtons, int selection = 0)
	{
		buttonsSet = true;
		if (EventSystem.current == null || DeviceSettings.Instance == null || !DeviceSettings.Instance.EnableInputDevices)
		{
			return;
		}
		if ((activeButtons.InMenu == InputMenu.Map || activeButtons.InMenu == InputMenu.Main || activeButtons.InMenu == InputMenu.Stats || activeButtons.InMenu == InputMenu.AdPopUp || activeButtons.InMenu == InputMenu.Challenges || activeButtons.InMenu == InputMenu.BuyBolts || activeButtons.InMenu == InputMenu.Settings || activeButtons.InMenu == InputMenu.FTTPopUp) && activeButtons.Panel != null && !panelStack.Contains(activeButtons.Panel))
		{
			if (activeButtons.InMenu == InputMenu.Map && panelStack.Count > 1 && panelStack.Peek().Inputs.InMenu == InputMenu.Settings)
			{
				PopPanel();
			}
			panelStack.Push(activeButtons.Panel);
		}
		currentButtons = activeButtons;
		if (eventSystem == null)
		{
			eventSystem = EventSystem.current;
		}
		if (currentButtons.InMenu == InputMenu.Main)
		{
			selected = lastSelectionOnMainMenu;
		}
		else
		{
			selected = selection;
		}
		SelectItem(0f);
	}

	private void SelectItem(float move)
	{
		if (move != 0f)
		{
			SelectNext(move);
		}
		bool flag = false;
		int num = 0;
		while (!flag && num < 10)
		{
			if (currentButtons.InObjects[selected] != null && currentButtons.InObjects[selected].activeSelf && (currentButtons.InObjects[selected].GetComponent<Button>() == null || currentButtons.InObjects[selected].GetComponent<Button>().interactable))
			{
				flag = true;
				continue;
			}
			if (move == 0f)
			{
				move = -1f;
			}
			SelectNext(move);
			num++;
		}
		if (num <= 10 && !(currentButtons.InObjects[selected] == null) && eventSystem != null && (bool)currentButtons.InObjects[selected].GetComponent<Button>())
		{
			if (currentButtons.InMenu == InputMenu.Main)
			{
				lastSelectionOnMainMenu = selected;
			}
			eventSystem.SetSelectedGameObject(currentButtons.InObjects[selected]);
			if (!firstSelectionAtStart)
			{
				firstSelectionAtStart = true;
			}
			else
			{
				buttonAudio.Play();
			}
		}
	}

	private void SelectNext(float move)
	{
		if (move < 0f)
		{
			selected++;
		}
		else
		{
			selected--;
		}
		if (currentButtons.InObjects.Length - 1 < selected)
		{
			selected = 0;
		}
		else if (selected < 0)
		{
			selected = currentButtons.InObjects.Length - 1;
		}
	}

	public void PopPanel()
	{
		if (panelStack.Count > 1)
		{
			panelStack.Pop();
			if (panelStack.Count > 1 && currentButtons.InMenu != InputMenu.AdPopUp && panelStack.Peek().Inputs.InMenu == InputMenu.Settings)
			{
				PopPanel();
			}
			else
			{
				panelStack.Peek().Activate(true);
			}
		}
	}

	private void Update()
	{
		if (DeviceSettings.Instance == null || Main.Instance == null || !buttonsSet || (!Main.Instance.IsPaused && !Main.Instance.InMenu && (!(GameController.Instance != null) || (GameController.Instance.State != GameController.GameState.End && GameController.Instance.State != GameController.GameState.Kill))))
		{
			return;
		}
		bool buttonUp = Input.GetButtonUp("Pause");
		if (Main.Instance.InMenu && buttonUp && !backBlocked)
		{
			if (DeviceSettings.Instance.EnableInputDevices && currentButtons.InMenu != 0 && panelStack.Count > 1 && currentButtons.CloseAction != null && (currentButtons.InMenu != InputMenu.Map || !map.ActiveObjects.IsItemSelected()))
			{
				if (currentButtons.InMenu == InputMenu.Map)
				{
					map.ActiveObjects.ClearSelections();
					map.BackToMenu();
					map.cameraSpline.mainController.MenuClose();
				}
				else
				{
					currentButtons.CloseAction.OnSubmit(new BaseEventData(eventSystem));
				}
				PopPanel();
				return;
			}
			IAPBuyBolts iAPBuyBolts = UnityEngine.Object.FindObjectOfType<IAPBuyBolts>();
			if (iAPBuyBolts != null && iAPBuyBolts.gameObject.activeSelf)
			{
				iAPBuyBolts.ClosePopUp();
				return;
			}
			if (map.cameraSpline.mainController.currentMenu == MenuType.Achievements || map.cameraSpline.mainController.currentMenu == MenuType.Options || map.cameraSpline.mainController.currentMenu == MenuType.Challenges)
			{
				map.cameraSpline.mainController.MenuClose();
				return;
			}
			if (PlayerStats.Instance.CloseHiddenObject.activeSelf)
			{
				PlayerStats.Instance.ShowStats();
				return;
			}
			if (Main.Instance.InCutScene)
			{
				UnityEngine.Object.FindObjectOfType<CutSceneController>().SkipCutScene();
				return;
			}
			if (map != null && currentButtons != null && map.cameraSpline.TF > 0f && (currentButtons.InMenu == InputMenu.Map || currentButtons.InMenu == InputMenu.Main))
			{
				map.ActiveObjects.SelectLastAndClear();
				map.BackToMenu();
				map.cameraSpline.mainController.MenuClose();
				if (currentButtons.InMenu == InputMenu.Map)
				{
					PopPanel();
				}
				return;
			}
			AndroidDialog androidDialog = AndroidDialog.Create(LanguageManager.Instance.GetTextValue("MainMenu.Exit"), LanguageManager.Instance.GetTextValue("MainMenu.Quit"), LanguageManager.Instance.GetTextValue("Ingame.Yes"), LanguageManager.Instance.GetTextValue("Ingame.No"));
			androidDialog.ActionComplete += OnDialogClose;
		}
		if (!DeviceSettings.Instance.EnableInputDevices)
		{
			return;
		}
		if (currentButtons.InMenu == InputMenu.Map && Main.Instance.InMenu)
		{
			UpdateMapControls();
			swipe = -1;
			return;
		}
		float num = Input.GetAxisRaw((!Main.Instance.InMenu) ? "Horizontal" : "Vertical");
		if (currentButtons.InMenu == InputMenu.Challenges || currentButtons.InMenu == InputMenu.Stats)
		{
			num = 0f - Input.GetAxisRaw("Horizontal");
		}
		if (Mathf.Abs(num) > 0.45f)
		{
			if (released)
			{
				SelectItem(num);
			}
			released = false;
			swipe = -1;
		}
		else if (released && eventSystem != null && currentButtons.InObjects[selected] != null)
		{
			if ((bool)currentButtons.InObjects[selected].GetComponent<Button>())
			{
				if (Input.GetButtonDown("Fire"))
				{
					currentButtons.InObjects[selected].GetComponent<Button>().OnSubmit(new BaseEventData(eventSystem));
					released = false;
				}
				else if (currentButtons.InObjects[selected].GetComponentInChildren<Scrollbar>() != null && Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.4f)
				{
					currentButtons.InObjects[selected].GetComponentInChildren<Scrollbar>().value += ((!(Input.GetAxisRaw("Horizontal") < 0f)) ? 0.1f : (-0.1f));
					released = false;
				}
			}
		}
		else
		{
			released = true;
		}
		gestureMove = 0f;
		swipe = -1;
	}

	private void UpdateStatControls()
	{
		float axisRaw = Input.GetAxisRaw("Horizontal");
		if (Input.GetButtonDown("Fire"))
		{
			if (released)
			{
				if (currentButtons.InObjects[2].activeSelf && currentButtons.InObjects[2].GetComponent<Button>().interactable)
				{
					currentButtons.InObjects[2].GetComponent<Button>().OnSubmit(new BaseEventData(eventSystem));
				}
				released = false;
			}
		}
		else if (axisRaw != 0f)
		{
			if (!released)
			{
				return;
			}
			if (axisRaw < -0.5f)
			{
				if (currentButtons.InObjects[1].activeSelf)
				{
					currentButtons.InObjects[1].GetComponent<Button>().OnSubmit(new BaseEventData(eventSystem));
				}
				released = false;
			}
			else if (axisRaw > 0.5f)
			{
				if (currentButtons.InObjects[0].activeSelf)
				{
					currentButtons.InObjects[0].GetComponent<Button>().OnSubmit(new BaseEventData(eventSystem));
				}
				released = false;
			}
		}
		else
		{
			released = true;
		}
	}

	public void SetMap(Map newMap)
	{
		map = newMap;
	}

	private void UpdateMapControls()
	{
		if (map == null)
		{
			map = UnityEngine.Object.FindObjectOfType<Map>();
		}
		float axisRaw = Input.GetAxisRaw("Vertical");
		float axisRaw2 = Input.GetAxisRaw("Horizontal");
		if (Input.GetButtonDown("Fire"))
		{
			if (released)
			{
				MapLevelSelector lastSelectedLevel = map.GetLastSelectedLevel();
				if (lastSelectedLevel != null && lastSelectedLevel.IsSelected)
				{
					lastSelectedLevel.SetSelected(true);
				}
				else if (map.ActiveObjects.MakeAction())
				{
					released = false;
				}
			}
		}
		else if (Input.GetButtonDown("Stats"))
		{
			if (released)
			{
				released = false;
				StartCoroutine(DisableBack());
				currentButtons.InObjects[1].GetComponent<Button>().OnSubmit(new BaseEventData(eventSystem));
			}
		}
		else if (Mathf.Abs(axisRaw) > 0.6f && Mathf.Abs(axisRaw) > Mathf.Abs(axisRaw2))
		{
			float num = axisRaw;
			if (!released)
			{
				return;
			}
			MapLevelSelector lastSelectedLevel2 = map.GetLastSelectedLevel();
			if (!map.ActiveObjects.IsItemSelected() && lastSelectedLevel2.sceneDetails.IsEndless)
			{
				if (lastSelectedLevel2.NonTouchSelectedGC != null && lastSelectedLevel2.NonTouchSelectedGC.activeSelf && num < 0f)
				{
					lastSelectedLevel2.GameCenterItem.GetComponentInChildren<PingPongMove>().moveLength = Vector3.zero;
					lastSelectedLevel2.NonTouchSelectedGC.SetActive(false);
				}
				else if (num > 0f)
				{
					lastSelectedLevel2.NonTouchSelectedGC.SetActive(true);
					lastSelectedLevel2.GameCenterItem.GetComponentInChildren<PingPongMove>().moveLength = new Vector3(0f, 10f, 0f);
				}
				else
				{
					map.ActiveObjects.GetClosestItem(map.cameraSpline.GetPanTarget(), (!(num > 0f)) ? InteractiveObjects.SelectionType.Down : InteractiveObjects.SelectionType.Up);
				}
				released = false;
			}
			else if (map.ActiveObjects.GetClosestItem(map.cameraSpline.GetPanTarget(), (!(num > 0f)) ? InteractiveObjects.SelectionType.Down : InteractiveObjects.SelectionType.Up))
			{
				released = false;
			}
		}
		else if (Mathf.Abs(axisRaw2) > 0.6f && Mathf.Abs(axisRaw2) > Mathf.Abs(axisRaw))
		{
			float num2 = axisRaw2;
			if (!released)
			{
				return;
			}
			if (!map.ActiveObjects.IsItemSelected())
			{
				MapLevelSelector lastSelectedLevel3 = map.GetLastSelectedLevel();
				SceneDetails sceneDetails = ((!(num2 < 0f)) ? SceneLoader.Instance.GetNextLevel(lastSelectedLevel3.sceneDetails.SceneName, lastSelectedLevel3.sceneDetails.Group) : SceneLoader.Instance.GetPreviousLevel(lastSelectedLevel3.sceneDetails.SceneName, lastSelectedLevel3.sceneDetails.Group));
				if (sceneDetails != null)
				{
					if (sceneDetails.Storage.IsOpen)
					{
						map.SelectOnMap(sceneDetails.SceneName);
						released = false;
						return;
					}
					MapLevelSelector mapLevelSelector = map.GetMapLevelSelector(sceneDetails.SceneName);
					if (sceneDetails.IsEndless && mapLevelSelector.MakeUnlock)
					{
						map.SelectOnMap(sceneDetails.SceneName);
					}
					else
					{
						InfoBox componentInChildren = mapLevelSelector.GetComponentInChildren<InfoBox>();
						if (componentInChildren != null && componentInChildren.enabled)
						{
							map.ShowLockedInfo(mapLevelSelector, componentInChildren);
						}
					}
					released = false;
				}
				else if (num2 < 0f && sceneDetails == null)
				{
					map.BackToMenu();
					map.ActiveObjects.ClearSelections();
					map.cameraSpline.mainController.MenuClose();
					panelStack.Pop();
					panelStack.Peek().Activate(true);
					released = false;
				}
				else if (num2 > 0f && sceneDetails == null && map.ActiveObjects.GetClosestItem(map.cameraSpline.GetPanTarget(), InteractiveObjects.SelectionType.Down))
				{
					released = false;
				}
			}
			else if (map.ActiveObjects.LastCutSceneSelected && num2 < 0f && map.ActiveObjects.GetClosestItem(map.cameraSpline.GetPanTarget(), InteractiveObjects.SelectionType.Up))
			{
				released = false;
			}
		}
		else
		{
			released = true;
		}
	}

	private IEnumerator DisableBack()
	{
		backBlocked = true;
		yield return new WaitForSeconds(1f);
		backBlocked = false;
	}

	private void OnDialogClose(AndroidDialogResult result)
	{
		switch (result)
		{
		case AndroidDialogResult.YES:
			Application.Quit();
			break;
		}
	}
}
