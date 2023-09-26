using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementsPanel : MenuPanel
{
	private const string FMT = "0 000";

	public GameObject contentRoot;

	public Scrollbar scroll;

	public Text completedText;

	public Text ContinueCount;

	public Text TimePlayed;

	public Text CoinsCollected;

	public Text ScoreTotal;

	public Text DeathCount;

	public Text StuffBroke;

	private GameObject achievementItemObj;

	private List<AchievementItem> items;

	public void Start()
	{
		items = new List<AchievementItem>();
		achievementItemObj = Object.Instantiate(Resources.Load("AchievementItem")) as GameObject;
		if (AchievementController.Instance != null)
		{
			int num = 0;
			foreach (AchievementController.LocalAchievement achievement in AchievementController.Instance.GetAchievements())
			{
				GameObject gameObject = Object.Instantiate(achievementItemObj);
				gameObject.transform.SetParent(contentRoot.transform);
				gameObject.transform.localScale = Vector3.one;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, 0f);
				AchievementItem component = gameObject.GetComponent<AchievementItem>();
				component.id = achievement.LocalId;
				component.UpdateDetails(achievement, num);
				items.Add(component);
				gameObject.SetActive(true);
				num++;
			}
		}
		Object.Destroy(achievementItemObj);
	}

	private void OnEnable()
	{
		if (items == null)
		{
			return;
		}
		int num = 0;
		foreach (AchievementItem item in items)
		{
			AchievementController.LocalAchievement achievement = AchievementController.Instance.GetAchievement(item.id);
			if (achievement != null)
			{
				item.UpdateDetails(achievement);
				if (achievement.Achieved)
				{
					num++;
				}
			}
		}
		LocalizationLoader.Instance.SetText(completedText, "MainMenu.Completed");
		completedText.text = string.Format("{0} {1}/{2}", completedText.text, num, items.Count);
	}

	public override void Activate(bool activate)
	{
		base.Activate(activate);
		if (activate)
		{
			OnEnable();
			scroll.value = 1f;
			if (!(GameStats.Instance == null))
			{
				ContinueCount.text = GameStats.Instance.GameOvers.ToString("0 000");
				CoinsCollected.text = GameStats.Instance.CoinsCollected.ToString("0 000");
				DeathCount.text = GameStats.Instance.DeathCount.ToString("0 000");
				ScoreTotal.text = (GameStats.Instance.UnlockedBattery + GameStats.Instance.TotalBattery).ToString("0 000");
				TimePlayed.text = Utils.FormatTime(GameStats.Instance.TimePlayed);
				StuffBroke.text = GameStats.Instance.BreakStuff.ToString("0 000");
			}
		}
	}

	public void OpenGameCenter()
	{
		GameCenter.Instance.ShowAchievementsUI();
	}

	public void ResetAchievements()
	{
		AchievementController.Instance.ClearLocalAchievements();
		OnEnable();
	}
}
