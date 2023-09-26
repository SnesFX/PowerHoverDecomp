using System.Collections;
using UnityEngine;

namespace FluffyUnderware.Curvy.Examples
{
	public class MouseAddControlPoint : MonoBehaviour
	{
		public bool RemoveUnusedSegments = true;

		private CurvySpline mSpline;

		private FollowSpline Walker;

		private IEnumerator Start()
		{
			mSpline = GetComponent<CurvySpline>();
			Walker = Object.FindObjectOfType(typeof(FollowSpline)) as FollowSpline;
			while (!mSpline.IsInitialized)
			{
				yield return null;
			}
		}

		private void Update()
		{
			if (!Input.GetMouseButtonDown(0))
			{
				return;
			}
			Vector3 mousePosition = Input.mousePosition;
			mousePosition.z = 10f;
			mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
			mSpline.Add(mousePosition);
			if (!RemoveUnusedSegments)
			{
				return;
			}
			CurvySplineSegment currentSegment = Walker.CurrentSegment;
			if (!currentSegment)
			{
				return;
			}
			int controlPointIndex = currentSegment.ControlPointIndex;
			int num = controlPointIndex - 2;
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					mSpline.Delete(mSpline.ControlPoints[0], false);
				}
				mSpline.RefreshImmediately();
			}
		}
	}
}
