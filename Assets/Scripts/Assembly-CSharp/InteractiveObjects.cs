using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObjects : MonoBehaviour
{
	public enum SelectionType
	{
		Up = 0,
		Down = 1,
		Left = 2,
		Right = 3
	}

	[Serializable]
	public class Interactable
	{
		public GameObject obj;

		public bool above;
	}

	private const int VerticalDistanceMax = 47;

	private const int HorizontaDistanceMax = 99;

	public List<Interactable> InteractiveObjectList = new List<Interactable>();

	private MapEnergyIcon selectedMEI;

	private MapCutSceneCasette selectedMCSC;

	private MapLevelSelector lastSelected;

	private Interactable selectedObject;

	public bool LastCutSceneSelected { get; private set; }

	public bool GetClosestItem(Vector3 pos, SelectionType selecting)
	{
		float num = 9999f;
		int num2 = -1;
		Map componentInParent = GetComponentInParent<Map>();
		if ((!(selectedMEI != null) && !(selectedMCSC != null)) || (selecting != 0 && selecting != SelectionType.Down))
		{
			for (int i = 0; i < InteractiveObjectList.Count; i++)
			{
				if ((selecting == SelectionType.Up && !InteractiveObjectList[i].above) || (selecting == SelectionType.Down && InteractiveObjectList[i].above))
				{
					continue;
				}
				MapObject component = InteractiveObjectList[i].obj.GetComponent<MapObject>();
				if (component != null && component.Unlocked)
				{
					MapLifeGenerator component2 = component.GetComponent<MapLifeGenerator>();
					if (component2 == null || !component2.HeartItem.activeSelf)
					{
						continue;
					}
				}
				MapCutSceneCasette component3 = InteractiveObjectList[i].obj.GetComponent<MapCutSceneCasette>();
				if (!(component3 != null) || component3.Casette.activeSelf)
				{
					float num3 = Vector3.Distance(InteractiveObjectList[i].obj.transform.position, pos);
					if (num3 < num && num3 < (float)((selecting != 0 && selecting != SelectionType.Down) ? 99 : 47))
					{
						num2 = i;
						num = num3;
					}
				}
			}
		}
		if (num2 > -1)
		{
			MapCutSceneCasette component4 = InteractiveObjectList[num2].obj.GetComponent<MapCutSceneCasette>();
			if (component4 != null)
			{
				lastSelected = componentInParent.GetLastSelectedLevel();
				componentInParent.SelectCutsenePlayer(component4);
				selectedMCSC = component4;
				selectedMEI = null;
				LastCutSceneSelected = num2 == InteractiveObjectList.Count - 1;
				selectedObject = InteractiveObjectList[num2];
				return true;
			}
			MapObject component5 = InteractiveObjectList[num2].obj.GetComponent<MapObject>();
			if (component5 != null)
			{
				if (component5.Unlocked)
				{
					MapLifeGenerator component6 = component5.GetComponent<MapLifeGenerator>();
					if (component6.HeartItem.activeSelf)
					{
						component6.CollectLife();
						component5.MakeUnlockEffect();
						return true;
					}
				}
				else
				{
					MapEnergyIcon componentInChildren = InteractiveObjectList[num2].obj.GetComponentInChildren<MapEnergyIcon>();
					if (componentInChildren != null)
					{
						lastSelected = componentInParent.GetLastSelectedLevel();
						componentInParent.SelectMapEnergyObject(componentInChildren);
						selectedMEI = componentInChildren;
						selectedMCSC = null;
						selectedObject = InteractiveObjectList[num2];
						return true;
					}
				}
			}
		}
		else if (selectedObject != null && lastSelected != null && ((selecting == SelectionType.Down && selectedObject.above) || (selecting == SelectionType.Up && !selectedObject.above)))
		{
			componentInParent.ClearCutSceneSelector();
			componentInParent.ClearSelectedMapItem();
			componentInParent.SelectOnMap(lastSelected.sceneDetails.SceneName);
			ClearSelections();
			return true;
		}
		return false;
	}

	public void SelectLastAndClear()
	{
		Map componentInParent = GetComponentInParent<Map>();
		if (IsItemSelected() && componentInParent != null && lastSelected != null)
		{
			componentInParent.ClearCutSceneSelector();
			componentInParent.ClearSelectedMapItem();
			componentInParent.SelectOnMap(lastSelected.sceneDetails.SceneName);
		}
		ClearSelections();
	}

	public void ClearSelections()
	{
		lastSelected = null;
		selectedMCSC = null;
		selectedMEI = null;
		LastCutSceneSelected = false;
	}

	public bool IsItemSelected()
	{
		return selectedMEI != null || selectedMCSC != null;
	}

	public bool MakeAction()
	{
		if (selectedMCSC != null)
		{
			Map componentInParent = GetComponentInParent<Map>();
			componentInParent.SelectCutsenePlayer(selectedMCSC);
			if (lastSelected != null)
			{
				componentInParent.SelectOnMap(lastSelected.sceneDetails.SceneName);
			}
			selectedMCSC = null;
			return true;
		}
		if (selectedMEI != null)
		{
			Map componentInParent2 = GetComponentInParent<Map>();
			if (componentInParent2.PowerButtonCheck() && lastSelected != null)
			{
				componentInParent2.SelectOnMap(lastSelected.sceneDetails.SceneName);
				selectedMEI = null;
			}
			return true;
		}
		return false;
	}
}
