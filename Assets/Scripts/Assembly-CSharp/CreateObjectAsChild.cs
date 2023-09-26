using UnityEngine;

public class CreateObjectAsChild : MonoBehaviour
{
	public GameObject lazerObjA;

	private void Awake()
	{
		GameObject gameObject = Object.Instantiate(lazerObjA);
		gameObject.transform.parent = base.transform;
		gameObject.gameObject.name = "child";
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.SetActive(true);
	}
}
