using UnityEngine;

public class DebugObjectEnabler : MonoBehaviour
{
	private void Start()
	{
		base.gameObject.SetActive(false);
	}
}
