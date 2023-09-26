using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ghost : ResetObject
{
	private int DiamondMax = 40;

	private int BombMax = 20;

	private const string PREFIX_TIME_RECORD = "rcrdScore_";

	private const string PREFIX_GHOST_DATA = "rcrdGhost_";

	private GameObject GhostPrefab;

	private AudioClip DropItemSound;

	private AudioClip DropBombSound;

	private GameObject Diamond;

	private GameObject Bomb;

	private const float RECORD_INTERVAL = 0.09f;

	private const float GHOST_EASING = 4f;

	private HoverController Player;

	private GameObject GhostObj;

	private Animator GhostAnimator;

	private GameObject Teleport;

	private Animator TeleportAnimator;

	private bool teleported;

	private Dictionary<float, GhostData> ghostDataRecording;

	private Dictionary<float, GhostData> ghostDataPlaying;

	private GhostData recordTempGhost;

	private GhostData playTempGhost;

	private float recordTimer;

	private bool ghostIndicatorCreated;

	private float diffOnTime;

	private float lastKey = -1f;

	private float startAhead = 1.2f;

	private float diamondDropTimer;

	private Stack<GameObject> DropDiamonds;

	private Stack<GameObject> DropBomb;

	private int diamondCounter;

	private int bombCounter;

	private AudioSource audioObject;

	private float followSpeed;

	public void Init()
	{
		Player = Object.FindObjectOfType<HoverController>();
		GhostObj = Object.Instantiate(Resources.Load("Ghost")) as GameObject;
		Teleport = Object.Instantiate(Resources.Load("GhostTeleport")) as GameObject;
		Teleport.transform.parent = GameController.Instance.transform;
		TeleportAnimator = Teleport.GetComponent<Animator>();
		DropItemSound = Object.Instantiate(Resources.Load("GhostDropBattery")) as AudioClip;
		DropBombSound = Object.Instantiate(Resources.Load("GhostDropBomb")) as AudioClip;
		GhostAnimator = GhostObj.GetComponentInChildren<Animator>();
		GhostAnimator.SetBool(Player.animationController.breakingHash, true);
		GhostAnimator.SetFloat(Player.animationController.noPressesHash, 1f);
		GhostAnimator.SetBool(Player.animationController.rightTurnHash, false);
		GhostAnimator.SetBool(Player.animationController.leftTurnHash, false);
		GhostAnimator.SetLayerWeight(GhostAnimator.GetLayerIndex("Jumping"), 0f);
		GhostObj.transform.parent = GameController.Instance.transform;
		ghostDataRecording = new Dictionary<float, GhostData>();
		audioObject = GhostObj.GetComponent<AudioSource>();
		GhostObj.SetActive(false);
	}

	private void FixedUpdate()
	{
		if (GhostObj != null && GhostObj.activeSelf && GameController.Instance.State != GameController.GameState.Kill)
		{
			PlayGhost(Time.fixedDeltaTime);
		}
	}

	public void EnableGhostMode(bool enable)
	{
		if (enable)
		{
			Diamond = Resources.Load((!SceneLoader.Instance.Current.GhostBattery.Equals(string.Empty)) ? SceneLoader.Instance.Current.GhostBattery : "GhostCoin") as GameObject;
			Bomb = Resources.Load("GhostBomb") as GameObject;
			LoadGhost();
			LevelStats.Instance.SetGhostItemCount(DiamondMax);
		}
		else
		{
			ActivateGhostObject(false);
		}
	}

	public void ResetRecord()
	{
		string sceneName = GetSceneName();
		ES2.Delete(string.Format("{0}{1}", "rcrdScore_", sceneName));
		ES2.Delete(string.Format("{0}{1}", "rcrdGhost_", sceneName));
	}

	private void PlayGhost(float delta)
	{
		if (ghostDataPlaying == null || ghostDataPlaying.Count <= 0)
		{
			return;
		}
		if (lastKey == ghostDataPlaying.Last().Key)
		{
			GhostAnimator.SetBool(Player.animationController.rightTurnHash, false);
			GhostAnimator.SetBool(Player.animationController.leftTurnHash, false);
			if (!teleported)
			{
				TeleportAnimator.Play("GhostTeleportActivate", -1, 0f);
				teleported = true;
			}
			GhostObj.transform.localScale = Vector3.Lerp(GhostObj.transform.localScale, Vector3.zero, Time.fixedDeltaTime * 5f);
			return;
		}
		float num = ghostDataPlaying.Keys.FirstOrDefault((float x) => x >= LevelStats.Instance.LevelTime + startAhead);
		if (num != lastKey)
		{
			lastKey = num;
			diffOnTime = num - LevelStats.Instance.LevelTime - startAhead;
			if (ghostDataPlaying.ContainsKey(lastKey))
			{
				playTempGhost = ghostDataPlaying[lastKey];
				GhostAnimator.SetBool(Player.animationController.rightTurnHash, playTempGhost.rightPress);
				GhostAnimator.SetBool(Player.animationController.leftTurnHash, playTempGhost.leftPress);
				GhostAnimator.SetInteger(Player.animationController.jumpHash, playTempGhost.jump);
				GhostAnimator.SetBool(Player.animationController.landHash, playTempGhost.landing);
				GhostAnimator.SetBool(Player.animationController.prejumpHash, playTempGhost.prejump);
				GhostAnimator.SetBool(Player.animationController.grindingHash, playTempGhost.grinding);
				GhostAnimator.SetBool(Player.animationController.breakingHash, false);
				if (playTempGhost.dropBomb)
				{
					DropItem(true);
				}
				else if (playTempGhost.dropItem)
				{
					DropItem(false);
				}
			}
		}
		if (GameController.Instance.State != GameController.GameState.Running)
		{
			GhostAnimator.SetBool(Player.animationController.rightTurnHash, false);
			GhostAnimator.SetBool(Player.animationController.leftTurnHash, false);
			GhostAnimator.SetBool(Player.animationController.breakingHash, true);
		}
		if (ghostDataPlaying.ContainsKey(lastKey))
		{
			GhostObj.transform.position = Vector3.Lerp(GhostObj.transform.position, playTempGhost.position, delta * (4f + diffOnTime));
			GhostObj.transform.rotation = Quaternion.Lerp(GhostObj.transform.rotation, playTempGhost.rotation, delta * (4f + diffOnTime));
		}
	}

	private void DropItem(bool bomb)
	{
		GameObject gameObject = ((!bomb) ? DropDiamonds.Pop() : DropBomb.Pop());
		gameObject.transform.parent = GameController.Instance.transform;
		gameObject.transform.position = GhostObj.transform.position;
		gameObject.transform.rotation = GhostObj.transform.rotation;
		gameObject.SetActive(true);
		if (bomb)
		{
			if (DropBombSound != null)
			{
				audioObject.PlayOneShot(DropBombSound);
			}
			Object.Destroy(gameObject, 0.4f);
			bombCounter--;
		}
		else
		{
			if (DropItemSound != null)
			{
				audioObject.PlayOneShot(DropItemSound);
			}
			diamondCounter--;
		}
	}

	private string GetSceneName()
	{
		return (Main.Instance.CurrentScene == null) ? Application.loadedLevelName : Main.Instance.CurrentScene;
	}

	private void LoadGhost()
	{
		if (SceneLoader.Instance.Current != null)
		{
			LoadGhostFile();
		}
		else
		{
			LoadGhostRecord();
		}
	}

	private void LoadGhostFile()
	{
		ES2Settings settings = new ES2Settings(ES2Settings.SaveLocation.Resources);
		string identifier = string.Format("{0}{1}", "/LevelGhosts/", SceneLoader.Instance.Current.GhostFile);
		if (SceneLoader.Instance.Current.GhostTimeDiff > 0f)
		{
			startAhead = SceneLoader.Instance.Current.GhostTimeDiff;
		}
		if (ES2.Exists(identifier, settings))
		{
			ghostDataPlaying = ES2.LoadDictionary<float, GhostData>(identifier, settings);
			ActivateGhostObject(true);
			Teleport.transform.position = ghostDataPlaying.Last().Value.position;
		}
		else
		{
			ActivateGhostObject(false);
		}
	}

	private void LoadGhostRecord()
	{
		string sceneName = GetSceneName();
		string identifier = string.Format("{0}{1}", "rcrdGhost_", sceneName);
		if (GameDataController.Exists(identifier))
		{
			ghostDataPlaying = ES2.LoadDictionary<float, GhostData>(identifier);
			ActivateGhostObject(true);
		}
		else
		{
			ActivateGhostObject(false);
		}
	}

	private void ActivateGhostObject(bool enable)
	{
		if (enable)
		{
			float key = ghostDataPlaying.Keys.FirstOrDefault((float x) => x >= 0f);
			GhostObj.transform.position = ghostDataPlaying[key].position;
			GhostObj.transform.rotation = ghostDataPlaying[key].rotation;
			GhostObj.SetActive(true);
			if (Diamond != null && SceneLoader.Instance.Current != null)
			{
				DiamondMax = ghostDataPlaying.Where((KeyValuePair<float, GhostData> kvp) => kvp.Value.dropItem).Count();
				BombMax = ghostDataPlaying.Where((KeyValuePair<float, GhostData> kvp) => kvp.Value.dropBomb).Count();
				DropDiamonds = new Stack<GameObject>(DiamondMax);
				DropBomb = new Stack<GameObject>(BombMax);
				diamondCounter = 0;
				bombCounter = 0;
				AddItems();
			}
		}
		else
		{
			GhostObj.SetActive(false);
			ghostDataPlaying = new Dictionary<float, GhostData>();
		}
	}

	private void AddItems()
	{
		while (diamondCounter < DiamondMax)
		{
			diamondCounter++;
			GameObject gameObject = Object.Instantiate(Diamond);
			gameObject.transform.parent = base.transform;
			gameObject.SetActive(false);
			DropDiamonds.Push(gameObject);
		}
		while (bombCounter < BombMax)
		{
			bombCounter++;
			GameObject gameObject2 = Object.Instantiate(Bomb);
			gameObject2.transform.parent = base.transform;
			gameObject2.SetActive(false);
			DropBomb.Push(gameObject2);
		}
	}

	public override void Reset(bool isRewind)
	{
		if (SceneLoader.Instance.Current.IsEndless || GhostObj == null)
		{
			return;
		}
		if (GhostObj.activeSelf && Diamond != null)
		{
			lastKey = -1f;
			GhostAnimator.SetBool(Player.animationController.breakingHash, true);
			AddItems();
			if (TeleportAnimator != null)
			{
				TeleportAnimator.Play("GhostTeleportOff", -1, 0f);
				teleported = false;
			}
		}
		if (ghostDataRecording != null && ghostDataRecording.Count > 0)
		{
			ghostDataRecording = ghostDataRecording.Where((KeyValuePair<float, GhostData> kvp) => kvp.Key <= LevelStats.Instance.LevelTime).ToDictionary((KeyValuePair<float, GhostData> kvp) => kvp.Key, (KeyValuePair<float, GhostData> kvp) => kvp.Value);
		}
		if (ghostDataPlaying == null || ghostDataPlaying.Count <= 0)
		{
			return;
		}
		GhostObj.transform.localScale = Vector3.one;
		startAhead = ((!(SceneLoader.Instance.Current.GhostTimeDiff > 0f)) ? 1.2f : SceneLoader.Instance.Current.GhostTimeDiff);
		float key = ghostDataPlaying.Keys.FirstOrDefault((float x) => x >= LevelStats.Instance.LevelTime + startAhead);
		if (!isRewind)
		{
			if (ghostDataPlaying.ContainsKey(key))
			{
				GhostObj.transform.position = ghostDataPlaying[key].position;
				GhostObj.transform.rotation = ghostDataPlaying[key].rotation;
			}
			return;
		}
		float num = 30f;
		int num2 = 0;
		while (num > 24f && num2 < 10)
		{
			startAhead -= 0.09f;
			key = ghostDataPlaying.Keys.FirstOrDefault((float x) => x >= LevelStats.Instance.LevelTime + startAhead);
			if (ghostDataPlaying.ContainsKey(key))
			{
				num = Vector3.Distance(ghostDataPlaying[key].position, Player.transform.position);
			}
			num2++;
		}
		if (ghostDataPlaying.ContainsKey(key))
		{
			GhostObj.transform.position = ghostDataPlaying[key].position;
			GhostObj.transform.rotation = ghostDataPlaying[key].rotation;
		}
	}
}
