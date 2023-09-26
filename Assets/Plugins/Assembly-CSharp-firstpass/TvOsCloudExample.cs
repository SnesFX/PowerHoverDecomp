using SA.Common.Pattern;
using UnityEngine;

public class TvOsCloudExample : MonoBehaviour
{
	private void Start()
	{
		Debug.Log("iCloudManager.Instance.init()");
		iCloudManager.OnCloudDataReceivedAction += OnCloudDataReceivedAction;
		Singleton<iCloudManager>.Instance.SetString("Test", "test");
		Singleton<iCloudManager>.Instance.RequestDataForKey("Test", delegate(iCloudData data)
		{
			Debug.Log("Internal callback");
			if (data.IsEmpty)
			{
				Debug.Log(data.Key + " / data is empty");
			}
			else
			{
				Debug.Log(data.Key + " / " + data.StringValue);
			}
		});
	}

	private void OnCloudDataReceivedAction(iCloudData data)
	{
		Debug.Log("OnCloudDataReceivedAction");
		if (data.IsEmpty)
		{
			Debug.Log(data.Key + " / data is empty");
		}
		else
		{
			Debug.Log(data.Key + " / " + data.StringValue);
		}
	}
}
