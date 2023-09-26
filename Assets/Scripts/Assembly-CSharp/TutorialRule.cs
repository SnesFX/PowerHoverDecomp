using System;

[Serializable]
public class TutorialRule
{
	public int SuccessLimit;

	public string TutorialText;

	public MoveTrigger FailedTrigger;

	public int SuccessCounter { get; set; }

	public bool Complete()
	{
		if (SuccessCounter == SuccessLimit)
		{
			if (FailedTrigger != null)
			{
				FailedTrigger.gameObject.SetActive(false);
			}
			return true;
		}
		return false;
	}
}
