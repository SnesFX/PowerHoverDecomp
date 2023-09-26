using System;
using UnityEngine.UI;

[Serializable]
public class PlayerStat
{
	public PlayerStatType StatType;

	public string Description;

	public int StartLevel;

	public Text textBox;

	public int Level { get; set; }
}
