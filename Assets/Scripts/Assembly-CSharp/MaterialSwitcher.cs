using UnityEngine;

public class MaterialSwitcher : MonoBehaviour
{
	public Renderer Rend;

	public Material OptimizedMaterial;

	private void Start()
	{
		if (!DeviceSettings.Instance.EnableOptimizedMaterials)
		{
			return;
		}
		if (OptimizedMaterial == null)
		{
			Rend.gameObject.SetActive(false);
			return;
		}
		Rend.material = OptimizedMaterial;
		if (GetComponent<MaterialColorFlicker>() != null)
		{
			GetComponent<MaterialColorFlicker>().enabled = false;
		}
	}
}
