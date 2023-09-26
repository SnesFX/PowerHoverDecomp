using UnityEngine;

public class CopyTransform : MonoBehaviour
{
	public Transform copyFrom;

	private void FixedUpdate()
	{
		base.transform.position = copyFrom.position;
	}
}
