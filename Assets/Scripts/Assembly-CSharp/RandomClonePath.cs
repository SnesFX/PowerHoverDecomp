using UnityEngine;

public class RandomClonePath : ResetObject
{
	private SplinePathCloneBuilder buildPath;

	private CloneRandomPart[] parts;

	private int activateIndex;

	private int disableIndex;

	private bool resetCalled;

	private void Start()
	{
		buildPath = GetComponent<SplinePathCloneBuilder>();
	}

	private void Update()
	{
		if (buildPath.Spline.IsInitialized && !resetCalled)
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
		disableIndex = -2;
		parts = GetComponentsInChildren<CloneRandomPart>(true);
		for (int i = 0; i < parts.Length; i++)
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
