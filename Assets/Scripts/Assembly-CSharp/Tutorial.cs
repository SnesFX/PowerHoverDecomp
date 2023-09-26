using System.Collections.Generic;
using SmartLocalization;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
	public Image tutorialBox;

	public Color boxSuccessColor;

	public Color boxFailColor;

	public TypeWriter writer;

	public Text[] hiddenTexts;

	public string[] hiddenTextIds;

	public List<TutorialRule> Rules;

	public string WelcomeText;

	public string CompletedText;

	public GameObject tutoBox;

	private int RuleCounter;

	private Color defaultColor;

	private float successTimer;

	private float clearState;

	private HoverController hoverController;

	private int TutorialCompleted;

	private AudioSource audioEffect;

	private float startTextTimer;

	private float ruleCheckTimer;

	private void Start()
	{
		hoverController = Object.FindObjectOfType<HoverController>();
		audioEffect = GetComponent<AudioSource>();
		defaultColor = tutorialBox.color;
		RuleCounter = 0;
		startTextTimer = 8.5f;
		LocalizationLoader.Instance.SetText(writer.GetComponent<Text>(), "MainMenu.On");
		string textValue = LanguageManager.Instance.GetTextValue(WelcomeText);
		writer.StartWriting(textValue, 3.8f);
		TutorialCompleted = 0;
		for (int i = 0; i < hiddenTexts.Length; i++)
		{
			LocalizationLoader.Instance.SetText(hiddenTexts[i], hiddenTextIds[i]);
		}
	}

	private void Update()
	{
		if (GameController.Instance.State == GameController.GameState.Paused)
		{
			tutoBox.SetActive(false);
		}
		else if (TutorialCompleted < 2 && !tutoBox.activeSelf)
		{
			tutoBox.SetActive(true);
		}
	}

	private void FixedUpdate()
	{
		if (startTextTimer > 0f)
		{
			startTextTimer -= Time.fixedDeltaTime;
			if (startTextTimer < 0f)
			{
				string textValue = LanguageManager.Instance.GetTextValue(Rules[RuleCounter].TutorialText);
				writer.StartWriting(textValue, 0.1f);
				ruleCheckTimer = 1f;
			}
			return;
		}
		ruleCheckTimer -= Time.fixedDeltaTime;
		if (clearState > 0f)
		{
			clearState -= Time.fixedDeltaTime;
			if (clearState <= 0f)
			{
				if (writer.FaceAnimator.GetInteger("State") == 2)
				{
					tutorialBox.color = defaultColor;
				}
				writer.FaceAnimator.SetInteger("State", 0);
			}
		}
		if (successTimer > 0f)
		{
			successTimer -= Time.fixedDeltaTime;
			if (!(successTimer <= 0f))
			{
				return;
			}
			if (TutorialCompleted == 0)
			{
				tutorialBox.color = defaultColor;
				string textValue2 = LanguageManager.Instance.GetTextValue(Rules[RuleCounter].TutorialText);
				writer.StartWriting(textValue2, 0.3f);
			}
			else if (TutorialCompleted == 1)
			{
				tutoBox.SetActive(false);
				TutorialCompleted = 2;
				successTimer = 1f;
				if (AudioController.Instance != null)
				{
					AudioController.Instance.SetListener(true);
				}
				hoverController.soundController.DisableAudioListener();
				if (!GameDataController.Exists("LastScene"))
				{
					GameDataController.Save(true, "FirstStartMusic");
					GameDataController.Save("Tutorial", "LastScene");
				}
			}
			else if (TutorialCompleted != 2)
			{
			}
		}
		else
		{
			if (TutorialCompleted > 0)
			{
				return;
			}
			if (ruleCheckTimer <= 0f && RuleCounter < Rules.Count && Rules[RuleCounter].Complete())
			{
				audioEffect.Play();
				writer.FaceAnimator.SetInteger("State", 3);
				clearState = 0.5f;
				if (RuleCounter < Rules.Count - 1)
				{
					RuleCounter++;
					successTimer = 2.5f;
				}
				else
				{
					TutorialCompleted = 1;
					writer.StartWriting(LanguageManager.Instance.GetTextValue(CompletedText), 0.1f);
					successTimer = 8f;
				}
				tutorialBox.color = boxSuccessColor;
			}
			else
			{
				if (ruleCheckTimer > 0f)
				{
					return;
				}
				TutorialRule tutorialRule = Rules[RuleCounter];
				switch (RuleCounter)
				{
				case 0:
					if (hoverController.animationController.easing < -2f || hoverController.animationController.easing > 2f)
					{
						RuleCounter++;
						tutorialRule.SuccessCounter++;
						if (DeviceSettings.Instance != null && !DeviceSettings.Instance.RunningOnTV && !DeviceSettings.Instance.EnableInputDevices)
						{
							UIController.Instance.buttonController.BlinkButtons(-1f, false, false);
						}
					}
					else if (DeviceSettings.Instance != null && !DeviceSettings.Instance.RunningOnTV && !DeviceSettings.Instance.EnableInputDevices)
					{
						UIController.Instance.buttonController.BlinkButtons(1f);
					}
					break;
				case 1:
					if (hoverController.animationController.easing > 2f)
					{
						tutorialRule.SuccessCounter++;
					}
					if (DeviceSettings.Instance != null && !DeviceSettings.Instance.RunningOnTV && !DeviceSettings.Instance.EnableInputDevices)
					{
						UIController.Instance.buttonController.BlinkButtons(0.4f);
					}
					break;
				case 2:
					if (hoverController.walker.TF > 0.395f)
					{
						tutorialRule.SuccessCounter++;
					}
					break;
				case 3:
					if (LevelStats.Instance.CollectebleCollectCount == 1)
					{
						tutorialRule.SuccessCounter++;
					}
					break;
				case 4:
					if (LevelStats.Instance.CollectebleCollectCount == 4)
					{
						tutorialRule.SuccessCounter++;
					}
					break;
				case 5:
					if (hoverController.animationController.animator.GetBool(hoverController.animationController.grindingHash) && hoverController.grindControl.completion > 0.5f)
					{
						tutorialRule.SuccessCounter++;
					}
					break;
				}
			}
		}
	}

	public void Failed()
	{
		writer.FaceAnimator.SetInteger("State", 2);
		tutorialBox.color = boxFailColor;
		clearState = 0.5f;
	}
}
