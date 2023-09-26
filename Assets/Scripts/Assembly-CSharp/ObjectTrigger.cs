using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectTrigger : GroundGizmos
{
	public Color gizmoColor = new Color(0f, 0.8f, 0f, 0.75f);

	private Rigidbody[] childComponents;

	private List<ObjectActivator> activators;

	private List<ObjectActivator> disablers;

	public override Color GizmoColor
	{
		get
		{
			return gizmoColor;
		}
	}

	private void Start()
	{
		if (base.transform.gameObject.GetComponent<MeshRenderer>() != null)
		{
			base.transform.gameObject.GetComponent<MeshRenderer>().enabled = false;
		}
		activators = new List<ObjectActivator>();
		disablers = new List<ObjectActivator>();
		ObjectActivator[] array = UnityEngine.Object.FindObjectsOfType<ObjectActivator>();
		foreach (ObjectActivator objectActivator in array)
		{
			if (objectActivator.Type == ObjectActivator.ActivationType.MultiTrigger)
			{
				if (objectActivator.AcivatorTriggers != null && objectActivator.AcivatorTriggers.Contains(this))
				{
					activators.Add(objectActivator);
				}
				else if (objectActivator.DisableTriggers != null && objectActivator.DisableTriggers.Contains(this))
				{
					disablers.Add(objectActivator);
				}
			}
			else if (objectActivator.Type == ObjectActivator.ActivationType.Trigger)
			{
				if (objectActivator.AcivatorTrigger == this)
				{
					activators.Add(objectActivator);
				}
				else if (objectActivator.DisableTrigger == this)
				{
					disablers.Add(objectActivator);
				}
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			for (int i = 0; i < activators.Count; i++)
			{
				activators[i].Activate();
			}
			for (int j = 0; j < disablers.Count; j++)
			{
				disablers[j].Disable();
			}
			if (LevelStats.Instance != null)
			{
				LevelStats.Instance.LightsHittedTomb = true;
			}
		}
	}
}
