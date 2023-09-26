using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeScreenAparaattus : MonoBehaviour
{
	private enum ApparatusState
	{
		None = 0,
		Waiting = 1,
		Rolling = 2,
		Done = 3
	}

	private const float FILL_SPEED = 0.2f;

	public Text StageName;

	public Text TriesLeft;

	public Text Score;

	public Text ChallengeLimit;

	public Text BoltsCollected;

	public Text HighScore;

	public TextMesh Ranking;

	public Text ChallengeNumber;

	public AudioSource audioLvlPassed;

	public AudioSource audioSource;

	public AudioSource audioSourceNumberRoll;

	public AudioSource audioSourceCompleted;

	public GameObject ChallengePassed;

	private ApparatusState State;

	private float StateTimer;

	private float WaitingTimer;

	private int score;

	private int oldBestScore;

	private float scoreFill;

	private int gainedScore;

	private float fillSpeedMultiplier;

	private float fillSpeedMultiplierMin;

	private bool skipPressed;

	private HoverController player;

	private void Awake()
	{
		player = UnityEngine.Object.FindObjectOfType<HoverController>();
	}

	private void Start()
	{
		if (Main.Instance.TestingLevel)
		{
			LevelStats.Instance.LevelDistance = 0f;
		}
	}

	private void OnEnable()
	{
		State = ApparatusState.None;
		if (!(SceneLoader.Instance == null) && !(Main.Instance == null))
		{
			bool flag = false;
			if (player.PlayerState != PlayerState.Dying)
			{
				audioLvlPassed.Play();
				ChallengePassed.SetActive(true);
				flag = true;
			}
			else
			{
				ChallengePassed.SetActive(false);
			}
			if (Main.Instance.TestingLevel)
			{
				LevelStats.Instance.CollectebleCollectCount += 20;
				LevelStats.Instance.PreviousBestHighScore = Mathf.FloorToInt((float)LevelStats.Instance.CollectebleCollectCount * 0.8f);
				LevelStats.Instance.LevelDistance += 4000f;
				LevelStats.Instance.PreviousBestDistance = LevelStats.Instance.LevelDistance * 0.8f;
			}
			switch ((ChallengeType)SceneLoader.Instance.Current.Group)
			{
			case ChallengeType.Distance:
			case ChallengeType.DontFall:
				score = Mathf.FloorToInt(LevelStats.Instance.LevelDistance);
				oldBestScore = Mathf.FloorToInt(LevelStats.Instance.PreviousBestDistance);
				scoreFill = (float)score * 0.9f;
				break;
			case ChallengeType.Collect:
				score = Mathf.FloorToInt(LevelStats.Instance.CollectebleCollectCount);
				oldBestScore = Mathf.FloorToInt(LevelStats.Instance.PreviousBestHighScore);
				scoreFill = (float)(score * score) * 5f;
				break;
			case ChallengeType.DontMiss:
				score = Mathf.FloorToInt(LevelStats.Instance.CollectebleCollectCount);
				oldBestScore = Mathf.FloorToInt(LevelStats.Instance.PreviousBestHighScore);
				scoreFill = (float)(score * score) * 20f;
				break;
			}
			if (flag)
			{
				scoreFill += (float)(SceneLoader.Instance.Current.Storage.CasetteState + 1) * 1000f;
			}
			scoreFill = Mathf.RoundToInt(scoreFill);
			GameStats.Instance.ChallengeMoney += Mathf.RoundToInt(scoreFill);
			int num = Mathf.Max(score, oldBestScore);
			ChallengeLimit.text = string.Format("{0}", SceneLoader.Instance.GetChallengeLevelLimit(Main.Instance.CurrentScene));
			TriesLeft.transform.parent.gameObject.SetActive(false);
			Score.text = string.Format("{0}", score);
			HighScore.text = string.Format("{0}", num);
			StageName.text = SceneLoader.Instance.Current.SceneName;
			ChallengeNumber.text = string.Format("{0}", 1 + SceneLoader.Instance.Current.Storage.CasetteState);
			BoltsCollected.text = string.Empty;
			if (UnityAnalyticsIntegration.Instance != null)
			{
				UnityAnalyticsIntegration.Instance.MakeEvent("LevelCompleted", new Dictionary<string, object>
				{
					{
						"stage",
						Main.Instance.CurrentScene
					},
					{ "score", score },
					{
						"kills",
						LevelStats.Instance.LevelKillCount
					}
				});
			}
			fillSpeedMultiplier = 2f;
			fillSpeedMultiplierMin = fillSpeedMultiplier;
			WaitingTimer = -0.75f;
			StateTimer = 0f;
			if (scoreFill > 0f)
			{
				State = ApparatusState.Waiting;
			}
			else
			{
				State = ApparatusState.Done;
			}
		}
	}

	private void FixedUpdate()
	{
		switch (State)
		{
		case ApparatusState.None:
			break;
		case ApparatusState.Done:
			StateTimer += Time.fixedDeltaTime;
			audioSourceNumberRoll.pitch = 1f + StateTimer * 0.7f;
			if (!(StateTimer < 1f) && audioSourceNumberRoll.isPlaying && gainedScore > 0)
			{
				audioSourceNumberRoll.Stop();
				audioSourceCompleted.Play();
			}
			break;
		case ApparatusState.Waiting:
			WaitingTimer += Time.fixedDeltaTime;
			if (WaitingTimer > 0f)
			{
				State = ApparatusState.Rolling;
				WaitingTimer = 0f;
			}
			break;
		case ApparatusState.Rolling:
		{
			UpdateSpeeding();
			StateTimer += Time.fixedDeltaTime * fillSpeedMultiplier;
			float num = StateTimer * 0.2f;
			if (!audioSource.isPlaying)
			{
				audioSource.Play();
			}
			else
			{
				audioSource.pitch = 1f + num * 0.7f;
			}
			int num2 = Mathf.RoundToInt(Mathf.Lerp(0f, scoreFill, Mathf.Sin(num * (float)Math.PI * 0.5f)));
			BoltsCollected.text = string.Format("{0}", num2);
			if (StateTimer * 0.2f > 1f)
			{
				BoltsCollected.text = string.Format("{0}", (int)scoreFill);
				StateTimer = 0f;
				audioSource.Stop();
				if (gainedScore > 0)
				{
					audioSourceNumberRoll.Play();
				}
				audioSourceCompleted.Play();
				State = ApparatusState.Done;
			}
			break;
		}
		}
	}

	private void UpdateSpeeding()
	{
		if (!skipPressed && fillSpeedMultiplier > fillSpeedMultiplierMin)
		{
			fillSpeedMultiplier -= 5f * Time.fixedDeltaTime;
		}
		else if (skipPressed && fillSpeedMultiplier < 10f)
		{
			fillSpeedMultiplier += 10f * Time.fixedDeltaTime;
		}
	}

	public void SkipFill(bool pressing)
	{
		skipPressed = pressing;
	}
}
