using System;
using System.Collections.Generic;
using SA.Common.Pattern;
using SA.Common.Util;
using UnityEngine;

public class GooglePlayRTM : Singleton<GooglePlayRTM>
{
	private const int BYTE_LIMIT = 256;

	private GP_RTM_Room _currentRoom = new GP_RTM_Room();

	private List<GP_Invite> _invitations = new List<GP_Invite>();

	private Dictionary<int, GP_RTM_ReliableMessageListener> _ReliableMassageListeners = new Dictionary<int, GP_RTM_ReliableMessageListener>();

	public GP_RTM_Room currentRoom
	{
		get
		{
			return _currentRoom;
		}
	}

	public List<GP_Invite> invitations
	{
		get
		{
			return _invitations;
		}
	}

	public static event Action<GP_RTM_Network_Package> ActionDataRecieved;

	public static event Action<GP_RTM_Room> ActionRoomUpdated;

	public static event Action<GP_RTM_ReliableMessageSentResult> ActionReliableMessageSent;

	public static event Action<GP_RTM_ReliableMessageDeliveredResult> ActionReliableMessageDelivered;

	public static event Action ActionConnectedToRoom;

	public static event Action ActionDisconnectedFromRoom;

	public static event Action<string> ActionP2PConnected;

	public static event Action<string> ActionP2PDisconnected;

	public static event Action<string[]> ActionPeerDeclined;

	public static event Action<string[]> ActionPeerInvitedToRoom;

	public static event Action<string[]> ActionPeerJoined;

	public static event Action<string[]> ActionPeerLeft;

	public static event Action<string[]> ActionPeersConnected;

	public static event Action<string[]> ActionPeersDisconnected;

	public static event Action ActionRoomAutomatching;

	public static event Action ActionRoomConnecting;

	public static event Action<GP_GamesStatusCodes> ActionJoinedRoom;

	public static event Action<GP_RTM_Result> ActionLeftRoom;

	public static event Action<GP_GamesStatusCodes> ActionRoomConnected;

	public static event Action<GP_GamesStatusCodes> ActionRoomCreated;

	public static event Action<AndroidActivityResult> ActionInvitationBoxUIClosed;

	public static event Action<AndroidActivityResult> ActionWatingRoomIntentClosed;

	public static event Action<GP_Invite> ActionInvitationAccepted;

	public static event Action<GP_Invite> ActionInvitationReceived;

