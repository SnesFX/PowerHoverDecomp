using System.Collections.Generic;
using UnityEngine;

public class ChallengePath : ResetObject
{
	private ChallengePart[] parts;

	private SplinePathCloneBuilder buildPath;

	private int activateIndex;

	private int disableIndex;

	private bool resetCalled;

	private void Start()
	{
		buildPath = GetComponent<SplinePathCloneBuilder>();
		if (SceneLoader.Instance.Current == null)
		{
			return;
		}
		int num = Mathf.Min(5, SceneLoader.Instance.Current.Storage.CasetteState);
		List<GameObject> list = new List<GameObject>();
		GameObject[] source = buildPath.Source;
		foreach (GameObject gameObject in source)
		{
			ChallengePart component = gameObject.GetComponent<ChallengePart>();
			if (!(component == null) && (component.PartLevel == -1 || num == component.PartLevel || (component.PartLevels != null && component.PartLevels.Contains(num))))
			{
				list.Add(component.gameObject);
			}
		}
		if (list.Count > 0)
		{
			buildPath.Source = list.ToArray();
		}
	}

	private void LateUpdate()
	{
		if (buildPath != null && buildPath.Spline != null && buildPath.Spline.IsInitialized && !resetCalled)
		{
			resetCalled = true;
			Reset(false);
		}
	}

	public override void Reset(bool isRewind)
	{
		base.Reset(isRewind);
		Transform transform = buildPath.Detach();
		transform.gameObject.SetActive(false);
		buildPath.Refresh(true);
		Object.Destroy(transform.gameObject);
		activateIndex = 5;
		disableIndex = -3;
		parts = GetComponentsInChildren<ChallengePart>(true);
		for (int i = 0; i < buildPath.ObjectCount; i++)
		{
			parts[i].gameObject.SetActive(true);
			if (parts[i] != null)
			{
				parts[i].Randomize();
			}
			if (i > 5)
			{
				parts[i].gameObject.SetActive(false);
			}
		}
	}

	public void MoveToNext()
	{
		if (parts != null)
		{
			activateIndex++;
			disableIndex++;
			if (activateIndex > parts.Length - 1)
			{
				activateIndex = 0;
			}
			if (disableIndex > parts.Length - 1)
			{
				disableIndex = 0;
			}
			parts[activateIndex].gameObject.SetActive(true);
			if (disableIndex > -1)
			{
				parts[disableIndex].gameObject.SetActive(false);
			}
		}
	}
}
