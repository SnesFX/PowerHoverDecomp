using UnityEngine;

public class UVrotation : MonoBehaviour
{
	public float Yspeed;

	public float Xspeed;

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

	private void Update()
	{
		if (usedMaterial != null)
		{
			XYoffset.x += Time.deltaTime * Xspeed;
			XYoffset.y += Time.deltaTime * Yspeed;
			usedMaterial.mainTextureOffset = XYoffset;
			XYoffset.x = ((!(XYoffset.x > 1f)) ? XYoffset.x : 0f);
			XYoffset.y = ((!(XYoffset.y > 1f)) ? XYoffset.y : 0f);
		}
	}
}
