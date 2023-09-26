using System;
using System.Collections.Generic;
using SA.Common.Pattern;
using UnityEngine;

public class ISN_GameSaves : Singleton<ISN_GameSaves>
{
	private static Dictionary<string, GK_SavedGame> _CachedGameSaves;

	public static event Action<GK_SaveRemoveResult> ActionSaveRemoved;

	public static event Action<GK_SaveResult> ActionGameSaved;

	public static event Action<GK_FetchResult> ActionSavesFetched;

	public static event Action<GK_SavesResolveResult> ActionSavesResolved;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void SaveGame(byte[] data, string name)
	{
	}

	public void FetchSavedGames()
	{
	}

	public void DeleteSavedGame(string name)
	{
	}

	public void ResolveConflictingSavedGames(List<GK_SavedGame> conflicts, byte[] data)
	{
	}

	public void LoadSaveData(GK_SavedGame save)
	{
	}

	public void OnSaveSuccess(string data)
	{
		GK_SavedGame save = DeserializeGameSave(data);
		GK_SaveResult obj = new GK_SaveResult(save);
		ISN_GameSaves.ActionGameSaved(obj);
	}

	public void OnSaveFailed(string erroData)
	{
		GK_SaveResult obj = new GK_SaveResult(erroData);
		ISN_GameSaves.ActionGameSaved(obj);
	}

	public void OnFetchSuccess(string data)
	{
		List<GK_SavedGame> list = new List<GK_SavedGame>();
		string[] array = data.Split(new string[1] { "|%|" }, StringSplitOptions.None);
		for (int i = 0; i < array.Length && !(array[i] == "endofline"); i++)
		{
			GK_SavedGame item = DeserializeGameSave(array[i]);
			list.Add(item);
		}
		GK_FetchResult obj = new GK_FetchResult(list);
		ISN_GameSaves.ActionSavesFetched(obj);
	}

	public void OnFetchFailed(string errorData)
	{
		GK_FetchResult obj = new GK_FetchResult(errorData);
		ISN_GameSaves.ActionSavesFetched(obj);
	}

	public void OnResolveSuccess(string data)
	{
		List<GK_SavedGame> list = new List<GK_SavedGame>();
		string[] array = data.Split(new string[1] { "|%|" }, StringSplitOptions.None);
		for (int i = 0; i < array.Length && !(array[i] == "endofline"); i++)
		{
			GK_SavedGame item = DeserializeGameSave(array[i]);
			list.Add(item);
		}
		GK_SavesResolveResult obj = new GK_SavesResolveResult(list);
		ISN_GameSaves.ActionSavesResolved(obj);
	}

	public void OnResolveFailed(string errorData)
	{
		GK_SavesResolveResult obj = new GK_SavesResolveResult(errorData);
		ISN_GameSaves.ActionSavesResolved(obj);
	}

	public void OnDeleteSuccess(string name)
	{
		GK_SaveRemoveResult obj = new GK_SaveRemoveResult(name);
		ISN_GameSaves.ActionSaveRemoved(obj);
	}

	public void OnDeleteFailed(string data)
	{
		string[] array = data.Split(new string[1] { "|%|" }, StringSplitOptions.None);
		string text = array[0];
		string errorData = array[1];
		GK_SaveRemoveResult obj = new GK_SaveRemoveResult(text, errorData);
		ISN_GameSaves.ActionSaveRemoved(obj);
	}

	private void OnSaveDataLoaded(string data)
	{
		string[] array = data.Split(new string[1] { "|%|" }, StringSplitOptions.None);
		string key = array[0];
		string base64Data = array[1];
		if (_CachedGameSaves.ContainsKey(key))
		{
			_CachedGameSaves[key].GenerateDataLoadEvent(base64Data);
		}
	}

	private void OnSaveDataLoadFailed(string data)
	{
		string[] array = data.Split(new string[1] { "|%|" }, StringSplitOptions.None);
		string key = array[0];
		string erorrData = array[1];
		if (_CachedGameSaves.ContainsKey(key))
		{
			_CachedGameSaves[key].GenerateDataLoadFailedEvent(erorrData);
		}
	}

	private GK_SavedGame DeserializeGameSave(string serializedData)
	{
		string[] array = serializedData.Split('|');
		string id = array[0];
		string text = array[1];
		string device = array[2];
		string dateString = array[3];
		GK_SavedGame gK_SavedGame = new GK_SavedGame(id, text, device, dateString);
		if (_CachedGameSaves.ContainsKey(gK_SavedGame.Id))
		{
			_CachedGameSaves[gK_SavedGame.Id] = gK_SavedGame;
		}
		else
		{
			_CachedGameSaves.Add(gK_SavedGame.Id, gK_SavedGame);
		}
		return gK_SavedGame;
	}

	static ISN_GameSaves()
	{
		ISN_GameSaves.ActionSaveRemoved = delegate
		{
		};
		ISN_GameSaves.ActionGameSaved = delegate
		{
		};
		ISN_GameSaves.ActionSavesFetched = delegate
		{
		};
		ISN_GameSaves.ActionSavesResolved = delegate
		{
		};
		_CachedGameSaves = new Dictionary<string, GK_SavedGame>();
	}
}
