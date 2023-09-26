using UnityEngine;

public class InfoController : MonoBehaviour
{
	public string infoPrefix;

	private InfoBox[] infoBoxes;

	private int infoBoxCounter;

	private float infoboxTimer;

	private SplineWalker walker;

	private bool infosShown;

	private bool doShow;

	private InfoBox.InfoBoxDetails infoDetails;

	private void Awake()
	{
		doShow = false;
		infoboxTimer = -1f;
		if (!PlayerPrefs.HasKey(infoPrefix))
		{
			infoBoxes = base.transform.GetComponents<InfoBox>();
		}
		else
		{
			infosShown = true;
		}
	}

	public void MarkAsRead()
	{
		PlayerPrefs.SetInt(infoPrefix, 1);
		PlayerPrefs.Save();
	}

	public bool ShowInfos(float startWaitTime = 0.5f)
	{
		if ((doShow && !infosShown) || (infoDetails != null && infoDetails.popUp != null && infoDetails.popUp.isActiveAndEnabled))
		{
			return true;
		}
		doShow = !infosShown && infoBoxes != null && infoBoxes.Length > 0;
		if (doShow)
		{
			infoBoxCounter = 0;
			infoDetails = new InfoBox.InfoBoxDetails(infoBoxCounter, infoBoxes.Length);
			infoboxTimer = 0.01f + startWaitTime;
			MarkAsRead();
		}
		return doShow;
	}

	private void Update()
	{
		if (!doShow)
		{
			return;
		}
		infoboxTimer -= Time.deltaTime;
		if (!(infoboxTimer <= 0f))
		{
			return;
		}
		if (infoBoxCounter > infoBoxes.Length - 1 || infoDetails.skipped)
		{
			doShow = false;
			infosShown = true;
			return;
		}
		infoDetails.page = infoBoxCounter + 1;
		infoDetails.pageCount = infoBoxes.Length;
		infoBoxes[infoBoxCounter].ShowInfo(infoDetails);
		infoBoxCounter++;
		if (infoBoxCounter < infoBoxes.Length)
		{
			infoboxTimer = 0.01f;
		}
	}
}
