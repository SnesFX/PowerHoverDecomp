using UnityEngine;

public class ChallengeHitLifes : MonoBehaviour
{
	public GameObject[] hitLifes;

	public void SetHitLifes(int count)
	{
		for (int i = 0; i < hitLifes.Length; i++)
		{
			hitLifes[i].SetActive(count > i);
		}
	}

	public void LooseLife()
	{
		int num = 0;
		for (int i = 0; i < hitLifes.Length; i++)
		{
			if (hitLifes[i].activeSelf)
			{
				num++;
			}
		}
		SetHitLifes(num - 1);
	}

	public bool HasLifes()
	{
		for (int i = 0; i < hitLifes.Length; i++)
		{
			if (hitLifes[i].activeSelf)
			{
				return true;
			}
		}
		return false;
	}
}
