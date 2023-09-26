using UnityEngine;

[ExecuteInEditMode]
public class BlobShadowOffset : MonoBehaviour
{
	public float XOffsetAmount;

	public float YOffsetAmount;

	private float XOld;

	private float YOld;

	private void Start()
	{
		XOld = XOffsetAmount;
		YOld = YOffsetAmount;
	}

	private void Update()
	{
		if (Application.isPlaying || (XOld == XOffsetAmount && YOld == YOffsetAmount))
		{
			return;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("Shadow");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			BlobShadowExtrawaganza component = gameObject.transform.parent.GetComponent<BlobShadowExtrawaganza>();
			if (component.positionAdjustable)
			{
				component.xOffset = XOffsetAmount;
				component.yOffset = YOffsetAmount;
				component.ShadowPosition();
			}
			XOld = XOffsetAmount;
			YOld = YOffsetAmount;
		}
	}
}
