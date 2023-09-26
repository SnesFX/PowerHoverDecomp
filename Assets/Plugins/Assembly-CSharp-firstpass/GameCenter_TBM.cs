using System;
using System.Collections.Generic;
using SA.Common.Models;
using SA.Common.Pattern;
using UnityEngine;

public class GameCenter_TBM : Singleton<GameCenter_TBM>
{
	private Dictionary<string, GK_TBM_Match> _Matches = new Dictionary<string, GK_TBM_Match>();

	public Dictionary<string, GK_TBM_Match> Matches
	{
		get
		{
			return _Matches;
		}
	}

	public List<GK_TBM_Match> MatchesList
	{
		get
		{
			List<GK_TBM_Match> list = new List<GK_TBM_Match>();
			foreach (KeyValuePair<string, GK_TBM_Match> match in _Matches)
			{
				list.Add(match.Value);
			}
			return list;
		}
	}

	public static event Action<GK_TBM_LoadMatchResult> ActionMatchInfoLoaded;

	public static event Action<GK_TBM_LoadMatchesResult> ActionMatchesInfoLoaded;

	public static event Action<GK_TBM_MatchDataUpdateResult> ActionMatchDataUpdated;

	public static event Action<GK_TBM_MatchInitResult> ActionMatchFound;

	public static event Action<GK_TBM_MatchQuitResult> ActionMatchQuit;

	public static event Action<GK_TBM_EndTrunResult> ActionTrunEnded;

	public static event Action<GK_TBM_MatchEndResult> ActionMacthEnded;

	public static event Action<GK_TBM_RematchResult> ActionRematched;

	public static event Action<GK_TBM_MatchRemovedResult> ActionMatchRemoved;

	public static event Action<GK_TBM_MatchInitResult> ActionMatchInvitationAccepted;

	public static event Action<GK_TBM_MatchRemovedResult> ActionMatchInvitationDeclined;

	public static event Action<GK_TBM_Match> ActionPlayerQuitForMatch;

	public static event Action<GK_TBM_MatchTurnResult> ActionTrunReceived;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void LoadMatchesInfo()
	{
	}

	public void LoadMatch(string matchId)
	{
	}

	public void FindMatch(int minPlayers, int maxPlayers, string msg = "", string[] playersToInvite = null)
	{
	}

	public void FindMatchWithNativeUI(int minPlayers, int maxPlayers, string msg = "", string[] playersToInvite = null)
	{
	}

	public void SetPlayerGroup(int group)
	{
	}

	public void SetPlayerAttributes(int attributes)
	{
	}

	public void SaveCurrentTurn(string matchId, byte[] matchData)
	{
	}

	public void EndTurn(string matchId, byte[] matchData, string nextPlayerId)
	{
	}

	public void QuitInTurn(string matchId, GK_TurnBasedMatchOutcome outcome, string nextPlayerId, byte[] matchData)
	{
	}

	public void QuitOutOfTurn(string matchId, GK_TurnBasedMatchOutcome outcome)
	{
	}

	public void EndMatch(string matchId, byte[] matchData)
	{
	}

	public void Rematch(string matchId)
	{
	}

	public void RemoveMatch(string matchId)
	{
	}

	public void AcceptInvite(string matchId)
	{
	}

	public void DeclineInvite(string matchId)
	{
	}

	public void UpdateParticipantOutcome(string matchId, int outcome, string playerId)
	{
	}

	public GK_TBM_Match GetMatchById(string matchId)
	{
		if (_Matches.ContainsKey(matchId))
		{
			return _Matches[matchId];
		}
		return null;
	}

