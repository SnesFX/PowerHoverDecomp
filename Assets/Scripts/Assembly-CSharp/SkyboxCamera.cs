using UnityEngine;

public class SkyboxCamera : MonoBehaviour
{
	public float Speed = 50f;

	public float xOffset;

	private Camera[] moveCameras;

	private Vector3 target;

	private Vector3[] startRotations;

	private Vector3 camFollowRotationStart;

	private Transform camFollow;

	private Vector3 mainCameraStartRotation;

	private void Start()
	{
		moveCameras = base.transform.GetComponentsInChildren<Camera>();
		camFollow = Object.FindObjectOfType<CameraFollowAnimation>().transform;
		camFollowRotationStart = camFollow.transform.localRotation.eulerAngles;
		mainCameraStartRotation = Camera.main.transform.localRotation.eulerAngles;
		if (moveCameras != null)
		{
			startRotations = new Vector3[moveCameras.Length];
			for (int i = 0; i < moveCameras.Length; i++)
			{
				startRotations[i] = moveCameras[i].transform.localRotation.eulerAngles;
			}
		}
	}

	private void FixedUpdate()
	{
		if (moveCameras != null)
		{
			Vector3 vector = Camera.main.transform.localRotation.eulerAngles + (Camera.main.transform.localRotation.eulerAngles - mainCameraStartRotation);
			for (int i = 0; i < moveCameras.Length; i++)
			{
				target = startRotations[i] + (camFollow.localRotation.eulerAngles - camFollowRotationStart);
				target.z = startRotations[i].z;
				target.x = xOffset;
				target += vector;
				moveCameras[i].transform.localRotation = Quaternion.Euler(target);
			}
			mainCameraStartRotation = Camera.main.transform.localRotation.eulerAngles;
		}
	}
}
