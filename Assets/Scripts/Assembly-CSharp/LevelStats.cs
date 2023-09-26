using System.Collections;
using UnityEngine;

public class LevelStats : MonoBehaviour
{
	private SceneStorage CurrentStorage;

	private int CollectablesInLevel;

	private float collectedTimer;

	public static LevelStats Instance { get; private set; }

	public float LevelTime { get; set; }

	public int LevelKillCount { get; set; }

	public bool UpdateProgress { get; set; }

	public int CollectebleCollectCountTricks { get; private set; }

	public int CollectebleCollectCount { get; set; }

	public float PreviousBestDistance { get; set; }

	public float PreviousBestHighScore { get; set; }

	public float PreviousBestKillCount { get; private set; }

	public float CollectablePitch { get; private set; }

	public int SuperCollectableCounter { get; private set; }

	public int GainedScore { get; set; }

	public float LevelDistance { get; set; }

	public int WormFood { get; set; }

	public bool Explorer { get; private set; }

	public bool AvoidTheLight { get; private set; }

	public bool LightsHittedTomb { get; set; }

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		if (!UpdateProgress || !UIController.Instance)
		{
			return;
		}
		if ((bool)GameController.Instance && GameController.Instance.State == GameController.GameState.Running)
		{
			LevelTime += Time.deltaTime;
			if (SceneLoader.Instance.Current.IsChallenge && (SceneLoader.Instance.Current.Group == 1 || SceneLoader.Instance.Current.Group == 3) && LevelDistance > (float)SceneLoader.Instance.GetChallengeLevelLimit(SceneLoader.Instance.Current))
			{
				ChallengeCompleted();
			}
		}
		if (collectedTimer > 0f)
		{
			collectedTimer -= Time.deltaTime;
			if (collectedTimer <= 0f)
			{
				CollectablePitch = 0f;
			}
		}
	}

	public void ResetProgress()
	{
		SuperCollectableCounter = 0;
		CollectablePitch = 0f;
		CollectebleCollectCountTricks = 0;
		CollectebleCollectCount = 0;
		LevelTime = 0f;
		LevelKillCount = 0;
		UpdateProgress = true;
		LightsHittedTomb = false;
		ClearCollectables();
	}

	public void StartProgress(SceneStorage storage)
	{
		CurrentStorage = storage;
		GainedScore = 0;
		ResetProgress();
	}

	private void SaveStorage(bool levelEnd)
	{
		if (CurrentStorage == null)
		{
			return;
		}
		if (levelEnd)
		{
			PreviousBestDistance = CurrentStorage.BestDistance;
			PreviousBestKillCount = CurrentStorage.KillCount;
			PreviousBestHighScore = CurrentStorage.HighScore;
			CurrentStorage.HighScore = Mathf.Max(CurrentStorage.HighScore, CollectebleCollectCount);
			CurrentStorage.KillCount += LevelKillCount;
			CurrentStorage.TrickCount = Mathf.Max(CurrentStorage.TrickCount, CollectablesMax());
			CurrentStorage.BestDistance = Mathf.Max(CurrentStorage.BestDistance, LevelDistance);
			if (SceneLoader.Instance.Current.IsChallenge)
			{
				GameStats.Instance.ChallengeDistance += LevelDistance;
				CurrentStorage.BestTime += LevelDistance;
				if (SceneLoader.Instance.Current.Group == 1 || SceneLoader.Instance.Current.Group == 3)
				{
					GameCenter.Instance.PostScore(CurrentStorage.BestDistance, Main.Instance.CurrentScene);
				}
				else
				{
					GameCenter.Instance.PostScore(Mathf.RoundToInt(CurrentStorage.HighScore), Main.Instance.CurrentScene);
				}
			}
			else if (SceneLoader.Instance.Current.IsEndless)
			{
				GameStats.Instance.BestDistance = Mathf.Max(GameStats.Instance.BestDistance, LevelDistance);
				GameCenter.Instance.PostScore(CurrentStorage.BestDistance, Main.Instance.CurrentScene);
			}
			else if (PreviousBestHighScore <= 0f && LevelKillCount == 0)
			{
				GameStats.Instance.NoCrashesOnLevels++;
				GameStats.Instance.ScoreTotal += CurrentStorage.HighScore;
			}
			if (SceneLoader.Instance.Current.SceneName.Equals("Hover23"))
			{
				Explorer = Mathf.CeilToInt(CurrentStorage.HighScore) >= CollectablesInLevel;
			}
			else if (SceneLoader.Instance.Current.SceneName.Equals("Hover24"))
			{
				AvoidTheLight = !LightsHittedTomb;
			}
		}
		if (!SceneLoader.Instance.Current.IsChallenge)
		{
			StartCoroutine(DelayedSave(false));
			return;
		}
		SceneLoader.Instance.TriesLeft--;
		StartCoroutine(DelayedSave(true));
	}

	private IEnumerator DelayedSave(bool challenge)
	{
		yield return new WaitForSeconds(0.5f);
		if (challenge)
		{
			SceneLoader.Instance.SaveChallenge(Main.Instance.CurrentScene, CurrentStorage);
		}
		else
		{
			SceneLoader.Instance.SaveSceneStorage(Main.Instance.CurrentScene, CurrentStorage);
		}
	}

	public void SetItemCount(int count)
	{
		CollectablesInLevel = count;
	}

	public void SetGhostItemCount(int count)
	{
		if (CurrentStorage != null)
		{
			CurrentStorage.GhostState = count;
		}
	}

	public int CollectablesMax()
	{
		return (CurrentStorage == null || CurrentStorage.GhostState <= 0) ? CollectablesInLevel : CurrentStorage.GhostState;
	}

	public int HighScore()
	{
		return (CurrentStorage != null && CurrentStorage.HighScore > 0f) ? Mathf.CeilToInt(CurrentStorage.HighScore) : 0;
	}

	public void LevelCompleted(float endlessDistance = 0f)
	{
		LevelDistance = endlessDistance;
		GameStats.Instance.LevelCompetions++;
		SaveStorage(true);
	}

	public void Collect()
	{
		CollectebleCollectCount++;
		CollectablePitch += 1f;
		collectedTimer = 0.5f;
		UIController.Instance.CollectableAdded();
		GameStats.Instance.CoinsCollected++;
		if (SceneLoader.Instance.Current.IsChallenge && SceneLoader.Instance.Current.Group == 0 && CollectebleCollectCount >= SceneLoader.Instance.GetChallengeLevelLimit(SceneLoader.Instance.Current.SceneName))
		{
			ChallengeCompleted();
		}
		else if (SceneLoader.Instance.Current.IsChallenge && SceneLoader.Instance.Current.Group == 2 && CollectebleCollectCount >= SceneLoader.Instance.GetChallengeLevelLimit(SceneLoader.Instance.Current.SceneName))
		{
			ChallengeCompleted();
		}
	}

	private void ChallengeCompleted()
	{
		if (UpdateProgress && (bool)UIController.Instance)
		{
			CurrentStorage.CasetteState++;
			GameController.Instance.SetState(GameController.GameState.End);
			GameController.Instance.GetComponentInChildren<SplineWalker>().EndLevel();
		}
	}

	public void ClearToCheckpoint(int collected)
	{
		CollectebleCollectCount = collected;
	}

	public void ClearCollectables(int removed)
	{
		SuperCollectableCounter = 0;
		CollectebleCollectCount -= removed;
	}

	public void ClearCollectables()
	{
		SuperCollectableCounter = 0;
		CollectebleCollectCount = 0;
		CollectebleCollectCountTricks = 0;
	}
}
