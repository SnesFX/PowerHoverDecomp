using System;
using UnityEngine;

public class StuckButtonVisibility : MonoBehaviour
{
	public static string STUCK_PREFIX = "StuckCounter";

	public static string STUCK_UNLOCKED_PREFIX = "StuckUnlocked";

	private const int VisibilityCount = 8;

	public GameObject content;

	public static StuckButtonVisibility Instance { get; private set; }

	public bool Unlocked
	{
		get
		{
			return CheckRules();
		}
	}

	private void Awake()
	{
		Instance = this;
		content.SetActive(false);
	}

	private void OnEnable()
	{
		if (GetStuckCounter() >= 8 && !CheckRules())
		{
			content.SetActive(true);
		}
		else
		{
			content.SetActive(false);
		}
	}

	public void SetVisible()
	{
		content.SetActive(true);
	}

	public void Hide()
	{
		content.SetActive(false);
	}

	private bool CheckRules()
	{
		if (GameDataController.Exists(STUCK_UNLOCKED_PREFIX) || (SceneLoader.Instance != null && SceneLoader.Instance.GetFirstLockedLevel(1) == null))
		{
			return true;
		}
		return false;
	}

	public int GetStuckCounter()
	{
		string identifier = string.Format("{0}_{1}", STUCK_PREFIX, DateTime.Today.DayOfYear);
		if (GameDataController.Exists(identifier))
		{
			return GameDataController.Load<int>(identifier);
		}
		return 0;
	}

	public void AddStuckCounter()
	{
		int obj = GetStuckCounter() + 1;
		string identifier = string.Format("{0}_{1}", STUCK_PREFIX, DateTime.Today.DayOfYear);
		GameDataController.Save(obj, identifier);
	}

	public void ClearStuckCounter()
	{
		string identifier = string.Format("{0}_{1}", STUCK_PREFIX, DateTime.Today.DayOfYear);
		if (GameDataController.Exists(identifier))
		{
			GameDataController.Delete(identifier);
		}
	}

	public void SetUnlocked()
	{
		GameDataController.Save(1, STUCK_UNLOCKED_PREFIX);
	}
}
