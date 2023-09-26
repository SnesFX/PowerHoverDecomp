using UnityEngine;

public class ObjectBlinker : MonoBehaviour
{
	public Renderer BlinkRenderer;

	public bool MakeItRandom;

	public float BlinkInterval = 20f;

	private float blinkStart;

	private void Start()
	{
		blinkStart = BlinkInterval;
	}

	private void FixedUpdate()
	{
		BlinkRenderer.enabled = Mathf.Sin(Time.time * BlinkInterval) >= 0f;
		if (MakeItRandom && !BlinkRenderer.enabled)
		{
			BlinkInterval = blinkStart + Random.value * BlinkInterval;
		}
	}
}
