using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

public class CheckPointContoller : MonoBehaviour
{
	private class CheckPointSave
	{
		public CurvySpline spline;

		public Vector3 walkerPosition;

		public float walkerTF;

		public Quaternion playerRotation;

		public GameObject cameraNull;

		public float time;

		public int trickCollectables;

		public float farDistance;
	}

	private SplineWalker walker;

	private CameraFollowAnimation cameraController;

	private GameObject dieEffect;

	private CheckPointSave startSave;

	private CheckPointSave checkpointSave;

	private List<CheckPointSave> checkPointSaves;

	private bool savedAtStart;

	private float fadeTimer;

	private bool resetBlock;

	private bool controlBlock;

	public HoverController hoverController { get; private set; }

	private void Start()
	{
		checkPointSaves = new List<CheckPointSave>();
		dieEffect = Object.Instantiate(Resources.Load("KillEffect")) as GameObject;
		dieEffect.transform.parent = GameController.Instance.transform;
		dieEffect.SetActive(false);
		cameraController = Object.FindObjectOfType<CameraFollowAnimation>();
		hoverController = Object.FindObjectOfType<HoverController>();
		walker = hoverController.walker;
	}

	private void Update()
	{
		if (GameController.Instance.State == GameController.GameState.Paused || GameController.Instance.State == GameController.GameState.End || !resetBlock)
		{
			return;
		}
		if (fadeTimer > 0f)
		{
			fadeTimer -= Time.deltaTime * 2f;
		}
		if (!UIController.Instance.rightPressed && !UIController.Instance.leftPressed && fadeTimer < 0.8f)
		{
			controlBlock = false;
		}
		if (!controlBlock && (UIController.Instance.leftPressed || UIController.Instance.rightPressed))
		{
			if (GameController.Instance.State == GameController.GameState.Ending)
			{
				GameController.Instance.SetState(GameController.GameState.End);
			}
			else if (LifeController.Instance.LifeCount >= 1)
			{
				Rewind(false);
			}
		}
	}

	private void FixedUpdate()
	{
		if (!savedAtStart && (UIController.Instance.leftPressed || UIController.Instance.rightPressed))
		{
			savedAtStart = true;
			SaveOnCheckpoint();
		}
	}

	public void Reset()
	{
		if (startSave != null)
		{
			hoverController.ResetFromUI();
			LoadCheckpoint(startSave, true);
			checkpointSave = startSave;
			checkPointSaves.Clear();
			ResetObject[] array = Object.FindObjectsOfType<ResetObject>();
			foreach (ResetObject resetObject in array)
			{
				resetObject.Reset(false);
			}
			LevelStats.Instance.ClearCollectables();
		}
	}

	public void Rewind(bool manualRewind)
	{
		if (startSave == null)
		{
			return;
		}
		hoverController.ResetFromUI();
		if (GameController.Instance.State == GameController.GameState.Start && checkPointSaves.Count > 1)
		{
			checkPointSaves.Remove(checkpointSave);
			checkpointSave = checkPointSaves[checkPointSaves.Count - 1];
		}
		LoadCheckpoint(checkpointSave);
		ResetObject[] array = Object.FindObjectsOfType<ResetObject>();
		foreach (ResetObject resetObject in array)
		{
			if (resetObject is Collectable)
			{
				resetObject.Reset(true);
			}
			else
			{
				resetObject.Reset((checkpointSave != startSave) ? true : false);
			}
		}
	}

	public void PlayerDie()
	{
		GameObject gameObject = Object.Instantiate(dieEffect);
		dieEffect.transform.parent = GameController.Instance.transform;
		gameObject.transform.position = hoverController.transform.position;
		gameObject.SetActive(true);
		gameObject.GetComponent<KillEffectRandomizer>().MakeKillEffect(walker);
		hoverController.KillPlayer();
		if (SceneLoader.Instance.Current.IsEndless || SceneLoader.Instance.Current.IsChallenge)
		{
			LevelStats.Instance.LevelKillCount++;
			GameController.Instance.SetState(GameController.GameState.End);
			walker.EndLevel();
		}
		else
		{
			if (!Main.Instance.TutorialLevel)
			{
				LifeController.Instance.ChangeLifes(false);
				LevelStats.Instance.LevelKillCount++;
			}
			GameController.Instance.SetState(GameController.GameState.Kill);
		}
		Vector3 position = cameraController.transform.position;
		position.x += 2f;
		HOTween.Punch(cameraController.transform, 0.35f, new TweenParms().Prop("position", position).UpdateType(UpdateType.TimeScaleIndependentUpdate));
		if ((bool)GameStats.Instance)
		{
			GameStats.Instance.DeathCount++;
		}
		if ((bool)StuckButtonVisibility.Instance && !SceneLoader.Instance.Current.IsChallenge && !SceneLoader.Instance.Current.IsEndless)
		{
			StuckButtonVisibility.Instance.AddStuckCounter();
		}
		ResetBlock();
	}

	public void ResetBlock()
	{
		resetBlock = (controlBlock = true);
		fadeTimer = 1f;
	}

	public void SaveOnCheckpoint()
	{
		checkpointSave = new CheckPointSave();
		checkpointSave.walkerTF = walker.TF;
		checkpointSave.spline = walker.Spline;
		checkpointSave.playerRotation = base.transform.rotation;
		checkpointSave.walkerPosition = base.transform.parent.position;
		checkpointSave.time = LevelStats.Instance.LevelTime;
		checkpointSave.cameraNull = cameraController.targetCameraNull;
		checkpointSave.trickCollectables = LevelStats.Instance.CollectebleCollectCount;
		if (startSave == null)
		{
			startSave = checkpointSave;
		}
		checkpointSave.farDistance = Camera.main.farClipPlane;
		checkPointSaves.Add(checkpointSave);
	}

	private void LoadCheckpoint(CheckPointSave save, bool reset = false)
	{
		GameController.Instance.SetState(GameController.GameState.Reverse);
		resetBlock = false;
		base.transform.localPosition = new Vector3(0f, GameController.Instance.PlayerLocalStartY, 0f);
		base.transform.rotation = save.playerRotation;
		walker.TF = save.walkerTF;
		walker.Spline = save.spline;
		walker.SetPositionOnSpline(save.walkerPosition);
		hoverController.ForcePosition(1000);
		UIController.Instance.PressLeft(false);
		UIController.Instance.PressRight(false);
		LevelStats.Instance.LevelTime = save.time;
		LevelStats.Instance.ClearToCheckpoint(save.trickCollectables);
		Camera.main.farClipPlane = save.farDistance;
		cameraController.SetState(reset, save.cameraNull);
	}
}
