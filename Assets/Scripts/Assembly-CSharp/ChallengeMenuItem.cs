using System;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeMenuItem : MonoBehaviour
{
	[Serializable]
	public class ChallengeItem
	{
		public string ChallengeSceneName;

		public int BoltCost;

		public Sprite Levelimg;
	}

	private const string ANIM_LOCKED = "ChallengeLocked";

	private const string ANIM_OPENED = "ChallengeOpened";

	private const string ANIM_UNLOCK = "ChallengeUnlock";

	public ChallengeItem[] challengeDetails;

	public Text ChallengeName;

	public Text RankText;

	public Text BestScore;

	public Text NextLevel;

	public Text CurrenentLvl;

	public Text ChallegeDescription;

	public Image LevelImage;

	public Renderer LockRenderer;

	public GameObject LockedObject;

	public Button UnlockButton;

	public ChallengeCharacterPrizeUI prizeUI;

	public GameObject Playbutton;

	public AudioSource buttonAudio;

	public Animator unlockAnimator;

	public InfoBox NoMoneyInfo;

	public IAPBuyBolts iAPBuyBolts;

	public AudioSource changeAudio;

	public GameObject outOfTriesPopUp;

	public Button previousButton;

	public Button nextButton;

	private int currentChallenge;

	public void NextChallenge()
	{
		if (currentChallenge < challengeDetails.Length - 1)
		{
			currentChallenge++;
		}
		SetUnlock();
		changeAudio.Play();
	}

	public void PreviousChallenge()
	{
		if (currentChallenge > 0)
		{
			currentChallenge--;
		}
		SetUnlock();
		changeAudio.Play();
	}

	private void SetupButtons()
	{
		if (currentChallenge == 0)
		{
			previousButton.interactable = false;
			if (!nextButton.interactable)
			{
				EnableButton(nextButton);
			}
			return;
		}
		if (currentChallenge >= challengeDetails.Length - 1)
		{
			nextButton.interactable = false;
			if (!previousButton.interactable)
			{
				EnableButton(previousButton);
			}
			return;
		}
		if (!nextButton.interactable)
		{
			EnableButton(nextButton);
		}
		if (!previousButton.interactable)
		{
			EnableButton(previousButton);
		}
	}

	private void EnableButton(Button but)
	{
		but.interactable = true;
		Animator component = but.GetComponent<Animator>();
		if (component != null)
		{
			component.ResetTrigger("Highlighted");
			component.SetTrigger("Normal");
		}
	}

	public void SetUnlock()
	{
		int num = currentChallenge;
		SetupButtons();
		ChallengeItem challengeItem = challengeDetails[num];
		SceneDetails sceneDetails = SceneLoader.Instance.GetSceneDetails(challengeItem.ChallengeSceneName);
		NextLevel.text = string.Format("{0}", SceneLoader.Instance.GetChallengeLevelLimit(sceneDetails));
		CurrenentLvl.text = string.Format("LVL {0}", 1 + sceneDetails.Storage.CasetteState);
		LevelImage.sprite = challengeItem.Levelimg;
		int num2 = 0;
		string localeID = "MainMenu.ChallengeDescriptionCollect";
		switch ((ChallengeType)sceneDetails.Group)
		{
		case ChallengeType.Distance:
		case ChallengeType.DontFall:
			localeID = "MainMenu.ChallengeDescriptionDistance";
			num2 = Mathf.FloorToInt(sceneDetails.Storage.BestDistance);
			break;
		case ChallengeType.Collect:
			localeID = "MainMenu.ChallengeDescriptionCollect";
			num2 = Mathf.FloorToInt(sceneDetails.Storage.HighScore);
			break;
		case ChallengeType.DontMiss:
			localeID = "MainMenu.ChallengeDescriptionBreakables";
			num2 = Mathf.FloorToInt(sceneDetails.Storage.HighScore);
			break;
		}
		ChallengeName.text = challengeItem.ChallengeSceneName;
		LocalizationLoader.Instance.SetText(ChallegeDescription, localeID);
		BestScore.text = string.Format("{0}", num2);
		if (challengeItem.BoltCost == 0 || sceneDetails.Storage.IsOpen)
		{
			UpdateRankingText();
			unlockAnimator.Play("ChallengeOpened", -1, 0f);
			Playbutton.SetActive(true);
			prizeUI.SetStat(false, 0);
			SetStatTexts(true);
		}
		else
		{
			unlockAnimator.Play("ChallengeLocked", -1, 0f);
			Playbutton.SetActive(false);
			prizeUI.SetStat(true, challengeItem.BoltCost);
			SetStatTexts(false);
		}
	}

	private void SetStatTexts(bool active)
	{
		LockedObject.SetActive(!active);
		UnlockButton.interactable = !active;
		RankText.transform.parent.gameObject.SetActive(active);
		BestScore.transform.parent.gameObject.SetActive(active);
		NextLevel.transform.parent.gameObject.SetActive(active);
	}

	public void UnlockChallenge()
	{
		ChallengeItem challengeItem = challengeDetails[currentChallenge];
		if (GameStats.Instance.ChallengeMoney >= challengeItem.BoltCost)
		{
			GameStats.Instance.ChallengeMoney -= challengeItem.BoltCost;
			prizeUI.SetStat(false, 0);
			SetStatTexts(true);
			Playbutton.SetActive(true);
			unlockAnimator.Play("ChallengeUnlock", -1, 0f);
			SceneDetails sceneDetails = SceneLoader.Instance.GetSceneDetails(challengeItem.ChallengeSceneName);
			sceneDetails.Storage.IsOpen = true;
			SceneLoader.Instance.SaveChallenge(challengeItem.ChallengeSceneName, sceneDetails.Storage);
			GPSSaveGame.Instance.SaveGame();
		}
		else
		{
			iAPBuyBolts.OpenPopUp();
		}
	}

	public void StartChallenge()
	{
		if (!SceneLoader.Instance.IsLoading && !CutSceneLoader.Instance.LoadingOperation)
		{
			if (SceneLoader.Instance.TriesLeft > 0 || IapManager.Instance.Unlocked)
			{
				ChallengeItem challengeItem = challengeDetails[currentChallenge];
				buttonAudio.Play();
				SceneLoader.Instance.LoadChallenge(challengeItem.ChallengeSceneName);
			}
			else if (SceneLoader.Instance.TriesLeft <= 0 && !IapManager.Instance.Unlocked)
			{
				outOfTriesPopUp.SetActive(true);
			}
		}
	}

	private void UpdateRankingText()
	{
		ChallengeItem challengeItem = challengeDetails[currentChallenge];
		int leaderboardRanking = GameCenter.Instance.GetLeaderboardRanking(challengeItem.ChallengeSceneName);
		if (leaderboardRanking > 0)
		{
			RankText.text = string.Format("#{0}", leaderboardRanking.ToString());
		}
		else
		{
			RankText.text = "-";
		}
	}

	public void OpenGameCenter()
	{
		ChallengeItem challengeItem = challengeDetails[currentChallenge];
		GameCenter.Instance.ShowLeaderboardsUI(challengeItem.ChallengeSceneName);
	}
}
