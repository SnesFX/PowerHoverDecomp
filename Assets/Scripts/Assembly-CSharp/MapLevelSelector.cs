using System.Collections;
using UnityEngine;

public class MapLevelSelector : MonoBehaviour
{
	public Transform lookAtCameraObject;

	public Animator levelSelectorAnimator;

	public TextMesh levelName;

	public TextMesh scoreText;

	public MenuStars starContoller;

	public AudioSource audioActivate;

	public AudioSource audioSelect;

	public AudioSource audioDeSelect;

	public AudioSource audioStartLevel;

	public GameObject fireFlies;

	public TextMesh scoreGainedText;

	public GameObject BossLazer;

	public TextMesh BossLockMissingText;

	public GameObject GameCenterItem;

	public TextMesh GameCenterRanking;

	public GameObject BossUnlockEffect;

	public GameObject NonTouchSelectedGC;

	public TextMesh bossChapterText1;

	public TextMesh bossChapterText2;

	private int number;

	private Animator GCAnimator;

	private bool bossChaptersSet;

	private double adTimer;

	public bool IsSelected { get; private set; }

	public SceneDetails sceneDetails { get; private set; }

	public Vector3 positionOnMap { get; private set; }

	public int BossEnergyDiff { get; private set; }

	public bool MakeUnlock { get; private set; }

	private void Awake()
	{
		fireFlies.SetActive(false);
	}

	private void Start()
	{
		lookAtCameraObject.rotation = Quaternion.Euler(new Vector3(28f, 336f, 0f));
		scoreGainedText.transform.parent.rotation = lookAtCameraObject.rotation;
		if (GameCenterItem != null)
		{
			GameCenterItem.transform.rotation = lookAtCameraObject.rotation;
			GCAnimator = GameCenterItem.GetComponent<Animator>();
		}
		scoreGainedText.transform.parent.gameObject.SetActive(false);
	}

	private void OnDisable()
	{
		if (BossUnlockEffect != null)
		{
			BossUnlockEffect.SetActive(false);
		}
	}

	private void UpdateRankingText()
	{
		int leaderboardRanking = GameCenter.Instance.GetLeaderboardRanking(sceneDetails.SceneName);
		if (leaderboardRanking > 0)
		{
			GameCenterRanking.text = string.Format("#{0}", leaderboardRanking.ToString());
		}
		else
		{
			GameCenterRanking.text = "-";
		}
	}

	public void SetState(SceneDetails sd, int levelNumber, Vector3 mapPosition)
	{
		number = levelNumber;
		positionOnMap = mapPosition;
		sceneDetails = sd;
		levelSelectorAnimator.Play((!sd.Storage.IsOpen) ? "LSoffState" : ((!IsSelected) ? "LSActivate" : "LSSelect"), -1, 0f);
		if (Application.isPlaying)
		{
			LocalizationLoader.Instance.SetText(levelName, "MainMenu.Locked");
			if (sd.Storage.IsOpen)
			{
				levelName.text = string.Format("{0}", sd.VisibleName.ToUpper());
			}
			if (!DeviceSettings.Instance.EnableOptimizedMaterials)
			{
				fireFlies.SetActive(sd.Storage.IsOpen && Random.Range(0f, 1f) <= 0.3f);
			}
		}
		if (!sd.IsEndless)
		{
			if (sd.Storage.HighScore > 0f)
			{
				scoreText.text = string.Format("{0}/{1}", sd.Storage.HighScore, sd.Storage.TrickCount);
				starContoller.UpdateStars(sd.Storage.HighScore / (float)sd.Storage.TrickCount);
			}
			else
			{
				scoreText.text = string.Empty;
				starContoller.UpdateStars(0f);
			}
		}
		else
		{
			if (!sd.IsEndless)
			{
				return;
			}
			if (sd.Storage.BestDistance > 0f)
			{
				scoreText.text = string.Format("{0}", sd.Storage.BestDistance.ToString("0000000"));
				starContoller.UpdateStars(sd.Storage.BestDistance / 4500f);
				if (sceneDetails.IsEndless && IsSelected)
				{
					UpdateRankingText();
					GameCenterItem.SetActive(true);
					GCAnimator.Play("GCActive", -1, 0f);
				}
			}
			else
			{
				scoreText.text = string.Empty;
				starContoller.UpdateStars(0f);
			}
			if (!bossChaptersSet)
			{
				bossChaptersSet = true;
			}
			SceneDetails nextLevel = SceneLoader.Instance.GetNextLevel(sd.SceneName, sd.Group);
			if (sd.Storage.IsOpen && (nextLevel == null || nextLevel.Storage.IsOpen))
			{
				BossLazer.SetActive(false);
				BossLockMissingText.gameObject.SetActive(false);
			}
			else if (sd.Storage.IsOpen && !nextLevel.Storage.IsOpen)
			{
				BossLazer.SetActive(true);
				BossLockMissingText.gameObject.SetActive(false);
			}
			else
			{
				if (sd.Storage.IsOpen || !(base.transform.GetComponentInParent<Map>() != null))
				{
					return;
				}
				int totalUnlockedEnergy = base.transform.GetComponentInParent<Map>().totalUnlockedEnergy;
				BossEnergyDiff = sd.StarLockCount - totalUnlockedEnergy;
				bool flag = SceneLoader.Instance.IsPreviousCompleted(sd.SceneName);
				BossLazer.SetActive(true);
				if (flag && BossEnergyDiff <= 0)
				{
					LocalizationLoader.Instance.SetText(BossLockMissingText, "MainMenu.Powered");
					levelSelectorAnimator.Play("LSBossPowered", -1, 0f);
					MakeUnlock = true;
					return;
				}
				BossLockMissingText.gameObject.SetActive(true);
				if (BossEnergyDiff <= 0)
				{
					LocalizationLoader.Instance.SetText(BossLockMissingText, "MainMenu.Powered");
				}
				else
				{
					BossLockMissingText.text = string.Format("{0}/{1}\nkwh", Mathf.Min(sd.StarLockCount, totalUnlockedEnergy), sd.StarLockCount);
				}
				if (flag)
				{
					BossLockMissingText.gameObject.GetComponent<BoxCollider>().enabled = true;
				}
			}
		}
	}