	public static void PrintMatchInfo(GK_TBM_Match match)
	{
		string empty = string.Empty;
		empty += "----------------------------------------\n";
		empty += "Printing basic match info, for \n";
		empty = empty + "Match ID: " + match.Id + "\n";
		empty = string.Concat(empty, "Status:", match.Status, "\n");
		empty = ((match.CurrentParticipant == null) ? (empty + "CurrentPlayerID: ---- \n") : ((match.CurrentParticipant.Player == null) ? (empty + "CurrentPlayerID: ---- \n") : (empty + "CurrentPlayerID: " + match.CurrentParticipant.Player.Id + "\n")));
		empty = empty + "Data: " + match.UTF8StringData + "\n";
		empty += "*******Participants*******\n";
		foreach (GK_TBM_Participant participant in match.Participants)
		{
			empty = ((participant.Player == null) ? (empty + "PlayerId: ---  \n") : (empty + "PlayerId: " + participant.Player.Id + "\n"));
			empty = string.Concat(empty, "Status: ", participant.Status, "\n");
			empty = string.Concat(empty, "MatchOutcome: ", participant.MatchOutcome, "\n");
			empty = empty + "TimeoutDate: " + participant.TimeoutDate.ToString("DD MMM YYYY HH:mm:ss") + "\n";
			empty = empty + "LastTurnDate: " + participant.LastTurnDate.ToString("DD MMM YYYY HH:mm:ss") + "\n";
			empty += "**********************\n";
		}
		empty += "----------------------------------------\n";
		ISN_Logger.Log(empty);
	}

	public void OnLoadMatchesResult(string data)
	{
		ISN_Logger.Log("TBM::OnLoadMatchesResult: " + data);
		GK_TBM_LoadMatchesResult gK_TBM_LoadMatchesResult = new GK_TBM_LoadMatchesResult(true);
		_Matches = new Dictionary<string, GK_TBM_Match>();
		if (data.Length == 0)
		{
			GameCenter_TBM.ActionMatchesInfoLoaded(gK_TBM_LoadMatchesResult);
			return;
		}
		string[] array = data.Split(new string[1] { "|%|" }, StringSplitOptions.None);
		if (array.Length > 0)
		{
			gK_TBM_LoadMatchesResult.LoadedMatches = new Dictionary<string, GK_TBM_Match>();
			for (int i = 0; i < array.Length && !(array[i] == "endofline"); i++)
			{
				GK_TBM_Match gK_TBM_Match = ParceMatchInfo(array[i]);
				UpdateMatchInfo(gK_TBM_Match);
				gK_TBM_LoadMatchesResult.LoadedMatches.Add(gK_TBM_Match.Id, gK_TBM_Match);
			}
		}
		GameCenter_TBM.ActionMatchesInfoLoaded(gK_TBM_LoadMatchesResult);
	}

	private void OnLoadMatchesResultFailed(string errorData)
	{
		GK_TBM_LoadMatchesResult obj = new GK_TBM_LoadMatchesResult(errorData);
		GameCenter_TBM.ActionMatchesInfoLoaded(obj);
	}

	private void OnLoadMatchResult(string data)
	{
		GK_TBM_Match match = ParceMatchInfo(data);
		GK_TBM_LoadMatchResult obj = new GK_TBM_LoadMatchResult(match);
		GameCenter_TBM.ActionMatchInfoLoaded(obj);
	}

	private void OnLoadMatchResultFailed(string errorData)
	{
		GK_TBM_LoadMatchResult obj = new GK_TBM_LoadMatchResult(errorData);
		GameCenter_TBM.ActionMatchInfoLoaded(obj);
	}

	private void OnUpdateMatchResult(string data)
	{
		string[] array = data.Split('|');
		string text = array[0];
		GK_TBM_Match matchById = GetMatchById(text);
		GK_TBM_MatchDataUpdateResult obj;
		if (matchById == null)
		{
			Error error = new Error(0, "Match with id: " + text + " not found");
			obj = new GK_TBM_MatchDataUpdateResult(error);
		}
		else
		{
			matchById.SetData(array[1]);
			obj = new GK_TBM_MatchDataUpdateResult(matchById);
		}
		GameCenter_TBM.ActionMatchDataUpdated(obj);
	}

