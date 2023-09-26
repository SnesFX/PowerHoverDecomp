using System;
using UnityEngine;

[Serializable]
public class GhostData
{
	public Vector3 position;

	public Quaternion rotation;

	public bool leftPress;

	public bool rightPress;

	public int jump;

	public bool landing;

	public bool prejump;

	public bool grinding;

	public bool dropItem;

	public bool dropBomb;
}
