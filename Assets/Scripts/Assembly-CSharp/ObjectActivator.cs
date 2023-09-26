using System.Collections.Generic;
using UnityEngine;

public class ObjectActivator : ResetObject
{
	public enum ActivationType
	{
		Spline = 0,
		Trigger = 1,
		MultiTrigger = 2
	}

	[SerializeField]
	[HideInInspector]
	public ActivationType Type;

	[SerializeField]
	[HideInInspector]
	public float ActivateTF;

	[SerializeField]
	[HideInInspector]
	public float DisableTF;

	[SerializeField]
	[HideInInspector]
	public ObjectTrigger AcivatorTrigger;

	[SerializeField]
	[HideInInspector]
	public ObjectTrigger DisableTrigger;

	[SerializeField]
	[HideInInspector]
	public List<ObjectTrigger> AcivatorTriggers;

	[SerializeField]
	[HideInInspector]
	public List<ObjectTrigger> DisableTriggers;

	public GameObject childObject;

	public bool DisableOnRewind = true;

	public bool ResetPositionOnReset = true;

	private SplineWalker walker;

	private Vector3 startPosition;

	private Quaternion startRotation;

	public bool IsActive { get; set; }

	public virtual void Start()
	{
		walker = Object.FindObjectOfType<SplineWalker>();
		startPosition = base.transform.position;
		startRotation = base.transform.rotation;
		if (childObject != null)
		{
			childObject.SetActive(false);
		}
	}

	public virtual void FixedUpdate()
	{
		if (Type == ActivationType.Spline)
		{
			if (!IsActive && walker.TF >= ActivateTF && walker.TF < DisableTF)
			{
				Activate();
			}
			else if (IsActive && (walker.TF < ActivateTF || walker.TF > DisableTF))
			{
				Disable();
			}
		}
	}

	public void Activate()
	{
		IsActive = true;
		if (childObject != null)
		{
			childObject.SetActive(true);
		}
	}

	public void Disable()
	{
		IsActive = false;
		if (childObject != null)
		{
			childObject.SetActive(false);
		}
	}

	public override void Reset(bool isRewind)
	{
		if (!isRewind || (isRewind && DisableOnRewind))
		{
			Disable();
		}
		if (ResetPositionOnReset)
		{
			base.transform.position = startPosition;
			base.transform.rotation = startRotation;
		}
		base.Reset(isRewind);
	}
}
