using UnityEngine;

public class CreateRotatingLazer : MonoBehaviour
{
	public GameObject lazerObjA;

	public GameObject lazerObjB;

	private void Awake()
	{
		GameObject gameObject = Object.Instantiate(lazerObjA);
		gameObject.transform.parent = base.transform;
		gameObject.gameObject.name = "LaserA";
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.SetActive(true);
		gameObject = Object.Instantiate(lazerObjB);
		gameObject.transform.parent = base.transform;
		gameObject.gameObject.name = "LaserB";
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.SetActive(true);
	}
}
