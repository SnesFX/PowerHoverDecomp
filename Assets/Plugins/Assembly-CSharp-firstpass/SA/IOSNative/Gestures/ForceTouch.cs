using System;
using SA.Common.Pattern;
using UnityEngine;

namespace SA.IOSNative.Gestures
{
	public class ForceTouch : Singleton<ForceTouch>
	{
		private static bool _IsTouchTrigerred;

		public static string AppOpenshortcutItem
		{
			get
			{
				return string.Empty;
			}
		}

		public event Action OnForceTouchStarted = delegate
		{
		};

		public event Action OnForceTouchFinished = delegate
		{
		};

		public event Action<ForceInfo> OnForceChanged = delegate
		{
		};

		public event Action<string> OnAppShortcutClick = delegate
		{
		};

		private void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}

		public void Setup(float forceTouchDelay, float baseForceTouchPressure, float triggeringForceTouchPressure)
		{
		}

		private void didStartForce(string array)
		{
			_IsTouchTrigerred = true;
			this.OnForceTouchStarted();
		}

		private void didForceChanged(string array)
		{
			if (_IsTouchTrigerred)
			{
				string[] array2 = array.Split('|');
				float force = Convert.ToSingle(array2[0]);
				float maxForce = Convert.ToSingle(array2[1]);
				ForceInfo obj = new ForceInfo(force, maxForce);
				this.OnForceChanged(obj);
			}
		}

		private void didForceEnded(string array)
		{
			_IsTouchTrigerred = false;
			this.OnForceTouchFinished();
		}

		private void performActionForShortcutItem(string action)
		{
			this.OnAppShortcutClick(action);
		}
	}
}
