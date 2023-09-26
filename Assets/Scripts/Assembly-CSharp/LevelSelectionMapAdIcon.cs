using UnityEngine;

public class LevelSelectionMapAdIcon : MonoBehaviour
{
	public Renderer renderer1;

	public Renderer shadow;

	public GameObject lvlSelector;

	private MapLevelSelector levelSelector;

	private bool itemEnabled;

	private float timer;

	private string lastScene;

	private bool loadedScene;

	private void Start()
	{
		levelSelector = GetComponentInParent<MapLevelSelector>();
		itemEnabled = false;
		Renderer renderer = renderer1;
		bool flag = false;
		shadow.enabled = flag;
		renderer.enabled = flag;
	}

	private void Update()
	{
		if (levelSelector.sceneDetails == null)
		{
			return;
		}
		timer += Time.deltaTime;
		if (timer > 1f)
		{
			timer = 0f;
			if (itemEnabled && !ShowAdItem())
			{
				itemEnabled = false;
				Renderer renderer = renderer1;
				bool flag = false;
				shadow.enabled = flag;
				renderer.enabled = flag;
			}
			else if ((!itemEnabled || lvlSelector.activeSelf) && ShowAdItem())
			{
				itemEnabled = true;
				renderer1.enabled = levelSelector.sceneDetails.Storage.IsOpen;
				shadow.enabled = !levelSelector.sceneDetails.Storage.IsOpen;
			}
		}
	}

	private bool ShowAdItem()
	{
		if (!GameDataController.Exists("LastScene"))
		{
			return false;
		}
		if (lastScene == null || !lastScene.Equals(levelSelector.sceneDetails.SceneName))
		{
			lastScene = levelSelector.sceneDetails.SceneName;
			loadedScene = GameDataController.Load<string>("LastScene").Equals(lastScene);
		}
		if (UnityAdsIngetration.Instance.IsAdsActivated && levelSelector.sceneDetails.AdBlock && SceneLoader.Instance.UnlockTimeLeft(levelSelector.sceneDetails) < 28800.0 && !loadedScene && levelSelector.sceneDetails.Storage.BestDistance <= 0f && levelSelector.sceneDetails.Storage.HighScore <= 0f)
		{
			return true;
		}
		return false;
	}
}
