using System;
using System.Collections.Generic;
using SA.Common.Pattern;
using UnityEngine;

public class AchievementController : MonoBehaviour
{
	public class LocalAchievement
	{
		public string Id { get; set; }

		public string LocalId { get; set; }

		public bool Achieved { get; set; }

		public string Description { get; set; }

		public float Value { get; set; }

		public float CompletionValue { get; set; }

		public string Title { get; set; }
	}

	public const string ACHIEVEMENT_PREFIX = "LcAchievement_";

	private List<LocalAchievement> Achievements;

	private Queue<Action> postActions = new Queue<Action>();

	private List<Action> analyzers;

	public static AchievementController Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
		analyzers = new List<Action>();
		CreateLocalAchievements();
		LoadLocalAchievements();
		CreateAnalyzers();
	}

	private void CreateAnalyzers()
	{
		analyzers.Add(AnalyzeScore);
	}

	private void FixedUpdate()
	{
		if (postActions.Count > 0)
		{
			Action action = postActions.Dequeue();
			action();
		}
		for (int i = 0; i < analyzers.Count; i++)
		{
			analyzers[i]();
		}
	}

	public void AchievementsRetrieved(GooglePlayResult achievements)
	{
		GooglePlayManager.ActionAchievementsLoaded -= Instance.AchievementsRetrieved;
		if (!achievements.IsSucceeded)
		{
			return;
		}
		foreach (GPAchievement achievement in Singleton<GooglePlayManager>.Instance.Achievements)
		{
			LocalAchievement achievementByNativeId = GetAchievementByNativeId(achievement.Id);
			if (achievementByNativeId != null && achievementByNativeId.Achieved && achievement.State == GPAchievementState.STATE_UNLOCKED)
			{
				PostAchievement(achievementByNativeId);
			}
		}
	}

	private void AnalyzeScore()
	{
		Analyze("A2", GameStats.Instance.RegenLifesCollected);
		Analyze("A3", GameStats.Instance.PoweredUp);
		Analyze("A5", LevelStats.Instance.WormFood);
		Analyze("A8", GameStats.Instance.BestDistance);
		Analyze("A10", LifeController.Instance.LifeCount);
		Analyze("A12", GameStats.Instance.NoCrashesOnLevels);
		Analyze("A13", LevelStats.Instance.Explorer ? 1 : 0);
		Analyze("A14", GameStats.Instance.GameOvers);
		Analyze("A15", GameStats.Instance.CoinsCollected);
		Analyze("A16", LevelStats.Instance.AvoidTheLight ? 1 : 0);
		Analyze("A17", GameStats.Instance.Chapter2 ? 1 : 0);
		Analyze("A18", GameStats.Instance.Bosses5000);
		Analyze("A19", GameStats.Instance.Ducky ? 1 : 0);
	}

	private void Analyze(string id, float value)
	{
		LocalAchievement achievement = GetAchievement(id);
		if (achievement == null)
		{
			return;
		}
		achievement.Value = value;
		if (!achievement.Achieved && value >= achievement.CompletionValue)
		{
			achievement.Achieved = true;
			PostAchievement(achievement);
			SaveLocalAchievements();
			if ((bool)UIController.Instance)
			{
				UIController.Instance.achievementNotification.Notify();
			}
		}
	}

	private LocalAchievement GetAchievementByNativeId(string id)
	{
		return Achievements.Find((LocalAchievement x) => x.Id == id);
	}

	public LocalAchievement GetAchievement(string id)
	{
		return Achievements.Find((LocalAchievement x) => x.LocalId == id);
	}

	public List<LocalAchievement> GetAchievements()
	{
		return Achievements;
	}

	private void CreateLocalAchievements()
	{
		Achievements = new List<LocalAchievement>
		{
			new LocalAchievement
			{
				Id = "CgkIsP-AhsgdEAIQAw",
				LocalId = "A2",
				CompletionValue = 20f,
				Title = "MainMenu.AchievementCollector",
				Description = "MainMenu.AchievementCollectablesCollected",
				Achieved = false
			},
			new LocalAchievement
			{
				Id = "CgkIsP-AhsgdEAIQBQ",
				LocalId = "A3",
				CompletionValue = 100f,
				Title = "MainMenu.AchievementPowerUp",
				Description = "MainMenu.AchievementDiamondsCollected",
				Achieved = false
			},
			new LocalAchievement
			{
				Id = "CgkIsP-AhsgdEAIQBg",
				LocalId = "A5",
				CompletionValue = 10f,
				Title = "MainMenu.AchievementEaten",
				Description = "MainMenu.AchievementWormFood",
				Achieved = false
			},
			new LocalAchievement
			{
				Id = "CgkIsP-AhsgdEAIQBw",
				LocalId = "A8",
				CompletionValue = 6000f,
				Title = "MainMenu.AchievementLongRunTittle",
				Description = "MainMenu.AchievementLongRun",
				Achieved = false
			},
			new LocalAchievement
			{
				Id = "CgkIsP-AhsgdEAIQCA",
				LocalId = "A10",
				CompletionValue = 10f,
				Title = "MainMenu.AchievementLives",
				Description = "MainMenu.AchievementLifeHoarder",
				Achieved = false
			},
			new LocalAchievement
			{
				Id = "CgkIsP-AhsgdEAIQCQ",
				LocalId = "A12",
				CompletionValue = 10f,
				Title = "MainMenu.AchievementUnscathed",
				Description = "MainMenu.AchievementUnscathed",
				Achieved = false
			},
			new LocalAchievement
			{
				Id = "CgkIsP-AhsgdEAIQDQ",
				LocalId = "A13",
				CompletionValue = 1f,
				Title = "MainMenu.AchievementExplorer",
				Description = "MainMenu.AchievementExplorer",
				Achieved = false
			},
			new LocalAchievement
			{
				Id = "CgkIsP-AhsgdEAIQDg",
				LocalId = "A14",
				CompletionValue = 20f,
				Title = "MainMenu.ContinueCount",
				Description = "MainMenu.ContinueCount",
				Achieved = false
			},
			new LocalAchievement
			{
				Id = "CgkIsP-AhsgdEAIQDw",
				LocalId = "A15",
				CompletionValue = 400f,
				Title = "MainMenu.CoinsCollected",
				Description = "MainMenu.CoinsCollected",
				Achieved = false
			},
			new LocalAchievement
			{
				Id = "CgkIsP-AhsgdEAIQEA",
				LocalId = "A16",
				CompletionValue = 1f,
				Title = "MainMenu.AchievementAvoid",
				Description = "MainMenu.AchievementAvoidLights",
				Achieved = false
			},
			new LocalAchievement
			{
				Id = "CgkIsP-AhsgdEAIQFg",
				LocalId = "A17",
				CompletionValue = 1f,
				Title = "MainMenu.AchievementChapter2",
				Description = "MainMenu.AchievementChapter2Desc",
				Achieved = false
			},
			new LocalAchievement
			{
				Id = "CgkIsP-AhsgdEAIQFw",
				LocalId = "A18",
				CompletionValue = 5f,
				Title = "MainMenu.AchievementBoss5000",
				Description = "MainMenu.AchievementBoss5000Desc",
				Achieved = false
			},
			new LocalAchievement
			{
				Id = "CgkIsP-AhsgdEAIQGA",
				LocalId = "A19",
				CompletionValue = 1f,
				Title = "MainMenu.AchievementDucky",
				Description = "MainMenu.AchievementDuckyDesc",
				Achieved = false
			}
		};
	}

	public void ClearLocalAchievements()
	{
		foreach (LocalAchievement achievement in Achievements)
		{
			string identifier = string.Format("{0}{1}", "LcAchievement_", achievement.Id);
			if (GameDataController.Exists(identifier))
			{
				GameDataController.Delete(identifier);
			}
			achievement.Value = 0f;
			achievement.Achieved = false;
		}
	}

	private void LoadLocalAchievements()
	{
		foreach (LocalAchievement achievement in Achievements)
		{
			string identifier = string.Format("{0}{1}", "LcAchievement_", achievement.Id);
			achievement.Achieved = GameDataController.Exists(identifier) && GameDataController.Load<bool>(identifier);
		}
	}

	private void SaveLocalAchievements()
	{
		foreach (LocalAchievement achievement in Achievements)
		{
			string identifier = string.Format("{0}{1}", "LcAchievement_", achievement.Id);
			GameDataController.Save(achievement.Achieved, identifier);
		}
	}

	private void PostAchievement(LocalAchievement localAchievement)
	{
		GameCenter.Instance.SubmitAchievement(100f, localAchievement.Id, false);
	}
}
