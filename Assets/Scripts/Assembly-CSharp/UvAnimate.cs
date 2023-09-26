using UnityEngine;

public class UvAnimate : MonoBehaviour
{
	public float OffSetY;

	public float OffSetX;

	private Material usedMaterial;

	private Vector2 XYoffset = new Vector2(0f, 0f);

	private Vector2 XYoffsetOG = new Vector2(0f, 0f);

	private void Start()
	{
		usedMaterial = base.transform.gameObject.GetComponent<Renderer>().material;
		XYoffsetOG.x = usedMaterial.mainTextureOffset.x;
		XYoffsetOG.y = usedMaterial.mainTextureOffset.y;
		XYoffset = XYoffsetOG;
	}

	private void OnDisable()
	{
		XYoffset = XYoffsetOG;
	}

	private void FixedUpdate()
	{
		if (usedMaterial != null)
		{
			XYoffset.x = OffSetX;
			XYoffset.y = OffSetY;
			usedMaterial.mainTextureOffset = XYoffset;
		}
	}
}
