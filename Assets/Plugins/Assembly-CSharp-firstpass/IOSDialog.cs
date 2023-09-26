using System;
using UnityEngine;

public class IOSDialog : BaseIOSPopup
{
	public string yes;

	public string no;

	public event Action<IOSDialogResult> OnComplete = delegate
	{
	};

	public static IOSDialog Create(string title, string message)
	{
		return Create(title, message, "Yes", "No");
	}

	public static IOSDialog Create(string title, string message, string yes, string no)
	{
		IOSDialog iOSDialog = new GameObject("IOSPopUp").AddComponent<IOSDialog>();
		iOSDialog.title = title;
		iOSDialog.message = message;
		iOSDialog.yes = yes;
		iOSDialog.no = no;
		iOSDialog.init();
		return iOSDialog;
	}

	public void init()
	{
		IOSNativePopUpManager.showDialog(title, message, yes, no);
	}

	public void onPopUpCallBack(string buttonIndex)
	{
		switch (Convert.ToInt16(buttonIndex))
		{
		case 0:
			this.OnComplete(IOSDialogResult.YES);
			break;
		case 1:
			this.OnComplete(IOSDialogResult.NO);
			break;
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
