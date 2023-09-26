using UnityEngine;

public class PopUpFreeToTryController : ControllerBase
{
	public RectTransform content;

	public override void Awake()
	{
		base.Awake();
		if (Application.loadedLevelName.Equals("MenuPopUpFreeToTry"))
		{
			if ((bool)debugEventSystem)
			{
				debugEventSystem.SetActive(true);
			}
		}
		else if ((bool)debugEventSystem)
		{
			debugEventSystem.SetActive(false);
		}
	}

	private void Start()
	{
		if (DeviceSettings.Instance != null && DeviceSettings.Instance.RunningOnTV)
		{
			Vector3 position = content.position;
			position.y -= 20f;
			content.position = position;
		}
	}

	public void Close()
	{
		Main.Instance.ClosePopUp();
	}
}
