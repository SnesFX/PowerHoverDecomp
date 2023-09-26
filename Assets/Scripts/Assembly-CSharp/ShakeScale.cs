using UnityEngine;

public class ShakeScale : MonoBehaviour
{
	public float shakeX;

	public float shakeY;

	public float shakeZ;

	public float randomRange;

	private Vector3 scaleShake;

	private Vector3 realScale;

	public bool uniformScale;

	public float amplitude;

	private float amplitudeUsed;

	private void Start()
	{
		amplitudeUsed = amplitude;
		realScale = (scaleShake = base.transform.localScale);
	}

	private void Update()
	{
		amplitudeUsed -= Time.fixedDeltaTime;
		if (amplitudeUsed < 0f)
		{
			float num = Random.Range(0f, randomRange);
			if (uniformScale)
			{
				scaleShake.x += shakeX + num;
				scaleShake.y += shakeX + num;
				scaleShake.z += shakeX + num;
				base.transform.localScale = scaleShake;
			}
			else
			{
				shakeX += shakeX + num;
				shakeY += shakeY + num;
				shakeZ += shakeZ + num;
				scaleShake.x += shakeX;
				scaleShake.y += shakeY;
				scaleShake.z += shakeZ;
				base.transform.localScale = scaleShake;
			}
			randomRange *= -1f;
			shakeX *= -1f;
			shakeY *= -1f;
			shakeZ *= -1f;
			amplitudeUsed = amplitude;
			scaleShake = realScale;
		}
	}
}
