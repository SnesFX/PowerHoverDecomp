using UnityEngine;

public class PlayServicesGamecenterButton : MonoBehaviour
{
	public Mesh AndroidMesh;

	public Mesh IOSMesh;

	private void Start()
	{
		GetComponent<MeshFilter>().mesh = AndroidMesh;
	}
}
