using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStats : MonoBehaviour
{
	private float sessionTimer;

	public static GameStats Instance { get; private set; }

	public int GamesPlayed { get; set; }

	public float TimePlayed { get; set; }

	public int CoinsCollected { get; set; }

	public int DiamondsCollected { get; set; }

	public int DiamondsBought { get; set; }

	public float ScoreTotal { get; set; }

	public int LevelCompetions { get; set; }

	public int GameOvers { get; set; }

	public int DeathCount { get; set; }

	public int BreakStuff { get; set; }

	public bool Breakables { get; set; }

	public int NoCrashesOnLevels { get; set; }

	public bool BigSpender { get; set; }

	public int TotalBattery { get; set; }

	public int UnlockedBattery { get; set; }

	public int RegenLifesCollected { get; set; }

	public float PoweredUp { get; set; }

	public float BestDistance { get; set; }

	public int ChallengeMoney { get; set; }

	public int ChallengeMoneySpent { get; set; }

	public int ChallengeTries { get; set; }

	public float ChallengeDistance { get; set; }

	public bool Chapter2 { get; set; }

	public int Bosses5000 { get; set; }

	public bool Ducky { get; set; }

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		sessionTimer = Time.time;
		if (GameDataController.AndroidInternalPathReady)
		{
			ReadStats();
		}
	}

	private void OnApplicationPause(bool pausing)
	{
		if (pausing)
		{
			SaveStats();
		}
	}

	private void OnApplicationQuit()
	{
		SaveStats();
	}

	public long GetPlayedTime()
	{
		TimePlayed += Time.time - sessionTimer;
		sessionTimer = Time.time;
		return (long)TimePlayed * 1000;
	}

	public void ReadStats()
	{
		if (GamesPlayed <= 0)
		{
			GamesPlayed = (GameDataController.Exists("GamesPlayed") ? GameDataController.Load<int>("GamesPlayed") : 0);
			TimePlayed = ((!GameDataController.Exists("TimePlayed")) ? 0f : GameDataController.Load<float>("TimePlayed"));
			CoinsCollected = (GameDataController.Exists("CoinsCollected") ? GameDataController.Load<int>("CoinsCollected") : 0);
			DiamondsCollected = (GameDataController.Exists("DiamondsCollected") ? GameDataController.Load<int>("DiamondsCollected") : 0);
			ScoreTotal = ((!GameDataController.Exists("ScoreTotall")) ? 0f : GameDataController.Load<float>("ScoreTotall"));
			LevelCompetions = (GameDataController.Exists("LevelCompetions") ? GameDataController.Load<int>("LevelCompetions") : 0);
			DiamondsBought = (GameDataController.Exists("DiamondsBought") ? GameDataController.Load<int>("DiamondsBought") : 0);
			GameOvers = (GameDataController.Exists("GameOvers") ? GameDataController.Load<int>("GameOvers") : 0);
			DeathCount = (GameDataController.Exists("DeathCount") ? GameDataController.Load<int>("DeathCount") : 0);
			BreakStuff = (GameDataController.Exists("BreakStuff") ? GameDataController.Load<int>("BreakStuff") : 0);
			Breakables = GameDataController.Exists("Breakables") && GameDataController.Load<bool>("Breakables");
			NoCrashesOnLevels = (GameDataController.Exists("NoCrashesOnLevels") ? GameDataController.Load<int>("NoCrashesOnLevels") : 0);
			BigSpender = GameDataController.Exists("BigSpender") && GameDataController.Load<bool>("BigSpender");
			TotalBattery = ((!GameDataController.Exists("TotalBattery")) ? 5 : GameDataController.Load<int>("TotalBattery"));
			UnlockedBattery = (GameDataController.Exists("UnlockedBattery") ? GameDataController.Load<int>("UnlockedBattery") : 0);
			RegenLifesCollected = (GameDataController.Exists("RegenLifesCollected") ? GameDataController.Load<int>("RegenLifesCollected") : 0);
			PoweredUp = ((!GameDataController.Exists("PoweredUp")) ? 0f : GameDataController.Load<float>("PoweredUp"));
			BestDistance = ((!GameDataController.Exists("BestDistance")) ? 0f : GameDataController.Load<float>("BestDistance"));
			ChallengeMoney = (GameDataController.Exists("ChallengeMoney") ? GameDataController.Load<int>("ChallengeMoney") : 0);
			ChallengeMoneySpent = (GameDataController.Exists("ChallengeMoneySpent") ? GameDataController.Load<int>("ChallengeMoneySpent") : 0);
			ChallengeTries = (GameDataController.Exists("ChallengeTries") ? GameDataController.Load<int>("ChallengeTries") : 0);
			ChallengeDistance = ((!GameDataController.Exists("ChallengeDistance")) ? 0f : GameDataController.Load<float>("ChallengeDistance"));
			Chapter2 = GameDataController.Exists("Chapter2") && GameDataController.Load<bool>("Chapter2");
			Bosses5000 = (GameDataController.Exists("Bosses5000") ? GameDataController.Load<int>("Bosses5000") : 0);
			Ducky = GameDataController.Exists("Ducky") && GameDataController.Load<bool>("Ducky");
		}
	}

	public void SaveStats()
	{
		GameDataController.Save(GamesPlayed, "GamesPlayed");
		GameDataController.Save(TimePlayed, "TimePlayed");
		GameDataController.Save(CoinsCollected, "CoinsCollected");
		GameDataController.Save(DiamondsCollected, "DiamondsCollected");
		GameDataController.Save(ScoreTotal, "ScoreTotall");
		GameDataController.Save(LevelCompetions, "LevelCompetions");
		GameDataController.Save(DiamondsBought, "DiamondsBought");
		GameDataController.Save(GameOvers, "GameOvers");
		GameDataController.Save(DeathCount, "DeathCount");
		GameDataController.Save(BreakStuff, "BreakStuff");
		GameDataController.Save(Breakables, "Breakables");
		GameDataController.Save(NoCrashesOnLevels, "NoCrashesOnLevels");
		GameDataController.Save(BigSpender, "BigSpender");
		GameDataController.Save(TotalBattery, "TotalBattery");
		GameDataController.Save(UnlockedBattery, "UnlockedBattery");
		GameDataController.Save(RegenLifesCollected, "RegenLifesCollected");
		GameDataController.Save(PoweredUp, "PoweredUp");
		GameDataController.Save(BestDistance, "BestDistance");
		GameDataController.Save(ChallengeMoney, "ChallengeMoney");
		GameDataController.Save(ChallengeMoneySpent, "ChallengeMoneySpent");
		GameDataController.Save(ChallengeTries, "ChallengeTries");
		GameDataController.Save(ChallengeDistance, "ChallengeDistance");
		GameDataController.Save(Chapter2, "Chapter2");
		GameDataController.Save(Bosses5000, "Bosses5000");
		GameDataController.Save(Ducky, "Ducky");
	}

	public Dictionary<string, object> ToJson()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("GamesPlayed", GamesPlayed);
		dictionary.Add("TimePlayed", TimePlayed);
		dictionary.Add("CoinsCollected", CoinsCollected);
		dictionary.Add("DiamondsCollected", DiamondsCollected);
		dictionary.Add("ScoreTotall", ScoreTotal);
		dictionary.Add("LevelCompetions", LevelCompetions);
		dictionary.Add("DiamondsBought", DiamondsBought);
		dictionary.Add("GameOvers", GameOvers);
		dictionary.Add("DeathCount", DeathCount);
		dictionary.Add("BreakStuff", BreakStuff);
		dictionary.Add("Breakables", Breakables);
		dictionary.Add("NoCrashesOnLevels", NoCrashesOnLevels);
		dictionary.Add("BigSpender", BigSpender);
		dictionary.Add("TotalBattery", TotalBattery);
		dictionary.Add("UnlockedBattery", UnlockedBattery);
		dictionary.Add("RegenLifesCollected", RegenLifesCollected);
		dictionary.Add("PoweredUp", PoweredUp);
		dictionary.Add("BestDistance", BestDistance);
		dictionary.Add("ChallengeMoney", ChallengeMoney);
		dictionary.Add("ChallengeMoneySpent", ChallengeMoneySpent);
		dictionary.Add("ChallengeTries", ChallengeTries);
		dictionary.Add("ChallengeDistance", ChallengeDistance);
		dictionary.Add("Chapter2", Chapter2);
		dictionary.Add("Bosses5000", Bosses5000);
		dictionary.Add("Ducky", Ducky);
		return dictionary;
	}

	public void FromJson(Dictionary<string, object> json)
	{
		GamesPlayed = Mathf.Max(GamesPlayed, json.ContainsKey("GamesPlayed") ? Convert.ToInt32(json["GamesPlayed"]) : 0);
		TimePlayed = Mathf.Max(TimePlayed, (!json.ContainsKey("TimePlayed")) ? 0f : Convert.ToSingle(json["TimePlayed"]));
		CoinsCollected = Mathf.Max(CoinsCollected, json.ContainsKey("CoinsCollected") ? Convert.ToInt32(json["CoinsCollected"]) : 0);
		DiamondsCollected = Mathf.Max(DiamondsCollected, json.ContainsKey("DiamondsCollected") ? Convert.ToInt32(json["DiamondsCollected"]) : 0);
		ScoreTotal = Mathf.Max(ScoreTotal, (!json.ContainsKey("ScoreTotall")) ? 0f : Convert.ToSingle(json["ScoreTotall"]));
		LevelCompetions = Mathf.Max(LevelCompetions, json.ContainsKey("LevelCompetions") ? Convert.ToInt32(json["LevelCompetions"]) : 0);
		DiamondsBought = Mathf.Max(DiamondsBought, json.ContainsKey("DiamondsBought") ? Convert.ToInt32(json["DiamondsBought"]) : 0);
		GameOvers = Mathf.Max(GameOvers, json.ContainsKey("GameOvers") ? Convert.ToInt32(json["GameOvers"]) : 0);
		DeathCount = Mathf.Max(DeathCount, json.ContainsKey("DeathCount") ? Convert.ToInt32(json["DeathCount"]) : 0);
		BreakStuff = Mathf.Max(BreakStuff, json.ContainsKey("BreakStuff") ? Convert.ToInt32(json["BreakStuff"]) : 0);
		Breakables = Breakables || (json.ContainsKey("Breakables") && (bool)json["Breakables"]);
		NoCrashesOnLevels = Mathf.Max(NoCrashesOnLevels, json.ContainsKey("NoCrashesOnLevels") ? Convert.ToInt32(json["NoCrashesOnLevels"]) : 0);
		BigSpender = BigSpender || (json.ContainsKey("BigSpender") && (bool)json["BigSpender"]);
		TotalBattery = Mathf.Max(TotalBattery, (!json.ContainsKey("TotalBattery")) ? 5 : Convert.ToInt32(json["TotalBattery"]));
		UnlockedBattery = Mathf.Max(UnlockedBattery, json.ContainsKey("UnlockedBattery") ? Convert.ToInt32(json["UnlockedBattery"]) : 0);
		RegenLifesCollected = Mathf.Max(RegenLifesCollected, json.ContainsKey("RegenLifesCollected") ? Convert.ToInt32(json["RegenLifesCollected"]) : 0);
		PoweredUp = Mathf.Max(PoweredUp, (!json.ContainsKey("PoweredUp")) ? 0f : Convert.ToSingle(json["PoweredUp"]));
		BestDistance = Mathf.Max(BestDistance, (!json.ContainsKey("BestDistance")) ? 0f : Convert.ToSingle(json["BestDistance"]));
		ChallengeMoney = Mathf.Max(ChallengeMoney, json.ContainsKey("ChallengeMoney") ? Convert.ToInt32(json["ChallengeMoney"]) : 0);
		ChallengeMoneySpent = Mathf.Max(ChallengeMoneySpent, json.ContainsKey("ChallengeMoneySpent") ? Convert.ToInt32(json["ChallengeMoneySpent"]) : 0);
		ChallengeTries = Mathf.Max(ChallengeTries, json.ContainsKey("ChallengeTries") ? Convert.ToInt32(json["ChallengeTries"]) : 0);
		ChallengeDistance = Mathf.Max(ChallengeDistance, (!json.ContainsKey("ChallengeDistance")) ? 0f : Convert.ToSingle(json["ChallengeDistance"]));
		Chapter2 = Chapter2 || (json.ContainsKey("Chapter2") && (bool)json["Chapter2"]);
		Bosses5000 = Mathf.Max(Bosses5000, json.ContainsKey("Bosses5000") ? Convert.ToInt32(json["Bosses5000"]) : 0);
		Ducky = Ducky || (json.ContainsKey("Ducky") && (bool)json["Ducky"]);
		SaveStats();
	}
}
