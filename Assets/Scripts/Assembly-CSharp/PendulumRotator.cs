using System;
using UnityEngine;

public class PendulumRotator : MonoBehaviour
{
	public float Amplitude;

	public Vector3 RotateVector;

	public bool RandomStart;

	private float startTime;

	private void Start()
	{
		startTime = ((!RandomStart) ? 0f : UnityEngine.Random.Range(0f, 5f));
	}

	private void FixedUpdate()
	{
		base.transform.localRotation = Quaternion.Euler(RotateVector * 90f * Mathf.Sin(Amplitude * (startTime + Time.time) * 2f * (float)Math.PI));
	}
}
