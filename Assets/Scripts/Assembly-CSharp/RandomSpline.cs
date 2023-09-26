using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CurvySpline))]
public class RandomSpline : ResetObject
{
	public const int DIFFICULTY_LIMIT = 1000;

	public List<RandomPart> RandomParts;

	public bool IsCarSpline;

	public float LaneWidth;

	private CurvySpline Spline;

	private SplineWalker walker;

	private CurvySplineSegment nextSegment;

	private CurvySplineSegment[] checkSegments;

	private HoverController hoverController;

	private float SplineMeshHalfWidth = -1f;

	private float CheckTimer;

	private void Awake()
	{
		hoverController = Object.FindObjectOfType<HoverController>();
		RandomParts = Object.FindObjectsOfType<RandomPart>().ToList();
		Spline = GetComponent<CurvySpline>();
		walker = Object.FindObjectOfType<SplineWalker>();
	}

	private IEnumerator Start()
	{
		if ((bool)Spline)
		{
			while (!Spline.IsInitialized)
			{
				yield return null;
			}
		}
		if (GetComponentInChildren<SplinePathMeshBuilder>() != null)
		{
			SplineMeshHalfWidth = GetComponentInChildren<SplinePathMeshBuilder>().CapWidth * 0.5f;
			LaneWidth = SplineMeshHalfWidth * 0.25f;
		}
		foreach (RandomPart randomPart in RandomParts)
		{
			randomPart.SetSpline(Spline);
			randomPart.Collectables = randomPart.GetComponentsInChildren<Collectable>();
			randomPart.gameObject.SetActive(false);
		}
		Reset(false);
	}

	private void FixedUpdate()
	{
		if (!Spline.IsInitialized || !(nextSegment != null))
		{
			return;
		}
		if (CheckTimer > 0f)
		{
			CheckTimer -= Time.fixedDeltaTime;
		}
		if (CheckTimer <= 0f && nextSegment != Spline.NextSegment(Spline.TFToSegment(Spline.GetNearestPointTFExt(walker.transform.position, checkSegments))))
		{
			if (nextSegment.NextSegment.NextSegment.NextTransform != null && nextSegment.NextSegment.NextSegment.NextTransform.GetComponent<RandomSegment>() != null)
			{
				RandomSegment component = nextSegment.NextSegment.NextSegment.NextTransform.GetComponent<RandomSegment>();
				component.Disable();
				if (nextSegment.PreviousSegment.PreviousTransform.GetComponent<RandomSegment>() != null)
				{
					nextSegment.PreviousSegment.PreviousTransform.GetComponent<RandomSegment>().Disable();
				}
				int difficulty = Mathf.Min(5, Mathf.FloorToInt(walker.TotalDistance / 1000f));
				if (difficulty > 0 && Random.Range(0, 100) > 80)
				{
					difficulty /= 2;
				}
				List<RandomPart> list = RandomParts.FindAll((RandomPart x) => !x.InUse && x.Difficulty == difficulty);
				if (difficulty > 0)
				{
					list.AddRange(RandomParts.FindAll((RandomPart x) => !x.InUse && x.Difficulty < difficulty).Take(Mathf.CeilToInt((float)list.Count * 0.7f)));
				}
				if (list.Count == 0)
				{
					int num = difficulty;
					while (list.Count == 0 && difficulty < 6)
					{
						difficulty++;
						list = RandomParts.FindAll((RandomPart x) => !x.InUse && x.Difficulty == difficulty);
					}
					if (list.Count == 0)
					{
						difficulty = num;
						while (list.Count == 0 && difficulty > -1)
						{
							difficulty--;
							list = RandomParts.FindAll((RandomPart x) => !x.InUse && x.Difficulty == difficulty);
						}
					}
				}
				component.Randomize(list);
			}
			nextSegment = nextSegment.NextSegment;
			checkSegments = new CurvySplineSegment[2] { nextSegment.PreviousSegment, nextSegment };
			CheckTimer = 0.5f;
		}
		if (SplineMeshHalfWidth != -1f)
		{
			if (Mathf.Abs(hoverController.transform.localPosition.x) > SplineMeshHalfWidth && !hoverController.OffLimits)
			{
				hoverController.SetOffLimits();
			}
			else if (hoverController.OffLimits && Mathf.Abs(hoverController.transform.localPosition.x) <= SplineMeshHalfWidth)
			{
				hoverController.SetOffLimits();
			}
		}
	}

	public override void Reset(bool isRewind)
	{
		CheckTimer = 0.01f;
		nextSegment = Spline.Segments[1];
		checkSegments = new CurvySplineSegment[2] { nextSegment.PreviousSegment, nextSegment };
		foreach (RandomPart randomPart in RandomParts)
		{
			randomPart.InUse = false;
			randomPart.gameObject.SetActive(false);
			randomPart.ResetRotators();
		}
		foreach (CurvySplineSegment segment in Spline.Segments)
		{
			if (segment.GetComponent<RandomSegment>() != null)
			{
				segment.GetComponent<RandomSegment>().Disable();
			}
		}
		foreach (CurvySplineSegment segment2 in Spline.Segments)
		{
			if (segment2.GetComponent<RandomSegment>() != null && segment2.SegmentIndex > 0 && segment2.SegmentIndex < 3)
			{
				segment2.GetComponent<RandomSegment>().Randomize(RandomParts.FindAll((RandomPart x) => !x.InUse && x.Difficulty <= 1));
			}
		}
	}
}
