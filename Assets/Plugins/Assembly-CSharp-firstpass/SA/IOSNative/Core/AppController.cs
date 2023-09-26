using System;
using SA.Common.Pattern;
using SA.IOSNative.Models;
using SA.IOSNative.UserNotifications;
using UnityEngine;

namespace SA.IOSNative.Core
{
	public class AppController : Singleton<AppController>
	{
		public static LaunchUrl LaunchUrl
		{
			get
			{
				return new LaunchUrl(string.Empty, string.Empty);
			}
		}

		public static UniversalLink LaunchUniversalLink
		{
			get
			{
				return new UniversalLink(string.Empty);
			}
		}

		public static NotificationRequest LaunchNotification
		{
			get
			{
				return new NotificationRequest();
			}
		}

		public static event Action OnApplicationDidEnterBackground;

		public static event Action OnApplicationDidBecomeActive;

		public static event Action OnApplicationDidReceiveMemoryWarning;

		public static event Action OnApplicationWillResignActive;

		public static event Action OnApplicationWillTerminate;

		public static event Action<LaunchUrl> OnOpenURL;

		public static event Action<UniversalLink> OnContinueUserActivity;

		private void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}

		public static void Subscribe()
		{
			Singleton<AppController>.Instance.enabled = true;
		}

		private void openURL(string data)
		{
			LaunchUrl obj = new LaunchUrl(data);
			AppController.OnOpenURL(obj);
		}

		private void continueUserActivity(string absoluteUrl)
		{
			UniversalLink obj = new UniversalLink(absoluteUrl);
			AppController.OnContinueUserActivity(obj);
		}

		private void applicationDidEnterBackground()
		{
			AppController.OnApplicationDidEnterBackground();
		}

		private void applicationDidBecomeActive()
		{
			AppController.OnApplicationDidBecomeActive();
		}

		private void applicationDidReceiveMemoryWarning()
		{
			AppController.OnApplicationDidReceiveMemoryWarning();
		}

		private void applicationWillResignActive()
		{
			AppController.OnApplicationWillResignActive();
		}

		private void applicationWillTerminate()
		{
			AppController.OnApplicationWillTerminate();
		}

		protected override void OnApplicationQuit()
		{
			base.OnApplicationQuit();
			AppController.OnApplicationWillTerminate();
		}

		static AppController()
		{
			AppController.OnApplicationDidEnterBackground = delegate
			{
			};
			AppController.OnApplicationDidBecomeActive = delegate
			{
			};
			AppController.OnApplicationDidReceiveMemoryWarning = delegate
			{
			};
			AppController.OnApplicationWillResignActive = delegate
			{
			};
			AppController.OnApplicationWillTerminate = delegate
			{
			};
			AppController.OnOpenURL = delegate
			{
			};
			AppController.OnContinueUserActivity = delegate
			{
			};
		}
	}
}
