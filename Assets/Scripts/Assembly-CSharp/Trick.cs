using System;
using UnityEngine;

[Serializable]
public class Trick
{
	public string Name;

	public int ID;

	[Range(0f, 720f)]
	public int Rotate;

	[Range(0f, 10f)]
	public int ExtraScore;

	[Range(0f, 1f)]
	public float Probability;

	[Range(0.25f, 2f)]
	public float RotationSpeedMultiplier = 1f;

	public int UsedCounter;
}
