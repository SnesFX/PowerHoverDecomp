using UnityEngine;

public class CloneActivator : GroundGizmos
{
	public Color gizmoColor = new Color(0f, 0.8f, 0.4f, 0.75f);

	private CloneRandomPart[] activators;

	public void SetRandomParts(CloneRandomPart[] parts)
	{
		activators = parts;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player") && activators != null)
		{
			for (int i = 0; i < activators.Length; i++)
			{
				activators[i].gameObject.SetActive(true);
			}
		}
	}
}
