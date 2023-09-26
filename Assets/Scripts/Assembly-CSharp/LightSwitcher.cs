using System;
using UnityEngine;

public class LightSwitcher : ResetObject
{
	[Serializable]
	public class ColorPair
	{
		public Color targetLightColor = Color.red;

		public Color targetBGColor;
	}

	public Light targetLight;

	public Camera targetCamera;

	public ColorPair[] RandomColors;

	private Color oldLightColor;

	private Color oldCameraBGColor;

	private float colorTimer;

	private Collider coll;

	private int randomColorID;

	private void Start()
	{
		oldLightColor = targetLight.color;
		oldCameraBGColor = targetCamera.backgroundColor;
		coll = GetComponent<BoxCollider>();
	}

	private void FixedUpdate()
	{
		if (colorTimer > 0f)
		{
			colorTimer -= Time.fixedDeltaTime;
			targetLight.color = Color.Lerp(targetLight.color, RandomColors[randomColorID].targetLightColor, Time.fixedDeltaTime);
			targetCamera.backgroundColor = Color.Lerp(targetCamera.backgroundColor, RandomColors[randomColorID].targetBGColor, Time.fixedDeltaTime);
			RenderSettings.fogColor = targetCamera.backgroundColor;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			randomColorID = UnityEngine.Random.Range(0, RandomColors.Length);
			colorTimer = 1f;
			coll.enabled = false;
		}
	}

	public override void Reset(bool isRewind)
	{
		base.Reset(isRewind);
		if (!coll.enabled)
		{
			coll.enabled = true;
			targetLight.color = oldLightColor;
			targetCamera.backgroundColor = oldCameraBGColor;
		}
	}
}
