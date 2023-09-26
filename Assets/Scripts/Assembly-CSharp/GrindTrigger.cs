using UnityEngine;

public class GrindTrigger : ResetObject
{
	public CurvySpline curve;

	public string specialName;

	public int JumpOffSpeedDiff;

	public float Speed = 35f;

	public float PlayerOffset;

	private HoverController hoverController;

	private bool used;

	private float usedTimer;

	private void Awake()
	{
		hoverController = Object.FindObjectOfType<HoverController>();
		used = false;
	}

	private void Update()
	{
		if (usedTimer > 0f)
		{
			usedTimer -= Time.deltaTime;
			if (usedTimer < 0f)
			{
				used = false;
			}
		}
	}

	private void OnEnable()
	{
		hoverController.grindControl.ClearTrigger(this);
	}

	private void OnTriggerEnter(Collider other)
	{
		SetOnTrigger(other);
	}

	private void OnTriggerStay(Collider other)
	{
		SetOnTrigger(other);
	}

	private void SetOnTrigger(Collider other)
	{
		if (!used && other.gameObject.CompareTag("Player"))
		{
			hoverController.SetOnRail(true, curve, this);
		}
	}

	public void SetUsed(float timeToWait = 0.5f)
	{
		used = true;
		usedTimer = timeToWait;
	}

	public override void Reset(bool isRewind)
	{
	}
}
