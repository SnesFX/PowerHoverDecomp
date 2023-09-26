using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapCameraSplineWalker : MonoBehaviour
{
	public CurvySplineBase Spline;

	public float InitialF;

	public float Speed;

	public Camera roadCamera;

	public Camera roadUICamera;

	public RectTransform mainPanel;

	public RectTransform mapPanel;

	public RectTransform itemPanel;

	public MainController mainController;

	public AudioListener listener;

	public MapPopUp mapPopUp;

	public GameObject mapStats;

	public GameObject mapChapter2Unlock;

	private const float DragSpeedLimit = 0.45f;

	private const float DragSensibility = 0.005f;

	private Vector3 lastPosition;

	private float autoPan;

	private Animator mainAnimator;

	private float panSpeed = 5f;

	private float panning;

	private Vector3 panTarget;

	private bool backToMenu;

	private float maxScrollTF = 0.94f;

	private float mTF;

	private Transform mTransform;

	private bool mouseDown;

	private bool mousePressed;

	private bool startPanning;

	private const float moveItemY = 100f;

	private const float moveY = -500f;

	private const float moveX = -1000f;

	private const float moveTFMax = 0.025f;

	private float SpeedFrom;

	private float lerpTimer;

	private Vector3 tempSplinePos;

	public float TF
	{
		get
		{
			return mTF;
		}
		set
		{
			mTF = value;
		}
	}

	private IEnumerator Start()
	{
		mTF = InitialF;
		Speed = Mathf.Abs(Speed);
		mTransform = base.transform;
		if ((bool)Spline)
		{
			while (!Spline.IsInitialized)
			{
				yield return null;
			}
			InitPosAndRot();
			mainAnimator = mainPanel.GetComponent<Animator>();
		}
	}

	private bool OnButton()
	{
		EventSystem current = EventSystem.current;
		if (!current.IsPointerOverGameObject())
		{
			return false;
		}
		if (!current.currentSelectedGameObject)
		{
			return false;
		}
		return true;
	}

	private void Update()
	{
		mouseDown = Input.GetMouseButtonDown(0);
		mousePressed = Input.GetMouseButton(0);
		if (!UpdateAutoPanning(Time.deltaTime) && (mainController.currentMenu == MenuType.Map || mainController.currentMenu == MenuType.Main))
		{
			UpdateDrag(Time.deltaTime);
		}
	}

	private void FixedUpdate()
	{
		if (!(autoPan > 0f) && (mainController.currentMenu == MenuType.Map || mainController.currentMenu == MenuType.Main))
		{
			UpdateSpline(Time.fixedDeltaTime);
			UpdateMenuPanels();
		}
	}

	private void UpdateMenuPanels()
	{
		Vector2 anchoredPosition = mainPanel.anchoredPosition;
		anchoredPosition.x = Mathf.Max(-1000f, mTF * -40000f);
		mainPanel.anchoredPosition = anchoredPosition;
		anchoredPosition = mapPanel.anchoredPosition;
		anchoredPosition.y = Mathf.Min(0f, -500f - mTF * -20000f);
		mapPanel.anchoredPosition = anchoredPosition;
		anchoredPosition = itemPanel.anchoredPosition;
		anchoredPosition.y = 100f - Mathf.Min(100f, mTF * 4000f);
		if (DeviceSettings.Instance != null && DeviceSettings.Instance.RunningOnTV)
		{
			anchoredPosition.y -= 10f;
		}
		itemPanel.anchoredPosition = anchoredPosition;
		roadUICamera.enabled = mTF > 0.025f;
		if (autoPan <= 0f)
		{
			if (roadUICamera.enabled && mainController.currentMenu == MenuType.Main)
			{
				mainController.SetOnMap();
			}
			else if (!roadUICamera.enabled && mainController.currentMenu == MenuType.Map)
			{
				mainController.SetOnMain();
			}
		}
	}

	private bool UpdateAutoPanning(float deltaTime)
	{
		if (autoPan > 0f)
		{
			roadUICamera.enabled = true;
			autoPan -= deltaTime;
			if (autoPan <= 0f && startPanning && ((backToMenu && mTF == 0f) || !backToMenu))
			{
				startPanning = false;
			}
			else if (backToMenu && mTF > 0f && autoPan < 0f)
			{
				autoPan = 0.05f;
			}
			mTF = Spline.GetNearestPointTF(base.transform.position);
			UpdateMenuPanels();
			base.transform.position = Vector3.Lerp(base.transform.position, panTarget, deltaTime * panSpeed * (panning - autoPan));
			if (!startPanning && mouseDown && !OnButton())
			{
				lastPosition = Input.mousePosition;
				if (mainAnimator != null)
				{
					mainAnimator.enabled = false;
				}
				autoPan = 0f;
			}
			return true;
		}
		return false;
	}

	private void UpdateDrag(float deltaTime)
	{
		if (mapPopUp.IsActive || mapStats.activeSelf || mapChapter2Unlock.activeSelf || OnButton() || (!mousePressed && !mouseDown))
		{
			return;
		}
		if (mouseDown)
		{
			lastPosition = Input.mousePosition;
			if (mainAnimator != null)
			{
				mainAnimator.enabled = false;
			}
			lerpTimer = 1f;
		}
		if (mousePressed)
		{
			Vector3 vector = Input.mousePosition - lastPosition;
			vector.z = (vector.y = 0f);
			Speed = 700f * Mathf.Min(0.005f * vector.magnitude, 0.45f) * (float)((!(vector.x > 0f)) ? 1 : (-1));
			lastPosition = Input.mousePosition;
			SpeedFrom = Speed;
			lerpTimer = 1f;
		}
	}

	public Vector3 GetPanTarget()
	{
		return panTarget;
	}

	private void UpdateSpline(float deltaTime)
	{
		if ((bool)Spline && Spline.IsInitialized && Application.isPlaying)
		{
			if (!backToMenu && Mathf.Abs(Speed) < 0.005f)
			{
				Speed = 0f;
			}
			else if (Speed != 0f)
			{
				Speed = Mathf.SmoothStep(SpeedFrom, 0f, 1f - (lerpTimer -= deltaTime * 0.9f));
			}
			int direction = 1;
			if (mTF < maxScrollTF || Speed < 0f)
			{
				tempSplinePos = Spline.MoveByLengthFast(ref mTF, ref direction, Speed * deltaTime, CurvyClamping.Clamp);
				mTransform.position = Vector3.Lerp(mTransform.position, tempSplinePos, deltaTime * 25f);
			}
		}
	}

	public void ScrollToStart(float mtf = -1f)
	{
		if (!roadUICamera.enabled)
		{
			return;
		}
		if (mtf == -1f)
		{
			SetOnMap(mainPanel.anchoredPosition, 1f);
		}
		else
		{
			mTF = 0f;
			panTarget = Spline.Interpolate(mTF);
			panTarget.x -= 2f;
			autoPan = 1f;
			panning = 1f;
			panSpeed = 8.5f;
			backToMenu = true;
			if (mainAnimator != null)
			{
				mainAnimator.enabled = false;
			}
		}
		startPanning = true;
	}

	public void SetOnMap(Vector3 pos, float panTime = 2f)
	{
		backToMenu = false;
		mTF = Spline.GetNearestPointTF(pos);
		panTarget = Spline.Interpolate(mTF);
		autoPan = panTime;
		panning = panTime;
		panSpeed = 11f - 2.5f * panTime;
	}

	public void SetScrollLimit(Vector3 pos)
	{
		maxScrollTF = -0.01f + Spline.GetNearestPointTF(pos);
	}

	public void ClearScrollLimit()
	{
		maxScrollTF = 0.94f;
	}

	private void InitPosAndRot()
	{
		if ((bool)Spline && Spline.Interpolate(InitialF) != mTransform.position)
		{
			mTransform.position = Spline.Interpolate(InitialF);
		}
	}
}
