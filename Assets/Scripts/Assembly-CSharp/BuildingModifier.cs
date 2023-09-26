using UnityEngine;

public class BuildingModifier : MonoBehaviour
{
	public bool RandomizeExtraModel;

	public GameObject ExtraModelRoot;

	private void Start()
	{
		if (RandomizeExtraModel && ExtraModelRoot != null)
		{
			int num = Random.Range(0, ExtraModelRoot.transform.childCount);
			for (int i = 0; i < ExtraModelRoot.transform.childCount; i++)
			{
				ExtraModelRoot.transform.GetChild(i).gameObject.SetActive(i == num);
			}
		}
		else if (!RandomizeExtraModel && ExtraModelRoot != null)
		{
			ExtraModelRoot.SetActive(false);
		}
	}
}
