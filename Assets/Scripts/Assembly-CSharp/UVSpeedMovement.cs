using UnityEngine;

public class UVSpeedMovement : MonoBehaviour
{
	public UVrotation uvMover;

	public float yMultiplier;

	private SplineWalker walker;

	private void Start()
	{
		walker = Object.FindObjectOfType<SplineWalker>();
	}

	private void FixedUpdate()
	{
		if (GameController.Instance.State == GameController.GameState.Running)
		{
			uvMover.Yspeed = walker.Speed * yMultiplier / 30f;
		}
		else
		{
			uvMover.Yspeed = 0f;
		}
	}
}
