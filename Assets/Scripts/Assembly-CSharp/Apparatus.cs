using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluffyUnderware.Curvy;
using FluffyUnderware.Curvy.Utils;
using Holoville.HOTween;
using UnityEngine;

public class Apparatus : CurvyComponent
{
	public enum DroppingState
	{
		Normal = 0,
		Fixed = 1,
		Singles = 2
	}

	private enum ApparatusState
	{
		Rotating = 0,
		Droping = 1,
		Waiting = 2,
		LazerRotating = 3
	}

	public enum FollowMode
	{
		Relative = 0,
		AbsoluteExtrapolate = 1,
		AbsolutePrecise = 2
	}

	protected delegate void Action();

	public bool RandomGroupsEnabled;

	public float InitialF;

	public ApparatusLazer[] Lazers;

	public ContinuosRotation LazerRotator;

	public Material ActivatedMaterial;

	public Material DisabledMaterial;

	public Material RotatingMaterial;

	public AudioClip DropSound;

	public AudioClip ActivateSound;

	public AudioClip DisableSound;

	private ApparatusState State;

	private DroppingState DropState;

	private DroppingState LastState;

	private float hazardTimer = 5f;

	private int hazardSerieCounter;

	private int hazardSerieCounterPrevious;

	private int hazardFixedSerieCounter;

	private int hazardSingleDirecton;

	private bool randomRotationEnabled;

	private Vector3 randomRotation;

	private ApparatusLazer hazardLazer;

	private Material NormalMaterial;

	private SplineWalker walker;

	[SerializeField]
	[Label(Tooltip = "The Spline or Splinegroup to use")]
	private CurvySplineBase m_Spline;

	[Label(Tooltip = "Interpolate cached values?")]
	public bool FastInterpolation;

	[SerializeField]
	[Label(Tooltip = "Movement Mode")]
	private FollowMode m_Mode = FollowMode.AbsolutePrecise;

	[SerializeField]
	private CurvyVector m_Initial = new CurvyVector();

	[Positive(Tooltip = "Speed in F or World Units (depending on Mode)")]
	public float Speed;

	[Label(Tooltip = "End of Spline Behaviour")]
	public CurvyClamping Clamping;

	[Label(Tooltip = "Align to Up-Vector?")]
	public bool SetOrientation = true;

	[SerializeField]
	[Label(Tooltip = "Enable if you plan to add/remove segments during movement")]
	private bool m_Dynamic;

	private bool mIsInitialized;

	private CurvyVector Current;

	private WeakReference mCurrentSeg;

	protected float mCurrentSegmentF = -1f;

	protected float mCurrentTF;

	protected Action UpdateAction;

