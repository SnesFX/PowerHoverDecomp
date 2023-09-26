using UnityEngine;

public class ES2UserType_GhostData : ES2Type
{
	public ES2UserType_GhostData()
		: base(typeof(GhostData))
	{
	}

	public override void Write(object obj, ES2Writer writer)
	{
		GhostData ghostData = (GhostData)obj;
		writer.Write(ghostData.position);
		writer.Write(ghostData.rotation);
		writer.Write(ghostData.leftPress);
		writer.Write(ghostData.rightPress);
		writer.Write(ghostData.jump);
		writer.Write(ghostData.landing);
		writer.Write(ghostData.prejump);
		writer.Write(ghostData.grinding);
		writer.Write(ghostData.dropItem);
		writer.Write(ghostData.dropBomb);
	}

	public override object Read(ES2Reader reader)
	{
		GhostData ghostData = new GhostData();
		ghostData.position = reader.Read<Vector3>();
		ghostData.rotation = reader.Read<Quaternion>();
		ghostData.leftPress = reader.Read<bool>();
		ghostData.rightPress = reader.Read<bool>();
		ghostData.jump = reader.Read<int>();
		ghostData.landing = reader.Read<bool>();
		ghostData.prejump = reader.Read<bool>();
		ghostData.grinding = reader.Read<bool>();
		ghostData.dropItem = reader.Read<bool>();
		ghostData.dropBomb = reader.Read<bool>();
		return ghostData;
	}
}
