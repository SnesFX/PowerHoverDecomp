using System;
using SA.Common.Pattern;
using UnityEngine;

public class FB_LikesRetrieveTask : MonoBehaviour
{
	private string _userId;

	public string userId
	{
		get
		{
			return _userId;
		}
	}

	public event Action<FB_Result, FB_LikesRetrieveTask> ActionComplete = delegate
	{
	};

	public static FB_LikesRetrieveTask Create()
	{
		return new GameObject("FBLikesRetrieveTask").AddComponent<FB_LikesRetrieveTask>();
	}

	public void LoadLikes(string userId)
	{
		_userId = userId;
		Singleton<SPFacebook>.Instance.FB.API("/" + userId + "/likes", FB_HttpMethod.GET, OnUserLikesResult);
	}

	public void LoadLikes(string userId, string pageId)
	{
		_userId = userId;
		Singleton<SPFacebook>.Instance.FB.API("/" + userId + "/likes/" + pageId, FB_HttpMethod.GET, OnUserLikesResult);
	}

	private void OnUserLikesResult(FB_Result result)
	{
		this.ActionComplete(result, this);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
