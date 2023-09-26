using UnityEngine;

public class BreakableTrigger : MonoBehaviour
{
	public Rigidbody[] BreakableGroup;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player") && BreakableGroup != null)
		{
			for (int i = 0; i < BreakableGroup.Length; i++)
			{
				BreakableGroup[i].isKinematic = false;
			}
		}
	}
}
