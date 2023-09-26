using UnityEngine;

public class LoadingController : ControllerBase
{
	public GameObject LevelSelectorCamera;

	public MenuType currentMenu { get; private set; }

	public override void Awake()
	{
		type = MenuType.Loading;
		currentMenu = MenuType.Loading;
		LevelSelectorCamera.SetActive(false);
		base.Awake();
		if (Application.loadedLevelName.Equals("MenuLoading"))
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
		base.OnEnable();
	}
}
