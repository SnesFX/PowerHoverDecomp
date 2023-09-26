using UnityEngine;

public class GameController : MonoBehaviour
{
	public enum GameState
	{
		Start = 0,
		Kill = 1,
		Reverse = 2,
		Resume = 3,
		Running = 4,
		End = 5,
		Paused = 6,
		Ending = 7
	}

	public GameState State;

	public float PlayerLocalStartY = 0.5f;

	public float HoverSpeed = 30f;

	public bool BoostSpeedOnDistance;

	public Ghost Ghost;

	public TargetIndicator TargetIndicator;

	private GameState PreviousState;

	private bool controlBlocked;

	private InfoBox[] infoBoxes;

	private int infoBoxCounter;

	private float infoboxTimer;

	private SplineWalker walker;

	private string infoPrefix;

	private bool tutorialStarted;

	private InfoBox.InfoBoxDetails infoDetails;

	public static GameController Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
		walker = Object.FindObjectOfType<SplineWalker>();
		if (Ghost == null)
		{
			Ghost = base.transform.gameObject.AddComponent<Ghost>();
			if (Main.Instance != null && !Main.Instance.TutorialLevel && !Main.Instance.TestingLevel && !Main.Instance.CreditsLevel && !SceneLoader.Instance.Current.GhostFile.Equals(string.Empty))
			{
				Ghost.Init();
				Ghost.EnableGhostMode(true);
			}
		}
		GameObject gameObject = Object.Instantiate(Resources.Load("TargetIndicator")) as GameObject;
		gameObject.transform.parent = base.transform;
		Instance.TargetIndicator = gameObject.GetComponent<TargetIndicator>();
		gameObject.SetActive(false);
		infoboxTimer = -1f;
		if (SceneLoader.Instance != null && SceneLoader.Instance.Current != null)
		{
			infoPrefix = string.Format("{0}{1}", "Tuto_", SceneLoader.Instance.Current.SceneName);
			infoBoxes = base.transform.GetComponents<InfoBox>();
			if (infoBoxes != null && infoBoxes.Length > 0 && (infoBoxes[0].ShowOnEveryRun || !PlayerPrefs.HasKey(infoPrefix)))
			{
				infoboxTimer = 0.01f;
			}
		}
	}

	private void Start()
	{
		if (infoboxTimer > 0f && UIController.Instance != null && UIController.Instance.hiddenText != null)
		{
			LocalizationLoader.Instance.SetText(UIController.Instance.hiddenText, infoBoxes[0].LocalizationID);
		}
	}

	public void SetTutorialAsRead()
	{
		if (infoBoxes != null && infoBoxes.Length > 0 && infoPrefix != null && !infoPrefix.Equals(string.Empty))
		{
			PlayerPrefs.SetInt(infoPrefix, 1);
		}
	}

	public void SetState(GameState state)
	{
		if (state != State)
		{
			PreviousState = State;
			State = state;
		}
	}

	public void ResumeLastState()
	{
		SetState(PreviousState);
	}

	private void Update()
	{
		if (!tutorialStarted && (Main.Instance.TutorialLevel || Main.Instance.CreditsLevel) && walker.State == SplineWalker.LevelState.Start)
		{
			walker.TutorialStart();
			tutorialStarted = true;
		}
		else if (walker.levelReady && infoboxTimer > 0f)
		{
			infoboxTimer -= Time.deltaTime;
			if (infoboxTimer <= 0f)
			{
				if (infoDetails == null)
				{
					infoDetails = new InfoBox.InfoBoxDetails(infoBoxCounter + 1, infoBoxes.Length);
					infoDetails.pauseHidden = infoBoxes[infoBoxCounter].ShowInfo(infoDetails);
					if (!Main.Instance.TestingLevel && SceneLoader.Instance.Current.IsEndless)
					{
						SetTutorialAsRead();
					}
				}
				else if (!infoDetails.skipped)
				{
					infoDetails.page = infoBoxCounter + 1;
					infoDetails.pageCount = infoBoxes.Length;
					infoBoxes[infoBoxCounter].ShowInfo(infoDetails);
				}
				infoBoxCounter++;
				if (infoBoxCounter < infoBoxes.Length)
				{
					infoboxTimer = 0.01f;
				}
			}
		}
		GameState state = State;
		if (state == GameState.Paused && Main.Instance != null && (UIController.Instance.leftPressed || UIController.Instance.rightPressed))
		{
			Main.Instance.ResumeGame();
		}
	}
}
