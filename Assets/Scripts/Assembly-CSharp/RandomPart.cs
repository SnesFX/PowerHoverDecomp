using System.Collections.Generic;
using UnityEngine;

public class RandomPart : MonoBehaviour
{
	[Range(0f, 5f)]
	public int Difficulty;

	[Range(0f, 1f)]
	public float Treasure;

	public List<RandomObject> InternalRandomObjects;

	public List<RandomPair> RandomPairObjects;

	public bool InUse;

	public Collectable[] Collectables;

	private ObjectActivator[] ActivationObjects;

	public RandomPartAligner[] AlignObjects;

	public ContinuosRotation[] RotatorObjects;

	public Vector3[] DefaultRotatorSpeeds;

	private AnimationTrigger[] triggers;

	private CurvySpline Spline;

	private void Start()
	{
		ActivationObjects = base.transform.GetComponentsInChildren<ObjectActivator>();
		AlignObjects = base.transform.GetComponentsInChildren<RandomPartAligner>();
		RotatorObjects = base.transform.GetComponentsInChildren<ContinuosRotation>();
		triggers = base.transform.GetComponentsInChildren<AnimationTrigger>();
		if (RotatorObjects != null)
		{
			DefaultRotatorSpeeds = new Vector3[RotatorObjects.Length];
			for (int i = 0; i < RotatorObjects.Length; i++)
			{
				DefaultRotatorSpeeds[i] = RotatorObjects[i].rotationVector;
			}
		}
	}

	private void OnDisable()
	{
		Collectable[] collectables = Collectables;
		foreach (Collectable collectable in collectables)
		{
			collectable.Reset(false);
		}
		if (RotatorObjects != null)
		{
			for (int j = 0; j < RotatorObjects.Length; j++)
			{
				RotatorObjects[j].rotationVector *= 1.05f;
			}
		}
		if (triggers != null)
		{
			for (int k = 0; k < triggers.Length; k++)
			{
				triggers[k].Reset(false);
			}
		}
	}

	private void OnEnable()
	{
		if (AlignObjects != null)
		{
			for (int i = 0; i < AlignObjects.Length; i++)
			{
				AlignObjects[i].Align(Spline);
			}
		}
		if (InternalRandomObjects != null)
		{
			foreach (RandomObject internalRandomObject in InternalRandomObjects)
			{
				internalRandomObject.internalObject.SetActive(Random.Range(0f, 1f) <= internalRandomObject.probability);
			}
		}
		if (RandomPairObjects != null)
		{
			foreach (RandomPair randomPairObject in RandomPairObjects)
			{
				if (Random.Range(0, 2) == 1)
				{
					randomPairObject.object1.SetActive(true);
					randomPairObject.object2.SetActive(false);
				}
				else
				{
					randomPairObject.object1.SetActive(false);
					randomPairObject.object2.SetActive(true);
				}
			}
		}
		if (ActivationObjects != null)
		{
			ObjectActivator[] activationObjects = ActivationObjects;
			foreach (ObjectActivator objectActivator in activationObjects)
			{
				objectActivator.Reset(false);
			}
		}
		if (Collectables != null)
		{
			Collectable[] collectables = Collectables;
			foreach (Collectable collectable in collectables)
			{
				collectable.Reset(false);
				collectable.gameObject.SetActive(Random.Range(0f, 1f) <= Treasure);
			}
		}
	}

	public void SetSpline(CurvySpline spline)
	{
		Spline = spline;
	}

	public void ResetRotators()
	{
		if (RotatorObjects != null)
		{
			for (int i = 0; i < RotatorObjects.Length; i++)
			{
				RotatorObjects[i].rotationVector = DefaultRotatorSpeeds[i];
			}
		}
	}
}
