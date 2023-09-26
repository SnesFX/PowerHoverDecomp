using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IapPanel : MenuPanel
{
	public GameObject content;

	private bool Closing;

	private void Start()
	{
		content.SetActive(false);
	}

	public override void Activate(bool active)
	{
		base.Activate(active);
	}

	public void OpenPopUp(bool fromOptions = false)
	{
		Activate(true);
		GetComponent<Animator>().Play("IAPUnlockBoxAppear", -1, 0f);
		content.SetActive(true);
		if (UnityAnalyticsIntegration.Instance != null)
		{
			UnityAnalyticsIntegration.Instance.MakeEvent("PurchasePopUpOpened", new Dictionary<string, object>
			{
				{
					"stage",
					Main.Instance.CurrentScene
				},
				{
					"firstLocked",
					string.Empty
				},
				{ "fromOptions", fromOptions }
			});
		}
	}

	public void ClosePopUp()
	{
		Inputs.CloseAction.OnSubmit(new BaseEventData(EventSystem.current));
		Activate(false);
		if (DeviceSettings.Instance.RunningOnTV)
		{
			CustomInputController.Instance.PopPanel();
		}
	}

	public void CloseThis(float time)
	{
		if (!Closing)
		{
			Closing = true;
			GetComponent<Animator>().Play("IAPUnlockBoxClose", -1, 0f);
			StartCoroutine(PopUpDisable(content, time));
			Activate(false);
		}
	}

	private IEnumerator PopUpDisable(GameObject box, float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		box.SetActive(false);
		Closing = false;
	}
}
