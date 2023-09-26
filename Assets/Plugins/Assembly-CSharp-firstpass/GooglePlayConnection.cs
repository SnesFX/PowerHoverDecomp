using System;
using SA.Common.Pattern;
using UnityEngine;

public class GooglePlayConnection : Singleton<GooglePlayConnection>
{
	private bool _IsInitialized;

	private static GPConnectionState _State;

	public bool IsConnected
	{
		get
		{
			return State == GPConnectionState.STATE_CONNECTED;
		}
	}

	[Obsolete("state is deprecated, please use State instead.")]
	public static GPConnectionState state
	{
		get
		{
			return State;
		}
	}

	public static GPConnectionState State
	{
		get
		{
			return _State;
		}
	}

	[Obsolete("isInitialized is deprecated, please use IsInitialized instead.")]
	public bool isInitialized
	{
		get
		{
			return IsInitialized;
		}
	}

	public bool IsInitialized
	{
		get
		{
			return _IsInitialized;
		}
	}

	public static event Action<GooglePlayConnectionResult> ActionConnectionResultReceived;

	public static event Action<GPConnectionState> ActionConnectionStateChanged;

	public static event Action ActionPlayerConnected;

	public static event Action ActionPlayerDisconnected;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		Singleton<GooglePlayManager>.Instance.Create();
		Init();
	}

	private void Init()
	{
		string text = string.Empty;
		if (AndroidNativeSettings.Instance.EnableGamesAPI)
		{
			text += "GamesAPI";
		}
		if (AndroidNativeSettings.Instance.EnablePlusAPI)
		{
			text += "PlusAPI";
		}
		if (AndroidNativeSettings.Instance.EnableDriveAPI)
		{
			text += "DriveAPI";
		}
		if (AndroidNativeSettings.Instance.EnableAppInviteAPI)
		{
			text += "AppInvite";
		}
		AN_GMSGeneralProxy.setConnectionParams(AndroidNativeSettings.Instance.ShowConnectingPopup);
		AN_GMSGeneralProxy.playServiceInit(text);
	}

	[Obsolete("connect is deprecated, please use Connect instead.")]
	public void connect()
	{
		Connect();
	}

	public void Connect()
	{
		Connect(null);
	}

	[Obsolete("connect is deprecated, please use Connect instead.")]
	public void connect(string accountName)
	{
		Connect(accountName);
	}

	public void Connect(string accountName)
	{
		if (_State != GPConnectionState.STATE_CONNECTED && _State != GPConnectionState.STATE_CONNECTING)
		{
			OnStateChange(GPConnectionState.STATE_CONNECTING);
			if (accountName != null)
			{
				AN_GMSGeneralProxy.playServiceConnect(accountName);
			}
			else
			{
				AN_GMSGeneralProxy.playServiceConnect();
			}
		}
	}

	[Obsolete("disconnect is deprecated, please use Disconnect instead.")]
	public void disconnect()
	{
		Disconnect();
	}

	public void Disconnect()
	{
		if (_State != GPConnectionState.STATE_DISCONNECTED && _State != GPConnectionState.STATE_CONNECTING)
		{
			OnStateChange(GPConnectionState.STATE_DISCONNECTED);
			AN_GMSGeneralProxy.playServiceDisconnect();
		}
	}

	public void SignOut()
	{
		if (_State != GPConnectionState.STATE_DISCONNECTED && _State != GPConnectionState.STATE_CONNECTING)
		{
			OnStateChange(GPConnectionState.STATE_DISCONNECTED);
			AN_GMSGeneralProxy.signOut();
		}
	}

	public static bool CheckState()
	{
		GPConnectionState gPConnectionState = _State;
		if (gPConnectionState == GPConnectionState.STATE_CONNECTED)
		{
			return true;
		}
		return false;
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		AN_GMSGeneralProxy.OnApplicationPause(pauseStatus);
	}

	private void OnPlayServiceDisconnected(string data)
	{
		OnStateChange(GPConnectionState.STATE_DISCONNECTED);
	}

	private void OnConnectionResult(string resultCode)
	{
		GooglePlayConnectionResult googlePlayConnectionResult = new GooglePlayConnectionResult();
		googlePlayConnectionResult.code = (GP_ConnectionResultCode)Convert.ToInt32(resultCode);
		if (googlePlayConnectionResult.IsSuccess)
		{
			OnStateChange(GPConnectionState.STATE_CONNECTED);
		}
		else
		{
			OnStateChange(GPConnectionState.STATE_DISCONNECTED);
		}
		GooglePlayConnection.ActionConnectionResultReceived(googlePlayConnectionResult);
	}

	private void OnStateChange(GPConnectionState connectionState)
	{
		_State = connectionState;
		switch (_State)
		{
		case GPConnectionState.STATE_CONNECTED:
			GooglePlayConnection.ActionPlayerConnected();
			break;
		case GPConnectionState.STATE_DISCONNECTED:
			GooglePlayConnection.ActionPlayerDisconnected();
			break;
		}
		GooglePlayConnection.ActionConnectionStateChanged(_State);
	}

	static GooglePlayConnection()
	{
		GooglePlayConnection.ActionConnectionResultReceived = delegate
		{
		};
		GooglePlayConnection.ActionConnectionStateChanged = delegate
		{
		};
		GooglePlayConnection.ActionPlayerConnected = delegate
		{
		};
		GooglePlayConnection.ActionPlayerDisconnected = delegate
		{
		};
		_State = GPConnectionState.STATE_UNCONFIGURED;
	}
}
