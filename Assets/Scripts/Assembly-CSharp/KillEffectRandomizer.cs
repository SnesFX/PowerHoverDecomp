using UnityEngine;

public class KillEffectRandomizer : MonoBehaviour
{
	private const float DisableTime = 6f;

	private const float ForceMultiplier = 2.5f;

	public GameObject commonRandom;

	public GameObject rareRandom;

	public GameObject rareRandom2;

	public GameObject superRareRandom3;

	public GameObject superRareRandom2;

	public GameObject superRareRandom;

	private float disableTimer;

	private BoxCollider[] colliders;

	private Rigidbody[] rigidbodies;

	private bool collidersDisabled;

	private bool rigidbodiesDisabled;

	public void MakeKillEffect(SplineWalker walker)
	{
		disableTimer = 6f;
		GameObject gameObject = RandomizeObject();
		colliders = gameObject.GetComponentsInChildren<BoxCollider>(true);
		rigidbodies = gameObject.GetComponentsInChildren<Rigidbody>(true);
		if (colliders != null)
		{
			for (int i = 0; i < colliders.Length; i++)
			{
				colliders[i].transform.localScale = colliders[i].transform.localScale * Random.Range(0.65f, 1f);
			}
		}
		if (rigidbodies != null)
		{
			Vector3 zero = Vector3.zero;
			for (int j = 0; j < rigidbodies.Length; j++)
			{
				zero.y = Random.Range(2f, 4f);
				zero.x = Random.Range(-2f, 2f);
				zero.z = Random.Range(-2f, 2f);
				rigidbodies[j].AddForce(zero * 2.5f, ForceMode.Impulse);
			}
		}
	}

	private GameObject RandomizeObject()
	{
		float value = Random.value;
		if (value > 0.5f && value < 0.8f)
		{
			if (value < 0.65f)
			{
				rareRandom.SetActive(true);
				return rareRandom;
			}
			rareRandom2.SetActive(true);
			return rareRandom;
		}
		if (value >= 0.8f)
		{
			if (value > 0.97f)
			{
				superRareRandom.SetActive(true);
				return superRareRandom;
			}
			if (value > 0.87f)
			{
				superRareRandom3.SetActive(true);
				return superRareRandom3;
			}
			superRareRandom2.SetActive(true);
			return superRareRandom2;
		}
		commonRandom.SetActive(true);
		return commonRandom;
	}

	private void Update()
	{
		if (!(disableTimer > 0f))
		{
			return;
		}
		disableTimer -= Time.deltaTime;
		if (disableTimer <= 0f)
		{
			Object.Destroy(base.gameObject);
		}
		else if (!collidersDisabled && disableTimer < 1f)
		{
			collidersDisabled = true;
			for (int i = 0; i < colliders.Length; i++)
			{
				colliders[i].enabled = false;
			}
			for (int j = 0; j < rigidbodies.Length; j++)
			{
				rigidbodies[j].WakeUp();
			}
		}
	}
}
