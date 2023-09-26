using UnityEngine;

public class ChallengeMenuNotification : MonoBehaviour
{
	private void Start()
	{
		base.gameObject.SetActive(!GameDataController.Exists("ChallengeOpenedd"));
	}

	public void SetOpened()
	{
		GameDataController.Save(1, "ChallengeOpenedd");
		base.gameObject.SetActive(false);
	}
}
