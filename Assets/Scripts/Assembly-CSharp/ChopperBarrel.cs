using UnityEngine;

public class ChopperBarrel : ResetObject
{
	public GameObject InnerBarrel;

	public GameObject ParticleObject;

	private FloatingObject floating;

	private LayerMask workMask;

	private Vector3 tempPosition;

	private float speed = 0.1f;

	private void Start()
	{
		workMask = LayerMask.GetMask("Ground");
		floating = GetComponent<FloatingObject>();
		floating.enabled = false;
		InnerBarrel.transform.Rotate(0f, Random.Range(0f, 90f), 0f, Space.World);
		FloatPosition();
	}

	private void FixedUpdate()
	{
		if (tempPosition != Vector3.zero)
		{
			speed += Time.fixedDeltaTime * 0.5f;
			FloatPosition();
			base.transform.position = Vector3.MoveTowards(base.transform.position, tempPosition, speed);
			if (base.transform.position.y - tempPosition.y < 0.2f)
			{
				tempPosition = Vector3.zero;
				floating.enabled = true;
				ParticleObject.SetActive(true);
			}
		}
	}

	private void FloatPosition()
	{
		Vector3 position = base.transform.position;
		Debug.DrawRay(position, Vector3.down, Color.red, 10f);
		RaycastHit hitInfo;
		if (Physics.Raycast(position, Vector3.down, out hitInfo, 999f, workMask.value))
		{
			tempPosition = hitInfo.point;
		}
	}

	public override void Reset(bool isRewind)
	{
		Object.Destroy(base.gameObject);
	}
}
