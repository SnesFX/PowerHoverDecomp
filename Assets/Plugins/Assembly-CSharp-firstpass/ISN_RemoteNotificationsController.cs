using System;
using SA.Common.Models;
using SA.Common.Pattern;
using UnityEngine;

public class ISN_RemoteNotificationsController : Singleton<ISN_RemoteNotificationsController>
{
	private static Action<ISN_RemoteNotificationsRegistrationResult> _RegistrationCallback = null;

	private ISN_RemoteNotification _LaunchNotification;

	public ISN_RemoteNotification LaunchNotification
	{
		get
		{
			return _LaunchNotification;
		}
	}

	public static event Action<ISN_RemoteNotification> OnRemoteNotificationReceived;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void RegisterForRemoteNotifications(Action<ISN_RemoteNotificationsRegistrationResult> callback = null)
	{
		_RegistrationCallback = callback;
	}

	private void DidFailToRegisterForRemoteNotifications(string errorData)
	{
		Error error = new Error(errorData);
		ISN_RemoteNotificationsRegistrationResult obj = new ISN_RemoteNotificationsRegistrationResult(error);
		if (_RegistrationCallback != null)
		{
			_RegistrationCallback(obj);
		}
	}

	private void DidRegisterForRemoteNotifications(string data)
	{
		string[] array = data.Split('|');
		string token = array[0];
		string base64String = array[1];
		ISN_DeviceToken token2 = new ISN_DeviceToken(base64String, token);
		ISN_RemoteNotificationsRegistrationResult obj = new ISN_RemoteNotificationsRegistrationResult(token2);
		if (_RegistrationCallback != null)
		{
			_RegistrationCallback(obj);
		}
	}

	private void DidReceiveRemoteNotification(string notificationBody)
	{
		ISN_RemoteNotification obj = new ISN_RemoteNotification(notificationBody);
		ISN_RemoteNotificationsController.OnRemoteNotificationReceived(obj);
	}

	static ISN_RemoteNotificationsController()
	{
		ISN_RemoteNotificationsController.OnRemoteNotificationReceived = delegate
		{
		};
	}
}
