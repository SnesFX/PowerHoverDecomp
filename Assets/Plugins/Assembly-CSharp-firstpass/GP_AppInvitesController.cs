using System;
using SA.Common.Pattern;
using UnityEngine;

public class GP_AppInvitesController : Singleton<GP_AppInvitesController>
{
	public static event Action<GP_SendAppInvitesResult> ActionAppInvitesSent;

	public static event Action<GP_RetrieveAppInviteResult> ActionAppInviteRetrieved;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void StartInvitationDialog(GP_AppInviteBuilder builder)
	{
		AN_AppInvitesProxy.StartInvitationDialog(builder.Id);
	}

	public void GetInvitation(bool autoLaunchDeepLink = false)
	{
		AN_AppInvitesProxy.GetInvitation(autoLaunchDeepLink);
	}

	private void OnInvitationDialogComplete(string InvitationIds)
	{
		string[] invites = AndroidNative.StringToArray(InvitationIds);
		GP_SendAppInvitesResult obj = new GP_SendAppInvitesResult(invites);
		GP_AppInvitesController.ActionAppInvitesSent(obj);
	}

	private void OnInvitationDialogFailed(string erroCode)
	{
		GP_SendAppInvitesResult obj = new GP_SendAppInvitesResult(erroCode);
		GP_AppInvitesController.ActionAppInvitesSent(obj);
	}

	private void OnInvitationLoadFailed(string erroCode)
	{
		GP_RetrieveAppInviteResult obj = new GP_RetrieveAppInviteResult(erroCode);
		GP_AppInvitesController.ActionAppInviteRetrieved(obj);
	}

	private void OnInvitationLoaded(string data)
	{
		string[] array = data.Split("|"[0]);
		string link = array[0];
		string id = array[1];
		bool isOpenedFromPlatStore = Convert.ToBoolean(array[2]);
		GP_AppInvite invite = new GP_AppInvite(id, link, isOpenedFromPlatStore);
		GP_RetrieveAppInviteResult obj = new GP_RetrieveAppInviteResult(invite);
		GP_AppInvitesController.ActionAppInviteRetrieved(obj);
	}

	static GP_AppInvitesController()
	{
		GP_AppInvitesController.ActionAppInvitesSent = delegate
		{
		};
		GP_AppInvitesController.ActionAppInviteRetrieved = delegate
		{
		};
	}
}
