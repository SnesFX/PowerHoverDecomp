using UnityEngine;

public class TargetIndicator : ResetObject
{
	private const string INDICATE_ANIM = "TargetIndicatorTargetting";

	public GameObject targetObject;

	public AudioClip onTargetAudio;

	public Material defaultMaterial;

	private Vector3 startPosition;

	private Animator indicatorAnimator;

	private AudioSource audioSource;

	private bool Targetting;

	private Material targettedMaterial;

	private Renderer targerRenderer;

	private bool lastTargetValue;

	private PingPongMove autoMover;

	private Vector3 tempMoveSpeed;

	public bool IsOnTarget { get; set; }

	private void Start()
	{
		startPosition = base.transform.position;
		audioSource = GetComponent<AudioSource>();
		indicatorAnimator = GetComponent<Animator>();
		autoMover = targetObject.GetComponent<PingPongMove>();
		autoMover.speed = 150f;
		autoMover.randomStart = true;
		targerRenderer = targetObject.GetComponentInChildren<Renderer>();
		targettedMaterial = targerRenderer.sharedMaterial;
		targetObject.SetActive(false);
	}

	private void Update()
	{
		if (!Targetting)
		{
			return;
		}
		if (UIController.Instance.leftPressed && UIController.Instance.rightPressed)
		{
			autoMover.speed += Time.deltaTime * (150f / (0.5f * (float)TrickController.Instance.TargetingLevel));
			targetObject.SetActive(true);
			autoMover.enabled = true;
			lastTargetValue = IsOnTarget;
			IsOnTarget = Vector3.Distance(Vector3.zero, targetObject.transform.localPosition) < 2.5f;
			if (lastTargetValue != IsOnTarget)
			{
				if (IsOnTarget)
				{
					targerRenderer.material = targettedMaterial;
					audioSource.pitch = 1f;
					audioSource.PlayOneShot(onTargetAudio, 1f);
				}
				else
				{
					targerRenderer.material = defaultMaterial;
				}
			}
		}
		else
		{
			autoMover.enabled = false;
		}
	}

	public void StartTargeting(Transform target)
	{
		base.transform.position = target.position;
		base.transform.rotation = target.rotation;
		base.transform.parent = target;
		targetObject.SetActive(false);
		indicatorAnimator.Play("TargetIndicatorTargetting", -1, 0f);
		float num = (float)(3 + TrickController.Instance.TargetingLevel) * 0.25f;
		tempMoveSpeed.z = Random.Range(6f, 12f) / num;
		tempMoveSpeed.y = Random.Range(7f, 10f) / num;
		Vector3 localPosition = autoMover.transform.localPosition;
		localPosition.y = (0f - tempMoveSpeed.y) * 0.5f;
		localPosition.z = (0f - tempMoveSpeed.z) * 0.5f;
		targerRenderer.material = defaultMaterial;
		autoMover.transform.localPosition = localPosition;
		autoMover.SetMove(tempMoveSpeed.magnitude * (Random.Range(2f, 5f) / num), tempMoveSpeed);
		audioSource.pitch = 1.3f;
		audioSource.Play();
		IsOnTarget = false;
		Targetting = true;
	}

	public override void Reset(bool isRewind)
	{
		base.transform.position = startPosition;
		base.transform.parent = GameController.Instance.transform;
		targetObject.SetActive(false);
		Targetting = false;
		IsOnTarget = false;
	}
}
