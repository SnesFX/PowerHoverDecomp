using UnityEngine;

public class LandingEffect : MonoBehaviour
{
	private const string TINT = "_TintColor";

	public float Yspeed;

	public Material usedMaterial;

	private Vector2 XYoffset = new Vector2(0f, 0f);

	private Color matColor;

	private float matAlpha = 0.2f;

	private void Start()
	{
		matColor = usedMaterial.GetColor("_TintColor");
		GetComponent<Renderer>().material = (usedMaterial = Object.Instantiate(usedMaterial));
	}

	private void Update()
	{
		if (XYoffset.y < 1f)
		{
			XYoffset.y += Time.deltaTime * Yspeed;
			matColor.a = matAlpha - XYoffset.y * 0.2f;
			usedMaterial.SetColor("_TintColor", matColor);
			usedMaterial.mainTextureOffset = XYoffset;
		}
		else
		{
			StartAgain();
		}
	}

	private void StartAgain()
	{
		XYoffset.y = 0f;
		matColor.a = 0.2f;
		base.gameObject.SetActive(false);
	}
}
