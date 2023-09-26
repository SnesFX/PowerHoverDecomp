using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenEndlessAparaattus : MonoBehaviour
{
	private enum Limit
	{
		Stage = 0,
		Bonus = 1,
		Bonus2 = 2,
		Maxed = 3
	}

	private enum ApparatusState
	{
		None = 0,
		Waiting = 1,
		Rolling = 2,
		Done = 3
	}

	private const float STOP_TIME = 0f;

	private const int BONUS_VALUE = 200;

	private const int MAXED_BONUS_VALUE = 20;

	private const float POINTER_MAX_ANGLE = 170f;

	private const float FILL_SPEED = 0.2f;

	private const float BatteryMax = 1.92f;

	public Text StageName;

	public TextMesh DistanceCounter;

	public Transform CollectedBar;

	public Transform CollectedBarGhost;

	public List<LampObject> Lamps;

	public Transform[] BatteryRollers;

	public AudioSource audioSource;

	public AudioSource audioSourceNumberRoll;

	public AudioSource audioSourceCompleted;

	public DiamondNotification levelNotification;

	public Button LevelButton;

	public GameObject LevelButtonOn;

	public Color MaxedColor;

	public GameObject NoAdsButton;

	private ApparatusState State;

	private float StateTimer;

	private float WaitingTimer;

	private int score;

	private int oldBestScore;

	private float scoreFill;

	private int gainedScore;

	private float fillSpeedMultiplier;

	private float fillSpeedMultiplierMin;

	private int[] levelLimits = new int[4];

	private bool skipPressed;

	private Vector3 batteryVectorTemp;

	private int[] batteryRollTargets = new int[4];

	private int[] missionLimits = new int[4] { 1500, 3000, 4500, 6000 };

	private bool pausedDuringEndscreen;

	private void Start()
	{
		if (Main.Instance.TestingLevel)
		{
			LevelStats.Instance.LevelDistance = 0f;
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			pausedDuringEndscreen = true;
		}
	}

	private void OnEnable()
	{
		State = ApparatusState.None;
		if (SceneLoader.Instance == null || Main.Instance == null)
		{
			return;
		}
		if (NoAdsButton != null && IapManager.Instance != null)
		{
			NoAdsButton.SetActive(!IapManager.Instance.Unlocked);
			if (NoAdsButton.activeSelf)
			{
				NoAdsButton.GetComponentInChildren<IapButton>().SetTexts(string.Empty, IapManager.Instance.GetNoAdsPrize());
			}
		}
		if (Main.Instance.TestingLevel)
		{
			LevelStats.Instance.LevelDistance += 4000f;
			LevelStats.Instance.PreviousBestDistance = LevelStats.Instance.LevelDistance * 0.8f;
		}
		StageName.text = SceneLoader.Instance.Current.VisibleName;
		score = Mathf.FloorToInt(LevelStats.Instance.LevelDistance);
		oldBestScore = Mathf.FloorToInt(LevelStats.Instance.PreviousBestDistance);
		float num = missionLimits[missionLimits.Length - 1];
		if ((float)oldBestScore > num)
		{
			num = oldBestScore;
		}
		DistanceCounter.color = Color.white;
		DistanceCounter.text = string.Format("{0}", score);
		batteryVectorTemp = new Vector3(CollectedBar.transform.localScale.x, (float)oldBestScore / num, CollectedBar.transform.localScale.z);
		batteryVectorTemp.y *= 1.92f;
		batteryVectorTemp.y = Mathf.Min(1.9f, batteryVectorTemp.y);
		CollectedBarGhost.transform.localScale = new Vector3(CollectedBarGhost.transform.localScale.x, batteryVectorTemp.y, CollectedBarGhost.transform.localScale.z);
		batteryVectorTemp.y = 0f;
		CollectedBar.transform.localScale = batteryVectorTemp;
		gainedScore = 0;
		for (int i = 0; i < missionLimits.Length; i++)
		{
			float num2 = missionLimits[i];
			if (i == 0 && SceneLoader.Instance.Current.SceneName.Equals("Endless9"))
			{
				num2 = 1000f;
			}
			bool flag = (float)oldBestScore >= num2;
			if (i != 3)
			{
				if (flag)
				{
					SetLampLimit(Lamps[i], Mathf.FloorToInt(num2));
					SetLampOn(Lamps[i], false);
				}
				else
				{
					SetLampLimit(Lamps[i], Mathf.FloorToInt(num2));
				}
			}
			levelLimits[i] = ((!flag) ? Mathf.FloorToInt(num2) : 99999);
			switch ((Limit)i)
			{
			case Limit.Stage:
				if (!flag && (float)score >= num2)
				{
					Main.Instance.UnlockLevel = true;
					gainedScore += 200;
				}
				else if (flag)
				{
					levelNotification.NotifyLocale("IngameUI.Endscreen.Notification.Passed");
				}
				LevelButton.interactable = flag;
				LevelButtonOn.SetActive(flag);
				break;
			case Limit.Bonus:
				if (!flag && (float)score >= num2)
				{
					gainedScore += 400;
				}
				break;
			case Limit.Bonus2:
				if (!flag && (float)score >= num2)
				{
					gainedScore += 800;
				}
				break;
			}
		}
		ClearRollers();
		SetBatteryDigits(0);
		CalculateRounds(0, gainedScore);
		if (!pausedDuringEndscreen)
		{
			GameStats.Instance.TotalBattery += gainedScore;
		}
		else
		{
			pausedDuringEndscreen = false;
		}
		LevelStats.Instance.GainedScore = gainedScore;
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
		scoreFill = (float)score / num;
		fillSpeedMultiplier = 1f + 0.2f * (num / (float)score);
		fillSpeedMultiplierMin = fillSpeedMultiplier;
		WaitingTimer = -0.75f;
		StateTimer = 0f;
		State = ApparatusState.Waiting;
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
			if (StateTimer < 1f)
			{
				UpdateBatteryRolls(Time.fixedDeltaTime);
			}
			else if (audioSourceNumberRoll.isPlaying)
			{
				if (gainedScore > 0)
				{
					audioSourceNumberRoll.Stop();
					audioSourceCompleted.Play();
					GameStats.Instance.SaveStats();
					State = ApparatusState.None;
				}
				SetBatteryDigits(gainedScore);
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
			float num2 = Mathf.Lerp(0f, scoreFill, Mathf.Sin(num * (float)Math.PI * 0.5f));
			int num3 = Mathf.CeilToInt((float)score * Mathf.Sin(num * (float)Math.PI * 0.5f));
			batteryVectorTemp.y = Mathf.Min(1.9f, num2 * 1.92f);
			CollectedBar.transform.localScale = batteryVectorTemp;
			if (batteryVectorTemp.y == 1.9f)
			{
				DistanceCounter.color = MaxedColor;
			}
			for (int i = 0; i < levelLimits.Length; i++)
			{
				if (num3 >= levelLimits[i])
				{
					levelLimits[i] = 9999999;
					if (i != 3)
					{
						SetLampOn(Lamps[i], true);
					}
					State = ApparatusState.Waiting;
					WaitingTimer = -0f;
					switch ((Limit)i)
					{
					case Limit.Stage:
						LevelButton.interactable = true;
						LevelButtonOn.SetActive(true);
						levelNotification.NotifyLocale("IngameUI.Endscreen.Notification.StageUnlocked");
						break;
					}
				}
			}
			if (StateTimer * 0.2f > 1f)
			{
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

	private void SetLampLimit(LampObject lamp, int limit)
	{
		lamp.onLamp.SetActive(false);
		lamp.offLamp.SetActive(true);
		lamp.limitText.text = string.Format("{0}", limit);
	}

	private void SetLampOn(LampObject lamp, bool playAudio)
	{
		lamp.onLamp.SetActive(true);
		lamp.offLamp.SetActive(false);
		if (playAudio)
		{
			lamp.onLamp.GetComponent<AudioSource>().Play();
		}
	}

	private void UpdateBatteryRolls(float t)
	{
		for (int i = 0; i < batteryRollTargets.Length; i++)
		{
			if (batteryRollTargets[i] > 0)
			{
				float xAngle = (float)batteryRollTargets[i] * t;
				BatteryRollers[i].Rotate(xAngle, 0f, 0f);
			}
		}
	}

	private void CalculateRounds(int current, int score)
	{
		int num = score / 10 * 360 + score % 10 * 36;
		int num2 = 36 * (Mathf.RoundToInt((current + score) / 10) - Mathf.RoundToInt(current / 10));
		int num3 = 36 * (Mathf.RoundToInt((current + score) / 100) - Mathf.RoundToInt(current / 100));
		int num4 = 36 * (Mathf.RoundToInt((current + score) / 1000) - Mathf.RoundToInt(current / 1000));
		batteryRollTargets[0] = num;
		batteryRollTargets[1] = num2;
		batteryRollTargets[2] = num3;
		batteryRollTargets[3] = num4;
	}

	private void ClearRollers()
	{
		for (int i = 0; i < BatteryRollers.Length; i++)
		{
			BatteryRollers[i].localRotation = Quaternion.Euler(Vector3.zero);
		}
	}

	private void SetBatteryDigits(int value)
	{
		List<byte> list = new List<byte>();
		while (value > 0)
		{
			byte item = (byte)(value % 10);
			list.Insert(0, item);
			value /= 10;
		}
		list.Reverse();
		for (int i = 0; i < list.Count; i++)
		{
			Vector3 zero = Vector3.zero;
			zero.x = (float)(int)list[i] * 36f;
			BatteryRollers[i].localRotation = Quaternion.Euler(zero);
		}
	}

	public void SkipFill(bool pressing)
	{
		skipPressed = pressing;
	}

	public void BuyNoAds()
	{
		IapManager.Instance.BuyAdsOff();
		StartCoroutine(HideNoAdsButton());
	}

	private IEnumerator HideNoAdsButton()
	{
		yield return new WaitForSeconds(1f);
		if (NoAdsButton != null)
		{
			NoAdsButton.SetActive(false);
		}
	}
}
