using System.Collections;
using System.Collections.Generic;
using FluffyUnderware.Curvy;
using UnityEngine;

[ExecuteInEditMode]
public class AIWalker : ResetObject
{
	private const int HazardMax = 20;

	public GameObject Barrel;

	public CurvySplineBase Spline;

	public CurvyClamping Clamping;

	public bool SetOrientation = true;

	public bool FastInterpolation;

	public bool MoveByWorldUnits;

	public float InitialF;

	public float Speed;

	public bool Forward = true;

	public CurvyUpdateMethod UpdateIn;

	private SplineWalker walker;

	public bool IsActive;

	public Stack<GameObject> Barrels;

	private float NormalSpeed;

	private float mTF;

	private Transform mTransform;

	private float hazardTimer;

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
		mTransform = base.transform;
		NormalSpeed = 26f;
		Speed = 0f;
		if (!Spline)
		{
			yield break;
		}
		while (!Spline.IsInitialized)
		{
			yield return null;
		}
		walker = Object.FindObjectOfType<SplineWalker>();
		InitPosAndRot();
		if (Application.isPlaying)
		{
			Barrels = new Stack<GameObject>(20);
			for (int i = 0; i < 20; i++)
			{
				GameObject gameObject = Object.Instantiate(Barrel);
				gameObject.transform.parent = Barrel.transform.parent;
				gameObject.SetActive(false);
				Barrels.Push(gameObject);
			}
			Barrel.SetActive(false);
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
		if (Application.isPlaying && walker != null)
		{
			if (walker.Speed != 0f)
			{
				Speed = Mathf.Lerp(Speed, NormalSpeed, Time.deltaTime);
				UpdateHazards();
				int direction = Dir;
				if (MoveByWorldUnits)
				{
					mTransform.position = ((!FastInterpolation) ? Spline.MoveBy(ref mTF, ref direction, Speed * Time.deltaTime, Clamping) : Spline.MoveByFast(ref mTF, ref direction, Speed * Time.deltaTime, Clamping));
				}
				else
				{
					mTransform.position = ((!FastInterpolation) ? Spline.Move(ref mTF, ref direction, Speed * Time.deltaTime, Clamping) : Spline.MoveFast(ref mTF, ref direction, Speed * Time.deltaTime, Clamping));
				}
				if (SetOrientation)
				{
					base.transform.rotation = Spline.GetOrientationFast(mTF);
				}
				if (mTF == 1f)
				{
					mTF = 0f;
					InitPosAndRot();
					IsActive = false;
				}
				Dir = direction;
			}
		}
		else
		{
			InitPosAndRot();
		}
	}

	private void UpdateHazards()
	{
		if (Barrels.Count != 0)
		{
			hazardTimer -= Time.fixedDeltaTime;
			if (hazardTimer < 0f)
			{
				hazardTimer = Random.Range(0.3f, 3f);
				GameObject gameObject = Barrels.Pop();
				gameObject.transform.parent = GameController.Instance.transform;
				gameObject.transform.position = Barrel.transform.position + Random.Range(-2f, 2f) * Vector3.right;
				gameObject.SetActive(true);
				Object.Destroy(gameObject, 8f);
			}
		}
	}

	public override void Reset(bool onlyNonSaved)
	{
		mTF = InitialF;
		IsActive = false;
		InitPosAndRot();
		Speed = 0f;
		while (Barrels.Count < 20)
		{
			GameObject gameObject = Object.Instantiate(Barrel);
			gameObject.transform.parent = Barrel.transform.parent;
			gameObject.SetActive(false);
			Barrels.Push(gameObject);
		}
	}

	private void InitPosAndRot()
	{
		if ((bool)Spline)
		{
			if (Spline.Interpolate(InitialF) != mTransform.position)
			{
				mTransform.position = Spline.Interpolate(InitialF);
			}
			if (SetOrientation && mTransform.rotation != Spline.GetOrientationFast(InitialF))
			{
				mTransform.rotation = Spline.GetOrientationFast(InitialF);
			}
		}
	}
}
