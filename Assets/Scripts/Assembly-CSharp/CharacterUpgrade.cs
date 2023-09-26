using System;
using UnityEngine;

[Serializable]
public class CharacterUpgrade
{
	public string CharacterName;

	public bool IsLocked;

	public bool IsDefaultCharacter;

	public Mesh CharacterMesh;

	public Material CharacterMainMaterial;

	public Mesh HoverBoard;

	public int ControlSpeedAdd;

	public int ControlMaxEasingAdd;

	public int Handling;

	public int Hits;

	public int Magnet;
}