	public static event Action<string> ActionInvitationRemoved;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		_currentRoom = new GP_RTM_Room();
		GooglePlayInvitationManager.ActionInvitationReceived += OnInvitationReceived;
		GooglePlayInvitationManager.ActionInvitationRemoved += OnInvitationRemoved;
		GooglePlayInvitationManager.ActionInvitationAccepted += OnInvitationAccepted;
		Singleton<GooglePlayInvitationManager>.Instance.Init();
		Debug.Log("GooglePlayRTM Created");
	}

	public void FindMatch(int minPlayers, int maxPlayers)
	{
		FindMatch(minPlayers, maxPlayers, new string[0]);
	}

	public void FindMatch(int minPlayers, int maxPlayers, params GooglePlayerTemplate[] playersToInvite)
	{
		List<string> list = new List<string>();
		foreach (GooglePlayerTemplate googlePlayerTemplate in playersToInvite)
		{
			list.Add(googlePlayerTemplate.playerId);
		}
		AN_GMSRTMProxy.RTMFindMatch(minPlayers, maxPlayers, list.ToArray());
	}

	public void FindMatch(int minPlayers, int maxPlayers, params string[] playersToInvite)
	{
		AN_GMSRTMProxy.RTMFindMatch(minPlayers, maxPlayers, playersToInvite);
	}

	public void FindMatch(GooglePlayerTemplate[] playersToInvite)
	{
		List<string> list = new List<string>();
		foreach (GooglePlayerTemplate googlePlayerTemplate in playersToInvite)
		{
			list.Add(googlePlayerTemplate.playerId);
		}
		AN_GMSRTMProxy.RTMFindMatch(list.ToArray());
	}

	public void FindMatch(string[] playersToInvite)
	{
		AN_GMSRTMProxy.RTMFindMatch(playersToInvite);
	}

	public void SendDataToAll(byte[] data, GP_RTM_PackageType sendType)
	{
		string data2 = Convert.ToBase64String(data);
		switch (sendType)
		{
		case GP_RTM_PackageType.RELIABLE:
		{
			GP_RTM_ReliableMessageListener gP_RTM_ReliableMessageListener = new GP_RTM_ReliableMessageListener(IdFactory.NextId, data);
			_ReliableMassageListeners.Add(gP_RTM_ReliableMessageListener.DataTokenId, gP_RTM_ReliableMessageListener);
			AN_GMSRTMProxy.sendDataToAll(data2, (int)sendType, gP_RTM_ReliableMessageListener.DataTokenId);
			break;
		}
		case GP_RTM_PackageType.UNRELIABLE:
			AN_GMSRTMProxy.sendDataToAll(data2, (int)sendType);
			break;
		}
	}

	public void SendDataToPlayers(byte[] data, GP_RTM_PackageType sendType, params string[] players)
	{
		string data2 = Convert.ToBase64String(data);
		string players2 = string.Join("|", players);
		switch (sendType)
		{
		case GP_RTM_PackageType.RELIABLE:
		{
			GP_RTM_ReliableMessageListener gP_RTM_ReliableMessageListener = new GP_RTM_ReliableMessageListener(IdFactory.NextId, data);
			_ReliableMassageListeners.Add(gP_RTM_ReliableMessageListener.DataTokenId, gP_RTM_ReliableMessageListener);
			AN_GMSRTMProxy.sendDataToPlayers(data2, players2, (int)sendType, gP_RTM_ReliableMessageListener.DataTokenId);
			break;
		}
		case GP_RTM_PackageType.UNRELIABLE:
			AN_GMSRTMProxy.sendDataToPlayers(data2, players2, (int)sendType);
			break;
		}
	}

	public void ShowWaitingRoomIntent()
	{
		AN_GMSRTMProxy.ShowWaitingRoomIntent();
	}

	public void OpenInvitationBoxUI(int minPlayers, int maxPlayers)
	{
		AN_GMSRTMProxy.InvitePlayers(minPlayers, maxPlayers);
	}

	public void LeaveRoom()
	{
		AN_GMSGiftsProxy.leaveRoom();
	}

	public void AcceptInvitation(string invitationId)
	{
		AN_GMSRTMProxy.RTM_AcceptInvitation(invitationId);
	}

	public void DeclineInvitation(string invitationId)
	{
		AN_GMSRTMProxy.RTM_DeclineInvitation(invitationId);
	}

	public void DismissInvitation(string invitationId)
	{
		AN_GMSRTMProxy.RTM_DismissInvitation(invitationId);
	}

	public void OpenInvitationInBoxUI()
	{
		AN_GMSGiftsProxy.showInvitationBox();
	}

	public void SetVariant(int val)
	{
		AN_GMSRTMProxy.RTM_SetVariant(val);
	}

	public void SetExclusiveBitMask(int val)
	{
		AN_GMSRTMProxy.RTM_SetExclusiveBitMask(val);
	}

	public void ClearReliableMessageListener(int dataTokenId)
	{
		if (_ReliableMassageListeners.ContainsKey(dataTokenId))
		{
			_ReliableMassageListeners.Remove(dataTokenId);
			Debug.Log("[ClearReliableMessageListener] Remove data with token " + dataTokenId);
		}
	}

	private void OnWatingRoomIntentClosed(string data)
	{
		Debug.Log("[OnWatingRoomIntentClosed] data " + data);
		string[] array = data.Split("|"[0]);
		AndroidActivityResult obj = new AndroidActivityResult(array[0], array[1]);
		GooglePlayRTM.ActionWatingRoomIntentClosed(obj);
	}

	private void OnRoomUpdate(string data)
	{
		string[] array = data.Split("|"[0]);
		_currentRoom = new GP_RTM_Room
		{
			id = array[0],
			creatorId = array[1]
		};
		string[] array2 = array[2].Split(',');
		for (int i = 0; i < array2.Length && !(array2[i] == "endofline"); i += 6)
		{
			GP_Participant p = new GP_Participant(array2[i], array2[i + 1], array2[i + 2], array2[i + 3], array2[i + 4], array2[i + 5]);
			_currentRoom.AddParticipant(p);
		}
		_currentRoom.status = (GP_RTM_RoomStatus)Convert.ToInt32(array[3]);
		_currentRoom.creationTimestamp = Convert.ToInt64(array[4]);
		Debug.Log("GooglePlayRTM OnRoomUpdate Room State: " + _currentRoom.status);
		GooglePlayRTM.ActionRoomUpdated(_currentRoom);
	}

	private void OnReliableMessageSent(string data)
	{
		Debug.Log("[OnReliableMessageSent] " + data);
		string[] array = data.Split("|"[0]);
		int messageTokedId = int.Parse(array[2]);
		int key = int.Parse(array[3]);
		if (_ReliableMassageListeners.ContainsKey(key))
		{
			GP_RTM_ReliableMessageSentResult obj = new GP_RTM_ReliableMessageSentResult(array[0], array[1], messageTokedId, _ReliableMassageListeners[key].Data);
			GooglePlayRTM.ActionReliableMessageSent(obj);
			_ReliableMassageListeners[key].ReportSentMessage();
		}
		else
		{
			GP_RTM_ReliableMessageSentResult obj2 = new GP_RTM_ReliableMessageSentResult(array[0], array[1], messageTokedId, null);
			GooglePlayRTM.ActionReliableMessageSent(obj2);
		}
	}

	private void OnReliableMessageDelivered(string data)
	{
		Debug.Log("[OnReliableMessageDelivered] " + data);
		string[] array = data.Split("|"[0]);
		int messageTokedId = int.Parse(array[2]);
		int key = int.Parse(array[3]);
		if (_ReliableMassageListeners.ContainsKey(key))
		{
			GP_RTM_ReliableMessageDeliveredResult obj = new GP_RTM_ReliableMessageDeliveredResult(array[0], array[1], messageTokedId, _ReliableMassageListeners[key].Data);
			GooglePlayRTM.ActionReliableMessageDelivered(obj);
			_ReliableMassageListeners[key].ReportDeliveredMessage();
		}
		else
		{
			GP_RTM_ReliableMessageDeliveredResult obj2 = new GP_RTM_ReliableMessageDeliveredResult(array[0], array[1], messageTokedId, null);
			GooglePlayRTM.ActionReliableMessageDelivered(obj2);
		}
	}

	private void OnMatchDataRecieved(string data)
	{
		if (data.Equals(string.Empty))
		{
			Debug.Log("OnMatchDataRecieved, no data avaiable");
			return;
		}
		string[] array = data.Split("|"[0]);
		GP_RTM_Network_Package obj = new GP_RTM_Network_Package(array[0], array[1]);
		GooglePlayRTM.ActionDataRecieved(obj);
		Debug.Log("GooglePlayManager -> DATA_RECEIVED");
	}

	private void OnConnectedToRoom(string data)
	{
		Debug.Log("[OnConnectedToRoom] data " + data);
		GooglePlayRTM.ActionConnectedToRoom();
	}

	private void OnDisconnectedFromRoom(string data)
	{
		Debug.Log("[OnDisconnectedFromRoom] data " + data);
		GooglePlayRTM.ActionDisconnectedFromRoom();
	}

	private void OnP2PConnected(string participantId)
	{
		Debug.Log("[OnP2PConnected] participantId " + participantId);
		GooglePlayRTM.ActionP2PConnected(participantId);
	}

	private void OnP2PDisconnected(string participantId)
	{
		Debug.Log("[OnP2PDisconnected] participantId " + participantId);
		GooglePlayRTM.ActionP2PDisconnected(participantId);
	}

	private void OnPeerDeclined(string data)
	{
		Debug.Log("[OnPeerDeclined] data " + data);
		string[] obj = data.Split(","[0]);
		GooglePlayRTM.ActionPeerDeclined(obj);
	}

	private void OnPeerInvitedToRoom(string data)
	{
		Debug.Log("[OnPeerInvitedToRoom] data " + data);
		string[] obj = data.Split(","[0]);
		GooglePlayRTM.ActionPeerInvitedToRoom(obj);
	}

	private void OnPeerJoined(string data)
	{
		Debug.Log("[OnPeerJoined] data " + data);
		string[] obj = data.Split(","[0]);
		GooglePlayRTM.ActionPeerJoined(obj);
	}

	private void OnPeerLeft(string data)
	{
		Debug.Log("[OnPeerLeft] data " + data);
		string[] obj = data.Split(","[0]);
		GooglePlayRTM.ActionPeerLeft(obj);
	}

	private void OnPeersConnected(string data)
	{
		Debug.Log("[OnPeersConnected] data " + data);
		string[] obj = data.Split(","[0]);
		GooglePlayRTM.ActionPeersConnected(obj);
	}

	private void OnPeersDisconnected(string data)
	{
		Debug.Log("[OnPeersDisconnected] data " + data);
		string[] obj = data.Split(","[0]);
		GooglePlayRTM.ActionPeersDisconnected(obj);
	}

	private void OnRoomAutoMatching(string data)
	{
		Debug.Log("[OnRoomAutoMatching] data " + data);
		GooglePlayRTM.ActionRoomAutomatching();
	}

	private void OnRoomConnecting(string data)
	{
		Debug.Log("[OnRoomConnecting] data " + data);
		GooglePlayRTM.ActionRoomConnecting();
	}

	private void OnJoinedRoom(string data)
	{
		Debug.Log("[OnJoinedRoom] data " + data);
		GP_GamesStatusCodes obj = (GP_GamesStatusCodes)Convert.ToInt32(data);
		GooglePlayRTM.ActionJoinedRoom(obj);
	}

	private void OnLeftRoom(string data)
	{
		Debug.Log("[OnLeftRoom] Created OnRoomUpdate data " + data);
		string[] array = data.Split("|"[0]);
		GP_RTM_Result obj = new GP_RTM_Result(array[0], array[1]);
		_currentRoom = new GP_RTM_Room();
		GooglePlayRTM.ActionRoomUpdated(_currentRoom);
		GooglePlayRTM.ActionLeftRoom(obj);
	}

	private void OnRoomConnected(string data)
	{
		Debug.Log("[OnRoomConnected] data " + data);
		GP_GamesStatusCodes obj = (GP_GamesStatusCodes)Convert.ToInt32(data);
		GooglePlayRTM.ActionRoomConnected(obj);
	}

	private void OnRoomCreated(string data)
	{
		Debug.Log("[OnRoomCreated] data " + data);
		GP_GamesStatusCodes obj = (GP_GamesStatusCodes)Convert.ToInt32(data);
		GooglePlayRTM.ActionRoomCreated(obj);
	}

	private void OnInvitationBoxUiClosed(string data)
	{
		Debug.Log("[OnInvitationBoxUiClosed] data " + data);
		string[] array = data.Split("|"[0]);
		AndroidActivityResult obj = new AndroidActivityResult(array[0], array[1]);
		GooglePlayRTM.ActionInvitationBoxUIClosed(obj);
	}

	private void OnInvitationReceived(GP_Invite inv)
	{
		if (inv.InvitationType == GP_InvitationType.INVITATION_TYPE_REAL_TIME)
		{
			_invitations.Add(inv);
			GooglePlayRTM.ActionInvitationReceived(inv);
		}
	}

	private void OnInvitationRemoved(string invitationId)
	{
		Debug.Log("[OnInvitationRemoved] invitationId " + invitationId);
		foreach (GP_Invite invitation in _invitations)
		{
			if (invitation.Id.Equals(invitationId))
			{
				_invitations.Remove(invitation);
				return;
			}
		}
		GooglePlayRTM.ActionInvitationRemoved(invitationId);
	}

	private void OnInvitationAccepted(GP_Invite inv)
	{
		GooglePlayRTM.ActionInvitationAccepted(inv);
	}

	static GooglePlayRTM()
	{
		GooglePlayRTM.ActionDataRecieved = delegate
		{
		};
		GooglePlayRTM.ActionRoomUpdated = delegate
		{
		};
		GooglePlayRTM.ActionReliableMessageSent = delegate
		{
		};
		GooglePlayRTM.ActionReliableMessageDelivered = delegate
		{
		};
		GooglePlayRTM.ActionConnectedToRoom = delegate
		{
		};
		GooglePlayRTM.ActionDisconnectedFromRoom = delegate
		{
		};
		GooglePlayRTM.ActionP2PConnected = delegate
		{
		};
		GooglePlayRTM.ActionP2PDisconnected = delegate
		{
		};
		GooglePlayRTM.ActionPeerDeclined = delegate
		{
		};
		GooglePlayRTM.ActionPeerInvitedToRoom = delegate
		{
		};
		GooglePlayRTM.ActionPeerJoined = delegate
		{
		};
		GooglePlayRTM.ActionPeerLeft = delegate
		{
		};
		GooglePlayRTM.ActionPeersConnected = delegate
		{
		};
		GooglePlayRTM.ActionPeersDisconnected = delegate
		{
		};
		GooglePlayRTM.ActionRoomAutomatching = delegate
		{
		};
		GooglePlayRTM.ActionRoomConnecting = delegate
		{
		};
		GooglePlayRTM.ActionJoinedRoom = delegate
		{
		};
		GooglePlayRTM.ActionLeftRoom = delegate
		{
		};
		GooglePlayRTM.ActionRoomConnected = delegate
		{
		};
		GooglePlayRTM.ActionRoomCreated = delegate
		{
		};
		GooglePlayRTM.ActionInvitationBoxUIClosed = delegate
		{
		};
		GooglePlayRTM.ActionWatingRoomIntentClosed = delegate
		{
		};
		GooglePlayRTM.ActionInvitationAccepted = delegate
		{
		};
		GooglePlayRTM.ActionInvitationReceived = delegate
		{
		};
		GooglePlayRTM.ActionInvitationRemoved = delegate
		{
		};
	}
}
