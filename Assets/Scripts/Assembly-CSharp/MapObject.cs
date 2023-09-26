using UnityEngine;

public class MapObject : MonoBehaviour
{
	private static string[] TipLocalizationIds = new string[6]
	{
		"Map.ObjectTip.Magnet",
		"Map.ObjectTip.LifeGenerator",
		"Map.ObjectTip.StartLife",
		"Map.ObjectTip.Lifesaver",
		string.Empty,
		"Map.ObjectTip.Character"
	};

	public int BatteryCost;

	public PlayerStatType StatToGet;

	public GameObject[] ActivateObjects;

	public GameObject[] DeActivateObjects;

	public string title;

	public string description;

	public InfoBox notification;

	public Camera roadCamera;

	private MapItemUnlock unlockEffect;

	private GameObject mapItemTip;

	public string ObjectIdentifier { get; private set; }

	public MapEnergyIcon energyIcon { get; private set; }

	public bool Unlocked { get; set; }

	public int defaultCost { get; private set; }

	private void Awake()
	{
		if (notification == null)
		{
			mapItemTip = Object.Instantiate(Resources.Load("MapItemTip")) as GameObject;
			mapItemTip.transform.parent = base.transform;
			mapItemTip.transform.localPosition = Vector3.zero;
			mapItemTip.GetComponent<InfoBox>().LocalizationID = TipLocalizationIds[(int)StatToGet];
			mapItemTip.SetActive(false);
		}
		defaultCost = BatteryCost;
		energyIcon = GetComponentInChildren<MapEnergyIcon>();
		energyIcon.transform.parent = null;
		energyIcon.transform.localScale = Vector3.one * 1.25f;
		energyIcon.transform.parent = base.transform;
		energyIcon.Cam = roadCamera;
		unlockEffect = GetComponentInChildren<MapItemUnlock>();
		if (unlockEffect != null)
		{
			unlockEffect.transform.parent = null;
			unlockEffect.transform.localScale = Vector3.one * 2f;
			unlockEffect.transform.rotation = Quaternion.Euler(0f, 228.7f, 0f);
			unlockEffect.transform.parent = base.transform;
			unlockEffect.currentType = StatToGet;
			unlockEffect.gameObject.SetActive(false);
		}
		ObjectIdentifier = string.Format("MapO_{0}", base.transform.position);
		if (GameDataController.Exists(ObjectIdentifier))
		{
			BatteryCost = GameDataController.Load<int>(ObjectIdentifier);
			Unlock(false);
		}
		energyIcon.SetMapObject(this);
	}

	public void EnableTip(bool enable)
	{
		if (mapItemTip != null)
		{
			mapItemTip.SetActive(enable);
		}
	}

	public bool IsObject(string compareTo)
	{
		if (compareTo != null && compareTo.Length > 1 && compareTo.Contains("_"))
		{
			string[] array = compareTo.Split('_');
			if (array[1].Length > 2 && array[1].Contains("(") && array[1].Contains(")"))
			{
				array[1] = array[1].Substring(1, array[1].Length - 2);
				if (array[1].Contains(","))
				{
					string[] array2 = array[1].Split(',');
					Vector3 a = new Vector3(float.Parse(array2[0]), float.Parse(array2[1]), float.Parse(array2[2]));
					if (Vector3.Distance(a, base.transform.position) < 2f)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public bool Unlock(bool isNew = true, bool fromSych = false)
	{
		if (Unlocked)
		{
			return false;
		}
		if (mapItemTip != null)
		{
			mapItemTip.SetActive(true);
		}
		Unlocked = true;
		bool flag = GameDataController.Exists(ObjectIdentifier);
		GameDataController.Save(0, ObjectIdentifier);
		energyIcon.gameObject.SetActive(false);
		if (isNew)
		{
			MakeUnlockEffect();
		}
		if ((isNew || !flag) && StatToGet != PlayerStatType.Character && StatToGet != PlayerStatType.Lives)
		{
			PlayerStats.Instance.UpdateStat(StatToGet);
		}
		if (StatToGet == PlayerStatType.Lives)
		{
			if (defaultCost > 10)
			{
				GetComponent<MapLifeGenerator>().EnableLifeGenerator();
			}
			else if (isNew || !flag)
			{
				PlayerStats.Instance.UpdateStat(StatToGet);
			}
		}
		if (ActivateObjects != null)
		{
			for (int i = 0; i < ActivateObjects.Length; i++)
			{
				ActivateObjects[i].SetActive(true);
				if (isNew && ActivateObjects[i].GetComponent<AudioSource>() != null)
				{
					ActivateObjects[i].GetComponent<AudioSource>().Play();
				}
			}
		}
		if (DeActivateObjects != null)
		{
			for (int j = 0; j < DeActivateObjects.Length; j++)
			{
				DeActivateObjects[j].SetActive(false);
			}
		}
		return true;
	}

	public void MakeUnlockEffect()
	{
		if (unlockEffect != null)
		{
			unlockEffect.StartEffect();
		}
	}

	public void Reset()
	{
		Unlocked = false;
		BatteryCost = defaultCost;
		if (GameDataController.Exists(ObjectIdentifier))
		{
			GameDataController.Delete(ObjectIdentifier);
		}
		energyIcon.gameObject.SetActive(true);
		if (ActivateObjects != null)
		{
			for (int i = 0; i < ActivateObjects.Length; i++)
			{
				ActivateObjects[i].SetActive(false);
			}
		}
		if (DeActivateObjects != null)
		{
			for (int j = 0; j < DeActivateObjects.Length; j++)
			{
				DeActivateObjects[j].SetActive(true);
			}
		}
	}
}
