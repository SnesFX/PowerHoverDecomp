using UnityEngine;

public class WarningAreaTrigger : MonoBehaviour
{
	private bool activate;

	public GameObject gfx;

	private void Update()
	{
		if (activate)
		{
			gfx.SetActive(Mathf.Sin(Time.time * 15f) > 0f);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<Collider>().gameObject.CompareTag("Player"))
		{
			activate = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.GetComponent<Collider>().gameObject.CompareTag("Player"))
		{
			activate = false;
			gfx.SetActive(false);
		}
	}
}
