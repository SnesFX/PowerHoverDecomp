using UnityEngine;

public class GhostBomb : MonoBehaviour
{
	public Animator anim;

	private HoverController hoverController;

	private float enableTimer;

	private bool checkDistance;

	private bool isWireBomb;

	private void Awake()
	{
		hoverController = Object.FindObjectOfType<HoverController>();
		isWireBomb = base.transform.GetComponentInParent<Collectable>().Type == Collectable.CollectableType.WireBomb;
	}

	public void SetCheckDistance()
	{
		checkDistance = true;
	}

	private void OnEnable()
	{
		if (!isWireBomb)
		{
			Object.Destroy(base.gameObject, 5f);
			checkDistance = true;
		}
		else
		{
			anim.Play("EmpBombExplosionStart", -1, 0f);
		}
	}

	private void FixedUpdate()
	{
		if (checkDistance)
		{
			enableTimer += Time.fixedDeltaTime;
			if (!(enableTimer > 0.01f))
			{
				return;
			}
			enableTimer = 0f;
			if (Vector3.Distance(base.transform.position, hoverController.transform.position) < 15f)
			{
				checkDistance = false;
				if (!isWireBomb)
				{
					enableTimer = -0.5f;
					anim.enabled = true;
					anim.Play("EmpBombExplosion", -1, 0f);
					Object.Destroy(base.gameObject, 2.4f);
				}
				else
				{
					enableTimer = -0.25f;
				}
			}
		}
		else if (enableTimer < 0f)
		{
			enableTimer += Time.fixedDeltaTime;
			if (enableTimer >= 0f && GameController.Instance.State == GameController.GameState.Running && Vector3.Distance(base.transform.position, hoverController.transform.position) < 4.2f)
			{
				hoverController.checkpointController.PlayerDie();
			}
		}
	}
}
