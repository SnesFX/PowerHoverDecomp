using UnityEngine;

public class ContinuosRotation : MonoBehaviour
{
	public bool rotateOnlyOnPlayerMove;

	public Vector3 rotationVector;

	private float RotatingSpeedMultiplier = 1f;

	private Vector3 lerpToVector;

	private float lerpTimer;

	private void Start()
	{
		lerpToVector = rotationVector;
		lerpTimer = 1f;
	}

	private void FixedUpdate()
	{
		if (!rotateOnlyOnPlayerMove || (rotateOnlyOnPlayerMove && GameController.Instance.State == GameController.GameState.Running))
		{
			base.transform.Rotate(rotationVector);
			if (lerpTimer < 1f)
			{
				lerpTimer += Time.fixedDeltaTime * RotatingSpeedMultiplier;
				rotationVector = Vector3.Lerp(rotationVector, lerpToVector, lerpTimer);
			}
		}
	}

	public void SetTargetRotation(Vector3 target, float overrideSpeed = 1f)
	{
		RotatingSpeedMultiplier = overrideSpeed;
		lerpToVector = target;
		lerpTimer = 0f;
	}
}
