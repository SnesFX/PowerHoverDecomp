using UnityEngine;

public class ScreenSpaceColorFader : MonoBehaviour
{
	public ColorAnimate colorAnimate;

	public Camera cam;

	private Color targetColor;

	private void Start()
	{
		targetColor = colorAnimate._Color;
	}

	private void Update()
	{
		if (cam.enabled)
		{
			Vector3 vector = cam.WorldToViewportPoint(base.transform.position);
			targetColor.a = 0.25f + ((!(vector.x > 0.5f)) ? (vector.x * 2.4f) : (1.2f - (vector.x - 0.5f) * 2.4f));
			colorAnimate._Color = Color.Lerp(colorAnimate._Color, targetColor, Time.deltaTime * 2f);
		}
	}
}
