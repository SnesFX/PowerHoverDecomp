using System;
using UnityEngine;

public class IOSMessage : BaseIOSPopup
{
	public string ok;

	public event Action OnComplete = delegate
	{
	};

	public static IOSMessage Create(string title, string message)
	{
		return Create(title, message, "Ok");
	}

	public static IOSMessage Create(string title, string message, string ok)
	{
		IOSMessage iOSMessage = new GameObject("IOSPopUp").AddComponent<IOSMessage>();
		iOSMessage.title = title;
		iOSMessage.message = message;
		iOSMessage.ok = ok;
		iOSMessage.init();
		return iOSMessage;
	}

	public void init()
	{
		IOSNativePopUpManager.showMessage(title, message, ok);
	}

	public void onPopUpCallBack(string buttonIndex)
	{
		this.OnComplete();
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
