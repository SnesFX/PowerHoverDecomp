using UnityEngine;
using UnityEngine.UI;

public class GameCenterConnectButton : MonoBehaviour
{
	public Text connectText;

	private int connected;

	private void Start()
	{
		base.gameObject.SetActive(true);
		connected = 0;
	}

	private void Update()
	{
		if (GameCenter.Instance != null)
		{
			if (GameCenter.Instance.LoggedIn && connected != 1)
			{
				LocalizationLoader.Instance.SetText(connectText, "MainMenu.Disconnect");
				connected = 1;
			}
			else if (!GameCenter.Instance.LoggedIn && connected != 2)
			{
				LocalizationLoader.Instance.SetText(connectText, "MainMenu.Connect");
				connected = 2;
			}
		}
	}

	public void ConnectGameCenter()
	{
		if (GameCenter.Instance.LoggedIn)
		{
			GameCenter.Instance.Disconnect();
		}
		else
		{
			GameCenter.Instance.Authenticate();
		}
	}
}
