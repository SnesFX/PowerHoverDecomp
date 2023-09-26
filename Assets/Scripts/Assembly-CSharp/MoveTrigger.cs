using UnityEngine;

public class MoveTrigger : ResetObject
{
	public Color gizmoColor = new Color(0.2f, 0.2f, 0.2f, 0.75f);

	public CurvySpline TargetSpline;

	public float TargetSplinePosition;

	private AudioSource audioS;

	private SplineWalker walker;

	private CameraFollowAnimation camFollow;

	private LayerMask workMask;

	private Tutorial tuto;

	private void Start()
	{
		workMask = LayerMask.GetMask("Ground", "Level");
		walker = Object.FindObjectOfType<SplineWalker>();
		camFollow = Object.FindObjectOfType<CameraFollowAnimation>();
		audioS = GetComponent<AudioSource>();
		tuto = Object.FindObjectOfType<Tutorial>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.gameObject.CompareTag("Player"))
		{
			return;
		}
		Quaternion rotation = camFollow.transform.rotation;
		Vector3 oldDistance = camFollow.transform.position - walker.transform.position;
		walker.hoverController.animationController.boardSmoke.Clear();
		walker.hoverController.animationController.boardParticles.Clear();
		walker.hoverController.animationController.DeleteTrail();
		walker.Spline = TargetSpline;
		walker.TF = TargetSplinePosition;
		walker.transform.position = TargetSpline.Interpolate(TargetSplinePosition);
		walker.transform.rotation = TargetSpline.GetOrientationFast(TargetSplinePosition);
		RaycastHit hitInfo;
		Physics.Raycast(walker.hoverController.transform.position, -walker.hoverController.transform.up, out hitInfo, 20f, workMask.value);
		if (Physics.Raycast(walker.hoverController.transform.position + walker.hoverController.transform.up * 2f, -walker.hoverController.transform.up, out hitInfo, 20f, workMask.value))
		{
			walker.hoverController.transform.position = (walker.hoverController.transform.position - hitInfo.point).normalized * 0.8f + hitInfo.point;
		}
		camFollow.MoveToTarget(oldDistance, rotation);
		if (audioS != null)
		{
			audioS.Play();
			if (tuto != null)
			{
				tuto.Failed();
			}
		}
	}

	public override void Reset(bool isRewind)
	{
		GetComponent<Collider>().enabled = true;
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
