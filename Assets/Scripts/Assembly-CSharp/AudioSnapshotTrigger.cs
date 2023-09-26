using UnityEngine;
using UnityEngine.Audio;

public class AudioSnapshotTrigger : ResetObject
{
	public Color gizmoColor = new Color(0.2f, 0.4f, 0.5f, 0.75f);

	public AudioMixerSnapshot triggerSnapshot;

	public float transitionTimer;

	private Collider coll;

	private void Start()
	{
		coll = GetComponent<Collider>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			triggerSnapshot.TransitionTo(transitionTimer);
			coll.enabled = false;
		}
	}

	public override void Reset(bool isRewind)
	{
		coll.enabled = true;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = gizmoColor;
		if (GetComponent<BoxCollider>() != null)
		{
			BoxCollider component = GetComponent<BoxCollider>();
			if (component.enabled)
			{
				Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, base.transform.lossyScale);
				Gizmos.DrawCube(component.center, component.size);
			}
		}
	}
}
