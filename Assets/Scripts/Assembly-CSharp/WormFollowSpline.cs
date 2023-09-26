using System;
using System.Collections;
using FluffyUnderware.Curvy;
using FluffyUnderware.Curvy.Utils;
using UnityEngine;

public class WormFollowSpline : CurvyComponent
{
	public enum FollowMode
	{
		Relative = 0,
		AbsoluteExtrapolate = 1,
		AbsolutePrecise = 2
	}

	protected delegate void Action();

	[SerializeField]
	[Label(Tooltip = "The Spline or Splinegroup to use")]
	private CurvySplineBase m_Spline;

	[Label(Tooltip = "Interpolate cached values?")]
	public bool FastInterpolation;

	[SerializeField]
	[Label(Tooltip = "Movement Mode")]
	private FollowMode m_Mode;

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

	private bool Dynamic
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

	public bool IsTail { get; set; }

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
		UpdateIn = CurvyUpdateMethod.FixedUpdate;
		WormFollowSpline wormFollowSpline = this;
		WormFollowSpline wormFollowSpline2 = this;
		bool dynamic = false;
		wormFollowSpline2.m_Dynamic = false;
		wormFollowSpline.Dynamic = dynamic;
		if ((bool)Spline)
		{
			while (!SourceIsInitialized)
			{
				yield return 0;
			}
			Initialize();
		}
	}

	private void Awake()
	{
		ContinuosRotation componentInChildren = GetComponentInChildren<ContinuosRotation>();
		if (DeviceSettings.Instance != null && DeviceSettings.Instance.OptimizedWorms && componentInChildren != null)
		{
			componentInChildren.enabled = false;
		}
	}

	private void OnEnable()
	{
		if (!Application.isPlaying)
		{
			Initialize();
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
		UpdateIn = CurvyUpdateMethod.FixedUpdate;
		Dynamic = false;
		switch (Mode)
		{
		case FollowMode.AbsoluteExtrapolate:
			UpdateAction = UpdateAbsoluteExtrapolate;
			Initial.Absolute(Spline.Length);
			Current = new CurvyVector(Initial);
			mCurrentTF = Spline.DistanceToTF(Current.m_Position);
			break;
		case FollowMode.AbsolutePrecise:
			UpdateAction = UpdateAbsolutePrecise;
			Initial.Absolute(Spline.Length);
			Current = new CurvyVector(Initial);
			mCurrentTF = Spline.DistanceToTF(Current.m_Position);
			break;
		default:
			UpdateAction = UpdateRelative;
			Initial.Relative();
			Current = new CurvyVector(Initial);
			mCurrentTF = Current.Position;
			break;
		}
		initialTF = Initial.m_Position;
		Reset();
		return true;
	}

	protected void UpdateRelative()
	{
		if (FastInterpolation)
		{
			base.Transform.position = Spline.MoveFast(ref mCurrentTF, ref Current.m_Direction, Speed * CurvyUtility.DeltaTime, Clamping);
		}
		else
		{
			base.Transform.position = Spline.Move(ref mCurrentTF, ref Current.m_Direction, Speed * CurvyUtility.DeltaTime, Clamping);
		}
		if (SetOrientation)
		{
			orientate();
		}
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

	protected void UpdateAbsolutePrecise()
	{
		base.Transform.position = Spline.MoveByLengthFast(ref mCurrentTF, ref Current.m_Direction, Speed * CurvyUtility.DeltaTime, Clamping);
		if (SetOrientation)
		{
			orientate();
		}
		if ((IsTail && Current.m_Direction == 1 && mCurrentTF == 1f) || (Current.m_Direction == 0 && mCurrentTF == 0f))
		{
			base.transform.GetComponentInParent<SplineWormActivator>().Disable();
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
		if (!IsActive)
		{
			return;
		}
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

	public void Reset()
	{
		mCurrentSegment = null;
		mCurrentSegmentF = 0f;
		mCurrentTF = initialTF;
		if (Spline.enabled)
		{
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
}
