using System.Collections;
using UnityEngine;

public class AnimationGroupTrigger : ResetObject
{
	public Color gizmoColor = new Color(0.5f, 0.2f, 0.2f, 0.75f);

	public string AnimationToPlay;

	public string StoppedAnimation = "Default";

	public float ShakeCameraAfterActivation;

	public Animator[] animatorObjects;

	public float ShakeCameraLength = 1f;

	public Vector3 ShakeCameraForce = new Vector3(1f, 1f, 0f);

	public AudioSource objAudio;

	public bool StopAnimationOnKill;

	public float AnimationTimeDiff;

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
			for (int i = 0; i < animatorObjects.Length; i++)
			{
				animatorObjects[i].speed = 0f;
				if (animatorObjects[i].GetComponent<AudioSource>() != null)
				{
					animatorObjects[i].GetComponent<AudioSource>().Stop();
				}
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
		if (activated || !other.gameObject.CompareTag("Player"))
		{
			return;
		}
		for (int i = 0; i < animatorObjects.Length; i++)
		{
			if (AnimationTimeDiff > 0f)
			{
				StartCoroutine(AnimateObjects(i, AnimationTimeDiff + AnimationTimeDiff * (float)i));
			}
			else
			{
				animatorObjects[i].Play(AnimationToPlay, -1, 0f);
			}
		}
		if (objAudio != null)
		{
			objAudio.Play();
		}
		coll.enabled = false;
		activated = true;
	}

	private IEnumerator AnimateObjects(int i, float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		animatorObjects[i].Play(AnimationToPlay, -1, 0f);
	}

	public override void Reset(bool isRewind)
	{
		for (int i = 0; i < animatorObjects.Length; i++)
		{
			if (StoppedAnimation.Equals(string.Empty))
			{
				animatorObjects[i].StopPlayback();
				continue;
			}
			animatorObjects[i].Play(StoppedAnimation, -1, 0f);
			animatorObjects[i].speed = 1f;
		}
		coll.enabled = true;
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
