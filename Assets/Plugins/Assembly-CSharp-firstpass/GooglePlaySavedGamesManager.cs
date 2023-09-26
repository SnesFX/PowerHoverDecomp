using System;
using System.Collections.Generic;
using SA.Common.Pattern;
using UnityEngine;

public class GooglePlaySavedGamesManager : Singleton<GooglePlaySavedGamesManager>
{
	private List<GP_SnapshotMeta> _AvailableGameSaves = new List<GP_SnapshotMeta>();

	public List<GP_SnapshotMeta> AvailableGameSaves
	{
		get
		{
			return _AvailableGameSaves;
		}
	}

	public static event Action ActionGameSaveUIClosed;

	public static event Action ActionNewGameSaveRequest;

	public static event Action<GooglePlayResult> ActionAvailableGameSavesLoaded;

	public static event Action<GP_SpanshotLoadResult> ActionGameSaveLoaded;

	public static event Action<GP_SpanshotLoadResult> ActionGameSaveResult;

	public static event Action<GP_SnapshotConflict> ActionConflict;

	public static event Action<GP_DeleteSnapshotResult> ActionGameSaveRemoved;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void ShowSavedGamesUI(string title, int maxNumberOfSavedGamesToShow, bool allowAddButton = true, bool allowDelete = true)
	{
		if (GooglePlayConnection.CheckState())
		{
			AN_GMSGeneralProxy.ShowSavedGamesUI_Bridge(title, maxNumberOfSavedGamesToShow, allowAddButton, allowDelete);
		}
	}

	public void CreateNewSnapshot(string name, string description, Texture2D coverImage, string spanshotData, long PlayedTime)
	{
		CreateNewSnapshot(name, description, coverImage, GetBytes(spanshotData), PlayedTime);
	}

	public void CreateNewSnapshot(string name, string description, Texture2D coverImage, byte[] spanshotData, long PlayedTime)
	{
		string imageData = string.Empty;
		if (coverImage != null)
		{
			byte[] inArray = coverImage.EncodeToPNG();
			imageData = Convert.ToBase64String(inArray);
		}
		else
		{
			Debug.LogWarning("GooglePlaySavedGmaesManager::CreateNewSnapshot:  coverImage is null");
		}
		string data = Convert.ToBase64String(spanshotData);
		AN_GMSGeneralProxy.CreateNewSpanshot_Bridge(name, description, imageData, data, PlayedTime);
	}

	public void LoadSpanshotByName(string name)
	{
		AN_GMSGeneralProxy.OpenSpanshotByName_Bridge(name);
	}

	public void DeleteSpanshotByName(string name)
	{
		AN_GMSGeneralProxy.DeleteSpanshotByName_Bridge(name);
	}

	public void LoadAvailableSavedGames()
	{
		AN_GMSGeneralProxy.LoadSpanshots_Bridge();
	}

	private static byte[] GetBytes(string str)
	{
		byte[] array = new byte[str.Length * 2];
		Buffer.BlockCopy(str.ToCharArray(), 0, array, 0, array.Length);
		return array;
	}

	private static string GetString(byte[] bytes)
	{
		char[] array = ((bytes.Length % 2 == 0) ? new char[bytes.Length / 2] : new char[bytes.Length / 2 + 1]);
		Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
		return new string(array);
	}

	private void OnLoadSnapshotsResult(string data)
	{
		string[] array = data.Split("|"[0]);
		GooglePlayResult googlePlayResult = new GooglePlayResult(array[0]);
		if (googlePlayResult.IsSucceeded)
		{
			_AvailableGameSaves.Clear();
			for (int i = 1; i < array.Length && !(array[i] == "endofline"); i += 5)
			{
				GP_SnapshotMeta gP_SnapshotMeta = new GP_SnapshotMeta();
				gP_SnapshotMeta.Title = array[i];
				gP_SnapshotMeta.LastModifiedTimestamp = Convert.ToInt64(array[i + 1]);
				gP_SnapshotMeta.Description = array[i + 2];
				gP_SnapshotMeta.CoverImageUrl = array[i + 3];
				gP_SnapshotMeta.TotalPlayedTime = Convert.ToInt64(array[i + 4]);
				_AvailableGameSaves.Add(gP_SnapshotMeta);
			}
		}
		GooglePlaySavedGamesManager.ActionAvailableGameSavesLoaded(googlePlayResult);
	}

	private void OnSavedGamePicked(string data)
	{
		string[] array = data.Split("|"[0]);
		GP_SpanshotLoadResult gP_SpanshotLoadResult = new GP_SpanshotLoadResult(array[0]);
		if (gP_SpanshotLoadResult.IsSucceeded)
		{
			string title = array[1];
			long lastModifiedTimestamp = Convert.ToInt64(array[2]);
			string description = array[3];
			string coverImageUrl = array[4];
			long totalPlayedTime = Convert.ToInt64(array[5]);
			byte[] bytes = Convert.FromBase64String(array[6]);
			GP_Snapshot gP_Snapshot = new GP_Snapshot();
			gP_Snapshot.meta.Title = title;
			gP_Snapshot.meta.Description = description;
			gP_Snapshot.meta.CoverImageUrl = coverImageUrl;
			gP_Snapshot.meta.LastModifiedTimestamp = lastModifiedTimestamp;
			gP_Snapshot.meta.TotalPlayedTime = totalPlayedTime;
			gP_Snapshot.bytes = bytes;
			gP_Snapshot.stringData = GetString(bytes);
			gP_SpanshotLoadResult.SetSnapShot(gP_Snapshot);
		}
		GooglePlaySavedGamesManager.ActionGameSaveLoaded(gP_SpanshotLoadResult);
	}

