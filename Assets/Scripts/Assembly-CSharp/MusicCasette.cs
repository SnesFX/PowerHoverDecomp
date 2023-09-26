using UnityEngine;

public class MusicCasette : MonoBehaviour
{
	public int musicId;

	public void InUse(bool inUse)
	{
		GetComponent<Collider>().enabled = !inUse;
		GetComponent<Renderer>().enabled = !inUse;
	}
}
