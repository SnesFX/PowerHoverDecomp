using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class SplineWalker : ResetObject
{
	public enum LevelState
	{
		Start = 0,
		Started = 1,
		Normal = 2,
		Ending = 3,
		Zooming = 4
	}

	private const string LEFT = "Left";

	private const string RIGHT = "Right";

	public CurvySpline Spline;

	public float InitialF;

	public float Speed;

	public bool Forward = true;

	public bool SlowdownEnabled;

	public LineGraphics lineGraphics;

	public HoverController hoverController;

	public bool CanBreak = true;

	private float specialTimer;

	private float BoostTime = 1f;

	private float BoostSpeed;

	private bool controlBlocked;

	private float changeSlowingTime;

	public float slowEasing;

	public bool slowing;

	private float stateTimer;

	private float LastDistance;

	private float distanceBoostTimer;

	private float mTF;

	private float AddSpeed;

	private float addOnDistance;

	public float NormalSpeed { get; private set; }

	public float OrginalSpeed { get; private set; }

	public float TFDistance { get; private set; }

	public bool Boosting
	{
		get
		{
			return specialTimer > 0f && Speed > NormalSpeed;
		}
	}

	public float TotalDistance { get; private set; }

	public LevelState State { get; private set; }

	public int DistanceBoostMultiplier { get; private set; }

	public bool levelReady { get; private set; }

	public float EndlessSpeed
	{
		get
		{
			return Speed + AddSpeed;
		}
	}

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

	public int Dir
	{
		get
		{
			return Forward ? 1 : (-1);
		}
		set
		{
			bool flag = value >= 0;
			if (flag != Forward)
			{
				Forward = flag;
			}
		}
	}

	private IEnumerator Start()
	{
		mTF = InitialF;
		while (GameController.Instance == null)
		{
			yield return null;
		}
		Speed = Mathf.Abs(GameController.Instance.HoverSpeed);
		SplineWalker splineWalker = this;
		float normalSpeed = (OrginalSpeed = Speed);
		splineWalker.NormalSpeed = normalSpeed;
		CanBreak = false;
		State = LevelState.Start;
		levelReady = false;
		stateTimer = 0f;
		controlBlocked = true;
		if ((bool)Spline)
		{
			while (!Spline.IsInitialized)
			{
				yield return null;
			}
			InitPosAndRot();
		}
		if (Application.isPlaying && !Spline.Closed)
		{
			Speed = 0f;
			lineGraphics.SetLineGraphics(Spline);
		}
	}

	private void FixedUpdate()
	{
		if (!Spline || !Spline.IsInitialized)
		{
			return;
		}
		if (Application.isPlaying)
		{
			if (Spline.Closed)
			{
				UpdateEndlessMode();
			}
			switch (GameController.Instance.State)
			{
			case GameController.GameState.Paused:
				return;
			case GameController.GameState.End:
				ResetSpeeds();
				return;
			case GameController.GameState.Kill:
			case GameController.GameState.Reverse:
				controlBlocked = true;
				State = LevelState.Start;
				stateTimer = 0f;
				ResetSpeeds();
				return;
			case GameController.GameState.Ending:
				Speed = Mathf.Lerp(NormalSpeed, 0f, stateTimer += Time.deltaTime);
				slowEasing = 1f;
				break;
			}
			if (hoverController.PlayerState == PlayerState.Grinding)
			{
				SetPositionOnSpline(hoverController.transform.position);
				return;
			}
			int direction = Dir;
			switch (State)
			{
			case LevelState.Zooming:
				stateTimer -= Time.deltaTime;
				if (stateTimer < 0f)
				{
					State = LevelState.Start;
				}
				return;
			case LevelState.Start:
				if (!Main.Instance.TestingLevel && Main.Instance.InMenu)
				{
					return;
				}
				stateTimer += Time.deltaTime;
				if (stateTimer > 0.5f)
				{
					levelReady = true;
					if (UIController.Instance.rightPressed || UIController.Instance.leftPressed)
					{
						stateTimer = 0f;
						State = LevelState.Started;
						Speed = 0f;
						controlBlocked = false;
						GameController.Instance.SetState(GameController.GameState.Running);
					}
				}
				return;
			case LevelState.Started:
				if (controlBlocked)
				{
					return;
				}
				Speed = Mathf.Lerp(0f, NormalSpeed, stateTimer += Time.deltaTime);
				if (stateTimer > 1f)
				{
					stateTimer = 0f;
					State = LevelState.Normal;
					CanBreak = true;
				}
				break;
			case LevelState.Ending:
				Speed = Mathf.Lerp(NormalSpeed, 0f, stateTimer += Time.deltaTime);
				break;
			}
			if (slowEasing <= 0f)
			{
				UpdateBoost();
			}
			CurvySpline spline = Spline;
			if (Main.Instance.CreditsLevel)
			{
				Speed = 30f;
				AddSpeed = 0f;
			}
			base.transform.position = Spline.MoveByConnectionFast(ref Spline, ref mTF, ref direction, (Speed + AddSpeed) * Time.deltaTime, CurvyClamping.Clamp, 1, CurrentConnectionTags());
			if (spline != Spline)
			{
				Camera.main.transform.GetComponentInParent<CameraFollowAnimation>().ClearSmoothDamp(6f);
			}
			base.transform.rotation = Spline.GetOrientationFast(mTF);
			Dir = direction;
			if (Spline.Closed && mTF == 1f)
			{
				mTF = 0f;
			}
		}
		else
		{
			InitPosAndRot();
		}
	}

	private void UpdateEndlessMode()
	{
		if (distanceBoostTimer > 0f)
		{
			distanceBoostTimer -= Time.fixedDeltaTime;
			if (distanceBoostTimer <= 0f)
			{
				UIController.Instance.DistanceBoost(0);
			}
		}
		if (GameController.Instance.BoostSpeedOnDistance && Speed < 40f)
		{
			AddSpeed = TotalDistance * 0.002f;
		}
		TFDistance = Spline.TFToDistance(TF);
		float num = TFDistance - LastDistance;
		if (num < -100f)
		{
			num = TFDistance + Spline.Length - LastDistance;
		}
		if (distanceBoostTimer > 0f)
		{
			float num2 = num * (float)(DistanceBoostMultiplier - 1);
			addOnDistance += num2;
			TotalDistance += num2;
		}
		if (num < 100f)
		{
			TotalDistance += num;
		}
		if (LevelStats.Instance != null)
		{
			LevelStats.Instance.LevelDistance = TotalDistance;
		}
		LastDistance = TFDistance;
	}

	public void TutorialStart()
	{
		levelReady = true;
		stateTimer = 0f;
		State = LevelState.Started;
		controlBlocked = false;
		GameController.Instance.SetState(GameController.GameState.Running);
	}

	public void EndLevel()
	{
		State = LevelState.Ending;
		stateTimer = 0f;
		LastDistance = (addOnDistance = 0f);
		if (Spline.Closed)
		{
			LevelStats.Instance.LevelCompleted(TotalDistance);
		}
	}

	public int AddScoreToDistance(float score)
	{
		if (score > 0f)
		{
			int num = Mathf.CeilToInt(score * 0.05f * (float)((!(distanceBoostTimer > 0f)) ? 1 : DistanceBoostMultiplier));
			addOnDistance += num;
			TotalDistance += num;
			return num;
		}
		return 0;
	}

	public void SetDistanceBoost(int boost)
	{
		distanceBoostTimer = 3f;
		DistanceBoostMultiplier = ((!(distanceBoostTimer > 0f)) ? boost : (DistanceBoostMultiplier + boost));
		UIController.Instance.DistanceBoost(boost);
	}

	private void UpdateBoost()
	{
		if (specialTimer > 0f)
		{
			Speed = Mathf.Lerp(Speed, BoostSpeed, BoostTime - (specialTimer -= Time.deltaTime * 0.5f));
		}
		else if (Mathf.Abs(Speed - NormalSpeed) > 0.02f)
		{
			Speed = Mathf.Lerp(Speed, NormalSpeed, Mathf.Abs(specialTimer -= Time.deltaTime * 0.25f));
		}
	}

	public void OnSand(bool enable)
	{
		if (!SlowdownEnabled)
		{
			enable = false;
			return;
		}
		if (slowing != enable)
		{
			changeSlowingTime = slowEasing;
			if (enable)
			{
				slowEasing = 0f;
				specialTimer = 0f;
			}
		}
		slowing = enable;
		if (slowing && slowEasing < 1f)
		{
			Speed = Mathf.Lerp(Speed, 0f, slowEasing += Time.deltaTime * 0.25f);
		}
		else if (!slowing && slowEasing > -1f)
		{
			Speed = Mathf.Lerp(Speed, NormalSpeed, changeSlowingTime - (slowEasing -= Time.deltaTime * 0.25f));
		}
	}

	private float EaseInOut(float t, float b, float c = 1f, float d = 1f)
	{
		t /= d / 2f;
		if (t < 1f)
		{
			return c / 2f * t * t + b;
		}
		t -= 1f;
		return (0f - c) / 2f * (t * (t - 2f) - 1f) + b;
	}

	public void UpdateSlowing(bool enable, float easingMultiplier)
	{
		if (slowing != enable)
		{
			changeSlowingTime = slowEasing;
			if (enable)
			{
				slowEasing = 0f;
				specialTimer = 0f;
			}
		}
		slowing = enable;
		if (slowing && slowEasing < 1f)
		{
			Speed = Mathf.Lerp(Speed, 20f, slowEasing = EaseInOut(slowEasing + Time.deltaTime * easingMultiplier, slowEasing * 0.8f));
			Speed = Mathf.Max(0.1f, Speed);
		}
		else if (!slowing && slowEasing > -1f)
		{
			Speed = Mathf.Lerp(Speed, NormalSpeed, changeSlowingTime - (slowEasing -= Time.deltaTime * 0.5f));
		}
	}

	public void SetBoostExtra(float extra)
	{
		NormalSpeed = Mathf.Lerp(NormalSpeed, OrginalSpeed + extra, Time.deltaTime * ((extra != 0f) ? 0.5f : 2f));
	}

	public void SetBoost(float boost, float speed = -1f)
	{
		BoostSpeed = ((speed != -1f) ? speed : (NormalSpeed * 1.5f));
		BoostTime = boost;
		if (specialTimer <= 0f)
		{
			specialTimer = BoostTime;
		}
	}

	private void ResetSpeeds()
	{
		NormalSpeed = ((!Spline.Closed) ? OrginalSpeed : 10f);
		BoostSpeed = NormalSpeed;
		Speed = 0f;
		AddSpeed = 0f;
		specialTimer = 0f;
	}

	public void SetZooming()
	{
		State = LevelState.Zooming;
		stateTimer = Camera.main.transform.GetComponent<FakeDollyZoom>().EffectTime - 1.25f;
	}

	public void SetPositionOnSpline(Vector3 pos)
	{
		Speed = NormalSpeed;
		mTF = Spline.GetNearestPointTF(pos);
		base.transform.position = Spline.Interpolate(mTF);
		base.transform.rotation = Spline.GetOrientationFast(mTF);
	}

	public override void Reset(bool isRewind)
	{
		float lastDistance = (TotalDistance = (addOnDistance = 0f));
		LastDistance = lastDistance;
		ResetSpeeds();
		State = LevelState.Start;
	}

	private void InitPosAndRot()
	{
		if ((bool)Spline)
		{
			if (Spline.Interpolate(InitialF) != base.transform.position)
			{
				base.transform.position = Spline.Interpolate(InitialF);
			}
			if (base.transform.rotation != Spline.GetOrientationFast(InitialF))
			{
				base.transform.rotation = Spline.GetOrientationFast(InitialF);
			}
		}
	}

	private string[] CurrentConnectionTags()
	{
		return (Spline.name + " " + ((!(hoverController.transform.position.z > base.transform.position.z)) ? "Right" : "Left")).Split(' ');
	}
}
