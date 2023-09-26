using UnityEngine;

public class CameraFollowAnimation : MonoBehaviour
{
	public const float MAX_DISTANCE_TO_FAIL = 450f;

	private const float MAX_DISTANCE_TO_SLOW = 35f;

	private float SMOOTH_DAMP = 50f;

	private const float REVERSE_DAMP = 0.5f;

	public bool UsePlayerRotation;

	public float OffsetOnPlayerRotation;

	private GameObject rewindEffect;

	private Transform walker;

	private HoverController player;

	private float zoomTimer;

	private float targetFOVEasingSpeed = 0.5f;

	private bool cameraTargetSet;

	private Vector3 targetPosition;

	private Quaternion targetRotation;

	private float targetFOV;

	private CameraParticleEffectContainer targetParticleEffects;

	private GameObject defaultCameraNull;

	private float reverseTimer = -1f;

	private Vector3 target;

	private float lastOffset;

	private bool zoomOutResolution;

	private CameraParticleEffects particleEffects;

	private Transform mTransform;

	private Camera mainCam;

	private Transform mainCamTranform;

	private GameController.GameState state;

	public GameObject targetCameraNull { get; set; }

	private void Awake()
	{
		mTransform = base.transform;
	}

	private void Start()
	{
		if (rewindEffect == null)
		{
			rewindEffect = Object.FindObjectOfType<RewindEffect>().gameObject;
			rewindEffect.SetActive(false);
		}
		targetCameraNull = Object.Instantiate(Camera.main.gameObject);
		targetCameraNull.GetComponent<Camera>().enabled = false;
		targetCameraNull.tag = "Untagged";
		targetCameraNull.transform.parent = base.transform;
		targetCameraNull.transform.localPosition = Camera.main.transform.localPosition;
		targetCameraNull.transform.localRotation = Camera.main.transform.localRotation;
		if (DeviceSettings.Instance.EnableScreenParticles)
		{
			targetParticleEffects = targetCameraNull.GetComponent<CameraParticleEffectContainer>();
		}
		if (DeviceSettings.Instance.EnableDrawDistanceLimitter)
		{
			Debug.Log("far clip from " + targetCameraNull.GetComponent<Camera>().farClipPlane + " to " + targetCameraNull.GetComponent<Camera>().farClipPlane * 0.7f);
			targetCameraNull.GetComponent<Camera>().farClipPlane *= 0.7f;
		}
		player = Object.FindObjectOfType<HoverController>();
		particleEffects = player.walker.GetComponentInChildren<CameraParticleEffects>();
		if (UsePlayerRotation)
		{
			walker = player.transform;
		}
		else
		{
			walker = player.walker.transform;
		}
		if (!Main.Instance.TutorialLevel)
		{
			ClearSmoothDamp();
		}
		mTransform.position = walker.transform.position;
		if (UsePlayerRotation)
		{
			base.transform.position += walker.transform.up * OffsetOnPlayerRotation;
		}
		mTransform.rotation = walker.transform.rotation;
		CameraTrigger[] array = Object.FindObjectsOfType<CameraTrigger>();
		foreach (CameraTrigger cameraTrigger in array)
		{
			if (cameraTrigger.cameraNull != null)
			{
				cameraTrigger.cameraNull.GetComponent<Camera>().enabled = false;
				cameraTrigger.cameraNull.tag = "Untagged";
				cameraTrigger.cameraNull.transform.parent = base.transform;
			}
		}
		zoomOutResolution = (float)Screen.width / (float)Screen.height < 1.5f;
		if (zoomOutResolution)
		{
			Vector3 localPosition = Camera.main.transform.localPosition;
			localPosition.z -= 1f / (Camera.main.fieldOfView * 0.01f) * 1.8f;
			Camera.main.transform.localPosition = localPosition;
		}
		mainCam = Camera.main;
		mainCamTranform = mainCam.transform;
	}

	private void SetAutoZoom()
	{
		if (zoomOutResolution)
		{
			float num = 1f / (targetFOV * 0.01f) * 1.8f;
			targetPosition.z -= num;
		}
	}

	public void ClearSmoothDamp(float smoothValue = 8f)
	{
		SMOOTH_DAMP = smoothValue;
	}

	public GameObject UpdateCamera(float speed, GameObject cameraNull)
	{
		cameraTargetSet = true;
		targetFOVEasingSpeed = speed;
		targetPosition = cameraNull.transform.localPosition;
		targetRotation = cameraNull.transform.localRotation;
		targetFOV = cameraNull.GetComponent<Camera>().fieldOfView;
		SetCameraParticles(cameraNull);
		zoomTimer = 0f;
		SetAutoZoom();
		GameObject result = targetCameraNull;
		targetCameraNull = cameraNull;
		return result;
	}

	public void SetState(bool reset, GameObject cameraNull)
	{
		targetPosition = cameraNull.transform.localPosition;
		targetRotation = cameraNull.transform.localRotation;
		targetFOV = cameraNull.GetComponent<Camera>().fieldOfView;
		targetCameraNull = cameraNull;
		SetCameraParticles(cameraNull);
		if ((bool)walker && reset)
		{
			mTransform.position = walker.transform.position;
			mTransform.rotation = walker.transform.rotation;
			SetAutoZoom();
			mainCam = Camera.main;
			mainCamTranform = mainCam.transform;
			mainCamTranform.localPosition = targetPosition;
			mainCamTranform.localRotation = targetRotation;
			mainCam.fieldOfView = targetFOV;
		}
	}

