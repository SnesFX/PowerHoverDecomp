using UnityEngine;

public class GiantMiningBotEffects : MonoBehaviour
{
	public GameObject effectPos;

	public float groundLevel = 15f;

	private Vector3 effectPosition;

	private bool effectOn;

	public GameObject Effect;

	private void Start()
	{
	}

	private void Update()
	{
		effectPosition = effectPos.transform.position;
		if (effectPosition.y < groundLevel + 1f && !effectOn)
		{
			ShootEffect(true);
		}
		if (effectPosition.y > groundLevel + 5f)
		{
			ShootEffect(false);
		}
	}

	private void ShootEffect(bool enable)
	{
		Effect.SetActive(enable);
		effectOn = enable;
	}
}
