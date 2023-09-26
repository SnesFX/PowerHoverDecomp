using UnityEngine;

public class Missile : ResetObject
{
	private const float GROUND_OFFSET = 5f;

	public GameObject missileObj;

	public Renderer groundRenderer;

	public GameObject particleObject;

	public ParticleSystem airParticles;

	public Animator anim;

	private LayerMask workMask;

	private float castTimer = 1f;

	private Vector3 hitpoint = Vector3.zero;

	private void Start()
	{
		workMask = LayerMask.GetMask("Ground", "Level");
		castTimer = 1f;
	}

	private void Update()
	{
		if (hitpoint == Vector3.zero)
		{
			castTimer -= Time.deltaTime;
			if (castTimer < 0f)
			{
				UpdateHitPosition();
			}
		}
		else
		{
			missileObj.transform.position = hitpoint;
		}
	}

	private void UpdateHitPosition()
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(missileObj.transform.position, missileObj.transform.up, out hitInfo, 5f, workMask.value))
		{
			anim.enabled = false;
			hitpoint = hitInfo.point + missileObj.transform.up * -3.6f * Mathf.Min(1f, base.transform.localScale.x);
			ParticleSystem.EmissionModule emission = airParticles.emission;
			emission.enabled = false;
			groundRenderer.transform.position = hitInfo.point + hitInfo.normal * 0.07f;
			groundRenderer.transform.up = hitInfo.normal;
			groundRenderer.enabled = true;
			particleObject.SetActive(true);
		}
	}

	public override void Reset(bool isRewind)
	{
		base.Reset(isRewind);
		if (base.gameObject.activeSelf)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