	private void SetCameraParticles(GameObject cameraNull)
	{
		if (DeviceSettings.Instance.EnableScreenParticles)
		{
			targetParticleEffects = cameraNull.GetComponent<CameraParticleEffectContainer>();
			if (targetParticleEffects == null)
			{
				particleEffects.DisableAllEffects();
			}
			else
			{
				particleEffects.UpdateEffects(targetParticleEffects.EnabledParticleEffects, GameController.Instance.State == GameController.GameState.Running);
			}
		}
	}

	private void FixedUpdate()
	{
		if (SMOOTH_DAMP < 50f)
		{
			SMOOTH_DAMP += Time.fixedDeltaTime * 3f;
		}
		switch (GameController.Instance.State)
		{
		case GameController.GameState.Kill:
		case GameController.GameState.End:
			reverseTimer = 0f;
			break;
		case GameController.GameState.Reverse:
			target = walker.transform.position;
			zoomTimer = 1f;
			if (UsePlayerRotation)
			{
				target += walker.transform.up * OffsetOnPlayerRotation;
			}
			mTransform.position = Vector3.Lerp(mTransform.position, target, reverseTimer += Time.fixedDeltaTime * 0.5f);
			mTransform.rotation = Quaternion.Lerp(mTransform.rotation, player.walker.Spline.GetOrientationFast(player.walker.TF + 0.01f), reverseTimer);
			mainCamTranform.localPosition = Vector3.Lerp(mainCamTranform.localPosition, targetPosition, reverseTimer);
			mainCamTranform.localRotation = Quaternion.Lerp(mainCamTranform.localRotation, targetRotation, reverseTimer);
			mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, targetFOV, reverseTimer);
			if (Vector3.Distance(mTransform.position, target) < 0.01f)
			{
				reverseTimer = 0f;
				GameController.Instance.SetState(GameController.GameState.Start);
				if (rewindEffect != null)
				{
					rewindEffect.SetActive(false);
				}
			}
			else if (rewindEffect != null && !rewindEffect.activeSelf)
			{
				rewindEffect.SetActive(true);
			}
			break;
		case GameController.GameState.Start:
			target = walker.transform.position;
			zoomTimer = 1f;
			if (UsePlayerRotation)
			{
				target += walker.transform.up * OffsetOnPlayerRotation;
			}
			mTransform.position = Vector3.Lerp(mTransform.position, target, reverseTimer += Time.fixedDeltaTime * SMOOTH_DAMP);
			if (player.walker.Spline.IsInitialized)
			{
				mTransform.rotation = Quaternion.Lerp(mTransform.rotation, player.walker.Spline.GetOrientationFast(player.walker.TF + 0.01f), reverseTimer);
			}
			break;
		case GameController.GameState.Resume:
		case GameController.GameState.Running:
		case GameController.GameState.Ending:
			reverseTimer = 0f;
			doUpdate();
			break;
		}
		if (GameController.Instance.State != state && targetParticleEffects != null)
		{
			particleEffects.UpdateEffects(targetParticleEffects.EnabledParticleEffects, GameController.Instance.State == GameController.GameState.Running);
		}
		state = GameController.Instance.State;
	}

	public void MoveToTarget(Vector3 oldDistance, Quaternion oldRotation)
	{
		target = walker.transform.position;
		target += oldDistance;
		mTransform.position = target;
		mTransform.rotation = oldRotation;
	}

	private void doUpdate()
	{
		if ((bool)walker)
		{
			target = walker.transform.position;
			if (UsePlayerRotation && !player.makingJump)
			{
				target += walker.transform.up * OffsetOnPlayerRotation;
				mTransform.position = Vector3.Lerp(mTransform.position, target, Time.fixedDeltaTime * SMOOTH_DAMP * 0.35f);
				mTransform.rotation = Quaternion.Lerp(mTransform.rotation, walker.transform.rotation, Time.fixedDeltaTime * SMOOTH_DAMP * 0.1f);
			}
			else
			{
				target += base.transform.right * player.transform.localPosition.x * 0.3f;
				target += base.transform.up * player.transform.localPosition.y * 0.3f;
				mTransform.position = Vector3.Lerp(mTransform.position, target, Time.fixedDeltaTime * SMOOTH_DAMP);
				mTransform.rotation = Quaternion.Lerp(mTransform.rotation, player.walker.Spline.GetOrientationFast(player.walker.TF + 0.01f), Time.fixedDeltaTime * SMOOTH_DAMP);
			}
		}
		if (cameraTargetSet && zoomTimer < 1f)
		{
			zoomTimer += Time.deltaTime * targetFOVEasingSpeed;
			mainCamTranform.localPosition = Vector3.Lerp(mainCamTranform.localPosition, targetPosition, zoomTimer);
			mainCamTranform.localRotation = Quaternion.Lerp(mainCamTranform.localRotation, targetRotation, zoomTimer);
			mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, targetFOV, zoomTimer);
		}
	}
}
