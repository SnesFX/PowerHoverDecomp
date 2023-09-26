using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SA.Common.Pattern;
using UnityEngine;

public class GameCenter : MonoBehaviour
{
	public class Leaderboard
	{
		public int MyRank { get; private set; }

		public List<LeaderboardScore> Scores { get; private set; }

		public Leaderboard(Dictionary<int, GPScore> scores, GPScore currentPlayerRank)
		{
			if (currentPlayerRank != null)
			{
				if (DEBUG)
				{
					Debug.Log("MyRank = " + currentPlayerRank.Rank);
				}
				MyRank = currentPlayerRank.Rank;
			}
			Scores = new List<LeaderboardScore>(scores.Count);
			foreach (GPScore value in scores.Values)
			{
				if (DEBUG)
				{
					Debug.Log("Friend " + value.Player.name + " rank " + value.Rank + " score " + value.LongScore);
				}
				if (currentPlayerRank.PlayerId.Equals(value.PlayerId))
				{
					UpdateRanking(value.Rank);
				}
				Scores.Add(new LeaderboardScore(value.LongScore, value.Player.name, value.Rank));
			}
			Scores.Sort();
			if (!DEBUG)
			{
				return;
			}
			foreach (LeaderboardScore score in Scores)
			{
				Debug.Log("sorted testing for " + score.UserID + " rank " + score.Rank + " score " + score.Score);
			}
		}

		public void UpdateRanking(int rank)
		{
			if (DEBUG)
			{
				Debug.Log("MyRank = " + MyRank + " new " + rank);
			}
			MyRank = ((MyRank > 0) ? Mathf.Min(MyRank, rank) : rank);
		}
	}

	public class LeaderboardScore : IComparable<LeaderboardScore>
	{
		public float Score;

		public string UserID;

		public int Rank;

		public LeaderboardScore(float score, string id, int rank)
		{
			Score = score;
			UserID = id;
			Rank = rank;
		}

		public int CompareTo(LeaderboardScore comp)
		{
			return Rank.CompareTo(comp.Rank);
		}
	}

	public static string STORAGE_CONNECT = "CPlayServicesDismissed";

	private static bool DEBUG;

	private const float REFRESH_INTERVAL = 90f;

	private string currentFetch = string.Empty;

	private bool Initialized;

	private Dictionary<string, string> LeaderboardMap = new Dictionary<string, string>
	{
		{ "Endless9", "CgkIsP-AhsgdEAIQAA" },
		{ "Endless5", "CgkIsP-AhsgdEAIQAQ" },
		{ "Endless6", "CgkIsP-AhsgdEAIQAg" },
		{ "Endless10", "CgkIsP-AhsgdEAIQDA" },
		{ "Endless12", "CgkIsP-AhsgdEAIQFQ" },
		{ "Challenge1", "CgkIsP-AhsgdEAIQEQ" },
		{ "Challenge2", "CgkIsP-AhsgdEAIQEg" },
		{ "Challenge3", "CgkIsP-AhsgdEAIQEw" },
		{ "Challenge4", "CgkIsP-AhsgdEAIQFA" }
	};

	private Dictionary<string, Leaderboard> leaderboards;

	private int startLoadingIndex;

	private float leaderboardRefreshTimer;

	private bool authenticatee;

	private bool achievementsAfterLogin;

	private string leaderboardsAfterLogin;

	public static GameCenter Instance { get; private set; }

	public bool LoggedIn { get; private set; }

	private void Awake()
	{
		Instance = this;
		startLoadingIndex = 0;
		leaderboards = new Dictionary<string, Leaderboard>();
		Singleton<GooglePlayManager>.Instance.Create();
		GooglePlayConnection.ActionConnectionResultReceived += OnAuthFinished;
		GooglePlayConnection.ActionPlayerDisconnected += Disconnect;
		GooglePlayManager.ActionLeaderboardsLoaded += OnLeaderBoardsLoaded;
		GooglePlayManager.ActionScoresListLoaded += OnScoresListLoaded;
		GooglePlayManager.ActionScoreSubmited += OnScoreSubmitted;
	}