	private void OnSavedGameSaveResult(string data)
	{
		string[] array = data.Split("|"[0]);
		GP_SpanshotLoadResult gP_SpanshotLoadResult = new GP_SpanshotLoadResult(array[0]);
		if (gP_SpanshotLoadResult.IsSucceeded)
		{
			string title = array[1];
			long lastModifiedTimestamp = Convert.ToInt64(array[2]);
			string description = array[3];
			string coverImageUrl = array[4];
			long totalPlayedTime = Convert.ToInt64(array[5]);
			byte[] bytes = Convert.FromBase64String(array[6]);
			GP_Snapshot gP_Snapshot = new GP_Snapshot();
			gP_Snapshot.meta.Title = title;
			gP_Snapshot.meta.Description = description;
			gP_Snapshot.meta.CoverImageUrl = coverImageUrl;
			gP_Snapshot.meta.LastModifiedTimestamp = lastModifiedTimestamp;
			gP_Snapshot.meta.TotalPlayedTime = totalPlayedTime;
			gP_Snapshot.bytes = bytes;
			gP_Snapshot.stringData = GetString(bytes);
			gP_SpanshotLoadResult.SetSnapShot(gP_Snapshot);
		}
		GooglePlaySavedGamesManager.ActionGameSaveResult(gP_SpanshotLoadResult);
	}

	private void OnConflict(string data)
	{
		string[] array = data.Split("|"[0]);
		string title = array[0];
		long lastModifiedTimestamp = Convert.ToInt64(array[1]);
		string description = array[2];
		string coverImageUrl = array[3];
		long totalPlayedTime = Convert.ToInt64(array[4]);
		byte[] bytes = Convert.FromBase64String(array[5]);
		GP_Snapshot gP_Snapshot = new GP_Snapshot();
		gP_Snapshot.meta.Title = title;
		gP_Snapshot.meta.Description = description;
		gP_Snapshot.meta.CoverImageUrl = coverImageUrl;
		gP_Snapshot.meta.LastModifiedTimestamp = lastModifiedTimestamp;
		gP_Snapshot.meta.TotalPlayedTime = totalPlayedTime;
		gP_Snapshot.bytes = bytes;
		gP_Snapshot.stringData = GetString(bytes);
		title = array[6];
		lastModifiedTimestamp = Convert.ToInt64(array[7]);
		description = array[8];
		coverImageUrl = array[9];
		totalPlayedTime = Convert.ToInt64(array[10]);
		bytes = Convert.FromBase64String(array[11]);
		GP_Snapshot gP_Snapshot2 = new GP_Snapshot();
		gP_Snapshot2.meta.Title = title;
		gP_Snapshot2.meta.Description = description;
		gP_Snapshot2.meta.CoverImageUrl = coverImageUrl;
		gP_Snapshot2.meta.LastModifiedTimestamp = lastModifiedTimestamp;
		gP_Snapshot2.meta.TotalPlayedTime = totalPlayedTime;
		gP_Snapshot2.bytes = bytes;
		gP_Snapshot2.stringData = GetString(bytes);
		GP_SnapshotConflict obj = new GP_SnapshotConflict(gP_Snapshot, gP_Snapshot2);
		GooglePlaySavedGamesManager.ActionConflict(obj);
	}

	private void OnNewGameSaveRequest(string data)
	{
		GooglePlaySavedGamesManager.ActionNewGameSaveRequest();
	}

	private void OnSavedGamesUIClosed(string data)
	{
		GooglePlaySavedGamesManager.ActionGameSaveUIClosed();
	}

	private void OnDeleteResult(string data)
	{
		string[] array = data.Split("|"[0]);
		GP_DeleteSnapshotResult gP_DeleteSnapshotResult = new GP_DeleteSnapshotResult(array[0]);
		if (gP_DeleteSnapshotResult.IsSucceeded)
		{
			gP_DeleteSnapshotResult.SetId(array[1]);
		}
		GooglePlaySavedGamesManager.ActionGameSaveRemoved(gP_DeleteSnapshotResult);
	}

	static GooglePlaySavedGamesManager()
	{
		GooglePlaySavedGamesManager.ActionGameSaveUIClosed = delegate
		{
		};
		GooglePlaySavedGamesManager.ActionNewGameSaveRequest = delegate
		{
		};
		GooglePlaySavedGamesManager.ActionAvailableGameSavesLoaded = delegate
		{
		};
		GooglePlaySavedGamesManager.ActionGameSaveLoaded = delegate
		{
		};
		GooglePlaySavedGamesManager.ActionGameSaveResult = delegate
		{
		};
		GooglePlaySavedGamesManager.ActionConflict = delegate
		{
		};
		GooglePlaySavedGamesManager.ActionGameSaveRemoved = delegate
		{
		};
	}
}