	private List<ApparatusLazer> listRandom;

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
		Speed = walker.EndlessSpeed;
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
		FollowMode mode = Mode;
		if (mode == FollowMode.AbsolutePrecise)
		{
			UpdateAction = UpdateAbsolutePrecise;
			Initial.Absolute(Spline.Length);
			Current = new CurvyVector(Initial);
			mCurrentTF = Spline.DistanceToTF(Current.m_Position);
		}
		Reset();
		base.Transform.position = Spline.Interpolate(mCurrentTF);
		if (SetOrientation)
		{
			orientate();
		}
		return true;
	}

	protected void UpdateAbsolutePrecise()
	{
		base.Transform.position = Spline.MoveByLengthFast(ref mCurrentTF, ref Current.m_Direction, Speed * CurvyUtility.DeltaTime, Clamping);
		if (SetOrientation)
		{
			orientate();
		}
	}

	private void orientate()
	{
		base.Transform.rotation = Spline.GetOrientationFast(mCurrentTF);
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
		hazardTimer -= Time.fixedDeltaTime;
		switch (State)
		{
		case ApparatusState.Waiting:
			if (hazardTimer <= 0f)
			{
				UnityEngine.Random.seed = (int)DateTime.Now.Ticks;
				LazerRotator.SetTargetRotation(UnityEngine.Random.Range(1f, 2.5f + Mathf.Min(1f, walker.TotalDistance / 10000f)) * ((UnityEngine.Random.Range(0, 2) != 0) ? Vector3.right : Vector3.left), 0.15f);
				Vector3 localPosition2 = LazerRotator.transform.localPosition;
				localPosition2.y += 1.5f;
				HOTween.Shake(LazerRotator.transform, 0.6f, new TweenParms().Prop("localPosition", localPosition2).UpdateType(UpdateType.TimeScaleIndependentUpdate));
				hazardTimer = 3f;
				State = ApparatusState.Rotating;
				hazardLazer = RandomizeLazer();
				PlayLazerAnimator(hazardLazer.gameObject, hazardLazer.ANIM_APPARATUS_ON);
				GetComponent<AudioSource>().PlayOneShot(ActivateSound);
			}
			break;
		case ApparatusState.Rotating:
		{
			if (!(hazardTimer <= 0f))
			{
				break;
			}
			DropState = (DroppingState)UnityEngine.Random.Range(0, 3);
			DropState = RandomizeDropState(hazardLazer.GetComponent<ApparatusLazer>());
			LastState = DropState;
			switch (DropState)
			{
			case DroppingState.Normal:
				hazardSerieCounter = UnityEngine.Random.Range(8, 18);
				break;
			case DroppingState.Fixed:
				hazardFixedSerieCounter = UnityEngine.Random.Range(2, 5);
				hazardSerieCounter = UnityEngine.Random.Range(3, 7);
				hazardSerieCounterPrevious = hazardSerieCounter;
				break;
			case DroppingState.Singles:
				if (hazardLazer.Type != 0)
				{
					hazardFixedSerieCounter = ((hazardLazer.Type != ApparatusLazer.LaserType.Probel) ? 1 : UnityEngine.Random.Range(1, 4));
					hazardSerieCounter = 1;
					hazardSerieCounterPrevious = hazardSerieCounter;
					hazardSingleDirecton = UnityEngine.Random.Range(0, 2);
				}
				else
				{
					hazardFixedSerieCounter = 4;
					hazardSerieCounter = 2;
					hazardSerieCounterPrevious = hazardSerieCounter;
					hazardSingleDirecton = UnityEngine.Random.Range(0, 2);
				}
				break;
			}
			int num2 = UnityEngine.Random.Range(-1, 2);
			if (UnityEngine.Random.Range(0f, walker.TotalDistance / 1000f) > 1f && num2 != 0)
			{
				randomRotationEnabled = true;
				randomRotation = new Vector3(UnityEngine.Random.Range(0.25f, 1.5f + Mathf.Min(1f, walker.TotalDistance / 10000f)) * (float)num2, 0f, 0f);
			}
			else
			{
				randomRotationEnabled = false;
			}
			if (hazardLazer.ChangeActivatedMaterial)
			{
				SwapMaterial(hazardLazer.gameObject, ActivatedMaterial);
			}
			State = ApparatusState.Droping;
			hazardTimer = 0.2f;
			break;
		}
		case ApparatusState.Droping:
			if (!(hazardTimer < 0f))
			{
				break;
			}
			switch (DropState)
			{
			case DroppingState.Normal:
			{
				hazardTimer = 0.2f;
				GameObject gameObject = UnityEngine.Object.Instantiate(hazardLazer.gameObject, hazardLazer.transform.position, hazardLazer.transform.rotation);
				UnityEngine.Object.Destroy(gameObject.GetComponent<Animator>());
				gameObject.transform.parent = GameController.Instance.transform;
				gameObject.transform.localScale = hazardLazer.transform.lossyScale;
				if (LastState != 0 && hazardLazer.CanBeRotated && randomRotationEnabled)
				{
					ContinuosRotation continuosRotation = gameObject.AddComponent<ContinuosRotation>();
					continuosRotation.rotationVector = randomRotation;
					SwapMaterial(gameObject, RotatingMaterial);
				}
				GetComponent<AudioSource>().PlayOneShot(DropSound);
				UnityEngine.Object.Destroy(gameObject, 5f);
				hazardSerieCounter--;
				if (hazardSerieCounter != 0)
				{
					break;
				}
				if (hazardFixedSerieCounter > 0)
				{
					DropState = LastState;
					switch (DropState)
					{
					case DroppingState.Fixed:
						LazerRotator.SetTargetRotation(3f * ((UnityEngine.Random.Range(0, 2) != 0) ? Vector3.right : Vector3.left), 0.15f);
						hazardTimer = ((!(UnityEngine.Random.Range(0f, 100f) < 50f)) ? 0.5f : 1f);
						hazardSerieCounter = hazardSerieCounterPrevious;
						break;
					case DroppingState.Singles:
						LazerRotator.SetTargetRotation(3f * ((hazardSingleDirecton != 0) ? Vector3.right : Vector3.left), 4f);
						hazardTimer = 0.5f;
						hazardSerieCounter = hazardSerieCounterPrevious;
						break;
					}
				}
				else
				{
					randomRotationEnabled = false;
					hazardTimer = Mathf.Max(1f, UnityEngine.Random.Range(3f, 5f) - walker.TotalDistance / 1000f);
					LazerRotator.SetTargetRotation(Vector3.zero, 0.01f);
					State = ApparatusState.Waiting;
					if (hazardLazer.ChangeActivatedMaterial)
					{
						SwapMaterial(hazardLazer.gameObject, DisabledMaterial);
					}
					PlayLazerAnimator(hazardLazer.gameObject, hazardLazer.ANIM_APPARATUS_OFF);
					GetComponent<AudioSource>().PlayOneShot(DisableSound);
					Vector3 localPosition = LazerRotator.transform.localPosition;
					localPosition.y -= 1.5f;
					HOTween.Shake(LazerRotator.transform, 0.6f, new TweenParms().Prop("localPosition", localPosition).UpdateType(UpdateType.TimeScaleIndependentUpdate));
				}
				break;
			}
			case DroppingState.Singles:
			{
				hazardTimer = 0.04f;
				float num = Mathf.Round(LazerRotator.transform.localRotation.eulerAngles.x / 45f) * 45f;
				if (Mathf.Abs(LazerRotator.transform.localRotation.eulerAngles.x - num) < 5f)
				{
					LazerRotator.SetTargetRotation(Vector3.zero, 50f);
					DropState = DroppingState.Normal;
					hazardFixedSerieCounter--;
				}
				break;
			}
			case DroppingState.Fixed:
			{
				hazardTimer = 0.04f;
				float num = Mathf.Round(LazerRotator.transform.localRotation.eulerAngles.x / 90f) * 90f;
				if (Mathf.Abs(LazerRotator.transform.localRotation.eulerAngles.x - num) < 5f)
				{
					LazerRotator.SetTargetRotation(Vector3.zero, 50f);
					DropState = DroppingState.Normal;
					hazardFixedSerieCounter--;
				}
				break;
			}
			}
			break;
		}
	}

	private ApparatusLazer RandomizeLazer()
	{
		if (listRandom == null)
		{
			listRandom = Lazers.ToList();
		}
		if (RandomGroupsEnabled)
		{
			float num = UnityEngine.Random.Range(0f, 1f);
			if (num < 0.5f)
			{
				listRandom = Lazers.Where((ApparatusLazer x) => x.RandomType == ApparatusLazer.RandomizeType.Common).ToList();
			}
			else if (num < 0.8f)
			{
				listRandom = Lazers.Where((ApparatusLazer x) => x.RandomType == ApparatusLazer.RandomizeType.Normal).ToList();
			}
			else
			{
				listRandom = Lazers.Where((ApparatusLazer x) => x.RandomType == ApparatusLazer.RandomizeType.Rare).ToList();
			}
			if (listRandom.Count == 0)
			{
				listRandom = Lazers.ToList();
			}
		}
		return listRandom[UnityEngine.Random.Range(0, listRandom.Count)];
	}

	private DroppingState RandomizeDropState(ApparatusLazer lazer)
	{
		List<DroppingState> list = new List<DroppingState>();
		list.Add(DroppingState.Fixed);
		list.Add(DroppingState.Normal);
		list.Add(DroppingState.Singles);
		DroppingState[] blacklistedDropTypes = lazer.BlacklistedDropTypes;
		foreach (DroppingState item in blacklistedDropTypes)
		{
			list.Remove(item);
		}
		if (list.Count == 0)
		{
			return DroppingState.Normal;
		}
		float num = UnityEngine.Random.Range(0f, 1f);
		float num2 = 1f / (float)list.Count;
		return list[Mathf.FloorToInt(num / num2)];
	}

	private void SwapMaterial(GameObject l, Material m)
	{
		MeshRenderer[] componentsInChildren = l.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			meshRenderer.material = m;
		}
		if (l.GetComponent<MeshRenderer>() != null)
		{
			l.GetComponent<MeshRenderer>().material = m;
		}
	}

	private void PlayLazerAnimator(GameObject l, string animName)
	{
		if (l.GetComponent<Animator>() != null)
		{
			l.GetComponent<Animator>().Play(animName);
		}
	}

	private void Reset()
	{
		ApparatusLazer[] lazers = Lazers;
		foreach (ApparatusLazer apparatusLazer in lazers)
		{
			PlayLazerAnimator(apparatusLazer.gameObject, apparatusLazer.ANIM_APPARATUS_DISABLED);
		}
		hazardSerieCounter = (hazardSerieCounterPrevious = (hazardFixedSerieCounter = 0));
		mCurrentSegment = null;
		mCurrentSegmentF = 0f;
		mCurrentTF = InitialF;
		hazardTimer = 1f;
		LazerRotator.rotationVector = Vector3.zero;
		State = ApparatusState.Waiting;
		base.Transform.position = Spline.Interpolate(mCurrentTF);
		if (SetOrientation)
		{
			orientate();
		}
	}
}
