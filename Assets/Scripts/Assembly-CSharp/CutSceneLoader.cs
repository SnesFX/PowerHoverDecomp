using UnityEngine;

public class CutSceneLoader : MonoBehaviour
{
	private float loadingTimer;

	private GameObject root;

	private string CutSceneName;

	private int state;

	public static CutSceneLoader Instance { get; private set; }

	public bool LoadingOperation { get; private set; }

	private void Awake()
	{
		Instance = this;
	}

	private void FixedUpdate()
	{
		if (!LoadingOperation)
		{
			return;
		}
		if (state == 0)
		{
			loadingTimer += Time.fixedDeltaTime;
			if (loadingTimer > 1.4f)
			{
				loadingTimer = 0f;
				state = 1;
				Application.LoadLevelAdditive(CutSceneName);
			}
		}
		else if (loadingTimer > 0f)
		{
			loadingTimer -= Time.fixedDeltaTime;
			if (loadingTimer < 0f)
			{
				Main.Instance.SwitchMenu(MenuType.CutScene);
				LoadingOperation = false;
				root.SetActive(true);
				if (root.GetComponent<MenuPanel>() != null)
				{
					root.GetComponent<MenuPanel>().Activate(true);
				}
				root = null;
			}
		}
		else
		{
			GameObject gameObject = GameObject.Find(CutSceneName);
			if (gameObject != null)
			{
				root = gameObject;
				root.SetActive(false);
				loadingTimer = 1.2f;
			}
		}
	}

	public void StartCutScene(string sceneName)
	{
		if (!SceneLoader.Instance.IsLoading && !LoadingOperation)
		{
			CutSceneName = sceneName;
			LoadingOperation = true;
			state = 0;
			Main.Instance.FakeFadeIn();
		}
	}
}
