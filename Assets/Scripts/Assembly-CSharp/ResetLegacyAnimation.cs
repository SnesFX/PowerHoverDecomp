using UnityEngine;

public class ResetLegacyAnimation : ResetObject
{
	public Animation legacyAnimation;

	private bool activated;

	private void OnTriggerEnter(Collider other)
	{
		if (!activated && other.gameObject.CompareTag("Player"))
		{
			legacyAnimation.Rewind();
			legacyAnimation.Play();
		}
	}

	public override void Reset(bool isRewind)
	{
		legacyAnimation.Rewind();
		legacyAnimation.Stop();
		GetComponent<Collider>().enabled = true;
		activated = false;
	}
}
