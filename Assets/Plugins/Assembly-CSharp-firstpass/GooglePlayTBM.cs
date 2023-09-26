using System;
using System.Collections.Generic;
using SA.Common.Pattern;
using UnityEngine;

public class GooglePlayTBM : Singleton<GooglePlayTBM>
{
	private Dictionary<string, GP_TBM_Match> _Matches = new Dictionary<string, GP_TBM_Match>();

	public Dictionary<string, GP_TBM_Match> Matches
	{
		get
		{
			return _Matches;
		}
	}

	public static event Action<GP_TBM_LoadMatchesResult> ActionMatchesResultLoaded;

	public static event Action<GP_TBM_MatchInitiatedResult> ActionMatchInitiated;

	public static event Action<GP_TBM_CancelMatchResult> ActionMatchCanceled;

	public static event Action<GP_TBM_LeaveMatchResult> ActionMatchLeaved;

	public static event Action<GP_TBM_LoadMatchResult> ActionMatchDataLoaded;

	public static event Action<GP_TBM_UpdateMatchResult> ActionMatchUpdated;

	public static event Action<GP_TBM_UpdateMatchResult> ActionMatchTurnFinished;

	public static event Action<GP_TBM_UpdateMatchResult> ActionMatchReceived;

	public static event Action<GP_TBM_MatchRemovedResult> ActionMatchRemoved;

	public static event Action<AndroidActivityResult> ActionMatchCreationCanceled;

	public static event Action<string, GP_TBM_MatchInitiatedResult> ActionMatchInvitationAccepted;

