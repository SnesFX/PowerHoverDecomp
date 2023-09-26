using SA.Common.Pattern;
using UnityEngine;

public class FacebookAndroidTurnBasedAndGiftsExample : MonoBehaviour
{
	private static bool IsUserInfoLoaded;

	private static bool IsAuntificated;

	public DefaultPreviewButton[] ConnectionDependedntButtons;

	public DefaultPreviewButton connectButton;

	public SA_Texture avatar;

	public SA_Label Location;

	public SA_Label Language;

	public SA_Label Mail;

	public SA_Label Name;

	private string BombItemId = "993386627342473";

	private void Awake()
	{
		SPFacebook.OnInitCompleteAction += OnInit;
		SPFacebook.OnFocusChangedAction += OnFocusChanged;
		SPFacebook.OnAuthCompleteAction += OnAuth;
		Singleton<SPFacebook>.Instance.Init();
		SA_StatusBar.text = "initializing Facebook";
	}

	private void FixedUpdate()
	{
		if (IsAuntificated)
		{
			connectButton.text = "Disconnect";
			Name.text = "Player Connected";
			DefaultPreviewButton[] connectionDependedntButtons = ConnectionDependedntButtons;
			foreach (DefaultPreviewButton defaultPreviewButton in connectionDependedntButtons)
			{
				defaultPreviewButton.EnabledButton();
			}
			if (IsUserInfoLoaded && Singleton<SPFacebook>.Instance.userInfo.GetProfileImage(FB_ProfileImageSize.square) != null)
			{
				avatar.texture = Singleton<SPFacebook>.Instance.userInfo.GetProfileImage(FB_ProfileImageSize.square);
				Name.text = Singleton<SPFacebook>.Instance.userInfo.Name + " aka " + Singleton<SPFacebook>.Instance.userInfo.UserName;
				Location.text = Singleton<SPFacebook>.Instance.userInfo.Location;
				Language.text = Singleton<SPFacebook>.Instance.userInfo.Locale;
			}
		}
		else
		{
			DefaultPreviewButton[] connectionDependedntButtons2 = ConnectionDependedntButtons;
			foreach (DefaultPreviewButton defaultPreviewButton2 in connectionDependedntButtons2)
			{
				defaultPreviewButton2.DisabledButton();
			}
			connectButton.text = "Connect";
			Name.text = "Player Disconnected";
		}
	}

	public void RetriveAppRequests()
	{
		Singleton<SPFacebook>.Instance.LoadPendingRequests();
		SPFacebook.OnAppRequestsLoaded += OnAppRequestsLoaded;
	}

	private void OnAppRequestsLoaded(FB_Result result)
	{
		if (result.IsSucceeded)
		{
			foreach (FB_AppRequest appRequest in Singleton<SPFacebook>.Instance.AppRequests)
			{
				Debug.Log("--------------------------");
				Debug.Log(appRequest.Id);
				Debug.Log(appRequest.Message);
				Debug.Log(appRequest.ActionType);
				Debug.Log(appRequest.State);
				Debug.Log(appRequest.Data);
				Debug.Log("--------------------------");
			}
		}
		SPFacebook.OnAppRequestsLoaded -= OnAppRequestsLoaded;
	}

	public void SendTrunhRequest()
	{
		Singleton<SPFacebook>.Instance.SendTrunRequest("Smaple title", "Smaple Message", string.Empty);
	}

	public void SendTrunhRequestToSpecifiedFriend()
	{
		string text = "1405568046403868";
		Singleton<SPFacebook>.Instance.SendTrunRequest("Sample Titile", "Sample message", "some_request_data", new string[2] { text, "716261781804613" });
		SPFacebook.OnAppRequestCompleteAction += OnAppRequestCompleteAction;
		Singleton<SPFacebook>.Instance.AppRequest("Play with me", new string[2] { text, "716261781804613" }, null, null, null, string.Empty, string.Empty);
	}

