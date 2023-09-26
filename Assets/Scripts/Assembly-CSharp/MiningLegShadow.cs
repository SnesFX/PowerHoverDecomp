using UnityEngine;

public class MiningLegShadow : MonoBehaviour
{
	private const float GROUND_OFFSET = 0.3f;

	public GameObject shadowBlob;

	public bool enableShadow = true;

	public Transform groundedPosition;

	public AnimationTrigger animationTrigger;

	private LayerMask workMask;

	private MeshRenderer shadowRenderer;

	private Vector3 startScale;

	private void Start()
	{
		workMask = LayerMask.GetMask("Ground", "Level");
		shadowBlob.transform.parent = GameController.Instance.transform;
		startScale = shadowBlob.transform.localScale;
		shadowRenderer = shadowBlob.GetComponentInChildren<MeshRenderer>();
		shadowRenderer.enabled = enableShadow;
		UpdateShadowPosition();
	}

	private void Update()
	{
		if (enableShadow)
		{
			GameController.GameState state = GameController.Instance.State;
			if ((state == GameController.GameState.Kill || state == GameController.GameState.Start || state == GameController.GameState.Running) && (animationTrigger == null || (animationTrigger != null && animationTrigger.activated)))
			{
				UpdateShadowPosition();
			}
		}
	}

	private void UpdateShadowPosition()
	{
		if (SetShadowFrom(base.transform.position))
		{
			return;
		}
		if (groundedPosition != null)
		{
			if (!SetShadowFrom(groundedPosition.position))
			{
				shadowRenderer.enabled = false;
			}
		}
		else
		{
			shadowRenderer.enabled = false;
		}
	}

	private bool SetShadowFrom(Vector3 rayPos)
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(rayPos, Vector3.down, out hitInfo, 99f, workMask.value))
		{
			Vector3 point = hitInfo.point;
			point += hitInfo.normal * 0.3f;
			shadowBlob.transform.position = point;
			shadowBlob.transform.up = hitInfo.normal;
			shadowRenderer.enabled = true;
			float a = Vector3.Distance(shadowBlob.transform.position - base.transform.position, Vector3.up);
			float num = Mathf.Max(1.4f, 50f / Mathf.Max(a, 12f) * 0.7f);
			shadowBlob.transform.localScale = startScale - num * startScale;
			return true;
		}
		return false;
	}
}
