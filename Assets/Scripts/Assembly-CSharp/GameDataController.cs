public static class GameDataController
{
	public static string AndroidInternalPath = string.Empty;

	public static bool AndroidInternalPathReady;

	public static void Save<T>(T obj, string identifier)
	{
		ES2.Save(obj, AndroidInternalPath + identifier);
	}

	public static T Load<T>(string identifier)
	{
		return ES2.Load<T>(AndroidInternalPath + identifier);
	}

	public static bool Exists(string identifier)
	{
		return ES2.Exists(AndroidInternalPath + identifier);
	}

	public static void Delete(string identifier)
	{
		ES2.Delete(AndroidInternalPath + identifier);
	}
}
