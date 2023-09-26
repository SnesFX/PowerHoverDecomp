using UnityEngine;

public class GrindControl : MonoBehaviour
{
	private const float SMOOTHER_START = 60f;

	public TrailRenderer GrindTrail;

	public HoverController hoverController;

	public BoxCollider grindCollider;

	public Transform GraphicsTransform;

	private float speedTimer;

	private bool grindEnabled;

	private CurvySpline Spline;

	private GrindTrigger Trigger;

	private float splineTF;

	private Vector3 largeSize;

	private Vector3 smallSize;

	private Vector3 smallPosition;

	private Vector3 largePosition;

	private float startPosition;

	private bool controlBlocked;

	private float lerpTimer;

	private Vector3 lerpPosition;

	private float MaxSpeed;

	private Vector3 tempPos;

	public float Speed { get; private set; }

	public float completion { get; private set; }

	public bool grinded { get; private set; }

	public int jumpOffDiff { get; private set; }

	private void Start()
	{
		largeSize = grindCollider.size;
		smallSize = largeSize;
		smallSize.x *= 0.5f;
		smallSize.z *= 0.5f;
		largeSize.y *= 0.5f;
		largePosition = grindCollider.center;
		smallPosition = largePosition;
		largePosition.y = 0.15f;
	}

	public bool CanGrind(CurvySpline path, GrindTrigger grindTrigger)
	{
		return Trigger != grindTrigger;
	}

	public void ClearTrigger(GrindTrigger grindTrigger)
	{
		if (Trigger == grindTrigger)
		{
			Trigger = null;
		}
	}

	public void SetGrindEnabled(CurvySpline path, GrindTrigger grindTrigger, float walkerSpeed)
	{
		controlBlocked = true;
		Trigger = grindTrigger;
		jumpOffDiff = Trigger.JumpOffSpeedDiff;
		Spline = path;
		splineTF = Spline.GetNearestPointTF(base.transform.position);
		startPosition = splineTF;
		completion = 0f;
		grindEnabled = true;
		Speed = walkerSpeed;
		MaxSpeed = grindTrigger.Speed;
		speedTimer = 0f;
		grinded = false;
		lerpTimer = 0f;
		lerpPosition = GraphicsTransform.position;
	}

	public void Reset()
	{
		GrindTrail.enabled = false;
		grindEnabled = false;
		Spline = null;
		splineTF = 0f;
		Trigger = null;
	}

	private void FixedUpdate()
	{
		UpdateGrindCollider();
		if (!grindEnabled)
		{
			return;
		}
		if (splineTF == 1f || (!controlBlocked && (UIController.Instance.leftPressed || UIController.Instance.rightPressed)))
		{
			if (Trigger != null)
			{
				Trigger.SetUsed();
			}
			Trigger = null;
			GrindTrail.enabled = false;
			grindEnabled = false;
			speedTimer = (lerpTimer = 0f);
			if (Speed - hoverController.walker.NormalSpeed > 0f)
			{
				hoverController.walker.SetBoostExtra(Speed - hoverController.walker.NormalSpeed);
			}
			hoverController.SetOnRail(false, null, null);
		}
		else if (GameController.Instance.State == GameController.GameState.Running)
		{
			if (!UIController.Instance.rightPressed && !UIController.Instance.leftPressed)
			{
				controlBlocked = false;
			}
			UpdateCurve();
			completion = splineTF - startPosition;
			GrindTrail.enabled = true;
			grinded = true;
		}
	}

	private void UpdateCurve()
	{
		if ((bool)Spline && Spline.IsInitialized)
		{
			int direction = 1;
			Speed = Mathf.Lerp(Speed, MaxSpeed, speedTimer += Time.fixedDeltaTime * 5f);
			tempPos = Spline.MoveByFast(ref splineTF, ref direction, Speed * Time.fixedDeltaTime, CurvyClamping.Clamp);
			if (Trigger.PlayerOffset > 0f)
			{
				tempPos += base.transform.up * Trigger.PlayerOffset;
			}
			base.transform.position = Vector3.Lerp(base.transform.position, tempPos, (speedTimer + (lerpTimer += 2f * Time.fixedDeltaTime)) * 2f);
			GraphicsTransform.position = Vector3.Lerp(lerpPosition, base.transform.position, (speedTimer + lerpTimer * lerpTimer) * Time.fixedDeltaTime * 60f);
			base.transform.rotation = Spline.GetOrientationFast(splineTF);
			if (splineTF >= 1f && Spline.Closed)
			{
				splineTF = 0f;
			}
		}
	}

	private void UpdateGrindCollider()
	{
		if (hoverController.PlayerState == PlayerState.InAir && grindCollider.size != largeSize)
		{
			grindCollider.center = largePosition;
			grindCollider.size = largeSize;
		}
		else if (hoverController.PlayerState != PlayerState.InAir && grindCollider.size != smallSize)
		{
			grindCollider.center = smallPosition;
			grindCollider.size = smallSize;
		}
	}
}
