using UnityEngine;

public class CopyPosition : MonoBehaviour
{
	public GameObject objectToCopyPos;

	private void Start()
	{
	}

	private void Update()
	{
		base.transform.localPosition = objectToCopyPos.transform.localPosition;
	}
}
