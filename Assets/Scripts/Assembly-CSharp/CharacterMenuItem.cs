using System;

[Serializable]
public class CharacterMenuItem
{
	public CharacterUpgrade Character;

	public int Prize;

	public CharacterObject characterObject { get; set; }
}
