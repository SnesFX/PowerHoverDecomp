using UnityEngine;

public class CameraShake : MonoBehaviour
{
	public Vector3 ShakeVector;

	private float ShakeTime;

	private Vector3 originalPos;

	private Vector3 shakeTemp;

	private float shakeTimeMax;

	private void FixedUpdate()
	{
		if (!(ShakeTime > 0f))
		{
			return;
		}
		if (GameController.Instance.State == GameController.GameState.Reverse || GameController.Instance.State == GameController.GameState.Resume)
		{
			ShakeTime = 0f;
			return;
		}
		ShakeTime -= Time.fixedDeltaTime;
		shakeTemp = Random.insideUnitSphere;
		shakeTemp.Scale(ShakeVector);
		shakeTemp = Vector3.Lerp(shakeTemp, Vector3.zero, (shakeTimeMax - ShakeTime) / shakeTimeMax);
		base.transform.localPosition = originalPos + shakeTemp;
		if (ShakeTime < 0f)
		{
			base.transform.localPosition = originalPos;
		}
	}

	public void StartShake(float time = 1f)
	{
		if (!(ShakeTime > 0f))
		{
			originalPos = base.transform.localPosition;
			ShakeTime = (shakeTimeMax = time);
		}
	}

	public void StartShake(float time, Vector3 vector)
	{
		if (!(ShakeTime > 0f))
		{
			ShakeVector = vector;
			originalPos = base.transform.localPosition;
			ShakeTime = (shakeTimeMax = time);
		}
	}
}
