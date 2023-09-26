using System.Collections.Generic;
using UnityEngine;

public class CloneRandomPart : MonoBehaviour
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

	public ContinuosRotation[] RotatorObjects;

	public Vector3 rotateAround = Vector3.up;

	private SplineWalker walker;

	private void Start()
	{
		walker = Object.FindObjectOfType<SplineWalker>();
		ActivationObjects = base.transform.GetComponentsInChildren<ObjectActivator>();
	}

	private void OnDisable()
	{
		Collectable[] collectables = Collectables;
		foreach (Collectable collectable in collectables)
		{
			collectable.Reset(false);
		}
	}

	private void OnEnable()
	{
		Randomize();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			base.transform.GetComponentInParent<RandomClonePath>().MoveToNext();
		}
	}

	public void Randomize()
	{
		if (RotatorObjects != null)
		{
			for (int i = 0; i < RotatorObjects.Length; i++)
			{
				if (RotatorObjects[i] == null)
				{
					continue;
				}
				if (Random.value < 0.05f)
				{
					RotatorObjects[i].enabled = false;
					continue;
				}
				RotatorObjects[i].enabled = true;
				float num = 0f;
				if (walker != null)
				{
					num = Mathf.Min(2.5f, walker.TotalDistance / 3000f);
					if (num > 0f && (float)Random.Range(0, 100) > 80f - walker.TotalDistance / 200f)
					{
						num *= 1.5f;
					}
				}
				RotatorObjects[i].rotationVector = rotateAround * Random.Range(1f + num, 1.5f + num) * (((double)Random.value < 0.5) ? 1 : (-1));
			}
		}
		if (InternalRandomObjects != null)
		{
			foreach (RandomObject internalRandomObject in InternalRandomObjects)
			{
				if (internalRandomObject != null)
				{
					internalRandomObject.internalObject.SetActive(Random.Range(0f, 1f) <= internalRandomObject.probability);
				}
			}
		}
		if (RandomPairObjects != null)
		{
			foreach (RandomPair randomPairObject in RandomPairObjects)
			{
				if (randomPairObject != null)
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
		}
		if (ActivationObjects != null)
		{
			ObjectActivator[] activationObjects = ActivationObjects;
			foreach (ObjectActivator objectActivator in activationObjects)
			{
				if (!(objectActivator == null))
				{
					objectActivator.Reset(false);
				}
			}
		}
		if (Collectables == null)
		{
			return;
		}
		Collectable[] collectables = Collectables;
		foreach (Collectable collectable in collectables)
		{
			if (!(collectable == null))
			{
				collectable.Reset(false);
				collectable.gameObject.SetActive(Random.Range(0f, 1f) <= Treasure);
			}
		}
	}
}
