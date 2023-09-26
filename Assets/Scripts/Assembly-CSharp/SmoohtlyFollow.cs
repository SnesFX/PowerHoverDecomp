using UnityEngine;

public class SmoohtlyFollow : MonoBehaviour
{
	public Transform target;

	public float smoothTime = 0.3f;

	private Vector3 velocity = Vector3.zero;

	private void FixedUpdate()
	{
		base.transform.position = Vector3.SmoothDamp(base.transform.position, target.position, ref velocity, smoothTime);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, target.rotation, Time.fixedDeltaTime * smoothTime);
	}
}
