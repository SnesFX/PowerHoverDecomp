using System;
using UnityEngine;

[Serializable]
public class RandomObject
{
	public GameObject internalObject;

	[Range(0f, 1f)]
	public float probability = 0.5f;
}
