using UnityEngine;

public class ColorAnimate : MonoBehaviour
{
	private const string TINT = "_TintColor";

	public Color _Color;

	private MeshRenderer rnder;

	private TextMesh txtMesh;

	private bool tintColorEnabl;

	private Color lastColor;

	private void Start()
	{
		rnder = GetComponent<MeshRenderer>();
		txtMesh = GetComponent<TextMesh>();
		tintColorEnabl = rnder.material.HasProperty("_TintColor");
	}

	private void FixedUpdate()
	{
		if (!(lastColor == _Color))
		{
			lastColor = _Color;
			if (txtMesh != null)
			{
				txtMesh.color = _Color;
			}
			else if (tintColorEnabl)
			{
				rnder.material.SetColor("_TintColor", _Color);
			}
			else
			{
				rnder.material.color = _Color;
			}
		}
	}
}
