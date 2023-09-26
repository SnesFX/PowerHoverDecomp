using UnityEngine;
using UnityEngine.UI;

public class ChallengeStartInfo : MonoBehaviour
{
	public Text challengeTarget;

	public Text challengeInfo;

	public Text passBonus;

	public Text currentLvl;

	private void OnEnable()
	{
		if (!(SceneLoader.Instance == null) && !(Main.Instance == null))
		{
			string localeID = "MainMenu.ChallengeDescriptionCollect";
			switch ((ChallengeType)SceneLoader.Instance.Current.Group)
			{
			case ChallengeType.Distance:
			case ChallengeType.DontFall:
				localeID = "MainMenu.ChallengeDescriptionDistance";
				break;
			case ChallengeType.Collect:
				localeID = "MainMenu.ChallengeDescriptionCollect";
				break;
			case ChallengeType.DontMiss:
				localeID = "MainMenu.ChallengeDescriptionBreakables";
				break;
			}
			LocalizationLoader.Instance.SetText(challengeInfo, localeID);
			challengeTarget.text = string.Format("{0}", SceneLoader.Instance.GetChallengeLevelLimit(Main.Instance.CurrentScene));
			passBonus.text = string.Format("{0}", (SceneLoader.Instance.Current.Storage.CasetteState + 1) * 1000);
			currentLvl.text = string.Format("LVL {0}", 1 + SceneLoader.Instance.Current.Storage.CasetteState);
		}
	}
}
