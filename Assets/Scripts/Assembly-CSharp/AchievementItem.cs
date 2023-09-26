using UnityEngine;
using UnityEngine.UI;

public class AchievementItem : MonoBehaviour
{
	public string id;

	public Text title;

	public Text description1;

	public Text description2;

	public Image BGFiller;

	public Color AchievevedColor;

	public Color EverySecondColor;

	public Image FillBarBg;

	public Image FillBarValue;

	public void UpdateDetails(AchievementController.LocalAchievement la, int counter = -1)
	{
		if (la.LocalId != id)
		{
			return;
		}
		LocalizationLoader.Instance.SetText(title, la.Title);
		LocalizationLoader.Instance.SetText(description1, la.Description);
		if (la.CompletionValue > 1f)
		{
			description1.text = string.Format("{0} {1}", la.CompletionValue, description1.text);
		}
		if (la.Achieved)
		{
			LocalizationLoader.Instance.SetText(description2, "MainMenu.AchievementAchieved");
			BGFiller.color = AchievevedColor;
			FillBarBg.enabled = false;
			FillBarValue.enabled = false;
			return;
		}
		if (counter > -1)
		{
			BGFiller.color = ((counter % 2 != 0) ? BGFiller.color : EverySecondColor);
		}
		FillBarBg.enabled = true;
		FillBarValue.enabled = true;
		FillBarValue.fillAmount = 0.01f + la.Value / la.CompletionValue;
		description2.text = string.Format("{0:F1}%", la.Value / la.CompletionValue * 100f);
	}
}
