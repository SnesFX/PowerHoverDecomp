using UnityEngine;
using UnityEngine.UI;

public class IngameMenuButtonController : MonoBehaviour
{
	private const float BlinkSpeed = 26f;

	public GameObject PausedNormal;

	public GameObject PausedEndless;

	public GameObject GameOver;

	public GameObject LeftButton;

	public GameObject RightButton;

	public GameObject PauseButton;

	public SocialShare SocialSharing;

	public MenuButton ExitToMenu;

	public MenuButton Rewind;

	public MenuButton Restart;

	public MenuButton Continue;

	public MenuButton NextLevel;

	public MenuButton MainMenuCorner;

	public MenuButton PlayBig;

	public MenuButton BackUpButton;

	public MenuButton LifeSaver;

	public MenuButton LifeSaverAd;

	public MenuButton RestartEnd;

	public MenuButton GameOverReset;

	public MenuButton AdForLifeButton;

	public MenuButton AdForRetries;

	public MenuButton BoltsForRetries;

	public Text endlessTryCounter;

	private MeshRenderer leftButtonImage;

	private MeshRenderer rightButtonImage;

	private float blinkTimer;

	private bool blinkLeft;

	private bool blinkRight;

	private void Start()
	{
		blinkTimer = 0f;
		leftButtonImage = LeftButton.GetComponentInChildren<MeshRenderer>();
		rightButtonImage = RightButton.GetComponentInChildren<MeshRenderer>();
		if (leftButtonImage != null)
		{
			leftButtonImage.enabled = false;
		}
		if (rightButtonImage != null)
		{
			rightButtonImage.enabled = false;
		}
		if (DeviceSettings.Instance != null && DeviceSettings.Instance.RunningOnTV)
		{
			Vector3 position = PauseButton.transform.position;
			position.x -= 10f;
			PauseButton.transform.position = position;
		}
	}

	private void Update()
	{
		if (blinkTimer > 0f && leftButtonImage != null)
		{
			blinkTimer -= Time.deltaTime;
			if (blinkRight)
			{
				rightButtonImage.enabled = !(Mathf.Sin(Time.time * 26f) >= 0f) && !(blinkTimer <= 0f);
			}
			if (blinkLeft)
			{
				leftButtonImage.enabled = !(Mathf.Sin(Time.time * 26f) >= 0f) && !(blinkTimer <= 0f);
			}
		}
	}

	public void BlinkButtons(float timer, bool left = true, bool right = true)
	{
		blinkLeft = left;
		blinkRight = right;
		blinkTimer = timer;
		if (timer <= 0f && leftButtonImage != null)
		{
			MeshRenderer meshRenderer = leftButtonImage;
			bool flag = false;
			rightButtonImage.enabled = flag;
			meshRenderer.enabled = flag;
		}
	}

