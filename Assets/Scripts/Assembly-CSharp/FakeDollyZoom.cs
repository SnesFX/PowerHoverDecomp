using System;
using Holoville.HOTween;
using UnityEngine;

public class FakeDollyZoom : MonoBehaviour
{
	public float EffectTime = 4f;

	private bool dzEnabled;

	private float timer;

	private Camera cameraZoom;

	private float startFOV;

	private float fovChange;

	public void StartZoom(float effectTime = 2f, float zoomDistance = 30f, float fovDiff = 10f)
	{
		EffectTime = effectTime;
		HoverController hoverController = UnityEngine.Object.FindObjectOfType<HoverController>();
		cameraZoom = GetComponent<Camera>();
		startFOV = cameraZoom.fieldOfView;
		fovChange = fovDiff;
		Vector3 vector = base.transform.position + (base.transform.position - hoverController.transform.position).normalized * zoomDistance;
		HOTween.From(base.transform, EffectTime - 0.45f, new TweenParms().Prop("position", vector).UpdateType(UpdateType.TimeScaleIndependentUpdate).Ease(EaseType.EaseOutQuart));
		timer = EffectTime;
	}

	private void Update()
	{
		if (timer > 0f)
		{
			timer -= Time.deltaTime;
			cameraZoom.fieldOfView = Mathf.Lerp(startFOV - fovChange, startFOV, EffectTime - Mathf.Sin(timer * (float)Math.PI * 0.5f));
		}
	}
}
