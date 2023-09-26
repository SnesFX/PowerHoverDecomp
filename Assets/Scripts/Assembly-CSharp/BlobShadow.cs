using UnityEngine;

public class BlobShadow : MonoBehaviour
{
	private const float GROUND_OFFSET = 0.25f;

	private const float CAMERA_DISTANCE_SCALER = 0.04f;

	private static Vector3 scaleMultiplier = Vector3.one;

	public GameObject shadowBlob;

	public bool enableShadow = true;

	private LayerMask workMask;

	private MeshRenderer shadowRenderer;

	private void Start()
	{
		workMask = LayerMask.GetMask("Ground", "Level", "Default");
		shadowBlob.transform.parent = GameController.Instance.transform;
		shadowRenderer = shadowBlob.GetComponentInChildren<MeshRenderer>();
		shadowRenderer.enabled = enableShadow;
	}

	private void Update()
	{
		if (!enableShadow)
		{
			return;
		}
		switch (GameController.Instance.State)
		{
		case GameController.GameState.Start:
		case GameController.GameState.Running:
			if (!shadowBlob.activeSelf)
			{
				shadowBlob.SetActive(true);
			}
			UpdateShadowPosition();
			break;
		case GameController.GameState.Kill:
		case GameController.GameState.End:
		case GameController.GameState.Ending:
			shadowBlob.SetActive(false);
			break;
		case GameController.GameState.Reverse:
		case GameController.GameState.Resume:
		case GameController.GameState.Paused:
			break;
		}
	}

	private void UpdateShadowPosition()
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(base.transform.position, -base.transform.up, out hitInfo, 9f, workMask.value))
		{
			Vector3 point = hitInfo.point;
			point += hitInfo.normal * 0.25f;
			shadowBlob.transform.position = point;
			shadowBlob.transform.up = hitInfo.normal;
			shadowRenderer.enabled = true;
		}
		else
		{
			shadowRenderer.enabled = false;
		}
		float b = 0.04f * Vector3.Dot(shadowBlob.transform.position - Camera.main.transform.position, Camera.main.transform.forward);
		shadowBlob.transform.localScale = scaleMultiplier * Mathf.Min(1.2f, Mathf.Max(0.5f, b));
	}
}
