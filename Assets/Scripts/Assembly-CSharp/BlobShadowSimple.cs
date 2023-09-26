using UnityEngine;

[ExecuteInEditMode]
public class BlobShadowSimple : ResetObject
{
	public bool DoNotAlign;

	public bool UseParentPosition;

	public float YOffSet = 0.1f;

	private Vector3 oldPosition;

	private LayerMask workMask;

	private Vector3 rayPosition;

	private Vector3 tempPosition;

	private void Start()
	{
		workMask = LayerMask.GetMask("Ground");
	}

	public void ShadowPosition()
	{
		if (UseParentPosition)
		{
			rayPosition = base.transform.parent.transform.position;
		}
		else
		{
			rayPosition = base.transform.position;
		}
		Debug.DrawRay(rayPosition + base.transform.parent.transform.up * 3f, base.transform.parent.transform.up * -1f, Color.red, 10f);
		RaycastHit hitInfo;
		if (Physics.Raycast(rayPosition + base.transform.parent.transform.up * 3f, base.transform.parent.transform.up * -1f, out hitInfo, 200f, workMask.value))
		{
			if (!DoNotAlign)
			{
				base.transform.up = hitInfo.normal;
			}
			tempPosition = hitInfo.point;
			tempPosition.y += YOffSet;
			base.transform.position = tempPosition;
			if (hitInfo.collider != null && hitInfo.collider.GetComponent<Renderer>() != null && !hitInfo.collider.GetComponent<Renderer>().enabled)
			{
				GetComponent<Renderer>().enabled = false;
			}
		}
	}

	public override void Reset(bool isRewind)
	{
	}
}
