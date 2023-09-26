using System;
using System.Collections.Generic;

[Serializable]
public class SceneDetails
{
	public string SceneName;

	public string VisibleName;

	public List<Mission> Missions;

	public string GhostFile;

	public string GhostBattery;

	public int Group;

	public bool HasCasette;

	public float GhostTimeDiff;

	public bool IsEndless;

	public bool IsChallenge;

	public SceneStorage Storage;

	public bool GhostAvailable;

	public int StarLockCount;

	public EffectType EffectType;

	public AudioController.MusicType SceneMusic;

	public bool AdBlock;

	public string ChallengePartIDs;
}