	public void SetMenuButtons(UIController.IngameMenuState menu)
	{
		switch (menu)
		{
		case UIController.IngameMenuState.None:
		case UIController.IngameMenuState.Normal:
			ExitToMenu.Show(false);
			MainMenuCorner.Show(false);
			Rewind.Show(false);
			Restart.Show(false);
			RestartEnd.Show(false);
			Continue.Show(false);
			NextLevel.Show(false);
			LifeSaver.Show(false);
			BackUpButton.Show(false);
			LifeSaverAd.Show(false);
			if (AdForRetries != null)
			{
				AdForRetries.Show(false);
			}
			if (BoltsForRetries != null)
			{
				BoltsForRetries.Show(false);
			}
			PausedEndless.SetActive(false);
			PausedNormal.SetActive(false);
			GameOver.SetActive(false);
			if (AdForLifeButton != null)
			{
				AdForLifeButton.Show(false);
			}
			if (SocialSharing != null)
			{
				SocialSharing.ShowSharing(false);
			}
			PauseButton.SetActive(menu == UIController.IngameMenuState.Normal);
			if (Main.Instance != null && Main.Instance.TutorialLevel && SceneLoader.Instance != null && !SceneLoader.Instance.ManualStartTutorial)
			{
				PauseButton.SetActive(false);
			}
			LeftButton.SetActive(menu == UIController.IngameMenuState.Normal);
			RightButton.SetActive(menu == UIController.IngameMenuState.Normal);
			if (menu == UIController.IngameMenuState.Normal && Main.Instance != null && !Main.Instance.TutorialLevel && Main.Instance.CurrentScene.Equals("Hover19"))
			{
				BlinkButtons(2f);
			}
			break;
		case UIController.IngameMenuState.Paused:
			MainMenuCorner.Show(false);
			ExitToMenu.Show(false);
			Rewind.Show(false);
			Restart.Show(false);
			Continue.Show(false);
			NextLevel.Show(false);
			PlayBig.Show(false);
			LifeSaver.Show(false);
			LifeSaverAd.Show(false);
			BackUpButton.Show(false);
			RestartEnd.Show(false);
			GameOver.SetActive(false);
			if (AdForRetries != null)
			{
				AdForRetries.Show(false);
			}
			if (BoltsForRetries != null)
			{
				BoltsForRetries.Show(false);
			}
			if (AdForLifeButton != null)
			{
				AdForLifeButton.Show(!SceneLoader.Instance.Current.IsChallenge && !SceneLoader.Instance.Current.IsEndless && UIController.Instance.AdForLifeUsage < 3 && UnityAdsIngetration.Instance != null && !DeviceSettings.Instance.RunningOnTV && UnityAdsIngetration.Instance.IsAdsActivated && UnityAdsIngetration.Instance.IsReady());
			}
			PausedEndless.SetActive(SceneLoader.Instance.Current.IsEndless || SceneLoader.Instance.Current.IsChallenge);
			PausedNormal.SetActive(!SceneLoader.Instance.Current.IsEndless && !SceneLoader.Instance.Current.IsChallenge);
			if (SocialSharing != null)
			{
				SocialSharing.ShowSharing(true);
			}
			PauseButton.SetActive(false);
			LeftButton.SetActive(false);
			RightButton.SetActive(false);
			break;
		case UIController.IngameMenuState.End:
		{
			MainMenuCorner.Show(false);
			PauseButton.SetActive(false);
			LeftButton.SetActive(false);
			RightButton.SetActive(false);
			LifeSaver.Show(false);
			LifeSaverAd.Show(false);
			PausedEndless.SetActive(false);
			PausedNormal.SetActive(false);
			GameOver.SetActive(false);
			if (BoltsForRetries != null)
			{
				BoltsForRetries.Show(false);
			}
			RestartEnd.Show(true);
			if (SocialSharing != null)
			{
				SocialSharing.ShowSharing(true);
			}
			if (SceneLoader.Instance.Current.IsChallenge)
			{
				if (SceneLoader.Instance.TriesLeft > 0 || !UnityAdsIngetration.Instance.IsAdsActivated)
				{
					endlessTryCounter.text = string.Format("{0}", SceneLoader.Instance.TriesLeft);
					AdForRetries.Show(false);
					BoltsForRetries.Show(false);
				}
				else
				{
					endlessTryCounter.text = string.Empty;
					RestartEnd.Show(false);
					AdForRetries.Show(UnityAdsIngetration.Instance.IsAdsActivated);
					BoltsForRetries.Show(GameStats.Instance.ChallengeMoney >= 1000);
				}
				if (!UnityAdsIngetration.Instance.IsAdsActivated)
				{
					endlessTryCounter.text = string.Empty;
				}
			}
			else
			{
				if (AdForRetries != null && SceneLoader.Instance.Current.IsEndless && SceneLoader.Instance.Current.Storage.BestTime <= 0f)
				{
					AdForRetries.Show(true);
					RestartEnd.Show(false);
				}
				else
				{
					AdForRetries.Show(false);
					if (UnityAdsIngetration.Instance.IsAdsActivated)
					{
						endlessTryCounter.text = string.Format("{0}", Mathf.RoundToInt(SceneLoader.Instance.Current.Storage.BestTime));
					}
				}
				if (!SceneLoader.Instance.Current.IsEndless || !UnityAdsIngetration.Instance.IsAdsActivated)
				{
					endlessTryCounter.text = string.Empty;
				}
			}
			ExitToMenu.Show(true);
			Restart.Show(false);
			if (SceneLoader.Instance.Current.IsChallenge && GameController.Instance.GetComponentInChildren<HoverController>().PlayerState != PlayerState.Dying)
			{
				RestartEnd.Show(false);
			}
			Continue.Show(false);
			SceneDetails nextLevel = SceneLoader.Instance.GetNextLevel(SceneLoader.Instance.Current.SceneName, SceneLoader.Instance.Current.Group);
			NextLevel.Show(nextLevel != null && nextLevel.Storage.IsOpen);
			break;
		}
		case UIController.IngameMenuState.PopUp:
			PauseButton.SetActive(false);
			LeftButton.SetActive(false);
			RightButton.SetActive(false);
			LifeSaver.Show(false);
			LifeSaverAd.Show(false);
			BackUpButton.Show(false);
			PausedEndless.SetActive(false);
			PausedNormal.SetActive(false);
			GameOver.SetActive(false);
			if (SocialSharing != null)
			{
				SocialSharing.ShowSharing(false);
			}
			NextLevel.Show(false);
			MainMenuCorner.Show(false);
			break;
		case UIController.IngameMenuState.Killed:
			PauseButton.SetActive(false);
			LeftButton.SetActive(true);
			RightButton.SetActive(true);
			NextLevel.Show(false);
			LifeSaver.Show(false);
			LifeSaverAd.Show(false);
			BackUpButton.Show(false);
			if (AdForLifeButton != null)
			{
				AdForLifeButton.Show(!SceneLoader.Instance.Current.IsChallenge && !SceneLoader.Instance.Current.IsEndless && UIController.Instance.AdForLifeUsage < 3 && UnityAdsIngetration.Instance != null && !DeviceSettings.Instance.RunningOnTV && UnityAdsIngetration.Instance.IsAdsActivated && UnityAdsIngetration.Instance.IsReady());
			}
			if (SocialSharing != null)
			{
				SocialSharing.ShowSharing(false);
			}
			break;
		case UIController.IngameMenuState.GameOver:
			ExitToMenu.Show(false);
			NextLevel.Show(false);
			PauseButton.SetActive(false);
			LeftButton.SetActive(false);
			RightButton.SetActive(false);
			if (SocialSharing != null)
			{
				SocialSharing.ShowSharing(false);
			}
			Restart.Show(false);
			LifeSaver.Show(true);
			GameOver.SetActive(true);
			if (AdForLifeButton != null)
			{
				AdForLifeButton.Show(false);
			}
			BackUpButton.Show(!UnityAdsIngetration.Instance.IsAdsActivated && LifeController.Instance.LifeSavers > 0);
			GameOverReset.Show(LifeController.Instance.LifeSavers <= 0 && !UnityAdsIngetration.Instance.IsAdsActivated);
			break;
		}
	}
}
