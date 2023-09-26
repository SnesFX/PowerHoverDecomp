using UnityEngine;
using UnityEngine.UI;

public class ChallengeIapButton : MonoBehaviour
{
	public Text textDesc;

	public Text textPrize;

	private void Start()
	{
	}

	public void SetTexts(string name, string prize)
	{
		if (textDesc != null)
		{
			textDesc.text = name;
		}
		if (textPrize != null)
		{
			textPrize.text = prize;
		}
	}
}
