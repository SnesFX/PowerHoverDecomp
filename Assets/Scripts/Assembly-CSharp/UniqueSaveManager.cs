using UnityEngine;

public class UniqueSaveManager : MonoBehaviour
{
	public string sceneObjectFile = "sceneObjectsFile.txt";

	public string createdObjectFile = "createdObjectsFile.txt";

	public void OnApplicationQuit()
	{
		Save();
	}

	public void Start()
	{
		if (ES2.Exists(sceneObjectFile))
		{
			int num = ES2.Load<int>(sceneObjectFile + "?tag=sceneObjectCount");
			for (int i = 0; i < num; i++)
			{
				LoadObject(i, sceneObjectFile);
			}
		}
		if (ES2.Exists(createdObjectFile))
		{
			int num2 = ES2.Load<int>(createdObjectFile + "?tag=createdObjectCount");
			for (int j = 0; j < num2; j++)
			{
				LoadObject(j, createdObjectFile);
			}
		}
	}

	private void Save()
	{
		ES2.Save(UniqueObjectManager.SceneObjects.Length, sceneObjectFile + "?tag=sceneObjectCount");
		for (int i = 0; i < UniqueObjectManager.SceneObjects.Length; i++)
		{
			SaveObject(UniqueObjectManager.SceneObjects[i], i, sceneObjectFile);
		}
		ES2.Save(UniqueObjectManager.CreatedObjects.Count, createdObjectFile + "?tag=createdObjectCount");
		for (int j = 0; j < UniqueObjectManager.CreatedObjects.Count; j++)
		{
			SaveObject(UniqueObjectManager.CreatedObjects[j], j, createdObjectFile);
		}
	}

	private void SaveObject(GameObject obj, int i, string file)
	{
		ES2UniqueID component = obj.GetComponent<ES2UniqueID>();
		ES2.Save(component.id, file + "?tag=uniqueID" + i);
		ES2.Save(component.prefabName, file + "?tag=prefabName" + i);
		ES2.Save(component.gameObject.activeSelf, file + "?tag=active" + i);
		Transform component2 = obj.GetComponent<Transform>();
		if (component2 != null)
		{
			ES2.Save(component2, file + "?tag=transform" + i);
			ES2UniqueID eS2UniqueID = ES2UniqueID.FindUniqueID(component2.parent);
			if (eS2UniqueID == null)
			{
				ES2.Save(-1, file + "?tag=parentID" + i);
			}
			else
			{
				ES2.Save(eS2UniqueID.id, file + "?tag=parentID" + i);
			}
		}
	}

	private void LoadObject(int i, string file)
	{
		int id = ES2.Load<int>(file + "?tag=uniqueID" + i);
		string text = ES2.Load<string>(file + "?tag=prefabName" + i);
		GameObject gameObject = ((!(text == string.Empty)) ? UniqueObjectManager.InstantiatePrefab(text) : ES2UniqueID.FindTransform(id).gameObject);
		gameObject.SetActive(ES2.Load<bool>(file + "?tag=active" + i));
		Transform component = gameObject.GetComponent<Transform>();
		if (component != null)
		{
			ES2.Load(file + "?tag=transform" + i, component);
			int id2 = ES2.Load<int>(file + "?tag=parentID" + i);
			Transform transform = ES2UniqueID.FindTransform(id2);
			if (transform != null)
			{
				component.parent = transform;
			}
		}
	}
}
