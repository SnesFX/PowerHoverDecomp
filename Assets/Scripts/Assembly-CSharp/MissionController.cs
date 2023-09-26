using System.Collections.Generic;
using UnityEngine;

public class MissionController : MonoBehaviour
{
	private List<MissionProgress> CurrentProgresses;

	public static MissionController Instance { get; private set; }

	private void Start()
	{
		Instance = this;
	}

	public List<MissionProgress> GetMissionProgresses(string SceneName)
	{
		List<MissionProgress> list = new List<MissionProgress>();
		foreach (Mission mission in SceneLoader.Instance.GetMissions(SceneName))
		{
			MissionProgress item = new MissionProgress(mission, SceneName);
			list.Add(item);
		}
		return list;
	}

	public void ClearSceneProgress(string SceneName)
	{
		List<MissionProgress> missionProgresses = GetMissionProgresses(SceneName);
		foreach (MissionProgress item in missionProgresses)
		{
			item.Clear();
		}
	}

	public void StartScene(string SceneName)
	{
		List<Mission> missions = SceneLoader.Instance.GetMissions(SceneName);
		if (missions == null)
		{
			return;
		}
		CurrentProgresses = new List<MissionProgress>();
		foreach (Mission mission in SceneLoader.Instance.GetMissions(SceneName))
		{
			MissionProgress item = new MissionProgress(mission, SceneName);
			CurrentProgresses.Add(item);
		}
	}
}
