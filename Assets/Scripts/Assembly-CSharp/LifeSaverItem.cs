using UnityEngine;
using UnityEngine.UI;

public class LifeSaverItem : MonoBehaviour
{
	public GameObject emptyItem;

	public GameObject fullItem;

	public Text ItemCount;

	public GameObject xItem;

	public GameObject AdDescription;

	private void OnEnable()
	{
		Vector3 localPosition = GetComponent<RectTransform>().localPosition;
		if (LifeController.Instance != null && LifeController.Instance.LifeSavers > 0)
		{
			ItemCount.text = string.Format("{0}", LifeController.Instance.LifeSavers);
			xItem.SetActive(true);
			emptyItem.SetActive(false);
			GetComponent<Button>().enabled = true;
			fullItem.SetActive(true);
			if (UnityAdsIngetration.Instance.IsAdsActivated)
			{
				AdDescription.SetActive(true);
				localPosition.x = 260f;
				GetComponent<RectTransform>().localPosition = localPosition;
			}
			else
			{
				AdDescription.SetActive(false);
				localPosition.x = 452f;
				GetComponent<RectTransform>().localPosition = localPosition;
			}
		}
		else
		{
			localPosition.x = 452f;
			GetComponent<RectTransform>().localPosition = localPosition;
			xItem.SetActive(false);
			ItemCount.gameObject.SetActive(false);
			emptyItem.SetActive(true);
			GetComponent<Button>().enabled = false;
			fullItem.SetActive(false);
			AdDescription.SetActive(false);
		}
	}
}
