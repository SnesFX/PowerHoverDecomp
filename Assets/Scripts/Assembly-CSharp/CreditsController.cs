using UnityEngine;

public class CreditsController : MonoBehaviour
{
	public Vector3 moveVector = new Vector3(0f, 0.3f, 0f);

	public Transform creditsText;

	public float loopY;

	private SplineWalker walker;

	private float timer = -1f;

	private bool flip;

	private Vector3 creditsStartPosition;

	private float changeDirTarget = 5f;

	private void Start()
	{
		walker = Object.FindObjectOfType<SplineWalker>();
		creditsStartPosition = creditsText.transform.position;
	}

	private void FixedUpdate()
	{
		if (GameController.Instance.State == GameController.GameState.Running)
		{
			Vector3 vector = walker.hoverController.transform.position - walker.transform.position;
			timer += Time.fixedDeltaTime;
			if ((timer > 1f && !flip && vector.x > changeDirTarget) || (flip && vector.x < 0f - changeDirTarget))
			{
				timer = 0f;
				flip = !flip;
				changeDirTarget = 5f + Random.Range(-0.5f, 0.5f);
			}
			if (timer >= 0f)
			{
				UIController.Instance.leftPressed = flip;
				UIController.Instance.rightPressed = !flip;
			}
			else
			{
				UIController.Instance.leftPressed = false;
				UIController.Instance.rightPressed = false;
			}
			creditsText.Translate(moveVector);
			if (creditsText.position.y > loopY)
			{
				creditsText.transform.position = creditsStartPosition;
			}
		}
	}

	public void OpenURL(string url)
	{
		Application.OpenURL(url);
	}
}
