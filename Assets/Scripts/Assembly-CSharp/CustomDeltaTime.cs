using UnityEngine;

public class CustomDeltaTime : MonoBehaviour
{
	public static float delta;

	private float lastInterval;

	private void Start()
	{
		lastInterval = Time.realtimeSinceStartup;
	}

	private void Update()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		delta = realtimeSinceStartup - lastInterval;
		lastInterval = realtimeSinceStartup;
	}
}
