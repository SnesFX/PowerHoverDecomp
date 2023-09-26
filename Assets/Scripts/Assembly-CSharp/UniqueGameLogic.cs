using System.Collections;
using UnityEngine;

public class UniqueGameLogic : MonoBehaviour
{
	public void Start()
	{
		StartCoroutine(DestroyOrCreateRoutine(3f, 1f));
		StartCoroutine(ScaleRoutine(3f, 1f));
		StartCoroutine(MakeChildRoutine(3f, 1f));
	}

	public IEnumerator DestroyOrCreateRoutine(float delaySeconds, float runEverySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		while (true)
		{
			if (UniqueObjectManager.CreatedObjects.Count > 9)
			{
				UniqueObjectManager.DestroyObject(UniqueObjectManager.CreatedObjects[Random.Range(0, UniqueObjectManager.CreatedObjects.Count)]);
			}
			else
			{
				UniqueObjectManager.InstantiatePrefab(UniqueObjectManager.Prefabs[Random.Range(0, UniqueObjectManager.Prefabs.Length)].name, Random.insideUnitSphere * 10f, Random.rotation);
			}
			yield return new WaitForSeconds(runEverySeconds);
		}
	}

	public IEnumerator MakeChildRoutine(float delaySeconds, float runEverySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		while (true)
		{
			if (UniqueObjectManager.CreatedObjects.Count > 4)
			{
				UniqueObjectManager.CreatedObjects[0].transform.parent = UniqueObjectManager.CreatedObjects[Random.Range(1, UniqueObjectManager.CreatedObjects.Count)].transform;
			}
			yield return new WaitForSeconds(runEverySeconds);
		}
	}

	public IEnumerator ScaleRoutine(float delaySeconds, float runEverySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		while (true)
		{
			UniqueObjectManager.SceneObjects[Random.Range(0, UniqueObjectManager.SceneObjects.Length)].transform.localScale = Random.insideUnitSphere;
			yield return new WaitForSeconds(runEverySeconds);
		}
	}
}
