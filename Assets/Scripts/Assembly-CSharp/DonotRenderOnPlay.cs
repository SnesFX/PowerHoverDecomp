using UnityEngine;

public class DonotRenderOnPlay : MonoBehaviour
{
	private void Start()
	{
		if (GetComponent<MeshRenderer>() != null)
		{
			GetComponent<MeshRenderer>().enabled = false;
		}
	}
}
