using System;
using System.Collections;
using UnityEngine;

public class MeshBrushParent : MonoBehaviour
{
	private Component[] meshes;

	private Component[] meshFilters;

	private Matrix4x4 myTransform;

	private Hashtable materialToMesh;

	private MeshFilter filter;

	private Renderer curRenderer;

	private Material[] materials;

	private MB_MeshCombineUtility.MeshInstance instance;

	private MB_MeshCombineUtility.MeshInstance[] instances;

	private ArrayList objects;

	private ArrayList elements;

	public void FlagMeshesAsStatic()
	{
		meshes = GetComponentsInChildren(typeof(Transform));
		Component[] array = meshes;
		for (int i = 0; i < array.Length; i++)
		{
			Transform transform = (Transform)array[i];
			transform.gameObject.isStatic = true;
		}
	}

	public void UnflagMeshesAsStatic()
	{
		meshes = GetComponentsInChildren(typeof(Transform));
		Component[] array = meshes;
		for (int i = 0; i < array.Length; i++)
		{
			Transform transform = (Transform)array[i];
			transform.gameObject.isStatic = false;
		}
	}

	public void DeleteAllMeshes()
	{
		UnityEngine.Object.DestroyImmediate(base.gameObject);
	}

	public void CombinePaintedMeshes()
	{
		meshFilters = GetComponentsInChildren(typeof(MeshFilter));
		myTransform = base.transform.worldToLocalMatrix;
		materialToMesh = new Hashtable();
		for (long num = 0L; num < meshFilters.Length; num++)
		{
			filter = (MeshFilter)meshFilters[num];
			curRenderer = meshFilters[num].GetComponent<Renderer>();
			instance = default(MB_MeshCombineUtility.MeshInstance);
			instance.mesh = filter.sharedMesh;
			if (!(curRenderer != null) || !curRenderer.enabled || !(instance.mesh != null))
			{
				continue;
			}
			instance.transform = myTransform * filter.transform.localToWorldMatrix;
			materials = curRenderer.sharedMaterials;
			for (int i = 0; i < materials.Length; i++)
			{
				instance.subMeshIndex = Math.Min(i, instance.mesh.subMeshCount - 1);
				objects = (ArrayList)materialToMesh[materials[i]];
				if (objects != null)
				{
					objects.Add(instance);
					continue;
				}
				objects = new ArrayList();
				objects.Add(instance);
				materialToMesh.Add(materials[i], objects);
			}
			UnityEngine.Object.DestroyImmediate(curRenderer.gameObject);
		}
		foreach (DictionaryEntry item in materialToMesh)
		{
			elements = (ArrayList)item.Value;
			instances = (MB_MeshCombineUtility.MeshInstance[])elements.ToArray(typeof(MB_MeshCombineUtility.MeshInstance));
			if (materialToMesh.Count == 1)
			{
				if (GetComponent(typeof(MeshFilter)) == null)
				{
					base.gameObject.AddComponent(typeof(MeshFilter));
				}
				if (!GetComponent(typeof(MeshRenderer)))
				{
					base.gameObject.AddComponent(typeof(MeshRenderer));
				}
				filter = (MeshFilter)GetComponent(typeof(MeshFilter));
				filter.mesh = MB_MeshCombineUtility.Combine(instances, false);
				GetComponent<Renderer>().material = (Material)item.Key;
				GetComponent<Renderer>().enabled = true;
			}
			else
			{
				GameObject gameObject = new GameObject("Combined mesh");
				gameObject.transform.parent = base.transform;
				gameObject.transform.localScale = Vector3.one;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.AddComponent(typeof(MeshFilter));
				gameObject.AddComponent<MeshRenderer>();
				gameObject.GetComponent<Renderer>().material = (Material)item.Key;
				gameObject.isStatic = true;
				filter = (MeshFilter)gameObject.GetComponent(typeof(MeshFilter));
				filter.mesh = MB_MeshCombineUtility.Combine(instances, false);
			}
		}
		base.gameObject.isStatic = true;
	}
}
