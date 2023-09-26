using UnityEngine;

public class TargetArea : MonoBehaviour
{
	private HoverController hoverController;

	private TargetObject parentTarget;

	private void Start()
	{
		hoverController = Object.FindObjectOfType<HoverController>();
		parentTarget = base.transform.parent.GetComponent<TargetObject>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && GameController.Instance.State == GameController.GameState.Running && hoverController.targetObject != parentTarget)
		{
			GameController.Instance.TargetIndicator.StartTargeting(base.transform.parent);
			hoverController.SetTargetObject(parentTarget);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player") && hoverController.targetObject == parentTarget && hoverController.targetActiveObject != parentTarget)
		{
			hoverController.SetTargetObject(null);
			GameController.Instance.TargetIndicator.Reset(false);
		}
	}
}
