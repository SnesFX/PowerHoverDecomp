using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SceneRenderSettings))]
public class ControllerBase : MonoBehaviour
{
	public MenuType type;

	public GameObject debugEventSystem;

	public bool renderSettingsInUse = true;

	protected SceneRenderSettings renderSettings;

	public virtual void Awake()
	{
		renderSettings = GetComponent<SceneRenderSettings>();
		base.gameObject.SetActive(false);
		if (Main.Instance != null)
		{
			Main.Instance.AddMenuBase(this);
		}
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public virtual void OnEnable()
	{
		if (renderSettingsInUse)
		{
			renderSettings.LoadSettings();
		}
	}

	public void DisablePanel(GameObject panel)
	{
		panel.GetComponent<MenuPanel>().Activate(false);
		Button[] componentsInChildren = panel.GetComponentsInChildren<Button>(true);
		foreach (Button button in componentsInChildren)
		{
			button.interactable = false;
			if (button.name.Equals("Back"))
			{
				button.gameObject.SetActive(false);
			}
		}
	}

	public void EnablePanel(GameObject panel)
	{
		Button[] componentsInChildren = panel.GetComponentsInChildren<Button>(true);
		foreach (Button button in componentsInChildren)
		{
			button.interactable = true;
			if (button.name.Equals("Back"))
			{
				button.gameObject.SetActive(true);
			}
		}
	}
}