	private void Start()
	{
		if (AchievementController.Instance != null)
		{
			GooglePlayManager.ActionAchievementsLoaded += AchievementController.Instance.AchievementsRetrieved;
		}
	}

	private void OnDestroy()
	{
		if (!Singleton<GooglePlayConnection>.IsDestroyed)
		{
			GooglePlayConnection.ActionConnectionResultReceived -= OnAuthFinished;
		}
		if (!Singleton<GooglePlayManager>.IsDestroyed)
		{
			GooglePlayManager.ActionScoreSubmited -= OnScoreSubmitted;
			GooglePlayManager.ActionLeaderboardsLoaded -= OnLeaderBoardsLoaded;
			GooglePlayManager.ActionScoresListLoaded -= OnScoresListLoaded;
			GooglePlayManager.ActionAchievementsLoaded -= AchievementController.Instance.AchievementsRetrieved;
		}
	}

	private void FixedUpdate()
	{
		if (LoggedIn && Main.Instance != null && leaderboardRefreshTimer > 0f)
		{
			leaderboardRefreshTimer -= Time.fixedDeltaTime;
			if (leaderboardRefreshTimer <= 0f)
			{
				startLoadingIndex = 0;
				if (Main.Instance.InMenu)
				{
					LoadScores(LeaderboardMap.Keys.ElementAt(startLoadingIndex));
				}
			}
		}
		else if (authenticatee)
		{
			authenticatee = false;
			Singleton<GooglePlayConnection>.Instance.Connect();
			Initialized = true;
		}
	}

	private void Update()
	{
		if (authenticatee)
		{
			authenticatee = false;
			Singleton<GooglePlayConnection>.Instance.Connect();
			Initialized = true;
		}
	}

	public void Disconnect()
	{
		Singleton<GooglePlayConnection>.Instance.Disconnect();
		LoggedIn = (Initialized = false);
	}

	public void Authenticate()
	{
		if (Initialized && LoggedIn)
		{
			return;
		}
		if (!GameDataController.Exists("LastScene") && !GameDataController.Exists("ConTimes"))
		{
			GameDataController.Save(1, "ConTimes");
			return;
		}
		if (DEBUG)
		{
			Debug.Log("GooglePlay Authenticate");
		}
		StartCoroutine(ConnectGooglePlayAfter(0.25f));
		Initialized = true;
	}

	private static IEnumerator ConnectGooglePlayAfter(float wait)
	{
		if (GooglePlayConnection.State != GPConnectionState.STATE_CONNECTED)
		{
			float timer = 0f;
			while (timer <= wait)
			{
				timer += Time.deltaTime;
				yield return null;
			}
			Singleton<GooglePlayConnection>.Instance.Connect();
		}
	}

	private void OnAuthFinished(GooglePlayConnectionResult res)
	{
		if (DEBUG)
		{
			Debug.Log("OnAuthFinished " + res.code);
		}
		if (res.code == GP_ConnectionResultCode.CANCELED || res.code == GP_ConnectionResultCode.INTERRUPTED)
		{
			if (DEBUG)
			{
				Debug.Log("OnAuthFinished store interruption");
			}
			GameDataController.Save(true, STORAGE_CONNECT);
			return;
		}
		GameDataController.Delete(STORAGE_CONNECT);
		if (!res.IsSuccess)
		{
			return;
		}
		if (achievementsAfterLogin)
		{
			achievementsAfterLogin = false;
			Singleton<GooglePlayManager>.Instance.ShowAchievementsUI();
		}
		else if (leaderboardsAfterLogin != null && leaderboardsAfterLogin.Length > 1)
		{
			if (LeaderboardMap.ContainsKey(leaderboardsAfterLogin))
			{
				Singleton<GooglePlayManager>.Instance.ShowLeaderBoardById(LeaderboardMap[leaderboardsAfterLogin]);
			}
			leaderboardsAfterLogin = string.Empty;
		}
		if (!LoggedIn)
		{
			Singleton<GooglePlayManager>.Instance.LoadAchievements();
			Singleton<GooglePlayManager>.Instance.LoadLeaderBoards();
		}
		LoggedIn = true;
	}

