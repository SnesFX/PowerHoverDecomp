using System;
using System.Collections.Generic;
using SA.Common.Pattern;
using UnityEngine;

public class GooglePlayInvitationManager : Singleton<GooglePlayInvitationManager>
{
	public static event Action<GP_Invite> ActionInvitationReceived;

	public static event Action<GP_Invite> ActionInvitationAccepted;

	public static event Action<List<GP_Invite>> ActionInvitationsListLoaded;

	public static event Action<AN_InvitationInboxCloseResult> ActionInvitationInboxClosed;

	public static event Action<string> ActionInvitationRemoved;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		Debug.Log("GooglePlayInvitationManager Created");
	}

	public void Init()
	{
	}

	private void OnInvitationReceived(string data)
	{
		string[] storeData = data.Split("|"[0]);
		GooglePlayInvitationManager.ActionInvitationReceived(InviteFromString(storeData, 0));
	}

	private void OnInvitationAccepted(string data)
	{
		string[] storeData = data.Split("|"[0]);
		GooglePlayInvitationManager.ActionInvitationAccepted(InviteFromString(storeData, 0));
		Debug.Log("OnInvitationAccepted+++");
	}

	private void OnInvitationRemoved(string invId)
	{
		GooglePlayInvitationManager.ActionInvitationRemoved(invId);
	}

	private void OnInvitationBoxUiClosed(string response)
	{
		AN_InvitationInboxCloseResult obj = new AN_InvitationInboxCloseResult(response);
		GooglePlayInvitationManager.ActionInvitationInboxClosed(obj);
	}

	private void OnLoadInvitationsResult(string data)
	{
		string[] array = data.Split(new string[1] { "|%|" }, StringSplitOptions.None);
		List<GP_Invite> list = new List<GP_Invite>();
		GooglePlayResult googlePlayResult = new GooglePlayResult(array[0]);
		if (googlePlayResult.IsSucceeded)
		{
			for (int i = 1; i < array.Length && !(array[i] == "endofline"); i++)
			{
				string[] storeData = array[i].Split("|"[0]);
				GP_Invite item = InviteFromString(storeData, 0);
				list.Add(item);
			}
		}
		GooglePlayInvitationManager.ActionInvitationsListLoaded(list);
	}

	private GP_Invite InviteFromString(string[] storeData, int offset)
	{
		GP_Invite gP_Invite = new GP_Invite();
		gP_Invite.Id = storeData[offset];
		gP_Invite.CreationTimestamp = Convert.ToInt64(storeData[1 + offset]);
		gP_Invite.InvitationType = (GP_InvitationType)Convert.ToInt32(storeData[2 + offset]);
		gP_Invite.Variant = Convert.ToInt32(storeData[3 + offset]);
		gP_Invite.Participant = GooglePlayManager.ParseParticipanData(storeData, 4 + offset);
		return gP_Invite;
	}

	public void RegisterInvitationListener()
	{
		AN_GMSInvitationProxy.registerInvitationListener();
	}

	public void UnregisterInvitationListener()
	{
		AN_GMSInvitationProxy.unregisterInvitationListener();
	}

	public void LoadInvitations()
	{
		AN_GMSInvitationProxy.LoadInvitations();
	}

	static GooglePlayInvitationManager()
	{
		GooglePlayInvitationManager.ActionInvitationReceived = delegate
		{
		};
		GooglePlayInvitationManager.ActionInvitationAccepted = delegate
		{
		};
		GooglePlayInvitationManager.ActionInvitationsListLoaded = delegate
		{
		};
		GooglePlayInvitationManager.ActionInvitationInboxClosed = delegate
		{
		};
		GooglePlayInvitationManager.ActionInvitationRemoved = delegate
		{
		};
	}
}
