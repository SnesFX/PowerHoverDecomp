using UnityEngine;

public class Laser : MonoBehaviour
{
	public GameObject laserLine;

	public GameObject endEffect;

	public bool isStatic;

	public float laserLenght = 60f;

	private LayerMask workMask;

	private Vector3 laserScale = new Vector3(1f, 1f, 1f);

	private Vector3 hitPointPos;

	private float distanceToHit;

	private RaycastHit hitProperty;

	private float checkTimer;

	private bool disabled;

	private void Start()
	{
		workMask = LayerMask.GetMask("Ground", "Level", "Default");
		checkTimer = Random.Range(0f, 0.1f);
		if (endEffect.GetComponent<AudioSource>() != null && endEffect.gameObject.activeSelf)
		{
			endEffect.GetComponent<AudioSource>().Play();
		}
	}

	private void OnEnable()
	{
		disabled = false;
	}

	private void FixedUpdate()
	{
		if (disabled)
		{
			return;
		}
		checkTimer += Time.fixedDeltaTime;
		if (!(checkTimer > 0.01f))
		{
			return;
		}
		checkTimer = 0f;
		if (Physics.Raycast(base.transform.position, -base.transform.up, out hitProperty, laserLenght, workMask.value))
		{
			if (hitProperty.point != endEffect.transform.position)
			{
				Updatelaser();
			}
		}
		else if (endEffect.activeSelf)
		{
			endEffect.SetActive(false);
		}
	}

	private void Updatelaser()
	{
		if (!endEffect.activeSelf)
		{
			endEffect.SetActive(true);
		}
		laserScale.y = hitProperty.distance;
		laserLine.transform.localScale = laserScale;
		endEffect.transform.position = hitProperty.point;
		endEffect.transform.up = hitProperty.normal;
		if (isStatic)
		{
			disabled = true;
		}
	}
}
