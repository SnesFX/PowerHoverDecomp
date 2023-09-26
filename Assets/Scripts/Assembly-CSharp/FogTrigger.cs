using UnityEngine;

public class FogTrigger : ResetObject
{
	public Color gizmoColor = new Color(0.3f, 0.2f, 0f, 0.75f);

	public Color FogColor;

	public Color CameraBGColor;

	public float FadeTime;

	public float FogEndDistance;

	private float FogEndDistanceFrom;

	private float fadeColorTimer;

	private Color fogColorFrom;

	private Color cameraColorFrom;

	private float FogEndDistanceFromReset;

	private float fadeColorTimerReset;

	private Color fogColorFromReset;

	private Color cameraColorFromReset;

	private BoxCollider coll;

	private bool fogChanged;

	private void Start()
	{
		SetStartFogValues();
		coll = GetComponent<BoxCollider>();
	}

	public void SetStartFogValues()
	{
		fogColorFromReset = RenderSettings.fogColor;
		cameraColorFromReset = Camera.main.backgroundColor;
		FogEndDistanceFromReset = RenderSettings.fogEndDistance;
	}

	private void FixedUpdate()
	{
		if (fadeColorTimer > 0f)
		{
			fadeColorTimer -= Time.fixedDeltaTime;
			float t = (FadeTime - fadeColorTimer) / FadeTime;
			RenderSettings.fogColor = Color.Lerp(fogColorFrom, FogColor, t);
			RenderSettings.fogEndDistance = Mathf.Lerp(FogEndDistanceFrom, FogEndDistance, t);
			Camera.main.backgroundColor = Color.Lerp(cameraColorFrom, CameraBGColor, t);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			coll.enabled = false;
			fogColorFrom = RenderSettings.fogColor;
			cameraColorFrom = Camera.main.backgroundColor;
			FogEndDistanceFrom = RenderSettings.fogEndDistance;
			fadeColorTimer = FadeTime;
			fogChanged = true;
		}
	}

	public override void Reset(bool isRewind)
	{
		if (fogChanged)
		{
			coll.enabled = true;
			fadeColorTimer = 0f;
			fogChanged = false;
			if (isRewind)
			{
				RenderSettings.fogColor = FogColor;
				RenderSettings.fogEndDistance = FogEndDistance;
				Camera.main.backgroundColor = CameraBGColor;
			}
		}
		else if (!isRewind)
		{
			RenderSettings.fogColor = fogColorFromReset;
			RenderSettings.fogEndDistance = FogEndDistanceFromReset;
			Camera.main.backgroundColor = cameraColorFromReset;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = gizmoColor;
		if (GetComponent<BoxCollider>() != null)
		{
			BoxCollider component = GetComponent<BoxCollider>();
			if (component.enabled)
			{
				Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, base.transform.lossyScale);
				Gizmos.DrawCube(component.center, component.size);
			}
		}
	}
}
