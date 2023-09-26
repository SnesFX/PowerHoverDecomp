using UnityEngine;

public class LookAtPlayerNew : MonoBehaviour
{
	private Transform player;

	private void Start()
	{
		player = Object.FindObjectOfType<HoverController>().transform;
	}

	private void Update()
	{
		base.transform.LookAt(player);
	}
}
