using UnityEngine;

namespace FluffyUnderware.Curvy.Examples
{
	public class UserValues : MonoBehaviour
	{
		private FollowSpline walkerScript;

		private Material mMat;

		private void Start()
		{
			walkerScript = GetComponent<FollowSpline>();
			mMat = GetComponent<Renderer>().material;
		}

		private void Update()
		{
			if ((bool)walkerScript && walkerScript.Spline.IsInitialized)
			{
				base.transform.localScale = walkerScript.Spline.InterpolateScale(walkerScript.CurrentTF);
				mMat.color = Vector3ToColor(walkerScript.Spline.InterpolateUserValue(walkerScript.CurrentTF, 0));
			}
		}

		private Color Vector3ToColor(Vector3 v)
		{
			return new Color(v.x, v.y, v.z);
		}
	}
}