	private void OnLeaderBoardsLoaded(GooglePlayResult result)
	{
		if (DEBUG)
		{
			Debug.Log("OnLeaderBoardsLoaded");
		}
		GooglePlayManager.ActionLeaderboardsLoaded -= OnLeaderBoardsLoaded;
		if (!result.IsSucceeded)
		{
			return;
		}
		foreach (string value in LeaderboardMap.Values)
		{
			if (Singleton<GooglePlayManager>.Instance.GetLeaderBoard(value) != null)
			{
				GPScore currentPlayerScore = Singleton<GooglePlayManager>.Instance.GetLeaderBoard(value).GetCurrentPlayerScore(GPBoardTimeSpan.ALL_TIME, GPCollectionType.GLOBAL);
				UpdateScore(currentPlayerScore.Rank, value);
			}
		}
		LoadScores(LeaderboardMap.Keys.ElementAt(startLoadingIndex));
	}

	public void SubmitAchievement(float percent, string achievementId, bool isCompleteNotification)
	{
		Singleton<GooglePlayManager>.Instance.UnlockAchievementById(achievementId);
	}

	public void PostScore(float score, string sceneName)
	{
		if (DEBUG)
		{
			Debug.Log("PostScore :: " + sceneName + " score = " + score);
		}
		if (!LoggedIn)
		{
			if (DEBUG)
			{
				Debug.Log("User not authenticated");
			}
		}
		else if (LeaderboardMap.ContainsKey(sceneName))
		{
			if (DEBUG)
			{
				Debug.Log("trying to post to " + LeaderboardMap[sceneName]);
			}
			Singleton<GooglePlayManager>.Instance.SubmitScoreById(LeaderboardMap[sceneName], (long)score, string.Empty);
		}
		else if (DEBUG)
		{
			Debug.Log("PostScore :: Cannot find board id for " + sceneName);
		}
	}

	private void LoadScores(string sceneName)
	{
		if (!LoggedIn)
		{
			if (DEBUG)
			{
				Debug.Log("LoadScores:: User not authenticated");
			}
		}
		else if (LeaderboardMap.ContainsKey(sceneName))
		{
			if (DEBUG)
			{
				Debug.Log("LoadScores :: for " + sceneName);
			}
			string text = LeaderboardMap[sceneName];
			currentFetch = sceneName;
			if (DEBUG)
			{
				Debug.Log("LoadScores:: board  = " + text);
			}
			Singleton<GooglePlayManager>.Instance.LoadTopScores(text, GPBoardTimeSpan.ALL_TIME, GPCollectionType.FRIENDS, 30);
		}
		else if (DEBUG)
		{
			Debug.Log("LoadScores :: Cannot find board id for " + sceneName);
		}
	}

	public Leaderboard GetLeaderboard(string sceneName)
	{
		if (DEBUG)
		{
			Debug.Log("GetLeaderboard :: id for " + sceneName);
		}
		if (leaderboards.ContainsKey(sceneName))
		{
			return leaderboards[sceneName];
		}
		if (DEBUG)
		{
			Debug.Log("GetLeaderboard :: Cannot find board id for " + sceneName);
		}
		return null;
	}

	public int GetLeaderboardRanking(string sceneName)
	{
		if (DEBUG)
		{
			Debug.Log("GetLeaderboardRanking :: id for " + sceneName);
		}
		if (leaderboards.ContainsKey(sceneName))
		{
			return leaderboards[sceneName].MyRank;
		}
		if (DEBUG)
		{
			Debug.Log("GetLeaderboardRanking :: Cannot find board id for " + sceneName);
		}
		return 0;
	}

