using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
	public float offset = 2f;

	public float distance = 4f;

	private float startZ;

	private Vector3 pos;

	private void Start()
	{
		startZ = base.transform.position.z;
		pos = base.transform.position;
	}

	private void Update()
	{
		pos.z = startZ + Mathf.SmoothStep(0f, distance, Mathf.PingPong(Time.time, 1f));
		base.transform.position = pos;
	}
}
