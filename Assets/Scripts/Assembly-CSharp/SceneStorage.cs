using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SceneStorage
{
	public bool IsOpen;

	public List<int> CollectableLetters;

	public float HighScore;

	public int TrickCount;

	public float BestTime;

	public int KillCount;

	public int CasetteState;

	public int GhostState;

	public float BestDistance;

	public Dictionary<string, object> ToJson()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("IsOpen", IsOpen);
		dictionary.Add("HighScore", HighScore);
		dictionary.Add("TrickCount", TrickCount);
		dictionary.Add("BestDistance", BestDistance);
		dictionary.Add("BestTime", BestTime);
		dictionary.Add("KillCount", KillCount);
		dictionary.Add("CasetteState", CasetteState);
		dictionary.Add("GhostState", GhostState);
		return dictionary;
	}

	public void FromJson(Dictionary<string, object> json)
	{
		IsOpen = IsOpen || (json.ContainsKey("IsOpen") && (bool)json["IsOpen"]);
		HighScore = Mathf.Max(HighScore, (!json.ContainsKey("HighScore")) ? 0f : Convert.ToSingle(json["HighScore"]));
		TrickCount = Mathf.Max(TrickCount, json.ContainsKey("TrickCount") ? Convert.ToInt32(json["TrickCount"]) : 0);
		BestDistance = Mathf.Max(BestDistance, (!json.ContainsKey("BestDistance")) ? 0f : Convert.ToSingle(json["BestDistance"]));
		BestTime = Mathf.Max(BestTime, (!json.ContainsKey("BestTime")) ? 0f : Convert.ToSingle(json["BestTime"]));
		KillCount = Mathf.Max(KillCount, json.ContainsKey("KillCount") ? Convert.ToInt32(json["KillCount"]) : 0);
		CasetteState = Mathf.Max(CasetteState, json.ContainsKey("CasetteState") ? Convert.ToInt32(json["CasetteState"]) : 0);
		GhostState = Mathf.Max(GhostState, json.ContainsKey("GhostState") ? Convert.ToInt32(json["GhostState"]) : 0);
	}
}
