using UnityEngine;

public class MapCutSceneCasette : MonoBehaviour
{
	public GameObject Casette;

	public GameObject Shadow;

	public bool EnabledAllways;

	public string OpenWhenUnlockSceneName;

	public string CutSceneName;

	public Animator casetteAnimator;

	private bool selected;

	private void Start()
	{
		CheckIfVisible();
	}

	private void OnEnable()
	{
		if (SceneLoader.Instance != null)
		{
			CheckIfVisible();
		}
		GameObject gameObject = GameObject.Find(CutSceneName);
		if (gameObject != null)
		{
			Object.Destroy(gameObject);
		}
	}

	public void Deselect()
	{
		if (selected)
		{
			selected = false;
			casetteAnimator.Play("CutSceneCasetteStart", -1, 0f);
		}
	}

	private void CheckIfVisible()
	{
		if (EnabledAllways)
		{
			Casette.SetActive(true);
			Shadow.SetActive(true);
			return;
		}
		SceneDetails sceneDetails = SceneLoader.Instance.GetSceneDetails(OpenWhenUnlockSceneName);
		if (sceneDetails.Storage.IsOpen && ((sceneDetails.SceneName.Equals("Endless9") && sceneDetails.Storage.BestDistance >= 1000f) || sceneDetails.Storage.BestDistance >= 1500f || sceneDetails.Storage.HighScore / (float)sceneDetails.Storage.TrickCount > 0.4f))
		{
			Casette.SetActive(true);
			Shadow.SetActive(true);
		}
		else
		{
			Casette.SetActive(false);
			Shadow.SetActive(false);
		}
	}

	public void StartCutScene()
	{
		if (selected)
		{
			casetteAnimator.Play("CutSceneCasetteStart", -1, 0f);
			CutSceneLoader.Instance.StartCutScene(CutSceneName);
		}
		else
		{
			casetteAnimator.Play("CutSceneCasetteOn", -1, 0f);
		}
		selected = !selected;
	}
}
