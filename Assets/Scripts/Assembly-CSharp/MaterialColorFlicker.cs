using UnityEngine;

public class MaterialColorFlicker : MonoBehaviour
{
	public float amountFlicker = 0.2f;

	public float speedFlicker = 0.1f;

	private const string TINT = "_TintColor";

	private Material matUsed;

	private Color colorUsed;

	private float orgAlpha;

	private float alphaValue;

	private void Start()
	{
		matUsed = GetComponent<Renderer>().material;
		colorUsed = matUsed.GetColor("_TintColor");
		orgAlpha = colorUsed.a;
	}

	private void FixedUpdate()
	{
		if (!(DeviceSettings.Instance == null) && DeviceSettings.Instance.EnableMaterialColorFlicker)
		{
			alphaValue = Mathf.PingPong(Time.time * speedFlicker, amountFlicker);
			colorUsed.a = orgAlpha + alphaValue;
			matUsed.SetColor("_TintColor", colorUsed);
		}
	}
}
