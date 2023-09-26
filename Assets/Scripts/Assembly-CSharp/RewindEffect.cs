using UnityEngine;

public class RewindEffect : MonoBehaviour
{
	private void Start()
	{
		GetComponent<AudioSource>().playOnAwake = true;
	}
}
