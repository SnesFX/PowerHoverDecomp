using UnityEngine;
using UnityEngine.UI;

public class ChallengeCharacterStatUI : MonoBehaviour
{
	public Image[] statImages;

	public Image[] hitImages;

	public Image[] magnetImages;

	public void SetStat(CharacterUpgrade cu)
	{
		for (int i = 0; i < statImages.Length; i++)
		{
			statImages[i].enabled = cu.Handling - 1 >= i;
			hitImages[i].enabled = cu.Hits - 1 >= i;
			magnetImages[i].enabled = cu.Magnet - 1 >= i;
		}
	}
}
