using System.Collections.Generic;
using UnityEngine;

public class ES2UniqueID : MonoBehaviour
{
	[HideInInspector]
	public int id;

	public string prefabName = string.Empty;

	private static List<ES2UniqueID> uniqueIDList = new List<ES2UniqueID>();

	public void Awake()
	{
		id = GenerateUniqueID();
		uniqueIDList.Add(this);
	}

	public void OnDestroy()
	{
		uniqueIDList.Remove(this);
	}

	private static int GenerateUniqueID()
	{
		if (uniqueIDList.Count == 0)
		{
			return 0;
		}
		return uniqueIDList[uniqueIDList.Count - 1].id + 1;
	}

	public static ES2UniqueID FindUniqueID(Transform t)
	{
		foreach (ES2UniqueID uniqueID in uniqueIDList)
		{
			if (uniqueID.transform == t)
			{
				return uniqueID;
			}
		}
		return null;
	}

	public static Transform FindTransform(int id)
	{
		foreach (ES2UniqueID uniqueID in uniqueIDList)
		{
			if (uniqueID.id == id)
			{
				return uniqueID.transform;
			}
		}
		return null;
	}
}
