using UnityEngine;

public class CutsceneMachineRotator : MonoBehaviour
{
	public CutSceneController cutController;

	public Camera musicCam;

	public float damping = 0.9f;

	public float speed = 0.1f;

	private bool dragging;

	private Vector3 vDown;

	private Vector3 vDrag;

	private Vector3 rotationAxis;

	private float angularVelocity;

	private void Start()
	{
	}

	private void Update()
	{
		UpdateRotator();
	}

	private void UpdateRotator()
	{
		if (damping < 1f && Input.GetMouseButton(0))
		{
			Ray ray = musicCam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo))
			{
				if (!dragging)
				{
					if (hitInfo.transform == base.transform)
					{
						vDown = hitInfo.point - base.transform.position;
						dragging = true;
					}
				}
				else
				{
					vDrag = hitInfo.point - base.transform.position;
					rotationAxis = Vector3.Cross(vDown, vDrag);
					rotationAxis.x = (rotationAxis.z = 0f);
					angularVelocity = Vector3.Angle(vDown, vDrag) * speed;
					if (damping > 0.965f)
					{
						damping = 1f;
						cutController.MoveToNextScene();
					}
					else
					{
						damping = Mathf.Lerp(damping, 1f, Time.deltaTime * 3f);
					}
				}
			}
			else
			{
				dragging = false;
			}
		}
		if (Input.GetMouseButtonUp(0))
		{
			dragging = false;
		}
		if (angularVelocity > 0f)
		{
			base.transform.Rotate(rotationAxis, ((damping != 1f) ? 1f : 2f) * angularVelocity * Time.deltaTime);
			angularVelocity = ((!(angularVelocity > 0.01f)) ? 0f : (angularVelocity * damping));
		}
	}
}
