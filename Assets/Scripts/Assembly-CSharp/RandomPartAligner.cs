using UnityEngine;

public class RandomPartAligner : MonoBehaviour
{
	public float OFFSET = 10.4f;

	public void Align(CurvySpline Spline)
	{
		float nearestPointTF = Spline.GetNearestPointTF(base.transform.position);
		base.transform.position = Spline.InterpolateFast(nearestPointTF);
		base.transform.rotation = Spline.GetOrientationFast(nearestPointTF);
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y - OFFSET, base.transform.localPosition.z);
	}
}