	public void ShowAchievementsUI()
	{
		if (LoggedIn)
		{
			Singleton<GooglePlayManager>.Instance.ShowAchievementsUI();
			return;
		}
		authenticatee = true;
		achievementsAfterLogin = true;
	}

	public void ShowLeaderboardsUI(string sceneName)
	{
		if (LoggedIn)
		{
			if (LeaderboardMap.ContainsKey(sceneName))
			{
				Singleton<GooglePlayManager>.Instance.ShowLeaderBoardById(LeaderboardMap[sceneName]);
			}
		}
		else if (LeaderboardMap.ContainsKey(sceneName))
		{
			authenticatee = true;
			leaderboardsAfterLogin = sceneName;
		}
	}

	private void OnScoreSubmitted(GP_LeaderboardResult result)
	{
		if (DEBUG)
		{
			Debug.Log("OnScoreSubmitted :: msg " + result.Message.ToString());
		}
		if (result.IsSucceeded)
		{
			if (DEBUG)
			{
				Debug.Log("OnScoreSubmitted :: Score Submitted");
			}
			GPScore currentPlayerScore = result.Leaderboard.GetCurrentPlayerScore(GPBoardTimeSpan.ALL_TIME, GPCollectionType.GLOBAL);
			UpdateScore(currentPlayerScore.Rank, result.Leaderboard.Id);
		}
	}

	private void UpdateScore(int rank, string boardGCId)
	{
		if (DEBUG)
		{
			Debug.Log("UpdateScore :: board = " + boardGCId + "  rank = " + rank);
		}
		if (!LeaderboardMap.ContainsValue(boardGCId))
		{
			return;
		}
		if (DEBUG)
		{
			Debug.Log("UpdateScore :: LeaderboardMap has id " + boardGCId);
		}
		string key = LeaderboardMap.FirstOrDefault((KeyValuePair<string, string> x) => x.Value == boardGCId).Key;
		if (leaderboards.ContainsKey(key))
		{
			if (DEBUG)
			{
				Debug.Log("UpdateScore :: update rank for " + key + " to " + rank);
			}
			leaderboards[key].UpdateRanking(rank);
		}
		else if (DEBUG)
		{
			Debug.Log("UpdateScore :: Cannot find board id for " + key);
		}
	}

	private void OnScoresListLoaded(GooglePlayResult res)
	{
		if (DEBUG)
		{
			Debug.Log("OnScoresListLoaded start :: " + res.Message.ToString());
		}
		if (res.IsSucceeded)
		{
			string key = currentFetch;
			GPLeaderBoard leaderBoard = Singleton<GooglePlayManager>.Instance.GetLeaderBoard(LeaderboardMap[key]);
			if (leaderBoard == null)
			{
				return;
			}
			if (DEBUG)
			{
				Debug.Log("LoadScores :: Got social " + leaderBoard.SocsialCollection.AllTimeScores.Count);
			}
			if (leaderBoard != null && leaderBoard.SocsialCollection.AllTimeScores.Count > 0)
			{
				if (DEBUG)
				{
					Debug.Log("LoadScores :: Got " + leaderBoard.SocsialCollection.AllTimeScores.Count + " scores");
				}
				Leaderboard value = new Leaderboard(leaderBoard.SocsialCollection.AllTimeScores, leaderBoard.GetCurrentPlayerScore(GPBoardTimeSpan.ALL_TIME, GPCollectionType.GLOBAL));
				if (leaderboards.ContainsKey(key))
				{
					leaderboards[key] = value;
				}
				else
				{
					leaderboards.Add(key, value);
				}
			}
			if (startLoadingIndex < LeaderboardMap.Count - 1)
			{
				startLoadingIndex++;
				LoadScores(LeaderboardMap.Keys.ElementAt(startLoadingIndex));
			}
			else
			{
				leaderboardRefreshTimer = 90f;
			}
		}
		else
		{
			if (DEBUG)
			{
				Debug.Log("OnScoresListLoaded :: Failed to load scores");
			}
			leaderboardRefreshTimer = 90f;
		}
	}
}
