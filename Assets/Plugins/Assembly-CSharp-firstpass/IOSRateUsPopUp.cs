using System;
using UnityEngine;

public class IOSRateUsPopUp : BaseIOSPopup
{
	public string rate;

	public string remind;

	public string declined;

	public event Action<IOSDialogResult> OnComplete = delegate
	{
	};

	public static IOSRateUsPopUp Create()
	{
		return Create("Like the Game?", "Rate Us");
	}

	public static IOSRateUsPopUp Create(string title, string message)
	{
		return Create(title, message, "Rate Now", "Ask me later", "No, thanks");
	}

	public static IOSRateUsPopUp Create(string title, string message, string rate, string remind, string declined)
	{
		IOSRateUsPopUp iOSRateUsPopUp = new GameObject("IOSRateUsPopUp").AddComponent<IOSRateUsPopUp>();
		iOSRateUsPopUp.title = title;
		iOSRateUsPopUp.message = message;
		iOSRateUsPopUp.rate = rate;
		iOSRateUsPopUp.remind = remind;
		iOSRateUsPopUp.declined = declined;
		iOSRateUsPopUp.init();
		return iOSRateUsPopUp;
	}

	public void init()
	{
		IOSNativePopUpManager.showRateUsPopUp(title, message, rate, remind, declined);
	}

	public void onPopUpCallBack(string buttonIndex)
	{
		switch (Convert.ToInt16(buttonIndex))
		{
		case 0:
			IOSNativeUtility.RedirectToAppStoreRatingPage();
			this.OnComplete(IOSDialogResult.RATED);
			break;
		case 1:
			this.OnComplete(IOSDialogResult.REMIND);
			break;
		case 2:
			this.OnComplete(IOSDialogResult.DECLINED);
			break;
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