	private IEnumerator DisableBossEffect()
	{
		yield return new WaitForSeconds(2.5f);
		if (BossUnlockEffect != null)
		{
			BossUnlockEffect.SetActive(false);
		}
	}

	public void GameCenterOpen()
	{
		GameCenter.Instance.ShowLeaderboardsUI(sceneDetails.SceneName);
		GCAnimator.Play("GCClicked", -1, 0f);
		audioStartLevel.Play();
	}

	public void UpdateState()
	{
		SetState(sceneDetails, number, positionOnMap);
	}

	public void Unlock()
	{
		levelSelectorAnimator.Play("LSActivate", -1, 0f);
		audioActivate.Play();
	}

	public void ShowGainedScore()
	{
		if (LevelStats.Instance.GainedScore > 0 && sceneDetails != null && sceneDetails.SceneName.Equals(Main.Instance.CurrentScene))
		{
			scoreGainedText.text = string.Format("+{0}", LevelStats.Instance.GainedScore);
			scoreGainedText.transform.parent.gameObject.SetActive(true);
			scoreGainedText.GetComponentInParent<Animator>().Play("RisingScore", -1, 0f);
			IsSelected = false;
			levelName.text = string.Format("{0}", sceneDetails.VisibleName.ToUpper());
			levelSelectorAnimator.Play("LSDeSelect", -1, 0f);
		}
	}

	public void SetSelected(bool enable)
	{
		if (enable && IsSelected)
		{
			if (NonTouchSelectedGC != null && NonTouchSelectedGC.activeSelf)
			{
				GameCenterOpen();
			}
			else if (ShowAdItem())
			{
				IapManager.Instance.ShowPopUp(true, SceneLoader.Instance.UnlockTimeLeft(sceneDetails));
			}
			else
			{
				StartScene(true);
			}
			return;
		}
		if (enable)
		{
			if (MakeUnlock)
			{
				base.transform.GetComponentInParent<Map>().Unlock();
				BossLazer.SetActive(true);
				if (BossUnlockEffect != null)
				{
					BossUnlockEffect.SetActive(true);
					StartCoroutine(DisableBossEffect());
				}
			}
			levelName.text = string.Format("{0}.{1}", number, sceneDetails.VisibleName.ToUpper());
			levelSelectorAnimator.Play("LSSelect", -1, 0f);
			audioSelect.Play();
			if (sceneDetails.IsEndless)
			{
				UpdateRankingText();
				GameCenterItem.SetActive(true);
				GCAnimator.Play("GCActivating", -1, 0f);
			}
		}
		else
		{
			levelName.text = string.Format("{0}", sceneDetails.VisibleName.ToUpper());
			levelSelectorAnimator.Play("LSDeSelect", -1, 0f);
			audioDeSelect.Play();
			if (sceneDetails.IsEndless)
			{
				GCAnimator.Play("GCDeActivate", -1, 0f);
				if (GameCenterItem != null)
				{
					GameCenterItem.GetComponentInChildren<PingPongMove>().moveLength = Vector3.zero;
				}
				if (NonTouchSelectedGC != null)
				{
					NonTouchSelectedGC.SetActive(false);
				}
			}
		}
		if (sceneDetails.IsEndless)
		{
			BossLockMissingText.gameObject.SetActive(false);
			MakeUnlock = false;
		}
		IsSelected = enable;
	}

	public void StartScene(bool playSound = false)
	{
		levelSelectorAnimator.Play("LSplay", -1, 0f);
		if (playSound)
		{
			audioStartLevel.Play();
		}
		StartLevel();
	}

	private bool ShowAdItem()
	{
		if (UnityAdsIngetration.Instance.IsAdsActivated && sceneDetails.AdBlock && SceneLoader.Instance.UnlockTimeLeft(sceneDetails) < 28800.0 && GameDataController.Exists("LastScene") && !GameDataController.Load<string>("LastScene").Equals(sceneDetails.SceneName) && sceneDetails.Storage.BestDistance <= 0f && sceneDetails.Storage.HighScore <= 0f)
		{
			return true;
		}
		return false;
	}

	private void StartLevel()
	{
		if (!SceneLoader.Instance.IsLoading && !CutSceneLoader.Instance.LoadingOperation)
		{
			if (UnityAdsIngetration.Instance.IsInitialized)
			{
				StartCoroutine(ShowBannerOnLoading());
			}
			SceneLoader.Instance.LoadLevel(sceneDetails.SceneName);
		}
	}

	private IEnumerator ShowBannerOnLoading()
	{
		yield return new WaitForSeconds(0.3f);
		UnityAdsIngetration.Instance.BannerShow();
	}
}
