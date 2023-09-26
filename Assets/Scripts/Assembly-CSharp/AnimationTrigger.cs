using UnityEngine;

public class AnimationTrigger : ResetObject
{
	public Color gizmoColor = new Color(0.4f, 0.1f, 0.1f, 0.75f);

	public string AnimationToPlay;

	public string StoppedAnimation = "Default";

	public float ShakeCameraAfterActivation;

	public Animator animatorObj;

	public float ShakeCameraLength = 1f;

	public Vector3 ShakeCameraForce = new Vector3(1f, 1f, 0f);

	public AudioSource objAudio;

	public bool StopAnimationOnKill;

	public AnimationTriggerAnimationSwapper animationSwapper;

	private float shakeCameraTime;

	private Collider coll;

	public bool activated { get; private set; }

	private void Start()
	{
		shakeCameraTime = ShakeCameraAfterActivation;
		coll = GetComponent<Collider>();
	}

	private void FixedUpdate()
	{
		if (!activated)
		{
			return;
		}
		if (StopAnimationOnKill && GameController.Instance.State == GameController.GameState.Kill)
		{
			animatorObj.speed = 0f;
			if (animatorObj.GetComponent<AudioSource>() != null)
			{
				animatorObj.GetComponent<AudioSource>().Stop();
			}
			if (objAudio != null)
			{
				objAudio.Stop();
			}
			activated = false;
		}
		else if (ShakeCameraAfterActivation > 0f)
		{
			ShakeCameraAfterActivation -= Time.fixedDeltaTime;
			if (ShakeCameraAfterActivation <= 0f && Camera.main.GetComponent<CameraShake>() != null)
			{
				Camera.main.GetComponent<CameraShake>().StartShake(ShakeCameraLength, ShakeCameraForce);
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!activated && other.gameObject.CompareTag("Player"))
		{
			animatorObj.Play(AnimationToPlay, -1, 0f);
			ActivateTrigger();
		}
	}

	public void PlayActiveAnimation()
	{
		animatorObj.Play(AnimationToPlay, -1, 0f);
	}

	public void ActivateTrigger()
	{
		if (animationSwapper != null)
		{
			animationSwapper.Swap();
		}
		if (objAudio != null)
		{
			objAudio.Play();
		}
		if (coll != null)
		{
			coll.enabled = false;
		}
		activated = true;
	}

	public override void Reset(bool isRewind)
	{
		if (StoppedAnimation.Equals(string.Empty))
		{
			animatorObj.StopPlayback();
		}
		else
		{
			animatorObj.Play(StoppedAnimation, -1, 0f);
			animatorObj.speed = 1f;
		}
		if (coll != null)
		{
			coll.enabled = true;
		}
		activated = false;
		ShakeCameraAfterActivation = shakeCameraTime;
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
