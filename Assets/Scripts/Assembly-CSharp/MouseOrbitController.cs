using UnityEngine;

public class MouseOrbitController : MonoBehaviour
{
	public Camera cam;

	public float minX = -360f;

	public float maxX = 360f;

	public float sensX = 50f;

	public float minFov;

	public float maxFov;

	public float zoomSpeed = 12f;

	private float rotationY;

	private float rotationX;

	private AudioSource audi;

	private float deltaX;

	private void Start()
	{
		audi = GetComponent<AudioSource>();
	}

	private void Update()
	{
		float axis = Input.GetAxis("Mouse ScrollWheel");
		float num = cam.fieldOfView + axis * (0f - zoomSpeed);
		if (num > minFov && num < maxFov)
		{
			cam.fieldOfView = num;
			if (audi != null)
			{
				audi.volume = (minFov + maxFov - cam.fieldOfView) / 100f;
			}
		}
		if (Input.GetMouseButton(0))
		{
			float axis2 = Input.GetAxis("Mouse X");
			rotationX = axis2 - deltaX * sensX * Time.deltaTime;
			deltaX = axis2;
			float num2 = base.transform.localEulerAngles.y + rotationX;
			if (!(num2 > minX) || !(num2 < maxX))
			{
				rotationX = 0f;
				num2 = base.transform.localEulerAngles.y;
			}
			base.transform.localRotation = Quaternion.Euler(base.transform.localEulerAngles.x, num2, base.transform.localEulerAngles.z);
		}
	}
}
