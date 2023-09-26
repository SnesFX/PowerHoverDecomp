using System;
using System.Collections;
using System.Collections.Generic;
using FluffyUnderware.Curvy;
using FluffyUnderware.Curvy.Utils;
using UnityEngine;

public class Chopper : CurvyComponent
{
	public enum FollowMode
	{
		Relative = 0,
		AbsoluteExtrapolate = 1,
		AbsolutePrecise = 2
	}

	protected delegate void Action();

	private const int HazardMax = 20;

	public GameObject Gfx;

	public GameObject Barrel;

	public Stack<GameObject> Barrels;

	private SplineWalker walker;

	private float hazardTimer;

	[SerializeField]
	[Label(Tooltip = "The Spline or Splinegroup to use")]
	private CurvySplineBase m_Spline;

	[Label(Tooltip = "Interpolate cached values?")]
	public bool FastInterpolation;

	[SerializeField]
	[Label(Tooltip = "Movement Mode")]
	private FollowMode m_Mode = FollowMode.AbsoluteExtrapolate;

	[SerializeField]
	private CurvyVector m_Initial = new CurvyVector();

	[Positive(Tooltip = "Speed in F or World Units (depending on Mode)")]
	public float Speed;

	[Label(Tooltip = "End of Spline Behaviour")]
	public CurvyClamping Clamping;

	[Label(Tooltip = "Align to Up-Vector?")]
	public bool SetOrientation = true;

	[Label(Text = "Use 2D Orientation", Tooltip = "Use 2D Orientation (along z-axis only)?")]
	public bool Use2DOrientation;

	[SerializeField]
	[Label(Tooltip = "Enable if you plan to add/remove segments during movement")]
	private bool m_Dynamic;

	private bool mIsInitialized;

	private CurvyVector Current;

	private WeakReference mCurrentSeg;

	protected float mCurrentSegmentF = -1f;

	protected float mCurrentTF;

	protected Action UpdateAction;

	private float initialTF;

	public bool IsActive { get; set; }

	public CurvySplineBase Spline
	{
		get
		{
			return m_Spline;
		}
		set
		{
			if (m_Spline != value)
			{
				m_Spline = value;
				Initialize();
			}
		}
	}

	public FollowMode Mode
	{
		get
		{
			return m_Mode;
		}
		set
		{
			if (m_Mode != value)
			{
				m_Mode = value;
				Initialize();
			}
		}
	}

	public CurvyVector Initial
	{
		get
		{
			return m_Initial;
		}
		set
		{
			if (m_Initial != value)
			{
				m_Initial = value;
				if (Mode != 0 && SourceIsInitialized)
				{
					m_Initial.Validate(Spline.Length);
				}
				else
				{
					m_Initial.Validate();
				}
			}
		}
	}

	public bool Dynamic
	{
		get
		{
			return m_Dynamic;
		}
		set
		{
			if (m_Dynamic != value)
			{
				if (SourceIsInitialized)
				{
					mCurrentSegment = Spline.TFToSegment(mCurrentTF, out mCurrentSegmentF);
				}
				m_Dynamic = value;
			}
		}
	}

	public CurvySplineSegment CurrentSegment
	{
		get
		{
			if (m_Dynamic)
			{
				return mCurrentSegment;
			}
			return Spline.TFToSegment(mCurrentTF, out mCurrentSegmentF);
		}
	}

	public float CurrentSegmentF
	{
		get
		{
			if (mCurrentSegmentF == -1f)
			{
				mCurrentSegment = Spline.TFToSegment(mCurrentTF, out mCurrentSegmentF);
			}
			return mCurrentSegmentF;
		}
	}

	public float CurrentTF
	{
		get
		{
			return mCurrentTF;
		}
	}

	protected CurvySplineSegment mCurrentSegment
	{
		get
		{
			return (mCurrentSeg != null) ? ((CurvySplineSegment)mCurrentSeg.Target) : null;
		}
		set
		{
			if (mCurrentSeg == null)
			{
				mCurrentSeg = new WeakReference(value);
			}
			else
			{
				mCurrentSeg.Target = value;
			}
		}
	}

	public virtual float Position
	{
		get
		{
			FollowMode mode = Mode;
			if (mode == FollowMode.AbsoluteExtrapolate || mode == FollowMode.AbsolutePrecise)
			{
				return Spline.TFToDistance(mCurrentTF);
			}
			return mCurrentTF;
		}
		set
		{
			Current.Position = value;
			FollowMode mode = Mode;
			if (mode == FollowMode.AbsoluteExtrapolate || mode == FollowMode.AbsolutePrecise)
			{
				mCurrentTF = Spline.DistanceToTF(value);
			}
			else
			{
				mCurrentTF = value;
			}
		}
	}