	private void OnUpdateMatchResultFailed(string errorData)
	{
		GK_TBM_MatchDataUpdateResult obj = new GK_TBM_MatchDataUpdateResult(errorData);
		GameCenter_TBM.ActionMatchDataUpdated(obj);
	}

	private void OnMatchFoundResult(string data)
	{
		GK_TBM_Match match = ParceMatchInfo(data);
		UpdateMatchInfo(match);
		GK_TBM_MatchInitResult obj = new GK_TBM_MatchInitResult(match);
		GameCenter_TBM.ActionMatchFound(obj);
	}

	private void OnMatchFoundResultFailed(string errorData)
	{
		GK_TBM_MatchInitResult obj = new GK_TBM_MatchInitResult(errorData);
		GameCenter_TBM.ActionMatchFound(obj);
	}

	private void OnPlayerQuitForMatch(string data)
	{
		GK_TBM_Match gK_TBM_Match = ParceMatchInfo(data);
		UpdateMatchInfo(gK_TBM_Match);
		GameCenter_TBM.ActionPlayerQuitForMatch(gK_TBM_Match);
	}

	private void OnMatchQuitResult(string matchId)
	{
		GK_TBM_MatchQuitResult obj = new GK_TBM_MatchQuitResult(matchId);
		GameCenter_TBM.ActionMatchQuit(obj);
	}

	private void OnMatchQuitResultFailed(string errorData)
	{
		GK_TBM_MatchQuitResult obj = new GK_TBM_MatchQuitResult(errorData);
		GameCenter_TBM.ActionMatchQuit(obj);
	}

	private void OnEndTurnResult(string data)
	{
		GK_TBM_Match match = ParceMatchInfo(data);
		UpdateMatchInfo(match);
		GK_TBM_EndTrunResult obj = new GK_TBM_EndTrunResult(match);
		GameCenter_TBM.ActionTrunEnded(obj);
	}

	private void OnEndTurnResultFailed(string errorData)
	{
		GK_TBM_EndTrunResult obj = new GK_TBM_EndTrunResult(errorData);
		GameCenter_TBM.ActionTrunEnded(obj);
	}

	private void OnEndMatch(string data)
	{
		GK_TBM_Match match = ParceMatchInfo(data);
		UpdateMatchInfo(match);
		GK_TBM_MatchEndResult obj = new GK_TBM_MatchEndResult(match);
		GameCenter_TBM.ActionMacthEnded(obj);
	}

	private void OnEndMatchResult(string errorData)
	{
		GK_TBM_MatchEndResult obj = new GK_TBM_MatchEndResult(errorData);
		GameCenter_TBM.ActionMacthEnded(obj);
	}

	private void OnRematchResult(string data)
	{
		GK_TBM_Match match = ParceMatchInfo(data);
		UpdateMatchInfo(match);
		GK_TBM_RematchResult obj = new GK_TBM_RematchResult(match);
		GameCenter_TBM.ActionRematched(obj);
	}

	private void OnRematchFailed(string errorData)
	{
		GK_TBM_RematchResult obj = new GK_TBM_RematchResult(errorData);
		GameCenter_TBM.ActionRematched(obj);
	}

	private void OnMatchRemoved(string matchId)
	{
		GK_TBM_MatchRemovedResult obj = new GK_TBM_MatchRemovedResult(matchId);
		if (_Matches.ContainsKey(matchId))
		{
			_Matches.Remove(matchId);
		}
		GameCenter_TBM.ActionMatchRemoved(obj);
	}

	private void OnMatchRemoveFailed(string errorData)
	{
		GK_TBM_MatchRemovedResult obj = new GK_TBM_MatchRemovedResult(errorData);
		GameCenter_TBM.ActionMatchRemoved(obj);
	}

	private void OnMatchInvitationAccepted(string data)
	{
		GK_TBM_Match match = ParceMatchInfo(data);
		UpdateMatchInfo(match);
		GK_TBM_MatchInitResult obj = new GK_TBM_MatchInitResult(match);
		GameCenter_TBM.ActionMatchInvitationAccepted(obj);
	}

