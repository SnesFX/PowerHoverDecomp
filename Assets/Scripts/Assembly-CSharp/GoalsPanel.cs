using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalsPanel : MonoBehaviour
{
	public const string ANIM_NAME_IN = "LevelGoalsFadeIn";

	public Animator animator;

	public Text[] deathLimits;

	public Text[] scoreLimits;

	public Text completionPercentage;

	public Text currentScore;

	public Text currentDeaths;

	private void Start()
	{
		if (!(SceneLoader.Instance != null) || !(Main.Instance != null))
		{
			return;
		}
		List<Mission> missions = SceneLoader.Instance.GetMissions(Main.Instance.CurrentScene);
		int num = 0;
		int num2 = 0;
		foreach (Mission item in missions)
		{
			switch (item.Type)
			{
			case MissionType.ScoreLimit:
				scoreLimits[num++].text = item.Limit.ToString("0000000");
				break;
			case MissionType.DeathLimit:
				deathLimits[num2++].text = item.Limit.ToString("00");
				break;
			}
		}
	}
}
