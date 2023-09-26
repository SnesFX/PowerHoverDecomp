using UnityEngine;
using UnityEngine.UI;

public class MusicPlayerController : ControllerBase
{
	private enum ButtonType
	{
		None = 0,
		Exit = 1,
		Next = 2,
		Previous = 3
	}

	public GameObject playerPanel;

	public GameObject LevelSelectorCamera;

	public Text songTitle;

	public override void Awake()
	{
		type = MenuType.Loading;
		LevelSelectorCamera.SetActive(false);
		base.Awake();
		if (Application.loadedLevelName.Equals("MenuMusicPlayer"))
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
		LevelSelectorCamera.SetActive(true);
		if ((bool)AudioController.Instance)
		{
			songTitle.text = AudioController.Instance.GetCurrentMusicTitle();
		}
		EnablePanel(playerPanel);
		base.OnEnable();
	}

	public void MenuClicked(int m)
	{
		switch (m)
		{
		case 1:
			Main.Instance.SwitchMenu(MenuType.Main);
			DisablePanel(playerPanel);
			break;
		case 2:
			songTitle.text = AudioController.Instance.PlayNext();
			break;
		case 3:
			songTitle.text = AudioController.Instance.PlayPrevious();
			break;
		}
	}
}
