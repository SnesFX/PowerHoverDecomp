using System.Collections;
using FluffyUnderware.Curvy;
using UnityEngine;

[ExecuteInEditMode]
public class CarAI : ResetObject
{
	private enum AiCarState
	{
		Normal = 0,
		Slowing = 1,
		ChangeLane = 2
	}

	private const string AI_TAG = "AI";

	public CurvySplineBase Spline;

	public CurvyClamping Clamping;

	public float InitialF;

	[Range(4f, 13f)]
	public float Speed;

	public bool Forward = true;

	public CurvyUpdateMethod UpdateIn;

	public Transform carTransform;

	public bool DontChangeLines;

	[Range(0f, 3f)]
	public int StartLane;

	private RandomSpline RandomSpline;

	private AiCarState State;

	private float StateTimer;

	private float LerpToSpeed;

	private Vector3 LerpToPosition;

	private float NormalSpeed;

	private bool RandomDone;

	private SplineWalker walker;

	private float CarLenght;

	private bool ritariassa;

	private float offTimer;

	private float mTF;

	private Transform mTransform;

	private int lastLaneDir;

	private float laneCheckDistance;

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
		walker = Object.FindObjectOfType<SplineWalker>();
		RandomSpline = Spline.GetComponent<RandomSpline>();
		mTF = InitialF;
		Speed = Mathf.Abs(Speed);
		NormalSpeed = Speed;
		mTransform = base.transform;
		mTransform.position = new Vector3(0f, 1000f, 0f);
		StateTimer = Random.Range(0.1f, 2f);
		offTimer = Random.Range(0f, 12f);
		carTransform.localRotation = Quaternion.Euler(new Vector3(0f, (float)Dir * 90f, 0f));
		if ((bool)Spline)
		{
			while (!Spline.IsInitialized)
			{
				yield return null;
			}
			CarLenght = carTransform.GetComponent<BoxCollider>().size.x;
		}
	}

	private void Update()
	{
		if (UpdateIn == CurvyUpdateMethod.Update)
		{
			doUpdate();
		}
	}

	private void LateUpdate()
	{
		if (UpdateIn == CurvyUpdateMethod.LateUpdate)
		{
			doUpdate();
		}
	}

	private void FixedUpdate()
	{
		if (UpdateIn == CurvyUpdateMethod.FixedUpdate)
		{
			doUpdate();
		}
	}

	private void doUpdate()
	{
		if (!Spline || !Spline.IsInitialized || !Application.isPlaying)
		{
			return;
		}
		if (offTimer > 0f)
		{
			offTimer -= Time.fixedDeltaTime;
			if (offTimer <= 0f)
			{
				MoveCar(Random.Range(0.08f, 0.1f), Dir);
			}
			return;
		}
		if (!RandomDone && RandomSpline.LaneWidth > 0f)
		{
			float num = (float)(Dir * (StartLane + 1)) * RandomSpline.LaneWidth - (float)(StartLane + 1);
			carTransform.localPosition = new Vector3(0f - num, carTransform.localPosition.y, carTransform.localPosition.z);
			Speed = NormalSpeed + Random.Range(-2f, 2f);
			laneCheckDistance = RandomSpline.LaneWidth * 1.5f;
			RandomDone = true;
		}
		if (RandomDone)
		{
			UpdateAI();
			UpdateMovement();
		}
	}

	private void UpdateAI()
	{
		RaycastHit hit;
		switch (State)
		{
		case AiCarState.Slowing:
			Speed = Mathf.Lerp(Speed, LerpToSpeed, Time.deltaTime * 5f);
			if (Mathf.Abs(Speed - LerpToSpeed) < 0.05f)
			{
				State = AiCarState.Normal;
				Speed = LerpToSpeed;
				StateTimer = Random.Range(0.1f, 3f);
			}
			else if (LerpToSpeed > 2f && CheckForward(1f + Speed * 0.1f, out hit) && hit.collider.transform.GetComponentInParent<CarAI>() != null)
			{
				LerpToSpeed -= 1f;
			}
			break;
		case AiCarState.ChangeLane:
			carTransform.localPosition = Vector3.Lerp(carTransform.localPosition, LerpToPosition, StateTimer += Time.fixedDeltaTime * 0.05f);
			if (CheckForward(7f + Speed * 0.3f, out hit) && hit.collider.transform.GetComponentInParent<CarAI>() != null && hit.distance < 4f)
			{
				CarAI componentInParent2 = hit.collider.transform.GetComponentInParent<CarAI>();
				Speed = Mathf.Lerp(Speed, componentInParent2.Speed, Time.deltaTime * 3f);
			}
			if (Mathf.Abs(carTransform.localPosition.x - LerpToPosition.x) < 0.05f)
			{
				State = AiCarState.Normal;
				carTransform.localPosition = LerpToPosition;
				StateTimer = Random.Range(1f, 6f);
			}
			break;
		case AiCarState.Normal:
		{
			if (NormalSpeed < 20f)
			{
				NormalSpeed += walker.TotalDistance * 1E-07f;
			}
			if (StateTimer > 0f && !DontChangeLines)
			{
				StateTimer -= Time.fixedDeltaTime;
				if (StateTimer <= 0f)
				{
					StateTimer = Random.Range(1f, 3f);
					if (ritariassa || (Speed > 3f && !DontChangeLines && Random.Range(0, 100) > 40))
					{
						if (ChangeLane())
						{
							break;
						}
					}
					else if (Speed < NormalSpeed && !CheckForward(2f, out hit))
					{
						Speed += Random.Range(1f, 2f);
					}
				}
			}
			if (Speed < NormalSpeed && !CheckForward(3f, out hit))
			{
				Speed = Mathf.Lerp(Speed, Speed + 0.15f, Time.fixedTime * 40f);
			}
			if (!CheckForward(7f + Speed * 0.3f, out hit) || !hit.collider.CompareTag("AI") || !(hit.collider.transform.GetComponentInParent<CarAI>() != null))
			{
				break;
			}
			CarAI componentInParent = hit.collider.transform.GetComponentInParent<CarAI>();
			if (!(componentInParent.Speed < Speed) && !(hit.distance < 4f))
			{
				break;
			}
			int num = Random.Range(0, 100);
			if ((ritariassa || (num > 80 && !DontChangeLines)) && ChangeLane())
			{
				break;
			}
			LerpToSpeed = componentInParent.Speed;
			if (hit.distance < 4f + Mathf.Abs(Speed - componentInParent.Speed))
			{
				if (base.transform.gameObject.name.Equals(hit.collider.transform.parent.gameObject.name))
				{
					Speed -= 4f;
				}
				if (Speed <= 0f)
				{
					Speed = 1.1f;
				}
				LerpToSpeed = Speed - 1f;
			}
			State = AiCarState.Slowing;
			break;
		}
		}
	}

	private bool CheckForward(float distance, out RaycastHit hit)
	{
		Vector3 vector = carTransform.position + base.transform.forward * CarLenght * Dir * 0.5f;
		if (Physics.Raycast(vector, base.transform.forward * Dir, out hit, distance) || Physics.Raycast(vector + Vector3.forward * RandomSpline.LaneWidth * 0.25f, base.transform.forward * Dir, out hit, distance) || Physics.Raycast(vector + Vector3.back * RandomSpline.LaneWidth * 0.25f, base.transform.forward * Dir, out hit, distance))
		{
			return true;
		}
		return false;
	}

	private bool ChangeLane()
	{
		Vector3 vector = -base.transform.right;
		int num = Mathf.FloorToInt(Mathf.Abs(carTransform.localPosition.x) / RandomSpline.LaneWidth);
		int num2 = 0;
		if (num > 0 && num < 3)
		{
			int num3 = ((Random.Range(0, 100) <= ((lastLaneDir != -1) ? 80 : 20)) ? 1 : (-1));
			vector = num3 * base.transform.right;
			lastLaneDir = num3;
			num2 = num - num3;
		}
		else if (num == 3)
		{
			num2 = 2;
			vector = base.transform.right;
			lastLaneDir = 1;
		}
		else
		{
			num2 = 1;
			lastLaneDir = -1;
		}
		vector *= (float)Dir;
		Vector3 vector2 = carTransform.position + vector * 1.2f;
		bool flag = true;
		for (int i = -3; i < 4; i++)
		{
			Debug.DrawRay(vector2 + i * base.transform.forward * (CarLenght + Speed * 0.1f), vector, Color.blue, 10f);
			if (Physics.Raycast(vector2 + i * base.transform.forward * (CarLenght + Speed * 0.1f), vector, laneCheckDistance))
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			LerpToPosition = carTransform.localPosition;
			LerpToPosition.x = (float)Dir * (0f - ((float)(num2 + 1) * RandomSpline.LaneWidth - (float)(num2 + 1)));
			LerpToPosition.x += Random.Range((0f - RandomSpline.LaneWidth) * 0.35f, RandomSpline.LaneWidth * 0.35f);
			State = AiCarState.ChangeLane;
			StateTimer = 0f;
			return true;
		}
		return false;
	}

	private void UpdateMovement()
	{
		int direction = Dir;
		mTransform.position = Spline.MoveByFast(ref mTF, ref direction, Speed * Time.deltaTime, Clamping);
		base.transform.rotation = Spline.GetOrientationFast(mTF);
		Dir = direction;
		if (Forward)
		{
			float num = walker.TFDistance - Spline.TFToDistance(mTF);
			if (num > 100f)
			{
				offTimer = GetWaitTime();
			}
		}
		else
		{
			float num2 = walker.TFDistance - Spline.TFToDistance(mTF);
			if ((num2 + Spline.Length > 60f && num2 + Spline.Length < 70f) || (num2 > 60f && num2 < 70f))
			{
				offTimer = GetWaitTime();
			}
		}
	}

	private float GetWaitTime()
	{
		float num = 10f - walker.TotalDistance / 1000f;
		return Random.Range(num * 0.5f, num);
	}

	private void MoveCar(float diff, int dir)
	{
		mTF = walker.TF + diff;
		mTransform.position = Spline.MoveByFast(ref mTF, ref dir, Speed * Time.deltaTime, Clamping);
		base.transform.rotation = Spline.GetOrientationFast(mTF);
		Speed = NormalSpeed + Random.Range(-2f, 2f);
		if (ChangeLane())
		{
			carTransform.localPosition = LerpToPosition;
			State = AiCarState.Normal;
			StateTimer = Random.Range(1f, 3f);
		}
	}

	public override void Reset(bool onlyNonSaved)
	{
		mTF = InitialF;
		RandomDone = false;
		State = AiCarState.Normal;
		offTimer = Random.Range(0f, 12f);
		mTransform.position = new Vector3(0f, 1000f, 0f);
	}

	private void InitPosAndRot()
	{
		if ((bool)Spline)
		{
			if (Spline.Interpolate(InitialF) != mTransform.position)
			{
				mTransform.position = Spline.Interpolate(InitialF);
			}
			if (mTransform.rotation != Spline.GetOrientationFast(InitialF))
			{
				mTransform.rotation = Spline.GetOrientationFast(InitialF);
			}
		}
	}
}
