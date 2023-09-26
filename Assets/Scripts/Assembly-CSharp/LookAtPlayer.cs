using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
	public Transform target;

	public float damping = 6f;

	public Vector3 targetPos;

	private void Start()
	{
		if ((bool)target)
		{
			targetPos = target.position;
			targetPos.z *= 0.25f;
			base.transform.rotation = Quaternion.LookRotation(targetPos - base.transform.position);
		}
	}

	private void Update()
	{
		if ((bool)target)
		{
			targetPos = target.position;
			targetPos.z *= 0.25f;
			Quaternion b = Quaternion.LookRotation(targetPos - base.transform.position);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.deltaTime * damping);
		}
	}
}
