using UnityEngine;

public class JumpTrigger : MonoBehaviour
{
	public bool isOffTrigger;

	public JumpTrigger attachedTrigger;

	private bool used;

	private float usedTimer;

	private void Start()
	{
		used = false;
	}

	private void FixedUpdate()
	{
		if (usedTimer > 0f)
		{
			usedTimer -= Time.fixedDeltaTime;
			if (usedTimer < 0f)
			{
				used = false;
			}
		}
	}

	public void SetUsed()
	{
		used = true;
		usedTimer = 0.3f;
	}

	private void OnTriggerEnter(Collider other)
	{
		Triggering(other);
	}

	private void OnTriggerStay(Collider other)
	{
		Triggering(other);
	}

	private void Triggering(Collider other)
	{
		if (used || !other.gameObject.CompareTag("Player"))
		{
			return;
		}
		if (isOffTrigger)
		{
			attachedTrigger.SetUsed();
			return;
		}
		SetUsed();
		HoverController component = other.gameObject.transform.parent.GetComponent<HoverController>();
		if (component.PlayerState != PlayerState.InAir)
		{
			component.Jump("Jump");
		}
	}
}
