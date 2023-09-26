using System;
using SA.Common.Pattern;
using UnityEngine;

public class IOSDateTimePicker : Singleton<IOSDateTimePicker>
{
	public Action<DateTime> OnDateChanged = delegate
	{
	};

	public Action<DateTime> OnPickerClosed = delegate
	{
	};

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void Show(IOSDateTimePickerMode mode)
	{
	}

	public void Show(IOSDateTimePickerMode mode, DateTime dateTime)
	{
	}

	private void DateChangedEvent(string time)
	{
		DateTime obj = DateTime.Parse(time);
		OnDateChanged(obj);
	}

	private void PickerClosed(string time)
	{
		DateTime obj = DateTime.Parse(time);
		OnPickerClosed(obj);
	}
}
