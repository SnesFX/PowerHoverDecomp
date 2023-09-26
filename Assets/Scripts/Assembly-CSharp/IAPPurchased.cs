using UnityEngine;

public class IAPPurchased : MonoBehaviour
{
	public GameObject innerObj;

	private void Start()
	{
		innerObj.SetActive(false);
	}

	private void Update()
	{
		if (IapManager.Instance != null && !innerObj.activeSelf && IapManager.Instance.Unlocked)
		{
			innerObj.SetActive(true);
		}
	}
}