	protected virtual bool SourceIsInitialized
	{
		get
		{
			return (bool)Spline && Spline.IsInitialized;
		}
	}

	private IEnumerator Start()
	{
		if ((bool)Spline)
		{
			while (!SourceIsInitialized)
			{
				yield return 0;
			}
			Initialize();
		}
	}

	private void OnEnable()
	{
		if (!Application.isPlaying)
		{
			Initialize();
		}
	}

	private void Update()
	{
		if (UpdateIn == CurvyUpdateMethod.Update && Application.isPlaying)
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

	private void OnValidate()
	{
		Initialize();
	}

	public virtual void Refresh()
	{
		Speed = walker.Speed;
		UpdateAction();
	}

	public override void EditorUpdate()
	{
		base.EditorUpdate();
		doUpdate();
	}

	public override bool Initialize()
	{
		if (!SourceIsInitialized)
		{
			return false;
		}
		walker = UnityEngine.Object.FindObjectOfType<SplineWalker>();
		UpdateAction = UpdateAbsoluteExtrapolate;
		Initial.Absolute(Spline.Length);
		Current = new CurvyVector(Initial);
		mCurrentTF = Spline.DistanceToTF(Current.m_Position);
		initialTF = Initial.m_Position;
		Barrels = new Stack<GameObject>(20);
		Reset();
		Barrel.SetActive(false);
		return true;
	}

	protected void UpdateAbsoluteExtrapolate()
	{
		if (FastInterpolation)
		{
			base.Transform.position = Spline.MoveByFast(ref mCurrentTF, ref Current.m_Direction, Speed * CurvyUtility.DeltaTime, Clamping);
		}
		else
		{
			base.Transform.position = Spline.MoveBy(ref mCurrentTF, ref Current.m_Direction, Speed * CurvyUtility.DeltaTime, Clamping);
		}
		if (SetOrientation)
		{
			orientate();
		}
	}

	private void orientate()
	{
		if (Use2DOrientation)
		{
			base.Transform.rotation = Quaternion.LookRotation(Vector3.forward, Spline.GetTangentFast(mCurrentTF));
		}
		else
		{
			base.Transform.rotation = Spline.GetOrientationFast(mCurrentTF);
		}
	}

	private void doUpdate()
	{
		if (!SourceIsInitialized)
		{
			return;
		}
		if (!mIsInitialized)
		{
			mIsInitialized = Initialize();
			if (!mIsInitialized)
			{
				return;
			}
		}
		if (GameController.Instance.State == GameController.GameState.Start)
		{
			Reset();
		}
		if (GameController.Instance.State != GameController.GameState.Running)
		{
			return;
		}
		UpdateHazards();
		if (Dynamic)
		{
			if ((bool)mCurrentSegment)
			{
				mCurrentTF = Spline.SegmentToTF(mCurrentSegment, mCurrentSegmentF);
			}
			Refresh();
			mCurrentSegment = Spline.TFToSegment(mCurrentTF, out mCurrentSegmentF);
		}
		else
		{
			Refresh();
		}
	}

	private void UpdateHazards()
	{
		if (Barrels.Count != 0)
		{
			hazardTimer -= Time.fixedDeltaTime;
			if (hazardTimer < 0f)
			{
				hazardTimer = UnityEngine.Random.Range(1f, 3f);
				GameObject gameObject = Barrels.Pop();
				gameObject.transform.parent = GameController.Instance.transform;
				gameObject.transform.position = Gfx.transform.position + UnityEngine.Random.Range(-1f, 1f) * Vector3.right;
				gameObject.SetActive(true);
				UnityEngine.Object.Destroy(gameObject, 8f);
			}
		}
	}

	public void Reset()
	{
		hazardTimer = 1.5f;
		if (Barrels != null && Application.isPlaying)
		{
			while (Barrels.Count < 20)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(Barrel);
				gameObject.transform.parent = Barrel.transform.parent;
				gameObject.SetActive(false);
				Barrels.Push(gameObject);
			}
		}
		mCurrentSegment = null;
		mCurrentSegmentF = 0f;
		mCurrentTF = initialTF;
		base.Transform.position = Spline.Interpolate(mCurrentTF);
		if (SetOrientation)
		{
			orientate();
		}
	}
}
