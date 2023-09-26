using System;
using System.Collections.Generic;
using SA.Common.Data;
using SA.Common.Models;
using SA.Common.Pattern;
using UnityEngine;

public class iCloudManager : Singleton<iCloudManager>
{
	private Dictionary<string, List<Action<iCloudData>>> s_requestDataCallbacks = new Dictionary<string, List<Action<iCloudData>>>();

	public static event Action<Result> OnCloudInitAction;

	public static event Action<iCloudData> OnCloudDataReceivedAction;

	public static event Action<List<iCloudData>> OnStoreDidChangeExternally;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	[Obsolete("use SetString instead")]
	public void setString(string key, string val)
	{
		SetString(key, val);
	}

	public void SetString(string key, string val)
	{
	}

	[Obsolete("use SetFloat instead")]
	public void setFloat(string key, float val)
	{
		SetFloat(key, val);
	}

	public void SetFloat(string key, float val)
	{
	}

	[Obsolete("use SetData instead")]
	public void setData(string key, byte[] val)
	{
		SetData(key, val);
	}

	public void SetData(string key, byte[] val)
	{
	}

	public void SetObject(string key, object val)
	{
	}

	public void SetInt(string key, int val)
	{
		string val2 = Convert.ToString(val);
		SetString(key, val2);
	}

	public void SetLong(string key, long val)
	{
		string val2 = Convert.ToString(val);
		SetString(key, val2);
	}

	public void SetUlong(string key, ulong val)
	{
		string val2 = Convert.ToString(val);
		SetString(key, val2);
	}

	public void SetArray(string key, List<object> val)
	{
		string val2 = Json.Serialize(val);
		SetString(key, val2);
	}

	public void SetDictionary(string key, Dictionary<object, object> val)
	{
		string val2 = Json.Serialize(val);
		SetString(key, val2);
	}

	[Obsolete("use RequestDataForKey instead")]
	public void requestDataForKey(string key)
	{
		RequestDataForKey(key);
	}

	public void RequestDataForKey(string key)
	{
		RequestDataForKey(key, null);
	}

	public void RequestDataForKey(string key, Action<iCloudData> callback)
	{
		if (callback != null)
		{
			if (s_requestDataCallbacks.ContainsKey(key))
			{
				s_requestDataCallbacks[key].Add(callback);
				return;
			}
			s_requestDataCallbacks.Add(key, new List<Action<iCloudData>> { callback });
		}
	}

	private void OnCloudInit()
	{
		Result obj = new Result();
		iCloudManager.OnCloudInitAction(obj);
	}

	private void OnCloudInitFail()
	{
		Result obj = new Result(new Error());
		iCloudManager.OnCloudInitAction(obj);
	}

	private void OnCloudDataChanged(string data)
	{
		List<iCloudData> list = new List<iCloudData>();
		string[] array = data.Split('|');
		for (int i = 0; i < array.Length && !(array[i] == "endofline"); i += 2)
		{
			iCloudData item = new iCloudData(array[i], array[i + 1]);
			list.Add(item);
		}
		iCloudManager.OnStoreDidChangeExternally(list);
	}

	private void OnCloudData(string array)
	{
		string[] array2 = array.Split('|');
		iCloudData iCloudData2 = new iCloudData(array2[0], array2[1]);
		if (s_requestDataCallbacks.ContainsKey(iCloudData2.Key))
		{
			List<Action<iCloudData>> list = s_requestDataCallbacks[iCloudData2.Key];
			s_requestDataCallbacks.Remove(iCloudData2.Key);
			foreach (Action<iCloudData> item in list)
			{
				item(iCloudData2);
			}
		}
		iCloudManager.OnCloudDataReceivedAction(iCloudData2);
	}

	private void OnCloudDataEmpty(string array)
	{
		string[] array2 = array.Split('|');
		iCloudData obj = new iCloudData(array2[0], "null");
		iCloudManager.OnCloudDataReceivedAction(obj);
	}

	static iCloudManager()
	{
		iCloudManager.OnCloudInitAction = delegate
		{
		};
		iCloudManager.OnCloudDataReceivedAction = delegate
		{
		};
		iCloudManager.OnStoreDidChangeExternally = delegate
		{
		};
	}
}
