using System;
using System.Collections.Generic;
using SA.Common.Models;
using SA.Common.Pattern;
using UnityEngine;

public class ISN_LocalNotificationsController : Singleton<ISN_LocalNotificationsController>
{
	private const string PP_KEY = "IOSNotificationControllerKey";

	private const string PP_ID_KEY = "IOSNotificationControllerrKey_ID";

	private ISN_LocalNotification _LaunchNotification;

	public static int AllowedNotificationsType
	{
		get
		{
			return 0;
		}
	}

	public ISN_LocalNotification LaunchNotification
	{
		get
		{
			return _LaunchNotification;
		}
	}

	public static event Action<Result> OnNotificationScheduleResult;

	public static event Action<ISN_LocalNotification> OnLocalNotificationReceived;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void RequestNotificationPermissions()
	{
		if (ISN_Device.CurrentDevice.MajorSystemVersion >= 8)
		{
		}
	}

	public void ShowGmaeKitNotification(string title, string message)
	{
		GameCenterManager.ShowGmaeKitNotification(title, message);
	}

	public void CancelAllLocalNotifications()
	{
		SaveNotifications(new List<ISN_LocalNotification>());
	}

	public void CancelLocalNotification(ISN_LocalNotification notification)
	{
		CancelLocalNotificationById(notification.Id);
	}

	public void CancelLocalNotificationById(int notificationId)
	{
	}

	public void ScheduleNotification(ISN_LocalNotification notification)
	{
	}

	public List<ISN_LocalNotification> LoadPendingNotifications(bool includeAll = false)
	{
		return null;
	}

	public void ApplicationIconBadgeNumber(int badges)
	{
	}

	private void OnNotificationScheduleResultAction(string array)
	{
		string[] array2 = array.Split("|"[0]);
		Result result = null;
		result = ((!array2[0].Equals("0")) ? new Result() : new Result(new Error()));
		ISN_LocalNotificationsController.OnNotificationScheduleResult(result);
	}

	private void OnLocalNotificationReceived_Event(string array)
	{
		string[] array2 = array.Split("|"[0]);
		string message = array2[0];
		int id = Convert.ToInt32(array2[1]);
		string data = array2[2];
		int badgesNumber = Convert.ToInt32(array2[3]);
		ISN_LocalNotification iSN_LocalNotification = new ISN_LocalNotification(DateTime.Now, message);
		iSN_LocalNotification.SetData(data);
		iSN_LocalNotification.SetBadgesNumber(badgesNumber);
		iSN_LocalNotification.SetId(id);
		ISN_LocalNotificationsController.OnLocalNotificationReceived(iSN_LocalNotification);
	}

	private void SaveNotifications(List<ISN_LocalNotification> notifications)
	{
		if (notifications.Count == 0)
		{
			PlayerPrefs.DeleteKey("IOSNotificationControllerKey");
			return;
		}
		string text = string.Empty;
		int count = notifications.Count;
		for (int i = 0; i < count; i++)
		{
			if (i != 0)
			{
				text += '|';
			}
			text += notifications[i].SerializedString;
		}
		PlayerPrefs.SetString("IOSNotificationControllerKey", text);
	}

	static ISN_LocalNotificationsController()
	{
		ISN_LocalNotificationsController.OnNotificationScheduleResult = delegate
		{
		};
		ISN_LocalNotificationsController.OnLocalNotificationReceived = delegate
		{
		};
	}
}
