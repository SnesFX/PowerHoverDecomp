using UnityEngine;

public class CallCameraShake : MonoBehaviour
{
	public float shakeScale = 1f;

	public float shakeTime = 1f;

	private HoverController player;

	private void Awake()
	{
		player = Object.FindObjectOfType<HoverController>();
	}

	private void OnEnable()
	{
		if (Camera.main != null && GameController.Instance != null && Camera.main.GetComponent<CameraShake>() != null && (GameController.Instance.State == GameController.GameState.Running || GameController.Instance.State == GameController.GameState.Kill))
		{
			float num = shakeScale * 100f / Vector3.Distance(player.transform.position, base.transform.position);
			if (num > 0.05f && num < 1.5f)
			{
				Camera.main.GetComponent<CameraShake>().StartShake(shakeTime, new Vector3(num, num, 0f));
			}
		}
	}
}