	public static event Action<string> ActionMatchInvitationDeclined;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		Singleton<GooglePlayInvitationManager>.Instance.Init();
	}

	public void Init()
	{
	}

	public void AcceptInvitation(string invitationId)
	{
		AN_GMSRTMProxy.TBM_AcceptInvitation(invitationId);
	}

	public void DeclineInvitation(string invitationId)
	{
		AN_GMSRTMProxy.TBM_DeclineInvitation(invitationId);
		GooglePlayTBM.ActionMatchInvitationDeclined(invitationId);
	}

	public void DismissInvitation(string invitationId)
	{
		AN_GMSRTMProxy.TBM_DismissInvitation(invitationId);
	}

	public void CreateMatch(int minPlayers, int maxPlayers, string[] playersIds = null)
	{
		AN_GMSRTMProxy.TBM_CreateMatch(minPlayers, maxPlayers, playersIds);
	}

	public void CancelMatch(string matchId)
	{
		AN_GMSRTMProxy.CancelMatch(matchId);
	}

	public void DismissMatch(string matchId)
	{
		AN_GMSRTMProxy.DismissMatch(matchId);
	}

	public void ConfirmMatchFinish(string matchId)
	{
		AN_GMSRTMProxy.TBM_FinishMatchWithId(matchId);
	}

	[Obsolete("This method is deprecated. Use ConfirmMatchFinish method instead")]
	public void ConfirmhMatchFinis(string matchId)
	{
		ConfirmMatchFinish(matchId);
	}

	public void FinishMatch(string matchId, byte[] matchData, params GP_ParticipantResult[] results)
	{
		string matchData2 = Convert.ToBase64String(matchData);
		List<string> list = new List<string>();
		List<int> list2 = new List<int>();
		List<int> list3 = new List<int>();
		List<int> list4 = new List<int>();
		foreach (GP_ParticipantResult gP_ParticipantResult in results)
		{
			list.Add(gP_ParticipantResult.ParticipantId);
			list2.Add(gP_ParticipantResult.VersionCode);
			list3.Add((int)gP_ParticipantResult.Result);
			list4.Add(gP_ParticipantResult.Placing);
		}
		AN_GMSRTMProxy.TBM_FinishMatch(matchId, matchData2, list.ToArray(), list3.ToArray(), list4.ToArray(), list2.ToArray());
	}

	public void LeaveMatch(string matchId)
	{
		AN_GMSRTMProxy.TBM_LeaveMatch(matchId);
	}

	public void LeaveMatchDuringTurn(string matchId, string pendingParticipantId)
	{
		AN_GMSRTMProxy.TBM_LeaveMatchDuringTurn(matchId, pendingParticipantId);
	}

	public void LoadMatchInfo(string matchId)
	{
		AN_GMSRTMProxy.TBM_LoadMatchInfo(matchId);
	}

	public void LoadMatchesInfo(GP_TBM_MatchesSortOrder sortOreder, params GP_TBM_MatchTurnStatus[] matchTurnStatuses)
	{
		List<int> list = new List<int>();
		foreach (GP_TBM_MatchTurnStatus item in matchTurnStatuses)
		{
			list.Add((int)item);
		}
		AN_GMSRTMProxy.TBM_LoadMatchesInfo((int)sortOreder, list.ToArray());
	}

	public void LoadAllMatchesInfo(GP_TBM_MatchesSortOrder sortOreder)
	{
		AN_GMSRTMProxy.TBM_LoadAllMatchesInfo((int)sortOreder);
	}

	public void Rematch(string matchId)
	{
		AN_GMSRTMProxy.TBM_Rematch(matchId);
	}

	public void RegisterMatchUpdateListener()
	{
		AN_GMSRTMProxy.TBM_RegisterMatchUpdateListener();
	}

	public void UnregisterMatchUpdateListener()
	{
		AN_GMSRTMProxy.TBM_UnregisterMatchUpdateListener();
	}

	public void TakeTrun(string matchId, byte[] matchData, string pendingParticipantId, params GP_ParticipantResult[] results)
	{
		List<string> list = new List<string>();
		List<int> list2 = new List<int>();
		List<int> list3 = new List<int>();
		List<int> list4 = new List<int>();
		foreach (GP_ParticipantResult gP_ParticipantResult in results)
		{
			list.Add(gP_ParticipantResult.ParticipantId);
			list2.Add(gP_ParticipantResult.VersionCode);
			list3.Add((int)gP_ParticipantResult.Result);
			list4.Add(gP_ParticipantResult.Placing);
		}
		string matchData2 = Convert.ToBase64String(matchData);
		AN_GMSRTMProxy.TBM_TakeTrun(matchId, matchData2, pendingParticipantId, list.ToArray(), list3.ToArray(), list4.ToArray(), list2.ToArray());
	}

	public void StartSelectOpponentsView(int minPlayers, int maxPlayers, bool allowAutomatch)
	{
		AN_GMSRTMProxy.StartSelectOpponentsView(minPlayers, maxPlayers, allowAutomatch);
	}

	public void ShowInbox()
	{
		AN_GMSRTMProxy.TBM_ShowInbox();
	}

	public void SetVariant(int val)
	{
		AN_GMSRTMProxy.TBM_SetVariant(val);
	}

	public void SetExclusiveBitMask(int val)
	{
		AN_GMSRTMProxy.TBM_SetExclusiveBitMask(val);
	}

	private void OnLoadMatchesResult(string data)
	{
		string[] array = data.Split(new string[1] { "|%|" }, StringSplitOptions.None);
		GP_TBM_LoadMatchesResult gP_TBM_LoadMatchesResult = new GP_TBM_LoadMatchesResult(array[0]);
		if (array.Length > 1)
		{
			gP_TBM_LoadMatchesResult.LoadedMatches = new Dictionary<string, GP_TBM_Match>();
			for (int i = 1; i < array.Length && !(array[i] == "endofline"); i++)
			{
				GP_TBM_Match gP_TBM_Match = ParceMatchInfo(array[i]);
				UpdateMatchInfo(gP_TBM_Match);
				gP_TBM_LoadMatchesResult.LoadedMatches.Add(gP_TBM_Match.Id, gP_TBM_Match);
			}
		}
		GooglePlayTBM.ActionMatchesResultLoaded(gP_TBM_LoadMatchesResult);
	}

	private void OnMatchInitiatedCallback(string data)
	{
		string[] array = data.Split("|"[0]);
		GP_TBM_MatchInitiatedResult gP_TBM_MatchInitiatedResult = new GP_TBM_MatchInitiatedResult(array[0]);
		if (array.Length > 1)
		{
			GP_TBM_Match match = ParceMatchInfo(array, 1);
			UpdateMatchInfo(match);
			gP_TBM_MatchInitiatedResult.Match = match;
		}
		GooglePlayTBM.ActionMatchInitiated(gP_TBM_MatchInitiatedResult);
	}

	private void OnInvitationAcceptedCallback(string data)
	{
		Debug.Log("OnInvitationAcceptedCallback received");
		string[] array = data.Split("|"[0]);
		string arg = string.Empty;
		GP_TBM_MatchInitiatedResult gP_TBM_MatchInitiatedResult = new GP_TBM_MatchInitiatedResult(array[0]);
		if (array.Length > 1)
		{
			arg = array[1];
			GP_TBM_Match match = ParceMatchInfo(array, 2);
			UpdateMatchInfo(match);
			gP_TBM_MatchInitiatedResult.Match = match;
		}
		Debug.Log("OnInvitationAcceptedCallback fired " + gP_TBM_MatchInitiatedResult.IsSucceeded);
		GooglePlayTBM.ActionMatchInvitationAccepted(arg, gP_TBM_MatchInitiatedResult);
	}

	private void OnCancelMatchResult(string data)
	{
		string[] array = data.Split("|"[0]);
		GP_TBM_CancelMatchResult gP_TBM_CancelMatchResult = new GP_TBM_CancelMatchResult(array[0]);
		if (array.Length > 1)
		{
			gP_TBM_CancelMatchResult.MatchId = array[1];
			if (Matches.ContainsKey(gP_TBM_CancelMatchResult.MatchId))
			{
				Matches[gP_TBM_CancelMatchResult.MatchId].Status = GP_TBM_MatchStatus.MATCH_STATUS_CANCELED;
			}
		}
		GooglePlayTBM.ActionMatchCanceled(gP_TBM_CancelMatchResult);
	}

	private void OnLeaveMatchResult(string data)
	{
		string[] array = data.Split("|"[0]);
		GP_TBM_LeaveMatchResult gP_TBM_LeaveMatchResult = new GP_TBM_LeaveMatchResult(array[0]);
		gP_TBM_LeaveMatchResult.MatchId = array[1];
		GooglePlayTBM.ActionMatchLeaved(gP_TBM_LeaveMatchResult);
	}

	private void OnLoadMatchResult(string data)
	{
		string[] array = data.Split("|"[0]);
		GP_TBM_LoadMatchResult gP_TBM_LoadMatchResult = new GP_TBM_LoadMatchResult(array[0]);
		if (array.Length > 1)
		{
			GP_TBM_Match match = ParceMatchInfo(array, 1);
			UpdateMatchInfo(match);
			gP_TBM_LoadMatchResult.Match = match;
		}
		GooglePlayTBM.ActionMatchDataLoaded(gP_TBM_LoadMatchResult);
	}

	private void OnUpdateMatchResult(string data)
	{
		string[] array = data.Split("|"[0]);
		GP_TBM_UpdateMatchResult gP_TBM_UpdateMatchResult = new GP_TBM_UpdateMatchResult(array[0]);
		if (array.Length > 1)
		{
			GP_TBM_Match match = ParceMatchInfo(array, 1);
			UpdateMatchInfo(match);
			gP_TBM_UpdateMatchResult.Match = match;
		}
		GooglePlayTBM.ActionMatchUpdated(gP_TBM_UpdateMatchResult);
	}

	private void AN_OnTurnResult(string data)
	{
		Debug.Log("AN_OnTurnResult");
		string[] array = data.Split("|"[0]);
		GP_TBM_UpdateMatchResult gP_TBM_UpdateMatchResult = new GP_TBM_UpdateMatchResult(array[0]);
		if (array.Length > 1)
		{
			GP_TBM_Match match = ParceMatchInfo(array, 1);
			UpdateMatchInfo(match);
			gP_TBM_UpdateMatchResult.Match = match;
		}
		Debug.Log("ActionMatchTurnFinished fired");
		GooglePlayTBM.ActionMatchTurnFinished(gP_TBM_UpdateMatchResult);
	}

	private void OnTurnBasedMatchRemoved(string matchId)
	{
		if (Matches.ContainsKey(matchId))
		{
			Matches.Remove(matchId);
		}
		GooglePlayTBM.ActionMatchRemoved(new GP_TBM_MatchRemovedResult(matchId));
	}

	private void OnTurnBasedMatchReceived(string data)
	{
		string[] matchData = data.Split("|"[0]);
		GP_TBM_UpdateMatchResult gP_TBM_UpdateMatchResult = new GP_TBM_UpdateMatchResult("0");
		GP_TBM_Match match = ParceMatchInfo(matchData, 0);
		UpdateMatchInfo(match);
		gP_TBM_UpdateMatchResult.Match = match;
		GooglePlayTBM.ActionMatchUpdated(gP_TBM_UpdateMatchResult);
		GooglePlayTBM.ActionMatchReceived(gP_TBM_UpdateMatchResult);
	}

	private void OnMatchCreationCanceled(string data)
	{
		string[] array = data.Split("|"[0]);
		AndroidActivityResult obj = new AndroidActivityResult(array[0], array[1]);
		GooglePlayTBM.ActionMatchCreationCanceled(obj);
	}

	public static GP_TBM_Match ParceMatchInfo(string data)
	{
		string[] matchData = data.Split("|"[0]);
		return ParceMatchInfo(matchData, 0);
	}

	public static GP_TBM_Match ParceMatchInfo(string[] MatchData, int index)
	{
		GP_TBM_Match gP_TBM_Match = new GP_TBM_Match();
		gP_TBM_Match.Id = MatchData[index];
		gP_TBM_Match.RematchId = MatchData[index + 1];
		gP_TBM_Match.Description = MatchData[index + 2];
		gP_TBM_Match.AvailableAutoMatchSlots = Convert.ToInt32(MatchData[index + 3]);
		gP_TBM_Match.CreationTimestamp = Convert.ToInt64(MatchData[index + 4]);
		gP_TBM_Match.CreatorId = MatchData[index + 5];
		gP_TBM_Match.LastUpdatedTimestamp = Convert.ToInt64(MatchData[index + 6]);
		gP_TBM_Match.LastUpdaterId = MatchData[index + 7];
		gP_TBM_Match.MatchNumber = Convert.ToInt32(MatchData[index + 8]);
		gP_TBM_Match.Status = (GP_TBM_MatchStatus)Convert.ToInt32(MatchData[index + 9]);
		gP_TBM_Match.TurnStatus = (GP_TBM_MatchTurnStatus)Convert.ToInt32(MatchData[index + 10]);
		gP_TBM_Match.CanRematch = Convert.ToBoolean(MatchData[index + 11]);
		gP_TBM_Match.Variant = Convert.ToInt32(MatchData[index + 12]);
		gP_TBM_Match.Version = Convert.ToInt32(MatchData[index + 13]);
		gP_TBM_Match.SetData(MatchData[index + 14]);
		gP_TBM_Match.SetPreviousMatchData(MatchData[index + 15]);
		gP_TBM_Match.PendingParticipantId = MatchData[index + 16];
		gP_TBM_Match.Participants = GooglePlayManager.ParseParticipantsData(MatchData, index + 17);
		return gP_TBM_Match;
	}

	private void UpdateMatchInfo(GP_TBM_Match match)
	{
		if (Matches.ContainsKey(match.Id))
		{
			Matches[match.Id] = match;
		}
		else
		{
			Matches.Add(match.Id, match);
		}
	}

	static GooglePlayTBM()
	{
		GooglePlayTBM.ActionMatchesResultLoaded = delegate
		{
		};
		GooglePlayTBM.ActionMatchInitiated = delegate
		{
		};
		GooglePlayTBM.ActionMatchCanceled = delegate
		{
		};
		GooglePlayTBM.ActionMatchLeaved = delegate
		{
		};
		GooglePlayTBM.ActionMatchDataLoaded = delegate
		{
		};
		GooglePlayTBM.ActionMatchUpdated = delegate
		{
		};
		GooglePlayTBM.ActionMatchTurnFinished = delegate
		{
		};
		GooglePlayTBM.ActionMatchReceived = delegate
		{
		};
		GooglePlayTBM.ActionMatchRemoved = delegate
		{
		};
		GooglePlayTBM.ActionMatchCreationCanceled = delegate
		{
		};
		GooglePlayTBM.ActionMatchInvitationAccepted = delegate
		{
		};
		GooglePlayTBM.ActionMatchInvitationDeclined = delegate
		{
		};
	}
}
