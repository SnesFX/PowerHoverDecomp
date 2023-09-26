using System.Collections;
using UnityEngine;

public class CameraPan : MonoBehaviour
{
	private const float DragSpeedLimit = 45f;

	public float MinX = -8.6f;

	public float MaxX = 31f;

	public float DragSpeed = 1.3f;

	public CutSceneController cutController;

	private Camera cam;

	private Vector3 lastPosition;

	private float Speed;

	private float SpeedFrom;

	private float lerpTimer;

	private bool autoControl;

	private bool exitting;

	public AudioSource windmillSound;

	public AudioSource structureMechanicalSound;

	private float structureSoundVolume;

	private float windmillVolume;

	private void Start()
	{
		cam = GetComponent<Camera>();
		exitting = (autoControl = false);
	}

	private void Update()
	{
		if (exitting || !cam.enabled)
		{
			return;
		}
		if (autoControl && InLimits())
		{
			Speed = Mathf.SmoothStep(Speed, 3f, Time.deltaTime * 0.5f);
			cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 34f, Time.deltaTime * 1.2f);
			Vector3 position = base.transform.position;
			position.x += Speed * 0.001f;
			base.transform.position = Vector3.Lerp(base.transform.position, position, Time.deltaTime * 10f);
			if (!InLimits())
			{
				StartCoroutine(FadeEndSound());
				cutController.MoveToNextScene();
				exitting = true;
			}
		}
		if (!autoControl)
		{
			float x = base.transform.localPosition.x;
			if (x > -512f && x < -504f)
			{
				windmillSound.volume = 0.125f * (4f - Mathf.Abs(x + 508f));
			}
			else
			{
				windmillSound.volume = 0f;
			}
			float axisRaw = Input.GetAxisRaw("Horizontal");
			if (axisRaw != 0f)
			{
				Speed = 10f * DragSpeed * Mathf.Min(25f * Mathf.Abs(axisRaw), 45f) * (float)((!(axisRaw < 0f)) ? 1 : (-1));
				SpeedFrom = Speed;
				lerpTimer = 1f;
			}
			else
			{
				UpdateDrag(Time.deltaTime);
			}
			UpdateMovement(Time.deltaTime);
		}
	}

	private IEnumerator FadeEndSound()
	{
		float fTimeCounter = 0f;
		while (fTimeCounter < 1f)
		{
			fTimeCounter += Time.fixedDeltaTime * 1.5f;
			structureMechanicalSound.volume = Mathf.Lerp(0f, 1f, fTimeCounter);
			yield return new WaitForSeconds(0.02f);
		}
		StopCoroutine(FadeEndSound());
	}

	private void UpdateMovement(float deltaTime)
	{
		if (Speed != 0f && InLimits())
		{
			Vector3 position = base.transform.position;
			position.x += Speed * 0.001f;
			base.transform.position = Vector3.Lerp(base.transform.position, position, deltaTime * 25f);
			if (!autoControl && base.transform.localPosition.x > MaxX - 3.5f)
			{
				lerpTimer = 1f;
				Speed *= 0.1f;
				autoControl = true;
			}
		}
		if (Mathf.Approximately(Speed * 100f, 0f))
		{
			Speed = 0f;
		}
		else if (Speed != 0f)
		{
			Speed = Mathf.SmoothStep(SpeedFrom, 0f, 1f - (lerpTimer -= deltaTime * 0.9f));
		}
	}

	private bool InLimits()
	{
		if (Speed > 0f)
		{
			return base.transform.localPosition.x < MaxX;
		}
		return Speed < 0f && base.transform.localPosition.x > MinX;
	}

	private void UpdateDrag(float deltaTime)
	{
		bool mouseButtonDown = Input.GetMouseButtonDown(0);
		bool mouseButton = Input.GetMouseButton(0);
		if (mouseButton || mouseButtonDown)
		{
			if (mouseButtonDown)
			{
				lastPosition = Input.mousePosition;
				lerpTimer = 1f;
			}
			if (mouseButton)
			{
				Vector3 vector = Input.mousePosition - lastPosition;
				vector.y *= 0.2f;
				vector.z = 0f;
				Speed = 10f * DragSpeed * Mathf.Min(5f * vector.magnitude, 45f) * (float)((!(vector.x > 0f)) ? 1 : (-1));
				lastPosition = Input.mousePosition;
				SpeedFrom = Speed;
				lerpTimer = 1f;
			}
		}
	}
}
