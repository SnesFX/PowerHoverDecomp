using UnityEngine;

public class TargetObject : ResetObject
{
	private Rigidbody[] childComponents;

	private Vector3[] childPositions;

	private Quaternion[] childRotations;

	private HoverController hoverController;

	private Laser[] lazerObjects;

	private KillTrigger[] childKillers;

	private bool IsBreaking;

	private float positionOnSpine;

	public TargetIndicator indicator { get; private set; }

	private void Start()
	{
		indicator = GameController.Instance.TargetIndicator;
		hoverController = Object.FindObjectOfType<HoverController>();
		childComponents = base.gameObject.GetComponentsInChildren<Rigidbody>(true);
		childKillers = base.gameObject.GetComponentsInChildren<KillTrigger>();
		lazerObjects = base.gameObject.GetComponentsInChildren<Laser>();
		if (childComponents != null)
		{
			int num = childComponents.Length;
			childPositions = new Vector3[num];
			childRotations = new Quaternion[num];
			for (int i = 0; i < num; i++)
			{
				childComponents[i].isKinematic = true;
				childPositions[i] = childComponents[i].transform.localPosition;
				childRotations[i] = childComponents[i].transform.localRotation;
			}
		}
	}

	private void FixedUpdate()
	{
		if (hoverController.walker.Spline.IsInitialized && (positionOnSpine <= 0f || positionOnSpine >= 1f))
		{
			positionOnSpine = hoverController.walker.Spline.GetNearestPointTF(base.transform.position);
		}
		if (hoverController.walker.Spline.IsInitialized && GameController.Instance.State == GameController.GameState.Running && childComponents != null && !IsBreaking && hoverController.targetActiveObject == this && hoverController.walker.TF > positionOnSpine)
		{
			if (indicator.IsOnTarget)
			{
				BreakTarget();
				return;
			}
			hoverController.SetTargetObject(null, true);
			indicator.Reset(false);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!IsBreaking && other.CompareTag("Player") && GameController.Instance.State == GameController.GameState.Running && childComponents != null)
		{
			if (hoverController.targetActiveObject == this && indicator.IsOnTarget)
			{
				BreakTarget();
			}
			else
			{
				hoverController.checkpointController.PlayerDie();
			}
		}
	}

	public void BreakTarget()
	{
		IsBreaking = true;
		Vector3 vector = hoverController.walker.transform.forward.normalized * 20f;
		for (int i = 0; i < childComponents.Length; i++)
		{
			childComponents[i].isKinematic = false;
			vector.y = Random.Range(1f, 4f);
			vector.x += Random.Range(0f, 3f);
			vector.z += Random.Range(0f, 3f);
			childComponents[i].AddForce(vector * 2f, ForceMode.Impulse);
		}
		if (lazerObjects != null)
		{
			for (int j = 0; j < lazerObjects.Length; j++)
			{
				lazerObjects[j].gameObject.SetActive(false);
			}
		}
		hoverController.SetTargetObject(null, true);
		indicator.Reset(false);
		GetComponent<AudioSource>().Play();
	}

	public void SetKillers(bool enable)
	{
		if (childKillers != null)
		{
			for (int i = 0; i < childKillers.Length; i++)
			{
				childKillers[i].IsEnabled = enable;
			}
		}
	}

	public override void Reset(bool isRewind)
	{
		IsBreaking = false;
		if (childComponents != null)
		{
			for (int i = 0; i < childComponents.Length; i++)
			{
				childComponents[i].transform.localPosition = childPositions[i];
				childComponents[i].transform.localRotation = childRotations[i];
				childComponents[i].isKinematic = true;
			}
		}
		if (lazerObjects != null)
		{
			for (int j = 0; j < lazerObjects.Length; j++)
			{
				lazerObjects[j].gameObject.SetActive(true);
			}
		}
		SetKillers(true);
	}
}
