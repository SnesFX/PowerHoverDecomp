using UnityEngine;

public class GPSSavedGamesButton : MonoBehaviour
{
	public GameObject enableObject;

	public InfoBox[] infos;

	private int connected;

	private void Start()
	{
		base.gameObject.SetActive(true);
		connected = 0;
	}

	public void ShowInfoSaved()
	{
		if (infos != null)
		{
			infos[0].ShowInfo();
		}
	}

	public void ShowInfoLoaded()
	{
		if (infos != null)
		{
			infos[1].ShowInfo();
		}
	}

	private void Update()
	{
		if (GameCenter.Instance != null)
		{
			if (GameCenter.Instance.LoggedIn && connected != 1)
			{
				enableObject.SetActive(true);
				connected = 1;
			}
			else if (!GameCenter.Instance.LoggedIn && connected != 2)
			{
				enableObject.SetActive(false);
				connected = 2;
			}
		}
	}
}
