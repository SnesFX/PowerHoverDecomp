using UnityEngine;

public class ContentLoader : MonoBehaviour
{
	private void Awake()
	{
		if (Main.Instance == null)
		{
			Application.LoadLevelAdditive("InitScene");
		}
	}
}
