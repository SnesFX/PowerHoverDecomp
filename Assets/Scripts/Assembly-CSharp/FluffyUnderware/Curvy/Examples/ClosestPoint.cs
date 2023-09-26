using UnityEngine;

namespace FluffyUnderware.Curvy.Examples
{
	public class ClosestPoint : MonoBehaviour
	{
		public CurvySplineBase Target;

		public Transform TargetTransform;

		private void LateUpdate()
		{
			if ((bool)Target && Target.IsInitialized && (bool)TargetTransform)
			{
				float nearestPointTF = Target.GetNearestPointTF(base.transform.position);
				TargetTransform.position = Target.Interpolate(nearestPointTF);
			}
		}
	}
}
