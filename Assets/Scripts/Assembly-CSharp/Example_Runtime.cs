using System.Collections;
using UnityEngine;

public class Example_Runtime : MonoBehaviour
{
	private MeshBrush_RuntimeAPI mb;

	private Ray paintRay;

	private RaycastHit hit;

	public GameObject[] exampleCubes = new GameObject[2];

	private void Start()
	{
		StartCoroutine(PaintExampleCubes());
		if (!GetComponent(typeof(MeshBrush_RuntimeAPI)))
		{
			base.gameObject.AddComponent<MeshBrush_RuntimeAPI>();
		}
		mb = GetComponent(typeof(MeshBrush_RuntimeAPI)) as MeshBrush_RuntimeAPI;
		for (byte b = 0; b < exampleCubes.Length; b++)
		{
			if (exampleCubes[b] == null)
			{
				Debug.LogError("One or more GameObjects in the set of meshes to paint are unassigned.");
			}
		}
		mb.brushRadius = 10f;
		mb.amount = 7;
		mb.delayBetweenPaintStrokes = 0.2f;
		mb.randomScale = new Vector4(0.4f, 1.4f, 0.5f, 1.5f);
		mb.randomRotation = 100f;
		mb.meshOffset = 1.5f;
		mb.scattering = 75f;
	}

	private IEnumerator PaintExampleCubes()
	{
		while (true)
		{
			if (Input.GetKey(KeyCode.P))
			{
				mb.setOfMeshesToPaint = exampleCubes;
				paintRay = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(paintRay, out hit))
				{
					mb.Paint_MultipleMeshes(hit);
				}
				yield return new WaitForSeconds(mb.delayBetweenPaintStrokes);
			}
			yield return null;
		}
	}
}
