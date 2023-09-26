using System.Collections.Generic;
using UnityEngine;

public class ItemCountController : MonoBehaviour
{
	public Transform BatteryObject;

	public Transform[] BatteryRollers;

	private int[] batteryRollTargets = new int[5];

	public AudioSource numberRoll;

	public AudioSource numberEnd;

	private int currency;

	private float rollTimer;

	private float pitchDir = 0.5f;

	private void Start()
	{
		SetBatteryDigits(GameStats.Instance.TotalBattery);
		currency = GameStats.Instance.TotalBattery;
		if ((float)Screen.width / (float)Screen.height < 1.5f)
		{
			BatteryObject.localPosition = new Vector3(BatteryObject.localPosition.x, BatteryObject.localPosition.y, 93f);
		}
		if (DeviceSettings.Instance != null && DeviceSettings.Instance.RunningOnTV)
		{
			Vector3 localPosition = BatteryObject.localPosition;
			localPosition.x -= 30f;
			BatteryObject.localPosition = localPosition;
		}
	}

	private void FixedUpdate()
	{
		if (!(GameStats.Instance != null))
		{
			return;
		}
		if (currency != GameStats.Instance.TotalBattery && rollTimer <= 0f)
		{
			int score = GameStats.Instance.TotalBattery - currency;
			CalculateRounds(currency, score);
			currency = GameStats.Instance.TotalBattery;
			rollTimer = 1f;
			numberRoll.Play();
		}
		if (rollTimer > 0f)
		{
			rollTimer -= Time.fixedDeltaTime;
			numberRoll.pitch = 1f + (1f - rollTimer) * pitchDir;
			UpdateBatteryRolls(Time.fixedDeltaTime);
			if (rollTimer <= 0f && GameStats.Instance.TotalBattery == currency)
			{
				pitchDir = 0.5f;
				numberRoll.Stop();
				numberEnd.Play();
				SetBatteryDigits(GameStats.Instance.TotalBattery);
			}
		}
	}

	private void CalculateRounds(int current, int score)
	{
		int num = score / 10 * 360 + score % 10 * 36;
		int num2 = 36 * (Mathf.RoundToInt((current + score) / 10) - Mathf.RoundToInt(current / 10));
		int num3 = 36 * (Mathf.RoundToInt((current + score) / 100) - Mathf.RoundToInt(current / 100));
		int num4 = 36 * (Mathf.RoundToInt((current + score) / 1000) - Mathf.RoundToInt(current / 1000));
		int num5 = 36 * (Mathf.RoundToInt((current + score) / 10000) - Mathf.RoundToInt(current / 10000));
		batteryRollTargets[0] = num;
		batteryRollTargets[1] = num2;
		batteryRollTargets[2] = num3;
		batteryRollTargets[3] = num4;
		batteryRollTargets[4] = num5;
	}

	private void UpdateBatteryRolls(float t)
	{
		for (int i = 0; i < batteryRollTargets.Length; i++)
		{
			if (batteryRollTargets[i] != 0)
			{
				float xAngle = (float)batteryRollTargets[i] * t;
				BatteryRollers[i].Rotate(xAngle, 0f, 0f);
			}
		}
	}

	private void SetBatteryDigits(int value)
	{
		if (value == 0)
		{
			for (int i = 0; i < BatteryRollers.Length; i++)
			{
				BatteryRollers[i].localRotation = Quaternion.Euler(Vector3.zero);
			}
			return;
		}
		List<byte> list = new List<byte>(BatteryRollers.Length);
		while (value > 0)
		{
			byte item = (byte)(value % 10);
			list.Insert(0, item);
			value /= 10;
		}
		list.Reverse();
		for (int j = 0; j < list.Count; j++)
		{
			Vector3 zero = Vector3.zero;
			zero.x = (float)(int)list[j] * 36f;
			BatteryRollers[j].localRotation = Quaternion.Euler(zero);
		}
	}
}
