using UnityEngine;

public class RunningMissile : MonoBehaviour
{
	public bool[] enabledMissiles;

	public Vector3[] positions;

	public GameObject missileObj;

	private void Start()
	{
		for (int i = 0; i < enabledMissiles.Length; i++)
		{
			if (enabledMissiles[i])
			{
				GameObject gameObject = Object.Instantiate(missileObj);
				gameObject.transform.parent = base.transform;
				gameObject.gameObject.name = string.Format("Missile_{0}", i);
				gameObject.transform.localPosition = positions[i];
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.SetActive(true);
			}
		}
	}
}
