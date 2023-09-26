using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IAPBoltsGainedPopUp : MonoBehaviour
{
	public GameObject content;

	public GameObject triesGained;

	public GameObject boltsGained;

	public Text boltCount;

	private float autoCloseTimer;

	public void ClosePopUp()
	{
		GetComponent<MenuPanel>().Inputs.CloseAction.OnSubmit(new BaseEventData(EventSystem.current));
	}

	public void OpenPopUp(bool tries, int amount = 0)
	{
		triesGained.SetActive(tries);
		boltsGained.SetActive(!tries);
		boltCount.text = string.Format("{0}", amount);
		content.SetActive(true);
		content.GetComponent<Animator>().Play("GainedPopUpAppear", -1, 0f);
		autoCloseTimer = 0f;
	}

	private void Update()
	{
		if (DeviceSettings.Instance != null && DeviceSettings.Instance.RunningOnTV && autoCloseTimer <= 2f)
		{
			autoCloseTimer += Time.deltaTime;
			if (autoCloseTimer > 2f)
			{
				ClosePopUp();
			}
		}
	}

	public void CloseThis(float time)
	{
		StartCoroutine(PopUpDisable(time));
	}

	private IEnumerator PopUpDisable(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		triesGained.SetActive(false);
		boltsGained.SetActive(false);
	}
}
