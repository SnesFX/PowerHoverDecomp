using System;
using System.Reflection;
using SA.Common.Pattern;
using UnityEngine;

public class AndroidTwitterManager : Singleton<AndroidTwitterManager>, TwitterManagerInterface
{
	private bool _IsAuthed;

	private bool _IsInited;

	private string _AccessToken = string.Empty;

	private string _AccessTokenSecret = string.Empty;

	private TwitterUserInfo _userInfo;

	public bool IsAuthed
	{
		get
		{
			return _IsAuthed;
		}
	}

	public bool IsInited
	{
		get
		{
			return _IsInited;
		}
	}

	public TwitterUserInfo userInfo
	{
		get
		{
			return _userInfo;
		}
	}

	public string AccessToken
	{
		get
		{
			return _AccessToken;
		}
	}

	public string AccessTokenSecret
	{
		get
		{
			return _AccessTokenSecret;
		}
	}

	public event Action OnTwitterLoginStarted = delegate
	{
	};

	public event Action OnTwitterLogOut = delegate
	{
	};

	public event Action OnTwitterPostStarted = delegate
	{
	};

	public event Action<TWResult> OnTwitterInitedAction = delegate
	{
	};

	public event Action<TWResult> OnAuthCompleteAction = delegate
	{
	};

	public event Action<TWResult> OnPostingCompleteAction = delegate
	{
	};

	public event Action<TWResult> OnUserDataRequestCompleteAction = delegate
	{
	};

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void Init()
	{
		try
		{
			Type type = Type.GetType("AN_SoomlaGrow");
			MethodInfo method = type.GetMethod("Init", BindingFlags.Static | BindingFlags.Public);
			method.Invoke(null, null);
		}
		catch (Exception ex)
		{
			Debug.LogError("AndroidNative: Soomla Initalization failed" + ex.Message);
		}
		Init(SocialPlatfromSettings.Instance.TWITTER_CONSUMER_KEY, SocialPlatfromSettings.Instance.TWITTER_CONSUMER_SECRET);
	}

	public void Init(string consumer_key, string consumer_secret)
	{
		if (!_IsInited)
		{
			_IsInited = true;
			AndroidNative.TwitterInit(consumer_key, consumer_secret);
		}
	}

	public void AuthenticateUser()
	{
		this.OnTwitterLoginStarted();
		if (_IsAuthed)
		{
			OnAuthSuccess();
		}
		else
		{
			AndroidNative.AuthificateUser();
		}
	}

	public void LoadUserData()
	{
		if (_IsAuthed)
		{
			AndroidNative.LoadUserData();
			return;
		}
		Debug.LogWarning("Auth user before loadin data, fail event generated");
		TWResult obj = new TWResult(false, null);
		this.OnUserDataRequestCompleteAction(obj);
	}

	public void Post(string status)
	{
		this.OnTwitterPostStarted();
		if (!_IsAuthed)
		{
			Debug.LogWarning("Auth user before posting data, fail event generated");
			TWResult obj = new TWResult(false, null);
			this.OnPostingCompleteAction(obj);
		}
		else
		{
			AndroidNative.TwitterPost(status);
		}
	}

	public void Post(string status, Texture2D texture)
	{
		this.OnTwitterPostStarted();
		if (!_IsAuthed)
		{
			Debug.LogWarning("Auth user before posting data, fail event generated");
			TWResult obj = new TWResult(false, null);
			this.OnPostingCompleteAction(obj);
		}
		else
		{
			byte[] inArray = texture.EncodeToPNG();
			string data = Convert.ToBase64String(inArray);
			AndroidNative.TwitterPostWithImage(status, data);
		}
	}

	public TwitterPostingTask PostWithAuthCheck(string status)
	{
		return PostWithAuthCheck(status, null);
	}

	public TwitterPostingTask PostWithAuthCheck(string status, Texture2D texture)
	{
		TwitterPostingTask twitterPostingTask = TwitterPostingTask.Cretae();
		twitterPostingTask.Post(status, texture, this);
		return twitterPostingTask;
	}

	public void LogOut()
	{
		this.OnTwitterLogOut();
		_IsAuthed = false;
		AndroidNative.LogoutFromTwitter();
	}

	private void OnInited(string data)
	{
		if (data.Equals("1"))
		{
			_IsAuthed = true;
		}
		TWResult obj = new TWResult(true, null);
		this.OnTwitterInitedAction(obj);
	}

	private void OnAuthSuccess()
	{
		_IsAuthed = true;
		TWResult obj = new TWResult(true, null);
		this.OnAuthCompleteAction(obj);
	}

	private void OnAuthFailed()
	{
		TWResult obj = new TWResult(false, null);
		this.OnAuthCompleteAction(obj);
	}

	private void OnPostSuccess()
	{
		TWResult obj = new TWResult(true, null);
		this.OnPostingCompleteAction(obj);
	}

	private void OnPostFailed()
	{
		TWResult obj = new TWResult(false, null);
		this.OnPostingCompleteAction(obj);
	}

	private void OnUserDataLoaded(string data)
	{
		_userInfo = new TwitterUserInfo(data);
		TWResult obj = new TWResult(true, data);
		this.OnUserDataRequestCompleteAction(obj);
	}

	private void OnUserDataLoadFailed()
	{
		TWResult obj = new TWResult(false, null);
		this.OnUserDataRequestCompleteAction(obj);
	}

	private void OnAuthInfoReceived(string data)
	{
		Debug.Log("OnAuthInfoReceived");
		string[] array = data.Split("|"[0]);
		_AccessToken = array[0];
		_AccessTokenSecret = array[1];
	}
}
