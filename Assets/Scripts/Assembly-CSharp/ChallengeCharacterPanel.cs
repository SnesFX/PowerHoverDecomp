using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeCharacterPanel : MonoBehaviour
{
	private const string CHARACTER_PREFIX = "ChallengeCharacter";

	public CharacterMenuItem[] Characters;

	public ChallengeCharacterStatUI CharacterStatUI;

	public ChallengeCharacterPrizeUI CharacterPrizeUI;

	public Button previousButton;

	public Button nextButton;

	public Button activateCharacter;

	public Button unlockCharacter;

	public GameObject active;

	public GameObject activateBg;

	public AudioSource changeAudio;

	public Material LockedMaterial;

	public GameObject menuCharacter;

	public InfoBox NotEnoughInfo;

	public IAPBuyBolts iAPBuyBolts;

	private int activeCharacter;

	private int currentCharacter;

	private Vector3 starPos;

	public static ChallengeCharacterPanel Instance { get; private set; }

	private void Awake()
	{
		currentCharacter = (GameDataController.Exists("ChallengeCharacter") ? GameDataController.Load<int>("ChallengeCharacter") : 0);
		activeCharacter = currentCharacter;
		starPos = menuCharacter.transform.localPosition;
		SetupCharacters();
		Instance = this;
	}

	private void Start()
	{
		SetIngameCharacter();
	}

	public void SetIngameCharacter()
	{
		TrickController.Instance.ChallengeCharacter = Characters[currentCharacter].Character;
	}

	private void OnEnable()
	{
		for (int i = 0; i < Characters.Length; i++)
		{
			Characters[i].characterObject.CharacterMenuAnimator.Play((currentCharacter != i) ? "CharacterFadeOut" : "CharacterInCenter");
			if (currentCharacter != i)
			{
				StartCoroutine(DisableCharacter(i));
			}
			Characters[i].characterObject.CharacterRenderer.material = ((!Characters[i].Character.IsLocked) ? Characters[i].Character.CharacterMainMaterial : LockedMaterial);
		}
		SetActiveCharacter();
	}

	private IEnumerator DisableCharacter(int i)
	{
		yield return new WaitForSeconds(0.25f);
		Characters[i].characterObject.CharacterAnimator.transform.parent.gameObject.SetActive(false);
	}

	private void SetupCharacters()
	{
		CharacterMenuItem[] characters = Characters;
		foreach (CharacterMenuItem characterMenuItem in characters)
		{
			GameObject gameObject = Object.Instantiate(menuCharacter);
			gameObject.transform.parent = base.transform;
			gameObject.transform.localScale = menuCharacter.transform.localScale;
			gameObject.transform.localPosition = starPos;
			characterMenuItem.characterObject = gameObject.GetComponent<CharacterObject>();
			characterMenuItem.characterObject.Board.mesh = characterMenuItem.Character.HoverBoard;
			characterMenuItem.characterObject.CharacterRenderer.sharedMesh = characterMenuItem.Character.CharacterMesh;
			characterMenuItem.Character.IsLocked = !characterMenuItem.Character.IsDefaultCharacter && (!GameDataController.Exists(string.Format("{0}{1}", "c_", characterMenuItem.Character.CharacterName)) || GameDataController.Load<bool>(string.Format("{0}{1}", "c_", characterMenuItem.Character.CharacterName)));
			characterMenuItem.characterObject.CharacterRenderer.material = ((!characterMenuItem.Character.IsLocked) ? characterMenuItem.Character.CharacterMainMaterial : LockedMaterial);
		}
		menuCharacter.SetActive(false);
	}

	public void Unlock()
	{
		CharacterMenuItem characterMenuItem = Characters[currentCharacter];
		if (characterMenuItem.Prize <= GameStats.Instance.ChallengeMoney)
		{
			GameStats.Instance.ChallengeMoney -= characterMenuItem.Prize;
			if (characterMenuItem.Character.IsLocked)
			{
				characterMenuItem.Character.IsLocked = false;
				characterMenuItem.characterObject.Board.mesh = characterMenuItem.Character.HoverBoard;
				characterMenuItem.characterObject.CharacterRenderer.material = characterMenuItem.Character.CharacterMainMaterial;
				GameDataController.Save(false, string.Format("{0}{1}", "c_", characterMenuItem.Character.CharacterName));
				ActivateCharacter();
				GPSSaveGame.Instance.SaveGame();
			}
		}
		else
		{
			iAPBuyBolts.OpenPopUp();
		}
	}

	public void SetupButtons()
	{
		if (currentCharacter == 0)
		{
			previousButton.interactable = false;
			if (!nextButton.interactable)
			{
				EnableButton(nextButton);
			}
			return;
		}
		if (currentCharacter >= Characters.Length - 1)
		{
			nextButton.interactable = false;
			if (!previousButton.interactable)
			{
				EnableButton(previousButton);
			}
			currentCharacter = Characters.Length - 1;
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

	public void SetActiveCharacter()
	{
		SetupButtons();
		if (Characters[currentCharacter].Character.IsLocked)
		{
			activateCharacter.gameObject.SetActive(false);
			unlockCharacter.gameObject.SetActive(true);
			active.SetActive(false);
			CharacterPrizeUI.SetStat(true, Characters[currentCharacter].Prize);
		}
		else
		{
			CharacterPrizeUI.SetStat(false, 0);
			bool flag = activeCharacter == currentCharacter;
			activateCharacter.gameObject.SetActive(!flag);
			if (activateCharacter.gameObject.activeSelf)
			{
				activateCharacter.GetComponent<Animator>().Play("Normal", -1, 0f);
			}
			unlockCharacter.gameObject.SetActive(false);
			activateCharacter.interactable = !flag;
			active.SetActive(flag);
		}
		CharacterStatUI.SetStat(Characters[currentCharacter].Character);
	}

	public void ChangeCharacter(bool next)
	{
		changeAudio.Play();
		if (next)
		{
			StartCoroutine(DisableCharacter(currentCharacter));
			Characters[currentCharacter].characterObject.CharacterMenuAnimator.Play("CharacterFadeLeft");
			currentCharacter++;
			SetActiveCharacter();
			Characters[currentCharacter].characterObject.CharacterAnimator.transform.parent.gameObject.SetActive(true);
			Characters[currentCharacter].characterObject.CharacterMenuAnimator.Play("CharacterFadeInRight");
		}
		else
		{
			StartCoroutine(DisableCharacter(currentCharacter));
			Characters[currentCharacter].characterObject.CharacterMenuAnimator.Play("CharacterFadeRight");
			currentCharacter--;
			SetActiveCharacter();
			Characters[currentCharacter].characterObject.CharacterAnimator.transform.parent.gameObject.SetActive(true);
			Characters[currentCharacter].characterObject.CharacterMenuAnimator.Play("CharacterFadeInLeft");
		}
	}

	public void ActivateCharacter()
	{
		activeCharacter = currentCharacter;
		if (!Characters[activeCharacter].Character.IsLocked)
		{
			GameDataController.Save(activeCharacter, "ChallengeCharacter");
			SetIngameCharacter();
			SetActiveCharacter();
		}
	}

	public void Reset()
	{
		currentCharacter = (activeCharacter = 0);
		GameDataController.Save(activeCharacter, "ChallengeCharacter");
		CharacterMenuItem[] characters = Characters;
		foreach (CharacterMenuItem characterMenuItem in characters)
		{
			GameDataController.Save(true, string.Format("{0}{1}", "c_", characterMenuItem.Character.CharacterName));
		}
		SetActiveCharacter();
	}
}
