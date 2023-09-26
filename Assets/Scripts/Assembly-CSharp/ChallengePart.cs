using System.Collections.Generic;
using UnityEngine;

public class ChallengePart : MonoBehaviour
{
	public int ID;

	public int PartLevel;

	public List<int> PartLevels;

	public List<RandomObject> InternalRandomObjects;

	public List<RandomPair> RandomPairObjects;

	private AnimationTrigger[] TriggersToActivate;

	private ObjectActivator[] ActivationObjects;

	private Collectable[] Collectables;

	private BreakWhenHit[] Breakables;

	private SplineWalker walker;

	private void Awake()
	{
		walker = Object.FindObjectOfType<SplineWalker>();
		Collectables = base.transform.GetComponentsInChildren<Collectable>();
		Breakables = base.transform.GetComponentsInChildren<BreakWhenHit>(true);
		ActivationObjects = base.transform.GetComponentsInChildren<ObjectActivator>();
		TriggersToActivate = base.transform.GetComponentsInChildren<AnimationTrigger>();
	}

	private void OnEnable()
	{
		Randomize();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			base.transform.GetComponentInParent<ChallengePath>().MoveToNext();
		}
	}

	public void Randomize()
	{
		if (Collectables != null)
		{
			Collectable[] collectables = Collectables;
			foreach (Collectable collectable in collectables)
			{
				collectable.Reset(true);
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
		if (TriggersToActivate != null)
		{
			AnimationTrigger[] triggersToActivate = TriggersToActivate;
			foreach (AnimationTrigger animationTrigger in triggersToActivate)
			{
				if (!(animationTrigger == null) && animationTrigger.gameObject.activeSelf)
				{
					animationTrigger.Reset(true);
					animationTrigger.ActivateTrigger();
					animationTrigger.PlayActiveAnimation();
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
		if (Breakables != null)
		{
			BreakWhenHit[] breakables = Breakables;
			foreach (BreakWhenHit breakWhenHit in breakables)
			{
				breakWhenHit.Reset();
			}
		}
	}
}
