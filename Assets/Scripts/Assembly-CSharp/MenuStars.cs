using UnityEngine;

public class MenuStars : MonoBehaviour
{
	public MenuStar[] Stars;

	public void UpdateStars(float scorePercentage)
	{
		for (int i = 0; i < Stars.Length; i++)
		{
			if (scorePercentage <= 0f)
			{
				Stars[i].baseStar.enabled = true;
				Stars[i].fill.enabled = false;
				Stars[i].shadow.enabled = false;
			}
			else
			{
				Stars[i].baseStar.enabled = false;
				Stars[i].fill.enabled = scorePercentage >= Stars[i].passLimit;
				Stars[i].shadow.enabled = true;
			}
		}
	}
}
