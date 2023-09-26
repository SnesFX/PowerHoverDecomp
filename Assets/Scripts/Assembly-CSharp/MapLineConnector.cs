using UnityEngine;

public class MapLineConnector : MonoBehaviour
{
	public Transform start;

	public void SetLinePosition(Vector3 end)
	{
		Vector3 vector = end - start.position;
		Vector3 position = vector / 2f + start.position;
		base.transform.position = position;
		base.transform.rotation = Quaternion.FromToRotation(Vector3.up, vector);
		Vector3 localScale = base.transform.localScale;
		localScale.y = vector.magnitude;
		base.transform.localScale = localScale;
	}
}
