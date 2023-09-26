using UnityEngine;

public class BoatBarrel : ResetObject
{
	public GameObject InnerBarrel;

	public GameObject ParticleObject;

	private void Start()
	{
		InnerBarrel.transform.Rotate(0f, Random.Range(0f, 90f), 0f, Space.World);
		ParticleObject.SetActive(true);
	}

	public override void Reset(bool isRewind)
	{
		Object.Destroy(base.gameObject);
	}
}
