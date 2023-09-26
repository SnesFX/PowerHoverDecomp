using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalsPanelEndless : MonoBehaviour
{
	public const string ANIM_NAME_IN = "LevelGoalsFadeIn";

	public Animator animator;

	public Text[] distanceLimits;

	public Text currentScore;

	private SplineWalker walker;

	private void Awake()
	{
		walker = Object.FindObjectOfType<SplineWalker>();
	}

	private void Start()
	{
		if (!(SceneLoader.Instance != null) || !(Main.Instance != null))
		{
			return;
		}
		List<Mission> missions = SceneLoader.Instance.GetMissions(Main.Instance.CurrentScene);
		int num = 0;
		foreach (Mission item in missions)
		{
			MissionType type = item.Type;
			if (type == MissionType.DistanceLimit)
			{
				distanceLimits[num++].text = item.Limit.ToString("0000000");
			}
		}
	}

	private void OnEnable()
	{
		currentScore.text = walker.TotalDistance.ToString("0000000");
		animator.Play("LevelGoalsFadeIn");
	}
}
