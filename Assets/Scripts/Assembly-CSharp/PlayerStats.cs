using System.Linq;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
	public PlayerStat[] Stats;

	public Animator statsAnimator;

	public AudioSource statAudio;

	public GameObject CharacterBox;

	public GameObject CloseHiddenObject;

	private bool shown;

	private float statChangeTimer;

	private int magnetIndex;

	public static PlayerStats Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
		for (int i = 0; i < Stats.Length; i++)
		{
			if (Stats[i].StatType == PlayerStatType.Magnet)
			{
				magnetIndex = i;
			}
			string identifier = string.Format("{0}{1}", Stats[i].StatType, "stat");
			if (GameDataController.Exists(identifier))
			{
				Stats[i].Level = GameDataController.Load<int>(identifier);
			}
			else
			{
				Stats[i].Level = Stats[i].StartLevel;
			}
		}
		CloseHiddenObject.SetActive(false);
	}

	private void Start()
	{
		UpdateTexts();
		GetComponentInChildren<CharacterPanel>().SetIngameCharacter();
	}

	private void FixedUpdate()
	{
		if (statChangeTimer > 0f)
		{
			statChangeTimer -= Time.fixedDeltaTime;
			CloseHiddenObject.SetActive(shown);
		}
	}

	public void ResetStats()
	{
		for (int i = 0; i < Stats.Length; i++)
		{
			string identifier = string.Format("{0}{1}", Stats[i].StatType, "stat");
			Stats[i].Level = Stats[i].StartLevel;
			GameDataController.Save(Stats[i].Level, identifier);
			LocalizationLoader.Instance.SetText(Stats[i].textBox, Stats[i].Description);
			Stats[i].textBox.text = string.Format("{0}: {1}", Stats[i].textBox.text, Stats[i].Level);
		}
	}

	public int GetMagnetLevel()
	{
		if (magnetIndex < 0 || magnetIndex >= Stats.Length)
		{
			return 0;
		}
		return Stats[magnetIndex].Level;
	}

	public int GetStatLevel(PlayerStatType type)
	{
		PlayerStat playerStat = Stats.ToList().Find((PlayerStat x) => x.StatType == type);
		return playerStat.Level;
	}

	public void UpdateStat(PlayerStatType type)
	{
		PlayerStat playerStat = Stats.ToList().Find((PlayerStat x) => x.StatType == type);
		if (playerStat != null)
		{
			if (type == PlayerStatType.Lives)
			{
				LifeController.Instance.ChangeLifes(true);
			}
			else
			{
				playerStat.Level++;
				GameDataController.Save(playerStat.Level, string.Format("{0}{1}", playerStat.StatType, "stat"));
				LocalizationLoader.Instance.SetText(playerStat.textBox, playerStat.Description);
				playerStat.textBox.text = string.Format("{0}: {1}", playerStat.textBox.text, playerStat.Level);
			}
			UpdateStatToGame(playerStat);
		}
	}

	public void ShowStats()
	{
		if (!(statChangeTimer > 0f))
		{
			shown = !shown;
			if (shown)
			{
				base.gameObject.SetActive(true);
				CloseHiddenObject.SetActive(shown);
				UpdateTexts();
				GetComponent<MenuPanel>().Activate(true);
			}
			else
			{
				GetComponent<MenuPanel>().Activate(false);
			}
			statChangeTimer = 0.5f;
			statAudio.Play();
			statsAnimator.Play((!shown) ? "MapPlayerStatsFadeOut" : "MapPlayerStatsFadeIn", -1, 1f);
			GetComponentInChildren<CharacterPanel>().SetActiveCharacter();
		}
	}

	private void UpdateStatToGame(PlayerStat stat)
	{
		switch (stat.StatType)
		{
		case PlayerStatType.BackUps:
			LifeController.Instance.UpdateLifeSavers(stat.Level);
			break;
		case PlayerStatType.StartLives:
		{
			LifeController.Instance.UpdateLifeMax(stat.Level);
			PlayerStat playerStat = Stats.ToList().Find((PlayerStat x) => x.StatType == PlayerStatType.Lives);
			playerStat.textBox.text = string.Format("{0}", LifeController.Instance.LifeCount);
			break;
		}
		case PlayerStatType.Lives:
			stat.textBox.text = string.Format("{0}", LifeController.Instance.LifeCount);
			break;
		}
	}

	private void UpdateTexts()
	{
		for (int i = 0; i < Stats.Length; i++)
		{
			LocalizationLoader.Instance.SetText(Stats[i].textBox, Stats[i].Description);
			Stats[i].textBox.text = string.Format("{0}: {1}", Stats[i].textBox.text, Stats[i].Level);
			UpdateStatToGame(Stats[i]);
		}
	}
}
