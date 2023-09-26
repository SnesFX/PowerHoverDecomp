using UnityEngine;

namespace FluffyUnderware.Curvy.Examples
{
	public class FollowSplineExampleController : MonoBehaviour
	{
		public FollowSpline[] Controllers;

		private int selection;

		private void Start()
		{
			SetRelative();
		}

		private void OnGUI()
		{
			if (GUILayout.Button("Reset"))
			{
				FollowSpline[] controllers = Controllers;
				foreach (FollowSpline followSpline in controllers)
				{
					followSpline.Initialize();
				}
			}
			GUILayout.BeginHorizontal();
			GUILayout.Label("Movement Mode: ");
			int num = GUILayout.SelectionGrid(selection, new string[2] { "Relative", "Absolute" }, 2);
			GUILayout.EndHorizontal();
			if (num != selection)
			{
				selection = num;
				switch (selection)
				{
				case 0:
					SetRelative();
					break;
				case 1:
					SetAbsolute();
					break;
				}
			}
		}

		private void SetRelative()
		{
			FollowSpline[] controllers = Controllers;
			foreach (FollowSpline followSpline in controllers)
			{
				followSpline.Mode = FollowSpline.FollowMode.Relative;
				followSpline.Speed = 0.2f;
			}
		}

		private void SetAbsolute()
		{
			FollowSpline[] controllers = Controllers;
			foreach (FollowSpline followSpline in controllers)
			{
				followSpline.Mode = FollowSpline.FollowMode.AbsoluteExtrapolate;
				followSpline.Speed = 4f;
			}
		}
	}
}
