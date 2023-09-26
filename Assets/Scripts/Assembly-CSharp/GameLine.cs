using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GameLine : ResetObject
{
	public enum LineType
	{
		Start = 0,
		End = 1,
		Checkpoint = 2
	}

	public Color gizmoColor = new Color(0.8f, 1f, 0.9f, 0.75f);

	public string ANIM_NAME_ACTIVATE = "CheckpointActivation";

	public string ANIM_NAME_NORMAL = "CheckpointNormal";

	public LineType type;

	public Material enteredMaterial;

	private Material startMaterial;

	private Renderer meshRenderer;

	public Animator CPlineAnimator;

	private InfoBox[] levelInfoBoxes;

	private AudioSource audioSource;

	private CheckPointContoller cpController;

	private Collider coll;

	private void Start()
	{
		if (GetComponent<MeshRenderer>() != null)
		{
			meshRenderer = GetComponent<MeshRenderer>();
			startMaterial = meshRenderer.material;
		}
		coll = GetComponent<Collider>();
		cpController = Object.FindObjectOfType<HoverController>().checkpointController;
		audioSource = GetComponent<AudioSource>();
		levelInfoBoxes = GameController.Instance.transform.GetComponentsInChildren<InfoBox>(true);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.gameObject.CompareTag("Player"))
		{
			return;
		}
		switch (type)
		{
		case LineType.Checkpoint:
			if (!SceneLoader.Instance.Current.IsEndless)
			{
				cpController.SaveOnCheckpoint();
			}
			if (CPlineAnimator != null && (bool)CPlineAnimator)
			{
				CPlineAnimator.Play(ANIM_NAME_ACTIVATE);
				audioSource.Play();
			}
			break;
		case LineType.End:
			if (CPlineAnimator != null && (bool)CPlineAnimator)
			{
				CPlineAnimator.Play(ANIM_NAME_ACTIVATE);
				audioSource.Play();
			}
			LevelStats.Instance.LevelCompleted();
			GameController.Instance.SetState(GameController.GameState.Ending);
			cpController.ResetBlock();
			GameController.Instance.SetTutorialAsRead();
			break;
		}
		if (meshRenderer != null && enteredMaterial != null)
		{
			meshRenderer.material = enteredMaterial;
		}
		coll.enabled = false;
	}

	public override void Reset(bool isRewind)
	{
		if (meshRenderer != null)
		{
			meshRenderer.material = startMaterial;
		}
		if (!isRewind && CPlineAnimator != null)
		{
			CPlineAnimator.Play(ANIM_NAME_NORMAL);
		}
		if (type != 0 && !isRewind)
		{
			coll.enabled = true;
		}
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
