using System;
using System.Collections.Generic;
using SA.Common.Models;
using SA.Common.Pattern;
using UnityEngine;

public class GameCenter_RTM : Singleton<GameCenter_RTM>
{
	private GK_RTM_Match _CurrentMatch;

	private Dictionary<string, GK_Player> _NearbyPlayers = new Dictionary<string, GK_Player>();

	public GK_RTM_Match CurrentMatch
	{
		get
		{
			return _CurrentMatch;
		}
	}

	public List<GK_Player> NearbyPlayersList
	{
		get
		{
			List<GK_Player> list = new List<GK_Player>();
			foreach (KeyValuePair<string, GK_Player> nearbyPlayer in _NearbyPlayers)
			{
				list.Add(nearbyPlayer.Value);
			}
			return list;
		}
	}

	public Dictionary<string, GK_Player> NearbyPlayers
	{
		get
		{
			return _NearbyPlayers;
		}
	}

	public static event Action<GK_RTM_MatchStartedResult> ActionMatchStarted;

	public static event Action<Error> ActionMatchFailed;

	public static event Action<GK_Player, bool> ActionNearbyPlayerStateUpdated;

	public static event Action<GK_RTM_QueryActivityResult> ActionActivityResultReceived;

	public static event Action<Error> ActionDataSendError;

	public static event Action<GK_Player, byte[]> ActionDataReceived;

	public static event Action<GK_Player, GK_PlayerConnectionState, GK_RTM_Match> ActionPlayerStateChanged;

	public static event Action<GK_Player> ActionDiconnectedPlayerReinvited;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
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

	public void StartMatchWithInvite(GK_Invite invite, bool useNativeUI)
	{
	}

	public void CancelPendingInviteToPlayer(GK_Player player)
	{
	}

	public void CancelMatchSearch()
	{
	}

	public void FinishMatchmaking()
	{
	}

	public void QueryActivity()
	{
	}

	public void QueryPlayerGroupActivity(int group)
	{
	}

	public void StartBrowsingForNearbyPlayers()
	{
	}

	public void StopBrowsingForNearbyPlayers()
	{
	}

	public void Rematch()
	{
	}

	public void Disconnect()
	{
		_CurrentMatch = null;
	}

	public void SendDataToAll(byte[] data, GK_MatchSendDataMode dataMode)
	{
	}

	public void SendData(byte[] data, GK_MatchSendDataMode dataMode, params GK_Player[] players)
	{
	}

	private void OnMatchStartFailed(string errorData)
	{
		GK_RTM_MatchStartedResult obj = new GK_RTM_MatchStartedResult(errorData);
		GameCenter_RTM.ActionMatchStarted(obj);
	}

	private void OnMatchStarted(string matchData)
	{
		GK_RTM_Match match = ParseMatchData(matchData);
		GK_RTM_MatchStartedResult obj = new GK_RTM_MatchStartedResult(match);
		GameCenter_RTM.ActionMatchStarted(obj);
	}

	private void OnMatchFailed(string errorData)
	{
		_CurrentMatch = null;
		Error obj = new Error(errorData);
		GameCenter_RTM.ActionMatchFailed(obj);
	}

	private void OnNearbyPlayerInfoReceived(string data)
	{
		string[] array = data.Split('|');
		string playerID = array[0];
		GK_Player playerById = GameCenterManager.GetPlayerById(playerID);
		bool flag = Convert.ToBoolean(array[1]);
		if (flag)
		{
			if (!_NearbyPlayers.ContainsKey(playerById.Id))
			{
				_NearbyPlayers.Add(playerById.Id, playerById);
			}
		}
		else if (_NearbyPlayers.ContainsKey(playerById.Id))
		{
			_NearbyPlayers.Remove(playerById.Id);
		}
		GameCenter_RTM.ActionNearbyPlayerStateUpdated(playerById, flag);
	}

	private void OnQueryActivity(string data)
	{
		int activity = Convert.ToInt32(data);
		GK_RTM_QueryActivityResult obj = new GK_RTM_QueryActivityResult(activity);
		GameCenter_RTM.ActionActivityResultReceived(obj);
	}

	private void OnQueryActivityFailed(string errorData)
	{
		GK_RTM_QueryActivityResult obj = new GK_RTM_QueryActivityResult(errorData);
		GameCenter_RTM.ActionActivityResultReceived(obj);
	}

	private void OnMatchInfoUpdated(string matchData)
	{
		GK_RTM_Match gK_RTM_Match = ParseMatchData(matchData);
		if (gK_RTM_Match.Players.Count == 0 && gK_RTM_Match.ExpectedPlayerCount == 0)
		{
			_CurrentMatch = null;
		}
	}

	private void OnMatchPlayerStateChanged(string data)
	{
		if (_CurrentMatch != null)
		{
			string[] array = data.Split('|');
			string playerID = array[0];
			GK_Player playerById = GameCenterManager.GetPlayerById(playerID);
			GK_PlayerConnectionState arg = (GK_PlayerConnectionState)Convert.ToInt32(array[1]);
			GameCenter_RTM.ActionPlayerStateChanged(playerById, arg, CurrentMatch);
		}
	}

	private void OnDiconnectedPlayerReinvited(string playerId)
	{
		GK_Player playerById = GameCenterManager.GetPlayerById(playerId);
		GameCenter_RTM.ActionDiconnectedPlayerReinvited(playerById);
	}

	private void OnMatchDataReceived(string data)
	{
		string[] array = data.Split('|');
		string playerID = array[0];
		GK_Player playerById = GameCenterManager.GetPlayerById(playerID);
		byte[] arg = Convert.FromBase64String(array[1]);
		GameCenter_RTM.ActionDataReceived(playerById, arg);
	}

	private void OnSendDataError(string errorData)
	{
		Error obj = new Error(errorData);
		GameCenter_RTM.ActionDataSendError(obj);
	}

	private GK_RTM_Match ParseMatchData(string matchData)
	{
		return _CurrentMatch = new GK_RTM_Match(matchData);
	}

	static GameCenter_RTM()
	{
		GameCenter_RTM.ActionMatchStarted = delegate
		{
		};
		GameCenter_RTM.ActionMatchFailed = delegate
		{
		};
		GameCenter_RTM.ActionNearbyPlayerStateUpdated = delegate
		{
		};
		GameCenter_RTM.ActionActivityResultReceived = delegate
		{
		};
		GameCenter_RTM.ActionDataSendError = delegate
		{
		};
		GameCenter_RTM.ActionDataReceived = delegate
		{
		};
		GameCenter_RTM.ActionPlayerStateChanged = delegate
		{
		};
		GameCenter_RTM.ActionDiconnectedPlayerReinvited = delegate
		{
		};
	}
}
