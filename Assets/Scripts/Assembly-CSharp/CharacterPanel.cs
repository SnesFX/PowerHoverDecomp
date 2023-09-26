using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPanel : MonoBehaviour
{
	private const string CHARACTER_PREFIX = "MainCurrentCharacter";

	public CharacterMenuItem[] Characters;

	public Button previousButton;

	public Button nextButton;

	public Button activateCharacter;

	public GameObject activateBg;

	public AudioSource changeAudio;

	public Material LockedMaterial;

	public GameObject menuCharacter;

	private int activeCharacter;

	private int currentCharacter;

	private Vector3 starPos;

	private void Awake()
	{
		currentCharacter = (GameDataController.Exists("MainCurrentCharacter") ? GameDataController.Load<int>("MainCurrentCharacter") : 0);
		activeCharacter = currentCharacter;
		starPos = menuCharacter.transform.localPosition;
		SetupCharacters();
	}

	public void SetIngameCharacter()
	{
		TrickController.Instance.CurrentCharacter = Characters[currentCharacter].Character;
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
			characterMenuItem.Character.IsLocked = !characterMenuItem.Character.IsDefaultCharacter && (!GameDataController.Exists(characterMenuItem.Character.CharacterName) || GameDataController.Load<bool>(characterMenuItem.Character.CharacterName));
			characterMenuItem.characterObject.CharacterRenderer.material = ((!characterMenuItem.Character.IsLocked) ? characterMenuItem.Character.CharacterMainMaterial : LockedMaterial);
		}
		menuCharacter.SetActive(false);
	}

	public void Unlock()
	{
		CharacterMenuItem[] characters = Characters;
		foreach (CharacterMenuItem characterMenuItem in characters)
		{
			if (characterMenuItem.Character.IsLocked)
			{
				characterMenuItem.Character.IsLocked = false;
				characterMenuItem.characterObject.Board.mesh = characterMenuItem.Character.HoverBoard;
				characterMenuItem.characterObject.CharacterRenderer.material = characterMenuItem.Character.CharacterMainMaterial;
				GameDataController.Save(false, characterMenuItem.Character.CharacterName);
				break;
			}
		}
	}

	public void SetActiveCharacter()
	{
		if (currentCharacter == 0)
		{
			previousButton.gameObject.SetActive(false);
			nextButton.gameObject.SetActive(true);
		}
		else if (currentCharacter >= Characters.Length - 1)
		{
			nextButton.gameObject.SetActive(false);
			previousButton.gameObject.SetActive(true);
		}
		else
		{
			nextButton.gameObject.SetActive(true);
			previousButton.gameObject.SetActive(true);
		}
		if (Characters[currentCharacter].Character.IsLocked)
		{
			activateCharacter.interactable = false;
			if (LocalizationLoader.Instance != null)
			{
				LocalizationLoader.Instance.SetText(activateCharacter.GetComponentInChildren<Text>(), "MainMenu.Locked");
			}
			activateCharacter.GetComponent<Image>().enabled = false;
			activateBg.SetActive(false);
			return;
		}
		bool flag = activeCharacter == currentCharacter;
		if (!flag)
		{
			EnableButton(activateCharacter);
		}
		else
		{
			activateCharacter.interactable = false;
		}
		activateBg.SetActive(!flag);
		if (LocalizationLoader.Instance != null)
		{
			if (flag)
			{
				LocalizationLoader.Instance.SetText(activateCharacter.GetComponentInChildren<Text>(), "MainMenu.Active");
			}
			else
			{
				LocalizationLoader.Instance.SetText(activateCharacter.GetComponentInChildren<Text>(), "MainMenu.Activate");
			}
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
			GameDataController.Save(activeCharacter, "MainCurrentCharacter");
			SetIngameCharacter();
			SetActiveCharacter();
		}
	}

	public void Reset()
	{
		currentCharacter = (activeCharacter = 0);
		GameDataController.Save(activeCharacter, "MainCurrentCharacter");
		CharacterMenuItem[] characters = Characters;
		foreach (CharacterMenuItem characterMenuItem in characters)
		{
			GameDataController.Save(true, characterMenuItem.Character.CharacterName);
		}
		SetActiveCharacter();
	}
}
