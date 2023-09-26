using UnityEngine;

public class GiantMiningBotWalkEffect : MonoBehaviour
{
	public GameObject effectPos;

	public float groundLevel = 15f;

	private Vector3 effectPosition;

	private bool effectOn;

	public GameObject Effect;

	private AudioSource stepSound;

	private GameObject Effectchild;

	private void Start()
	{
		Effectchild = Effect.transform.GetChild(0).gameObject;
		stepSound = Effect.GetComponent<AudioSource>();
	}

	private void Update()
	{
		effectPosition = effectPos.transform.position;
		if (effectPosition.y < groundLevel + 1f && !effectOn)
		{
			ShootEffect(true);
		}
		else if (effectPosition.y > groundLevel + 5f && effectOn)
		{
			ShootEffect(false);
		}
	}

	private void ShootEffect(bool enable)
	{
		stepSound.Play();
		Effectchild.SetActive(enable);
		effectOn = enable;
	}
}
