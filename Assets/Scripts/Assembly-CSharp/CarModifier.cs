using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CarModifier : MonoBehaviour
{
	public bool LightsOn;

	public bool RandomizeMainMaterial;

	public bool RandomizeMainModel;

	public bool RandomizeExtraModel;

	public GameObject ActiveMainModel;

	public List<Material> ModelMaterials;

	public GameObject LightsRoot;

	public GameObject MainModelRoot;

	public GameObject ExtraModelRoot;

	private void Start()
	{
		if (LightsRoot != null)
		{
			for (int i = 0; i < LightsRoot.transform.childCount; i++)
			{
				LightsRoot.transform.GetChild(i).gameObject.SetActive(LightsOn);
			}
		}
		if (LightsOn)
		{
			GetComponent<AudioSource>().Play();
		}
		if (RandomizeExtraModel && ExtraModelRoot != null)
		{
			int num = Random.Range(0, ExtraModelRoot.transform.childCount);
			for (int j = 0; j < ExtraModelRoot.transform.childCount; j++)
			{
				ExtraModelRoot.transform.GetChild(j).gameObject.SetActive(j == num);
			}
		}
		if (RandomizeMainModel && MainModelRoot != null)
		{
			int num2 = Random.Range(0, MainModelRoot.transform.childCount);
			for (int k = 0; k < MainModelRoot.transform.childCount; k++)
			{
				if (k == num2)
				{
					ActiveMainModel = MainModelRoot.transform.GetChild(k).gameObject;
					ActiveMainModel.SetActive(true);
				}
				else
				{
					MainModelRoot.transform.GetChild(k).gameObject.SetActive(false);
				}
			}
		}
		if (RandomizeMainMaterial && ModelMaterials != null)
		{
			ActiveMainModel.GetComponent<MeshRenderer>().sharedMaterial = ModelMaterials[Random.Range(0, ModelMaterials.Count)];
		}
	}
}
