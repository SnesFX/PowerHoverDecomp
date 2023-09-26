using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CurvySplineSegment))]
public class RandomSegment : MonoBehaviour
{
	[Range(0f, 5f)]
	public int Difficulty;

	private RandomSpline randomSpline;

	private CurvySplineSegment segment;

	private bool randomized;

	private GameObject lastPart;

	private void Awake()
	{
		segment = GetComponent<CurvySplineSegment>();
		randomSpline = GetComponentInParent<RandomSpline>();
	}

	private void Update()
	{
		if (!randomized && segment.Spline.IsInitialized && segment.SegmentIndex == 1)
		{
			Randomize(randomSpline.RandomParts);
		}
	}

	public void Disable()
	{
		if (lastPart != null)
		{
			lastPart.GetComponent<RandomPart>().InUse = false;
			lastPart.SetActive(false);
			lastPart = null;
		}
	}

	public void Randomize(List<RandomPart> parts)
	{
		if (parts == null || parts.Count == 0)
		{
			return;
		}
		randomized = true;
		int index = Random.Range(0, parts.Count);
		if (!parts[index].InUse)
		{
			parts[index].InUse = true;
			GameObject gameObject = parts[index].gameObject;
			gameObject.transform.position = base.transform.position;
			if (randomSpline.IsCarSpline && gameObject.GetComponent<CarFollowSpline>() != null)
			{
				CarFollowSpline component = gameObject.GetComponent<CarFollowSpline>();
				component.enabled = true;
				gameObject.transform.position = base.transform.position;
				component.SetPosition(base.transform.position);
				Debug.Log("das follow it");
			}
			else
			{
				gameObject.transform.rotation = segment.GetOrientationFast(0f);
			}
			gameObject.transform.rotation = segment.GetOrientationFast(0f);
			gameObject.transform.parent = base.transform;
			gameObject.transform.Rotate(new Vector3(0f, -90f, 0f));
			lastPart = gameObject;
			gameObject.SetActive(true);
		}
	}
}
