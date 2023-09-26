using UnityEngine;

public class SplashRotator : MonoBehaviour
{
	public Vector3 rotationTarget;

	public float rotationTime;

	private Quaternion target;

	private Quaternion start;

	private float timeTotal;

	private void Start()
	{
		timeTotal = rotationTime;
		start = base.transform.localRotation;
		target = Quaternion.Euler(rotationTarget);
	}

	private void FixedUpdate()
	{
		if (rotationTime > 0f)
		{
			rotationTime -= Time.fixedDeltaTime;
			base.transform.localRotation = Quaternion.Lerp(start, target, timeTotal - rotationTime);
		}
	}
}
