using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
	public GameObject prefab;

	public string filename = "SavedPrefabs.txt";

	public int buttonPositionX;

	private List<GameObject> createdPrefabs = new List<GameObject>();

	private void Start()
	{
		if (ES2.Exists(filename))
		{
			LoadAllPrefabs();
		}
	}

	private void LoadAllPrefabs()
	{
		int num = ES2.Load<int>(filename + "?tag=prefabCount");
		for (int i = 0; i < num; i++)
		{
			LoadPrefab(i);
		}
	}

	private void LoadPrefab(int tag)
	{
		GameObject gameObject = Object.Instantiate(prefab);
		ES2.Load(filename + "?tag=" + tag, gameObject.transform);
		createdPrefabs.Add(gameObject);
	}

	private void CreateRandomPrefab()
	{
		GameObject item = Object.Instantiate(prefab, Random.insideUnitSphere * 5f, Random.rotation);
		createdPrefabs.Add(item);
	}

	private void OnApplicationQuit()
	{
		ES2.Save(createdPrefabs.Count, filename + "?tag=prefabCount");
		for (int i = 0; i < createdPrefabs.Count; i++)
		{
			SavePrefab(createdPrefabs[i], i);
		}
	}

	private void SavePrefab(GameObject prefabToSave, int tag)
	{
		ES2.Save(prefabToSave.transform, filename + "?tag=" + tag);
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(buttonPositionX, 0f, 250f, 100f), "Create Random " + prefab.name))
		{
			CreateRandomPrefab();
		}
		if (GUI.Button(new Rect(buttonPositionX, 100f, 250f, 100f), "Delete Saved " + prefab.name))
		{
			ES2.Delete(filename);
			for (int i = 0; i < createdPrefabs.Count; i++)
			{
				Object.Destroy(createdPrefabs[i]);
			}
			createdPrefabs.Clear();
		}
	}
}
