using UnityEngine;

public class ObjectDisabler : MonoBehaviour
{
	public float DisableTime = 5f;

	private float disableTimer;

	private void OnEnable()
	{
		disableTimer = DisableTime;
	}

	private void Update()
	{
		if (disableTimer > 0f)
		{
			disableTimer -= Time.deltaTime;
			if (disableTimer <= 0f)
			{
				base.gameObject.SetActive(false);
			}
		}
	}
}
