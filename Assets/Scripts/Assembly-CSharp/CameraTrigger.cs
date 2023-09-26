using UnityEngine;

public class CameraTrigger : ResetObject
{
	public Color gizmoColor = new Color(0.7f, 0.6f, 0.5f, 0.75f);

	public float speed = 0.5f;

	public GameObject cameraNull;

	private GameObject defaultCameraNull;

	private CameraFollowAnimation mainCameraFollow;

	private void Start()
	{
		if (GetComponent<MeshRenderer>() != null)
		{
			GetComponent<MeshRenderer>().enabled = false;
		}
		mainCameraFollow = Camera.main.transform.parent.GetComponent<CameraFollowAnimation>();
		defaultCameraNull = cameraNull;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player") && cameraNull != null)
		{
			cameraNull = mainCameraFollow.UpdateCamera(speed, cameraNull);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			cameraNull = defaultCameraNull;
		}
	}

	public override void Reset(bool isRewind)
	{
		GetComponent<Collider>().enabled = true;
		cameraNull = defaultCameraNull;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = gizmoColor;
		if (GetComponent<BoxCollider>() != null)
		{
			BoxCollider component = GetComponent<BoxCollider>();
			if (component.enabled)
			{
				Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, base.transform.lossyScale);
				Gizmos.DrawCube(component.center, component.size);
			}
		}
	}
}
