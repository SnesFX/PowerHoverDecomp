using System;
using SA.Common.Pattern;
using UnityEngine;

public class ISN_GestureRecognizer : Singleton<ISN_GestureRecognizer>
{
	public event Action<ISN_SwipeDirection> OnSwipe = delegate
	{
	};

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnSwipeAction(string data)
	{
		int obj = Convert.ToInt32(data);
		this.OnSwipe((ISN_SwipeDirection)obj);
	}
}
