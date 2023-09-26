using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IAPBuyBolts : MonoBehaviour
{
	public Button openButton;

	public void ClosePopUp()
	{
		GetComponent<MenuPanel>().Inputs.CloseAction.OnSubmit(new BaseEventData(EventSystem.current));
	}

	public void OpenPopUp()
	{
		openButton.OnSubmit(new BaseEventData(EventSystem.current));
	}
}
