using UnityEngine;

public class AudioTrigger : ResetObject
{
	private AudioSource audioS;

	private void Start()
	{
		audioS = GetComponent<AudioSource>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player") && audioS != null)
		{
			audioS.Play();
		}
	}

	public override void Reset(bool isRewind)
	{
		GetComponent<Collider>().enabled = true;
	}
}
