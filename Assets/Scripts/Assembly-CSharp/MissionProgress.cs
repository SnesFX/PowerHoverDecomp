public class MissionProgress
{
	public string Id;

	public Mission Mission;

	public string StageName;

	public float CurrentValue;

	public bool Completed;

	private const string RecordValue = "_CurrentValue";

	private const string RecordCompleted = "_Completed";

	public MissionProgress(Mission m, string stage)
	{
		Mission = m;
		StageName = stage;
		Id = string.Format("{0}_{1}_{2}", stage, Mission.Type.ToString(), Mission.Limit);
		Load();
	}

	public void SetCompleted()
	{
		Completed = true;
		Save();
	}

	public void Clear()
	{
		GameDataController.Delete(Id + "_CurrentValue");
		GameDataController.Delete(Id + "_Completed");
		CurrentValue = -1f;
		Completed = false;
	}

	public void Load()
	{
		if (!GameDataController.Exists(Id + "_CurrentValue"))
		{
			CurrentValue = -1f;
			Completed = false;
		}
		else
		{
			CurrentValue = GameDataController.Load<float>(Id + "_CurrentValue");
			Completed = GameDataController.Exists(Id + "_Completed") && GameDataController.Load<bool>(Id + "_Completed");
		}
	}

	private void Save()
	{
		GameDataController.Save(Completed, Id + "_Completed");
		GameDataController.Save(CurrentValue, Id + "_CurrentValue");
	}
}
