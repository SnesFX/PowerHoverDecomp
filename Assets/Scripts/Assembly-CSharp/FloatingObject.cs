using UnityEngine;

public class FloatingObject : ResetObject
{
	public float YOffSet = 0.1f;

	public Transform surfaceObject;

	private Vector3 oldPosition;

	private LayerMask workMask;

	private Vector3 rayPosition;

	private Vector3 tempPosition;

	private float timer;

	private void Start()
	{
		oldPosition = base.transform.position;
		workMask = LayerMask.GetMask("Ground");
		FloatPosition();
		timer = Random.Range(0.1f, 0.5f);
	}

	private void FixedUpdate()
	{
		if (timer > 0f)
		{
			timer -= Time.fixedDeltaTime;
			if (timer <= 0f)
			{
				FloatPosition();
				timer = 0.25f;
			}
		}
		base.transform.position = Vector3.Lerp(base.transform.position, oldPosition, Time.fixedDeltaTime * 2f);
	}

	private void FloatPosition()
	{
		rayPosition = base.transform.position;
		rayPosition.y = surfaceObject.transform.position.y;
		RaycastHit hitInfo;
		if (Physics.Raycast(rayPosition + base.transform.transform.up * 3f, base.transform.transform.up * -1f, out hitInfo, 20f, workMask.value))
		{
			tempPosition = hitInfo.point;
			tempPosition.y += YOffSet;
			oldPosition = tempPosition;
		}
	}

	public override void Reset(bool isRewind)
	{
		timer = 0.25f;
		FloatPosition();
	}
}
