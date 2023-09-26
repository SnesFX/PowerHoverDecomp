using UnityEngine;

public class CreateLazer : MonoBehaviour
{
	public GameObject lazerObj;

	public int length = 60;

	private void Awake()
	{
		GameObject gameObject = Object.Instantiate(lazerObj);
		gameObject.transform.parent = base.transform;
		gameObject.gameObject.name = "Laser";
		gameObject.GetComponent<Laser>().laserLenght = length;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.SetActive(true);
	}
}
