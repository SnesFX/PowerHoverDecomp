using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenAparaattus : MonoBehaviour
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

	private const int BONUS_VALUE = 100;

	private const int MAXED_BONUS_VALUE = 0;

	private const float POINTER_MAX_ANGLE = 170f;

	private const float FILL_SPEED = 0.2f;

	public Text StageName;

	public TextMesh CollectableCounter;

	public Transform CollectedBar;

	public Transform CollectedBarGhost;

	public List<LampObject> Lamps;

	public Transform PointerObject;

	public Transform PointerRecordObject;

	public Transform[] BatteryRollers;

	public Button LevelButton;

	public GameObject LevelButtonOn;

	public AudioSource audioSource;

	public AudioSource audioSourceNumberRoll;

	public AudioSource audioSourceCompleted;

	public DiamondNotification levelNotification;

	public DiamondNotification scoreNotification;

	public DiamondNotification batteryNotification;

	public Animator maxedOutAnimation;

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

	private Vector3 pointerVectorTemp;

	private Vector3 batteryVectorTemp;

	private int[] batteryRollTargets = new int[4];

	private int[] missionLimits = new int[4] { 40, 60, 90, 100 };

	private bool pausedDuringEndscreen;

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
		if (Main.Instance.TestingLevel)
		{
			LevelStats.Instance.SetItemCount(50);
			LevelStats.Instance.Collect();
			LevelStats.Instance.Collect();
			LevelStats.Instance.Collect();
			LevelStats.Instance.Collect();
			LevelStats.Instance.Collect();
			LevelStats.Instance.Collect();
			LevelStats.Instance.Collect();
			LevelStats.Instance.Collect();
			LevelStats.Instance.Collect();
			LevelStats.Instance.Collect();
		}
		if (NoAdsButton != null && IapManager.Instance != null)
		{
			NoAdsButton.SetActive(!IapManager.Instance.Unlocked);
			if (NoAdsButton.activeSelf)
			{
				NoAdsButton.GetComponentInChildren<IapButton>().SetTexts(string.Empty, IapManager.Instance.GetNoAdsPrize());
			}
		}
		StageName.text = SceneLoader.Instance.Current.VisibleName;
		score = LevelStats.Instance.CollectebleCollectCount;
		oldBestScore = Mathf.FloorToInt(LevelStats.Instance.PreviousBestHighScore);
		float num = LevelStats.Instance.CollectablesMax();
		CollectableCounter.text = string.Format("{0}/{1}", score, LevelStats.Instance.CollectablesMax());
		batteryVectorTemp = new Vector3(CollectedBar.transform.localScale.x, (float)oldBestScore / num, CollectedBar.transform.localScale.z);
		CollectedBarGhost.transform.localScale = batteryVectorTemp;
		batteryVectorTemp.y = 0f;
		CollectedBar.transform.localScale = batteryVectorTemp;
		gainedScore = 0;
		for (int i = 0; i < missionLimits.Length; i++)
		{
			float num2 = Mathf.CeilToInt(num * (float)missionLimits[i] / 100f);
			bool flag = (float)oldBestScore >= num2;
			if (i != 3)
			{
				if (flag)
				{
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
					gainedScore += 100;
				}
				else if (flag)
				{
					levelNotification.Notify(string.Format("+{0}", 100));
				}
				LevelButton.interactable = flag;
				LevelButtonOn.SetActive(flag);
				break;
			case Limit.Bonus:
				if (!flag && (float)score >= num2)
				{
					gainedScore += 200;
				}
				else if (flag)
				{
					scoreNotification.Notify(string.Format("+{0}", 200));
				}
				break;
			case Limit.Bonus2:
				if (!flag && (float)score >= num2)
				{
					gainedScore += 300;
				}
				else if (flag)
				{
					batteryNotification.Notify(string.Format("+{0}", 300));
				}
				break;
			case Limit.Maxed:
				if (!flag && (float)score >= num2)
				{
					gainedScore = gainedScore;
				}
				else if (flag)
				{
					maxedOutAnimation.enabled = true;
					maxedOutAnimation.Play("EndMaxOutIdle", -1, 0f);
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
		pointerVectorTemp = Vector3.zero;
		PointerObject.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 170f * ((float)oldBestScore / num)));
		PointerRecordObject.gameObject.SetActive(false);
		scoreFill = (float)score / num;
		fillSpeedMultiplier = 1f + 0.2f * (num / (float)score);
		fillSpeedMultiplierMin = fillSpeedMultiplier;
		WaitingTimer = -1f;
		StateTimer = 0f;
		State = ApparatusState.Waiting;
	}

	private void Update()
	{
		switch (State)
		{
		case ApparatusState.None:
			break;
		case ApparatusState.Done:
			StateTimer += Time.deltaTime;
			audioSourceNumberRoll.pitch = 1f + StateTimer * 0.7f;
			if (StateTimer < 1f)
			{
				UpdateBatteryRolls(Time.deltaTime);
			}
			else if (audioSourceNumberRoll.isPlaying)
			{
				audioSourceNumberRoll.Stop();
				if (gainedScore > 0)
				{
					audioSourceCompleted.Play();
				}
				SetBatteryDigits(gainedScore);
				GameStats.Instance.SaveStats();
				State = ApparatusState.None;
			}
			break;
		case ApparatusState.Waiting:
			WaitingTimer += Time.deltaTime;
			if (WaitingTimer > 0f)
			{
				State = ApparatusState.Rolling;
				WaitingTimer = 0f;
			}
			break;
		case ApparatusState.Rolling:
		{
			UpdateSpeeding();
			StateTimer += Time.deltaTime * fillSpeedMultiplier;
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
			batteryVectorTemp.y = num2;
			CollectedBar.transform.localScale = batteryVectorTemp;
			pointerVectorTemp.z = 170f * num2;
			if (PointerObject.localRotation.eulerAngles.z < pointerVectorTemp.z)
			{
				PointerObject.localRotation = Quaternion.Euler(pointerVectorTemp);
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
						levelNotification.Notify(string.Format("+{0}", 100));
						LevelButton.interactable = true;
						LevelButtonOn.SetActive(true);
						break;
					case Limit.Bonus:
						scoreNotification.Notify(string.Format("+{0}", 200));
						break;
					case Limit.Bonus2:
						batteryNotification.Notify(string.Format("+{0}", 300));
						break;
					case Limit.Maxed:
						maxedOutAnimation.enabled = true;
						maxedOutAnimation.Play("EndMaxOut", -1, 0f);
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
			fillSpeedMultiplier -= 5f * Time.deltaTime;
		}
		else if (skipPressed && fillSpeedMultiplier < 10f)
		{
			fillSpeedMultiplier += 10f * Time.deltaTime;
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
