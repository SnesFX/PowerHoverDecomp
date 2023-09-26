using UnityEngine;

[ExecuteInEditMode]
public class BlobShadowExtrawaganza : MonoBehaviour
{
	public enum ShadowSelect
	{
		Circle = 0,
		Square = 1,
		BlobShapeA = 2,
		BlobShapeB = 3,
		BlopShapeC = 4
	}

	public float xOffset;

	public float yOffset;

	public bool positionAdjustable = true;

	public bool isDynamic;

	public Material shadowMaterialUsedInthisScene;

	public GameObject shadowObject;

	public GameObject[] forms;

	public ShadowSelect shadowForm;

	private GameObject loadObject;

	private Material shadowMaterial;

	private void Update()
	{
		if (isDynamic)
		{
			ShadowPosition();
		}
	}

	private void LoadStuff()
	{
		if (shadowMaterial == null && shadowMaterialUsedInthisScene == null)
		{
			shadowMaterial = Resources.Load("Blobshadow", typeof(Material)) as Material;
		}
		else if (shadowMaterialUsedInthisScene != null)
		{
			GenerateMaterial();
		}
		switch (shadowForm)
		{
		case ShadowSelect.Circle:
			loadObject = Resources.Load("CircleShadow", typeof(GameObject)) as GameObject;
			break;
		case ShadowSelect.Square:
			loadObject = Resources.Load("SquareShadow", typeof(GameObject)) as GameObject;
			break;
		case ShadowSelect.BlobShapeA:
			loadObject = Resources.Load("ShadowFormA", typeof(GameObject)) as GameObject;
			break;
		case ShadowSelect.BlobShapeB:
			loadObject = Resources.Load("ShadowFormB", typeof(GameObject)) as GameObject;
			break;
		case ShadowSelect.BlopShapeC:
			loadObject = Resources.Load("ShadowFormC", typeof(GameObject)) as GameObject;
			break;
		}
	}

	public void GenerateShadow()
	{
		LoadStuff();
		if (shadowObject == null)
		{
			shadowObject = Object.Instantiate(loadObject, base.transform.position, base.transform.rotation);
			shadowObject.transform.parent = base.transform;
			shadowObject.transform.localPosition = Vector3.zero;
			shadowObject.GetComponent<MeshRenderer>().material = shadowMaterial;
			shadowObject.name = loadObject.name;
			shadowObject.tag = "Shadow";
		}
		else
		{
			shadowObject.GetComponent<MeshFilter>().mesh = loadObject.GetComponent<MeshFilter>().sharedMesh;
			shadowObject.name = loadObject.name;
		}
	}

	public void ShadowPosition()
	{
		int value = ((LayerMask)LayerMask.GetMask("Level")).value;
		if (shadowObject != null)
		{
			Vector3 zero = Vector3.zero;
			if (positionAdjustable)
			{
				zero.x = xOffset;
				zero.z = yOffset;
			}
			RaycastHit hitInfo;
			if (Physics.Raycast(base.transform.position + zero, Vector3.down, out hitInfo, 200f, value))
			{
				shadowObject.transform.up = hitInfo.normal;
				shadowObject.transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y + 0.1f, hitInfo.point.z);
			}
		}
	}

	public void GenerateMaterial()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Shadow");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			gameObject.gameObject.GetComponent<Renderer>().material = shadowMaterialUsedInthisScene;
		}
	}
}
