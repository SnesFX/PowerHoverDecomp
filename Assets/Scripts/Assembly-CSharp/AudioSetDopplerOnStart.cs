using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSetDopplerOnStart : MonoBehaviour
{
	public float dopplerValue = 1f;

	private void Start()
	{
		AudioSource[] components = GetComponents<AudioSource>();
		foreach (AudioSource audioSource in components)
		{
			audioSource.dopplerLevel = dopplerValue;
		}
	}
}
