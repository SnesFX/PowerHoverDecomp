using UnityEngine;

public class CameraFarDistanceTrigger : ResetObject
{
	public float targetValue;

	private float changed;

	private float timer;

	private void Update()
	{
		if (changed > 0f && timer < 1f)
		{
			timer += Time.deltaTime * 0.5f;
			Camera.main.farClipPlane = Mathf.Lerp(changed, targetValue, timer);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			changed = Camera.main.farClipPlane;
		}
	}

	public override void Reset(bool isRewind)
	{
		base.Reset(isRewind);
		changed = (timer = 0f);
	}
}
