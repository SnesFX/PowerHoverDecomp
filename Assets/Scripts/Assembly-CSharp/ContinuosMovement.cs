using UnityEngine;

public class ContinuosMovement : MonoBehaviour
{
	public bool moveOnlyWhenPlayerMoves;

	public Vector3 moveVector;

	private float moveSpeedMultiplier = 1f;

	private Vector3 lerpToVector;

	private float lerpTimer;

	private void Start()
	{
		lerpToVector = moveVector;
		lerpTimer = 1f;
	}

	private void FixedUpdate()
	{
		if (!moveOnlyWhenPlayerMoves || (moveOnlyWhenPlayerMoves && GameController.Instance.State == GameController.GameState.Running))
		{
			base.transform.Translate(moveVector);
			if (lerpTimer < 1f)
			{
				lerpTimer += Time.fixedDeltaTime * moveSpeedMultiplier;
				moveVector = Vector3.Lerp(moveVector, lerpToVector, lerpTimer);
			}
		}
	}

	public void SetTargetMove(Vector3 target, float overrideSpeed = 1f)
	{
		moveSpeedMultiplier = overrideSpeed;
		lerpToVector = target;
		lerpTimer = 0f;
	}
}
