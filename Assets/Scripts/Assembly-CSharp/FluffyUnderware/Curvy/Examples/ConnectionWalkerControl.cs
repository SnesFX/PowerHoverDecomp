using UnityEngine;

namespace FluffyUnderware.Curvy.Examples
{
	public class ConnectionWalkerControl : MonoBehaviour
	{
		public SplineWalkerCon Walker;

		private int mDirection;

		private int mPreferredTrack;

		private void Update()
		{
			if ((bool)Walker && (bool)Walker.Spline)
			{
				Walker.Clamping = (Walker.Spline.Closed ? CurvyClamping.Loop : CurvyClamping.Clamp);
			}
		}

		private void OnGUI()
		{
			if ((bool)Walker)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("Movement: ");
				mDirection = GUILayout.Toolbar(mDirection, new string[2] { "Forward", "Backward" });
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Follow Track: ");
				mPreferredTrack = GUILayout.Toolbar(mPreferredTrack, new string[3] { "Main", "Upper", "Lower" });
				GUILayout.EndHorizontal();
				Walker.Forward = mDirection == 0;
				switch (mPreferredTrack)
				{
				case 0:
					Walker.AdditionalTags = "MainTrack";
					break;
				case 1:
					Walker.AdditionalTags = "UpperTrack";
					break;
				case 2:
					Walker.AdditionalTags = "LowerTrack";
					break;
				}
				if (Walker.Spline.name == "Main")
				{
					Walker.MinTagMatches = 2;
				}
				else
				{
					Walker.MinTagMatches = 1;
				}
				GUILayout.Label("Current active Tags: " + string.Join(" ", Walker.ResultingTags));
			}
		}
	}
}
