using UnityEngine;

public class DuckyAchievement : MonoBehaviour
{
	private void OnEnable()
	{
		if (GameStats.Instance != null)
		{
			GameStats.Instance.Ducky = true;
		}
	}
}
