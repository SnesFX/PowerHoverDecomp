using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class IAPTriesPopUp : MonoBehaviour
{
	public void ClosePopUp()
	{
		GetComponent<MenuPanel>().Inputs.CloseAction.OnSubmit(new BaseEventData(EventSystem.current));
	}

	public void CloseThis(float time)
	{
		StartCoroutine(PopUpDisable(base.gameObject, time));
	}

	private IEnumerator PopUpDisable(GameObject box, float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		box.SetActive(false);
	}
}
