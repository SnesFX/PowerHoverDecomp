using System.Collections;
using UnityEngine;

namespace FluffyUnderware.Curvy.Examples
{
	[ExecuteInEditMode]
	public class EndlessRunnerPlayer : MonoBehaviour
	{
		public float Speed = 8f;

		public float SwitchSpeed = 2f;

		public float Gravity = 6f;

		public float JumpTime = 1f;

		public AnimationCurve JumpCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0.3f);

		public CurvySplineBase Spline;

		public float TF;

		private Transform mTransform;

		private bool mInAir;

		private float mJumpDelta;

		private float mStartTF;

		private Vector3 mLastPosOnSpline;

		private const int LAYER_LANE = 21;

		private bool Jumping
		{
			get
			{
				return mJumpDelta > 0f;
			}
		}

		private void Awake()
		{
			mTransform = base.transform;
			mStartTF = TF;
		}

		private IEnumerator Start()
		{
			if ((bool)Spline)
			{
				while (!Spline.IsInitialized)
				{
					yield return 0;
				}
				StartOver();
			}
		}

		private void Update()
		{
			if (!Spline || !Spline.IsInitialized)
			{
				return;
			}
			if (!Application.isPlaying)
			{
				if (Spline.Interpolate(TF) != mTransform.position)
				{
					mTransform.position = Spline.Interpolate(TF);
				}
				return;
			}
			int direction = 1;
			Vector3 vector = Spline.MoveBy(ref TF, ref direction, Speed * Time.deltaTime, CurvyClamping.PingPong);
			Vector3 tangent = Spline.GetTangent(TF);
			mTransform.position += vector - mLastPosOnSpline;
			if (Input.GetButtonDown("Horizontal") && TrySwitchLane((!(Input.GetAxis("Horizontal") < 0f)) ? Vector3.right : Vector3.left))
			{
				vector = Spline.Interpolate(TF);
			}
			if (!mInAir && !Jumping && Input.GetKey(KeyCode.Space))
			{
				StartCoroutine(Jump());
			}
			mTransform.forward = tangent;
			if (direction != 1)
			{
				CurvySplineBase newSpline;
				float newTF;
				if (RaycastForFollowUpSpline(Vector3.down, out newSpline, out newTF))
				{
					Spline = newSpline;
					TF = newTF;
					vector = Spline.Interpolate(TF);
					mInAir = (vector - mTransform.position).sqrMagnitude > 0.001f;
				}
				else
				{
					StartCoroutine(Die());
				}
			}
			if (!Jumping)
			{
				Vector3 vector2 = mTransform.position - vector;
				if (vector2.sqrMagnitude > 0.001f)
				{
					if (mInAir)
					{
						mTransform.position -= vector2.normalized * Gravity * Time.deltaTime;
					}
					else
					{
						mTransform.position -= vector2.normalized * SwitchSpeed * Time.deltaTime;
					}
				}
				else
				{
					mInAir = false;
				}
			}
			else
			{
				mTransform.Translate(0f, mJumpDelta * Time.deltaTime, 0f, Space.Self);
			}
			mLastPosOnSpline = vector;
		}

		private void OnTriggerEnter(Collider other)
		{
			StartCoroutine(Die());
		}

		private void StartOver()
		{
			TF = mStartTF;
			mTransform.position = Spline.Interpolate(TF);
			mLastPosOnSpline = mTransform.position;
		}

		private IEnumerator Die()
		{
			float tmp = Speed;
			Speed = 0f;
			yield return new WaitForSeconds(1f);
			Speed = tmp;
			StartOver();
		}

		private IEnumerator Jump()
		{
			mInAir = true;
			float tend = Time.time + JumpTime;
			while (Time.time <= tend)
			{
				mJumpDelta = JumpCurve.Evaluate(Time.time / tend);
				yield return new WaitForEndOfFrame();
			}
			mJumpDelta = 0f;
		}

		private bool TrySwitchLane(Vector3 dir)
		{
			CurvySplineBase newSpline;
			float newTF;
			if (RaycastForFollowUpSpline(dir, out newSpline, out newTF))
			{
				Spline = newSpline;
				TF = newTF;
				return true;
			}
			return false;
		}

		private bool RaycastForFollowUpSpline(Vector3 dir, out CurvySplineBase newSpline, out float newTF)
		{
			newSpline = null;
			newTF = 0f;
			Ray ray = new Ray(mTransform.position, dir);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 21f))
			{
				newSpline = hitInfo.collider.transform.parent.GetComponent<CurvySplineBase>();
				if ((bool)newSpline)
				{
					newTF = newSpline.GetNearestPointTF(hitInfo.point);
					return true;
				}
			}
			return false;
		}
	}
}
