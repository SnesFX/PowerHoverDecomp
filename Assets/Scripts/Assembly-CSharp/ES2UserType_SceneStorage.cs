public class ES2UserType_SceneStorage : ES2Type
{
	public ES2UserType_SceneStorage()
		: base(typeof(SceneStorage))
	{
	}

	public override void Write(object obj, ES2Writer writer)
	{
		SceneStorage sceneStorage = (SceneStorage)obj;
		writer.Write(sceneStorage.IsOpen);
		writer.Write(sceneStorage.CollectableLetters);
		writer.Write(sceneStorage.HighScore);
		writer.Write(sceneStorage.TrickCount);
		writer.Write(sceneStorage.BestTime);
		writer.Write(sceneStorage.KillCount);
		writer.Write(sceneStorage.CasetteState);
		writer.Write(sceneStorage.GhostState);
		writer.Write(sceneStorage.BestDistance);
	}

	public override object Read(ES2Reader reader)
	{
		SceneStorage sceneStorage = new SceneStorage();
		sceneStorage.IsOpen = reader.Read<bool>();
		sceneStorage.CollectableLetters = reader.ReadList<int>();
		sceneStorage.HighScore = reader.Read<float>();
		sceneStorage.TrickCount = reader.Read<int>();
		sceneStorage.BestTime = reader.Read<float>();
		sceneStorage.KillCount = reader.Read<int>();
		sceneStorage.CasetteState = reader.Read<int>();
		sceneStorage.GhostState = reader.Read<int>();
		sceneStorage.BestDistance = reader.Read<float>();
		return sceneStorage;
	}
}
