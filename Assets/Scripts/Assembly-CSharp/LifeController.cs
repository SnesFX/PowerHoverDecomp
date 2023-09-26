using System.Collections.Generic;
using UnityEngine;

public class LifeController : MonoBehaviour
{
	private const string LIFE_COUNT = "LifeCountr";

	private int LifeSaversMax;

	public static LifeController Instance { get; private set; }

	public int LifeCount { get; set; }

	public int LifeSavers { get; set; }

	public int LifeMax { get; set; }

	private void Awake()
	{
		Instance = this;
		LifeMax = 2;
		LifeCount = ((!GameDataController.Exists("LifeCountr")) ? 2 : GameDataController.Load<int>("LifeCountr"));
		Reset();
	}

	public void UpdateLifeMax(int upgrade, bool fullReset = false)
	{
		LifeMax = upgrade;
		if (fullReset)
		{
			LifeCount = LifeMax;
		}
		Reset();
	}

	public void UpdateLifeSavers(int upgrade)
	{
		LifeSavers = upgrade;
		LifeSaversMax = upgrade;
	}

	public void UseLifeSaver(bool maxLifes, bool withAd)
	{
		if (UnityAnalyticsIntegration.Instance != null)
		{
			UnityAnalyticsIntegration.Instance.MakeEvent("BackupUse", new Dictionary<string, object>
			{
				{
					"stage",
					Main.Instance.CurrentScene
				},
				{ "withAd", withAd },
				{
					"kills",
					LevelStats.Instance.LevelKillCount
				}
			});
		}
		if (LifeSavers > 0)
		{
			LifeSavers--;
		}
		if (maxLifes)
		{
			LifeCount = LifeMax;
		}
		else
		{
			LifeCount = 1;
		}
	}

	public void ChangeLifes(bool add)
	{
		if (add)
		{
			LifeCount++;
		}
		else if (!add && LifeCount > -1)
		{
			LifeCount--;
		}
		GameDataController.Save(LifeCount, "LifeCountr");
	}

	public void Reset()
	{
		if (LifeCount < LifeMax)
		{
			LifeCount = LifeMax;
		}
		LifeSavers = LifeSaversMax;
	}

	public void HardReset()
	{
		LifeCount = 2;
		GameDataController.Save(LifeCount, "LifeCountr");
	}
}
