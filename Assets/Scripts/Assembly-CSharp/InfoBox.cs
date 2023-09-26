using SmartLocalization;
using UnityEngine;

public class InfoBox : ResetObject
{
	public class InfoBoxDetails
	{
		public int page;

		public int pageCount;

		public bool pauseHidden;

		public string text;

		public PopUpController popUp;

		public bool skipped;

		public InfoBoxDetails(int pageNumber, int count)
		{
			page = pageNumber;
			pageCount = count;
		}
	}

	public Color gizmoColor = new Color(0f, 0.9f, 0.5f, 0.75f);

	public string popupDebugText;

	public string LocalizationID;

	public bool ShowOnEveryRun;

	private string infoPrefix;

	private Collider coll;

	private void Awake()
	{
		infoPrefix = ((LocalizationID == null || LocalizationID.Equals(string.Empty)) ? popupDebugText : LocalizationID);
		coll = GetComponent<Collider>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			ShowInfo();
			coll.enabled = false;
		}
	}

	public void MarkAsRead()
	{
		PlayerPrefs.SetInt(infoPrefix, 1);
		PlayerPrefs.Save();
	}

	public bool ShowInfo()
	{
		return ShowInfo(new InfoBoxDetails(1, 1));
	}

	public bool ShowInfo(InfoBoxDetails details)
	{
		details.popUp = Main.Instance.ShowPopUp(MenuType.PopUp) as PopUpController;
		if (UIController.Instance != null && UIController.Instance.buttonController.PauseButton.activeSelf)
		{
			UIController.Instance.buttonController.PauseButton.SetActive(false);
			details.pauseHidden = true;
		}
		details.text = popupDebugText;
		if (LocalizationID != null && !LocalizationID.Equals(string.Empty))
		{
			details.text = LanguageManager.Instance.GetTextValue(LocalizationID);
		}
		else
		{
			details.text = popupDebugText;
		}
		details.popUp.SetText(details);
		return details.pauseHidden;
	}

	public override void Reset(bool isRewind)
	{
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = gizmoColor;
		if (GetComponent<BoxCollider>() != null)
		{
			BoxCollider component = GetComponent<BoxCollider>();
			if (component.enabled)
			{
				Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, base.transform.lossyScale);
				Gizmos.DrawCube(component.center, component.size);
			}
		}
	}
}
