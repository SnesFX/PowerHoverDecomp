using UnityEngine;

public class ShowCurtainAndLoad : MonoBehaviour
{
	private void Awake()
	{
		base.enabled = false;
	}

	private void OnEnable()
	{
		if ((bool)Main.Instance)
		{
			Main.Instance.FakeFadeing(0.6f);
		}
	}
}
