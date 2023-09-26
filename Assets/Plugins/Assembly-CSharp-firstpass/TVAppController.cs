using System;
using SA.Common.Pattern;
using UnityEngine;

public class TVAppController : Singleton<TVAppController>
{
	private bool _IsRuningOnTVDevice;

	public bool IsRuningOnTVDevice
	{
		get
		{
			return _IsRuningOnTVDevice;
		}
	}

	public static event Action DeviceTypeChecked;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void CheckForATVDevice()
	{
		AN_TVControllerProxy.AN_CheckForATVDevice();
	}

	private void OnDeviceStateResponce(string data)
	{
		if (data.Equals("1"))
		{
			_IsRuningOnTVDevice = true;
		}
		TVAppController.DeviceTypeChecked();
	}

	static TVAppController()
	{
		TVAppController.DeviceTypeChecked = delegate
		{
		};
	}
}