	public void AskItem()
	{
		Singleton<SPFacebook>.Instance.AskGift("Sample Titile", "Sample message", BombItemId, string.Empty);
	}

	public void SendItem()
	{
		Singleton<SPFacebook>.Instance.SendGift("Sample Titile", "Sample message", BombItemId, string.Empty);
	}

	public void SendInv()
	{
		string title = "Hello";
		string message = "Play with me";
		Singleton<SPFacebook>.Instance.SendInvite(title, message, string.Empty);
		SPFacebook.OnAppRequestCompleteAction += OnAppRequestCompleteAction;
	}

	public void SendInvToSpecifayedFirend()
	{
		string text = "1405568046403868";
		string title = "Hello";
		string message = "Play with me";
		string data = "some_request_data";
		Singleton<SPFacebook>.Instance.SendInvite(title, message, data, new string[1] { text });
		SPFacebook.OnAppRequestCompleteAction += OnAppRequestCompleteAction;
	}

	public void SendToSpecifiedFriend()
	{
		string text = "1405568046403868";
		Singleton<SPFacebook>.Instance.SendGift("Sample Titile", "Sample message", BombItemId, "some_request_dara", new string[1] { text });
		SPFacebook.OnAppRequestCompleteAction += OnAppRequestCompleteAction;
	}

	private void OnAppRequestCompleteAction(FB_AppRequestResult result)
	{
		if (result.IsSucceeded)
		{
			Debug.Log("App request succeeded");
			Debug.Log("ReuqetsId: " + result.ReuqestId);
			foreach (string recipient in result.Recipients)
			{
				Debug.Log(recipient);
			}
			Debug.Log("Original Facebook Responce: " + result.RawData);
		}
		else
		{
			Debug.Log("App request has failed");
		}
		SPFacebook.OnAppRequestCompleteAction -= OnAppRequestCompleteAction;
	}

	private void Connect()
	{
		if (!IsAuntificated)
		{
			Singleton<SPFacebook>.Instance.Login("email,publish_actions");
			SA_StatusBar.text = "Log in...";
		}
		else
		{
			LogOut();
			SA_StatusBar.text = "Logged out";
		}
	}

	private void LoadUserData()
	{
		SPFacebook.OnUserDataRequestCompleteAction += OnUserDataLoaded;
		Singleton<SPFacebook>.Instance.LoadUserData();
		SA_StatusBar.text = "Loadin user data..";
	}

	private void OnFocusChanged(bool focus)
	{
		if (!focus)
		{
			Time.timeScale = 0f;
		}
		else
		{
			Time.timeScale = 1f;
		}
	}

	private void OnUserDataLoaded(FB_Result result)
	{
		SPFacebook.OnUserDataRequestCompleteAction -= OnUserDataLoaded;
		if (result.IsSucceeded)
		{
			SA_StatusBar.text = "User data loaded";
			IsUserInfoLoaded = true;
			Singleton<SPFacebook>.Instance.userInfo.LoadProfileImage(FB_ProfileImageSize.square);
		}
		else
		{
			SA_StatusBar.text = "Opps, user data load failed, something was wrong";
			Debug.Log("Opps, user data load failed, something was wrong");
		}
	}

	private void OnInit()
	{
		if (Singleton<SPFacebook>.Instance.IsLoggedIn)
		{
			IsAuntificated = true;
		}
		else
		{
			SA_StatusBar.text = "user Login -> fale";
		}
	}

	private void OnAuth(FB_Result result)
	{
		if (Singleton<SPFacebook>.Instance.IsLoggedIn)
		{
			IsAuntificated = true;
			LoadUserData();
			SA_StatusBar.text = "user Login -> true";
		}
		else
		{
			Debug.Log("Failed to log in");
		}
	}

	private void LogOut()
	{
		IsUserInfoLoaded = false;
		IsAuntificated = false;
		Singleton<SPFacebook>.Instance.Logout();
	}
}
