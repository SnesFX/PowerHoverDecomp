using System.IO;
using SA.Common.Pattern;
using UnityEngine;

public class AndroidPaths : MonoBehaviour
{
	private void Awake()
	{
		AndroidNativeUtility.InternalStoragePathLoaded += InternalStoragePathLoaded;
		Singleton<AndroidNativeUtility>.Instance.GetInternalStoragePath();
	}

	private void InternalStoragePathLoaded(string path)
	{
		GameDataController.AndroidInternalPath = path + Path.DirectorySeparatorChar;
		GameDataController.AndroidInternalPathReady = true;
		AndroidNativeUtility.InternalStoragePathLoaded -= InternalStoragePathLoaded;
		GameStats.Instance.ReadStats();
	}
}
