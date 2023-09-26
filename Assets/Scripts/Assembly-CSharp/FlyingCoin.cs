using System.Collections;
using FluffyUnderware.Curvy;
using UnityEngine;

public class FlyingCoin : ResetObject
{
	public CurvySpline Spline;

	private CurvyClamping Clamping;

	public float InitialF;

	[Range(0f, 13f)]
	public float Speed;

	public bool Forward = true;

	public CurvyUpdateMethod UpdateIn;

	[Range(0f, 4f)]
	public int MinLane;

	[Range(0f, 4f)]
	public int MaxLane;

	private int StartLane;

	public Collectable collectable;

	private RandomSpline RandomSpline;

	private float NormalSpeed;

	private SplineWalker walker;

	private bool RandomDone = true;

	private float offTime;

	private float mTF;

	private Transform mTransform;

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
		offTime = Random.Range(2f, 10f);
		if ((bool)Spline)
		{
			while (!Spline.IsInitialized)
			{
				yield return null;
			}
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
		if (!Spline || !Spline.IsInitialized)
		{
			return;
		}
		if (Application.isPlaying)
		{
			if (!RandomDone && RandomSpline.LaneWidth > 0f)
			{
				StartLane = Random.Range(MinLane, MaxLane);
				float num = (float)(Dir * (StartLane + 1)) * RandomSpline.LaneWidth - (float)(StartLane + 1);
				num += Random.Range((0f - RandomSpline.LaneWidth) * 0.5f, RandomSpline.LaneWidth * 0.5f);
				collectable.transform.localPosition = new Vector3(0f - num, collectable.transform.localPosition.y, collectable.transform.localPosition.z);
				Speed = NormalSpeed + Random.Range(-2f, 2f);
				RandomDone = true;
			}
			if (GameController.Instance.State != GameController.GameState.Running)
			{
				return;
			}
			if (offTime > 0f)
			{
				offTime -= Time.fixedDeltaTime;
				if (offTime <= 0f)
				{
					MoveCollectable(Random.Range(0.015f, 0.05f), -1);
					collectable.Reset(false);
				}
			}
			else
			{
				UpdateMovement();
			}
		}
		else
		{
			InitPosAndRot();
		}
	}

	private void UpdateMovement()
	{
		int direction = -1;
		mTransform.position = Spline.MoveByFast(ref mTF, ref direction, Speed * Time.deltaTime, Clamping);
		base.transform.rotation = Spline.GetOrientationFast(mTF);
		float num = walker.TFDistance - Spline.TFToDistance(mTF);
		if ((num + Spline.Length > 60f && num + Spline.Length < 70f) || (num > 60f && num < 70f))
		{
			offTime = Random.Range(2f, 10f);
		}
	}

	private void MoveCollectable(float diff, int dir)
	{
		mTF = walker.TF + diff;
		mTransform.position = Spline.MoveByFast(ref mTF, ref dir, Speed * Time.deltaTime, Clamping);
		base.transform.rotation = Spline.GetOrientationFast(mTF);
		RandomDone = false;
	}

	public override void Reset(bool onlyNonSaved)
	{
		mTF = InitialF;
		offTime = Random.Range(2f, 10f);
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