	private void OnMatchInvitationAcceptedFailed(string errorData)
	{
		GK_TBM_MatchInitResult obj = new GK_TBM_MatchInitResult(errorData);
		GameCenter_TBM.ActionMatchInvitationAccepted(obj);
	}

	private void OnMatchInvitationDeclined(string matchId)
	{
		GK_TBM_MatchRemovedResult obj = new GK_TBM_MatchRemovedResult(matchId);
		if (_Matches.ContainsKey(matchId))
		{
			_Matches.Remove(matchId);
		}
		GameCenter_TBM.ActionMatchInvitationDeclined(obj);
	}

	private void OnMatchInvitationDeclineFailed(string errorData)
	{
		GK_TBM_MatchRemovedResult obj = new GK_TBM_MatchRemovedResult(errorData);
		GameCenter_TBM.ActionMatchInvitationDeclined(obj);
	}

	private void OnTrunReceived(string data)
	{
		GK_TBM_Match match = ParceMatchInfo(data);
		UpdateMatchInfo(match);
		GK_TBM_MatchTurnResult obj = new GK_TBM_MatchTurnResult(match);
		GameCenter_TBM.ActionTrunReceived(obj);
	}

	private void UpdateMatchInfo(GK_TBM_Match match)
	{
		if (_Matches.ContainsKey(match.Id))
		{
			_Matches[match.Id] = match;
		}
		else
		{
			_Matches.Add(match.Id, match);
		}
	}

	private static GK_TBM_Match ParceMatchInfo(string data)
	{
		string[] matchData = data.Split('|');
		return ParceMatchInfo(matchData, 0);
	}

	public static GK_TBM_Match ParceMatchInfo(string[] MatchData, int index)
	{
		GK_TBM_Match gK_TBM_Match = new GK_TBM_Match();
		gK_TBM_Match.Id = MatchData[index];
		gK_TBM_Match.Status = (GK_TurnBasedMatchStatus)Convert.ToInt64(MatchData[index + 1]);
		gK_TBM_Match.Message = MatchData[index + 2];
		gK_TBM_Match.CreationTimestamp = DateTime.Parse(MatchData[index + 3]);
		gK_TBM_Match.SetData(MatchData[index + 4]);
		string playerId = MatchData[index + 5];
		gK_TBM_Match.Participants = GameCenterManager.ParseParticipantsData(MatchData, index + 6);
		foreach (GK_TBM_Participant participant in gK_TBM_Match.Participants)
		{
			participant.SetMatchId(gK_TBM_Match.Id);
		}
		gK_TBM_Match.CurrentParticipant = gK_TBM_Match.GetParticipantByPlayerId(playerId);
		return gK_TBM_Match;
	}

	static GameCenter_TBM()
	{
		GameCenter_TBM.ActionMatchInfoLoaded = delegate
		{
		};
		GameCenter_TBM.ActionMatchesInfoLoaded = delegate
		{
		};
		GameCenter_TBM.ActionMatchDataUpdated = delegate
		{
		};
		GameCenter_TBM.ActionMatchFound = delegate
		{
		};
		GameCenter_TBM.ActionMatchQuit = delegate
		{
		};
		GameCenter_TBM.ActionTrunEnded = delegate
		{
		};
		GameCenter_TBM.ActionMacthEnded = delegate
		{
		};
		GameCenter_TBM.ActionRematched = delegate
		{
		};
		GameCenter_TBM.ActionMatchRemoved = delegate
		{
		};
		GameCenter_TBM.ActionMatchInvitationAccepted = delegate
		{
		};
		GameCenter_TBM.ActionMatchInvitationDeclined = delegate
		{
		};
		GameCenter_TBM.ActionPlayerQuitForMatch = delegate
		{
		};
		GameCenter_TBM.ActionTrunReceived = delegate
		{
		};
	}
}
