using System;
using SA.Common.Data;
using SA.Common.Models;
using SA.Common.Pattern;
using UnityEngine;

public class ISN_ReplayKit : Singleton<ISN_ReplayKit>
{
	private bool _IsRecodingAvailableToShare;

	public bool IsRecording
	{
		get
		{
			return false;
		}
	}

	public bool IsRecodingAvailableToShare
	{
		get
		{
			return _IsRecodingAvailableToShare;
		}
	}

	public bool IsAvailable
	{
		get
		{
			return false;
		}
	}

	public bool IsMicEnabled
	{
		get
		{
			return false;
		}
	}

	public static event Action<Result> ActionRecordStarted;

	public static event Action<Result> ActionRecordStoped;

	public static event Action<ReplayKitVideoShareResult> ActionShareDialogFinished;

	public static event Action<Error> ActionRecordInterrupted;

	public static event Action<bool> ActionRecorderDidChangeAvailability;

	public static event Action ActionRecordDiscard;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void StartRecording(bool microphoneEnabled = true)
	{
		_IsRecodingAvailableToShare = false;
	}

	public void StopRecording()
	{
	}

	public void DiscardRecording()
	{
		_IsRecodingAvailableToShare = false;
	}

	public void ShowVideoShareDialog()
	{
		_IsRecodingAvailableToShare = false;
	}

	private void OnRecorStartSuccess(string data)
	{
		Result obj = new Result();
		ISN_ReplayKit.ActionRecordStarted(obj);
	}

	private void OnRecorStartFailed(string errorData)
	{
		Result obj = new Result(new Error(errorData));
		ISN_ReplayKit.ActionRecordStarted(obj);
	}

	private void OnRecorStopFailed(string errorData)
	{
		Result obj = new Result(new Error(errorData));
		ISN_ReplayKit.ActionRecordStoped(obj);
	}

	private void OnRecorStopSuccess()
	{
		_IsRecodingAvailableToShare = true;
		Result obj = new Result();
		ISN_ReplayKit.ActionRecordStoped(obj);
	}

	private void OnRecordInterrupted(string errorData)
	{
		_IsRecodingAvailableToShare = false;
		Error obj = new Error(errorData);
		ISN_ReplayKit.ActionRecordInterrupted(obj);
	}

	private void OnRecorderDidChangeAvailability(string data)
	{
		ISN_ReplayKit.ActionRecorderDidChangeAvailability(IsAvailable);
	}

	private void OnSaveResult(string sourcesData)
	{
		string[] sourcesArray = Converter.ParseArray(sourcesData);
		ReplayKitVideoShareResult obj = new ReplayKitVideoShareResult(sourcesArray);
		ISN_ReplayKit.ActionShareDialogFinished(obj);
	}

	public void OnRecordDiscard(string data)
	{
		_IsRecodingAvailableToShare = false;
		ISN_ReplayKit.ActionRecordDiscard();
	}

	static ISN_ReplayKit()
	{
		ISN_ReplayKit.ActionRecordStarted = delegate
		{
		};
		ISN_ReplayKit.ActionRecordStoped = delegate
		{
		};
		ISN_ReplayKit.ActionShareDialogFinished = delegate
		{
		};
		ISN_ReplayKit.ActionRecordInterrupted = delegate
		{
		};
		ISN_ReplayKit.ActionRecorderDidChangeAvailability = delegate
		{
		};
		ISN_ReplayKit.ActionRecordDiscard = delegate
		{
		};
	}
}
