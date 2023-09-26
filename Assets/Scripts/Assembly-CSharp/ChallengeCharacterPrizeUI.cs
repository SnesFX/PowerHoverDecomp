using UnityEngine;
using UnityEngine.UI;

public class ChallengeCharacterPrizeUI : MonoBehaviour
{
	public Text statImages;

	public void SetStat(bool enable, int prize)
	{
		base.gameObject.SetActive(enable);
		if (enable)
		{
			statImages.text = string.Format("{0}", prize);
		}
	}
}
