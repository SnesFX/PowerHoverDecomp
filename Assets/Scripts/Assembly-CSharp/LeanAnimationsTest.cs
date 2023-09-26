using UnityEngine;

public class LeanAnimationsTest : ResetObject
{
	public const int SIDE_CONTROL_SPEED = 25;

	public const int SIDE_CONTROL_SPEED_KIRRE = 25;

	public const int MAX_EASING = 20;

	public const int KIRRE_EASING_LIMIT = 8;

	public const float KIRRE_SLOW_MULTIPLIER = 0.9f;

	public Animator animator;

	public float easing;

	public GameObject trusters;

	public GameObject trustersA;

	public bool easingDone;

	public GameObject hoverBoard;

	public GameObject hoverBoardHandPos;

	public GameObject hoverBoardFootPos;

	private float boardToHandTransitionTime;

	private float waitTimeBoardTohand;

	public ParticleSystem boardParticles;

	public ParticleSystem boardSmoke;

	private GameObject trailObject;

	public GameObject trailPrefab;

	private GameObject boostEffectObject;

	public GameObject boostEffectPrefab;

	public SplineWalker walker;

	public HoverController hoverController;

	private CharacterSetup characterSetup;

	private Vector3 trusterScale = new Vector3(1f, 0.5f, 1f);

	private float addMoreParticles;

	private float stateClearTimer;

	private float wobbleClearTimer;

	private float noPressingTimer;

	[HideInInspector]
	public int leftTurnHash = Animator.StringToHash("leftTurn");

	[HideInInspector]
	public int finishHash = Animator.StringToHash("finish");

	[HideInInspector]
	public int rightTurnHash = Animator.StringToHash("rightTurn");

	private int wobbleHash = Animator.StringToHash("wobble");

	[HideInInspector]
	public int breakingHash = Animator.StringToHash("breaking");

	private int speedHash = Animator.StringToHash("speed");

	private int backwardHash = Animator.StringToHash("backward");

	[HideInInspector]
	public int grindingHash = Animator.StringToHash("grinding");

	[HideInInspector]
	public int jumpHash = Animator.StringToHash("jump");

	[HideInInspector]
	public int landHash = Animator.StringToHash("land");

	[HideInInspector]
	public int prejumpHash = Animator.StringToHash("preJump");

	[HideInInspector]
	public int noPressesHash = Animator.StringToHash("noPresses");

	private int jumpingLayerIndex;

	private bool wasOnSand;

	private bool trustersSetUp;

	private bool wasInAir;

	public JumpAnimationState currentState { get; private set; }

	private void Awake()
	{
		jumpingLayerIndex = animator.GetLayerIndex("Jumping");
		characterSetup = walker.GetComponent<CharacterSetup>();
	}

	public override void Reset(bool onlyNonSaved)
	{
		Object.Destroy(boostEffectObject);
		boostEffectObject = null;
		ShowTrails(false);
		ResetAnimator();
		easing = 0f;
	}

	private void Update()
	{
		switch (GameController.Instance.State)
		{
		case GameController.GameState.Ending:
			if (!animator.GetBool(breakingHash))
			{
				boardSmoke.emissionRate = 0f;
				boardParticles.emissionRate = 0f;
				if (trustersA.activeSelf)
				{
					trustersA.SetActive(false);
				}
				if (trusterScale.y > 0.6f)
				{
					trusterScale.y = 0.6f;
				}
				HoverBoardPosition(hoverBoardHandPos, true);
				animator.SetBool(grindingHash, false);
				animator.SetBool(leftTurnHash, false);
				animator.SetBool(rightTurnHash, false);
				animator.SetBool(landHash, false);
				animator.SetFloat(speedHash, walker.Speed);
				animator.SetBool(breakingHash, true);
				animator.SetFloat(noPressesHash, 1f);
				if (boostEffectObject != null)
				{
					boostEffectObject.GetComponent<BoostEffect>().StartFade();
					boostEffectObject.transform.parent = GameController.Instance.transform;
					boostEffectObject = null;
				}
			}
			break;
		case GameController.GameState.Kill:
		case GameController.GameState.End:
			boardSmoke.emissionRate = 0f;
			boardParticles.emissionRate = 0f;
			ShowTrails(false);
			wobbleClearTimer = 0f;
			break;
		case GameController.GameState.Running:
			doUpdate();
			break;
		case GameController.GameState.Start:
			if (!trustersSetUp && characterSetup != null && characterSetup.Character != null && characterSetup.Character.CharacterName != null && characterSetup.Character.CharacterName.Contains("UFO"))
			{
				trustersSetUp = true;
				MeshRenderer[] componentsInChildren = trusters.GetComponentsInChildren<MeshRenderer>();
				foreach (MeshRenderer meshRenderer in componentsInChildren)
				{
					meshRenderer.enabled = false;
				}
			}
			break;
		case GameController.GameState.Reverse:
		case GameController.GameState.Resume:
		case GameController.GameState.Paused:
			break;
		}
	}

	private void doUpdate()
	{
		AnimateTrust();
		if (stateClearTimer > 0f)
		{
			stateClearTimer -= Time.deltaTime;
			if (stateClearTimer <= 0f)
			{
				if (!UIController.Instance.leftPressed || !UIController.Instance.rightPressed || currentState != JumpAnimationState.PreJump)
				{
					SetJumpState(JumpAnimationState.None);
				}
				else
				{
					stateClearTimer = 0.2f;
				}
			}
		}
		ShowTrailsNew(currentState == JumpAnimationState.Jump, GameController.Instance.State == GameController.GameState.Running && walker.Speed > 10f && currentState != JumpAnimationState.Sliding);
		if (wobbleClearTimer > 0f)
		{
			wobbleClearTimer -= Time.deltaTime;
			if (wobbleClearTimer <= 0f)
			{
				animator.SetBool(wobbleHash, false);
			}
		}
		if (!UIController.Instance.rightPressed && !UIController.Instance.leftPressed)
		{
			animator.SetFloat(noPressesHash, noPressingTimer += Time.deltaTime);
		}
		if (currentState != JumpAnimationState.Jump && (UIController.Instance.rightPressed || UIController.Instance.leftPressed) && (!UIController.Instance.rightPressed || !UIController.Instance.leftPressed))
		{
			noPressingTimer = 0f;
			if (!trustersA.activeSelf)
			{
				trustersA.SetActive(true);
			}
			if (trusterScale.y < 1f)
			{
				trusterScale.y += Time.deltaTime * 2.5f;
				addMoreParticles = 100f + 20f * trusterScale.y;
			}
			bool flag = ((!hoverController.isPlayerFlipped) ? UIController.Instance.leftPressed : UIController.Instance.rightPressed);
			bool flag2 = ((!hoverController.isPlayerFlipped) ? UIController.Instance.rightPressed : UIController.Instance.leftPressed);
			animator.SetBool(leftTurnHash, flag2);
			animator.SetBool(rightTurnHash, flag);
			float num = 25 + characterSetup.Character.ControlSpeedAdd;
			if (Mathf.Abs(easing) > 8f && !hoverController.isOnPipe())
			{
				num = 25 + characterSetup.Character.ControlSpeedAdd;
				walker.UpdateSlowing(true, 0.9f);
			}
			else
			{
				walker.UpdateSlowing(false, 0.9f);
			}
			if (flag)
			{
				if (easing < (float)(20 + characterSetup.Character.ControlMaxEasingAdd))
				{
					easing += Time.deltaTime * num;
				}
			}
			else if (flag2 && easing > (float)(-(20 + characterSetup.Character.ControlMaxEasingAdd)))
			{
				easing -= Time.deltaTime * num;
			}
			walker.SetBoostExtra(2f);
		}
		else
		{
			walker.SetBoostExtra(0f);
			addMoreParticles = 0f;
			if (UIController.Instance.rightPressed && UIController.Instance.leftPressed)
			{
				noPressingTimer = 0f;
			}
			if (trustersA.activeSelf)
			{
				trustersA.SetActive(false);
			}
			if (trusterScale.y > 0.6f)
			{
				trusterScale.y = 0.6f;
			}
			if (easing > 0f)
			{
				easing = Mathf.Max(0f, easing - Time.deltaTime * 6.5f);
			}
			else if (easing < 0f)
			{
				easing += Time.deltaTime * 6.5f;
			}
			walker.UpdateSlowing(false, 0.9f);
			animator.SetBool(leftTurnHash, false);
			animator.SetBool(rightTurnHash, false);
		}
		animator.SetFloat(speedHash, walker.Speed);
		animator.SetBool(backwardHash, hoverController.isPlayerFlipped);
	}

	public void ResetAnimator()
	{
		easing = 0f;
		waitTimeBoardTohand = 0f;
		boardToHandTransitionTime = 0f;
		animator.gameObject.SetActive(true);
		HoverBoardPosition(hoverBoardFootPos, false);
		animator.SetLayerWeight(jumpingLayerIndex, 0f);
		animator.SetBool(leftTurnHash, false);
		animator.SetBool(rightTurnHash, false);
		animator.SetBool(breakingHash, false);
		animator.SetBool(backwardHash, false);
		animator.SetFloat(speedHash, 0f);
		animator.SetBool(wobbleHash, false);
		animator.SetBool(grindingHash, false);
		animator.SetBool(landHash, true);
		animator.SetFloat(noPressesHash, 1f);
		SetJumpState(JumpAnimationState.None);
	}

	public void PlayerHitWall()
	{
		walker.CanBreak = false;
		animator.SetBool(wobbleHash, true);
		wobbleClearTimer = 0.5f;
	}

	public void HoverBoardPosition(GameObject parentObject, bool animate)
	{
		if (animate && waitTimeBoardTohand < 0.33f)
		{
			waitTimeBoardTohand += Time.fixedDeltaTime;
		}
		if (boardToHandTransitionTime < 1f && animate && waitTimeBoardTohand > 0.33f)
		{
			boardToHandTransitionTime += Time.fixedDeltaTime * 2f;
			hoverBoard.transform.position = Vector3.Lerp(hoverBoard.transform.position, parentObject.transform.position, boardToHandTransitionTime);
			hoverBoard.transform.rotation = Quaternion.Lerp(hoverBoard.transform.rotation, parentObject.transform.rotation, boardToHandTransitionTime);
		}
		else if (boardToHandTransitionTime > 1f || !animate)
		{
			hoverBoard.transform.position = parentObject.transform.position;
			hoverBoard.transform.rotation = parentObject.transform.rotation;
			hoverBoard.transform.parent = parentObject.transform;
		}
	}

	public void SetJumpState(JumpAnimationState jumpState, int extraValue = 0)
	{
		if (currentState == jumpState)
		{
			return;
		}
		switch (jumpState)
		{
		case JumpAnimationState.Jump:
			animator.SetLayerWeight(jumpingLayerIndex, 1f);
			walker.CanBreak = false;
			animator.SetInteger(jumpHash, extraValue);
			animator.SetBool(prejumpHash, false);
			animator.SetBool(landHash, false);
			animator.SetBool(grindingHash, false);
			stateClearTimer = 0f;
			break;
		case JumpAnimationState.Landing:
			walker.CanBreak = true;
			animator.SetInteger(jumpHash, 0);
			animator.SetBool(prejumpHash, false);
			animator.SetBool(landHash, true);
			animator.SetBool(grindingHash, false);
			animator.SetBool(backwardHash, extraValue == 1);
			stateClearTimer = 0.6f;
			break;
		case JumpAnimationState.PreJump:
			animator.SetLayerWeight(jumpingLayerIndex, 1f);
			walker.CanBreak = false;
			animator.SetInteger(jumpHash, 0);
			animator.SetBool(prejumpHash, true);
			animator.SetBool(landHash, false);
			animator.SetBool(grindingHash, false);
			animator.SetBool(backwardHash, extraValue == 1);
			stateClearTimer = 0.2f;
			break;
		case JumpAnimationState.Sliding:
			walker.CanBreak = false;
			animator.SetInteger(jumpHash, 0);
			animator.SetBool(prejumpHash, false);
			animator.SetBool(landHash, false);
			animator.SetBool(breakingHash, false);
			animator.SetBool(grindingHash, true);
			animator.SetBool(backwardHash, false);
			stateClearTimer = 0f;
			break;
		default:
			if (currentState == JumpAnimationState.Landing)
			{
				animator.SetLayerWeight(jumpingLayerIndex, 0f);
			}
			walker.CanBreak = true;
			animator.SetInteger(jumpHash, 0);
			animator.SetBool(prejumpHash, false);
			animator.SetBool(landHash, false);
			stateClearTimer = 0f;
			break;
		}
		currentState = jumpState;
	}

	private void AnimateTrust()
	{
		if (trusters != null)
		{
			trusters.transform.localScale = trusterScale * Random.Range(1f, 1.25f);
		}
		if (walker.Speed > 0f)
		{
			animateAcceleration();
		}
		if ((hoverController.PlayerState == PlayerState.Boosting || walker.Boosting) && boostEffectObject == null)
		{
			boostEffectObject = Object.Instantiate(boostEffectPrefab, base.transform.position, Quaternion.identity);
			boostEffectObject.transform.parent = base.transform;
		}
		else if (hoverController.PlayerState != PlayerState.Boosting && !walker.Boosting && boostEffectObject != null)
		{
			boostEffectObject.GetComponent<BoostEffect>().StartFade();
			boostEffectObject.transform.parent = GameController.Instance.transform;
			boostEffectObject = null;
		}
	}

	public void DeleteTrail()
	{
		if (trailObject != null)
		{
			trailObject.transform.parent = GameController.Instance.transform;
			Object.Destroy(trailObject);
			trailObject = null;
		}
	}

	private void ShowTrailsNew(bool inAir, bool enable)
	{
		if (trailObject != null && (inAir != wasInAir || !enable))
		{
			wasInAir = inAir;
			trailObject.GetComponent<TrailEffect>().StartFade();
			trailObject.transform.parent = GameController.Instance.transform;
			trailObject = null;
		}
		if (trailObject == null && enable)
		{
			trailObject = Object.Instantiate(trailPrefab, trusters.transform.position, trusters.transform.rotation);
			trailObject.transform.parent = trusters.transform;
			trailObject.GetComponent<TrailEffect>().StartTrail(!inAir);
		}
	}

	private void ShowTrails(bool show)
	{
		if (show && trailObject == null)
		{
			trailObject = Object.Instantiate(trailPrefab, trusters.transform.position, trusters.transform.rotation);
			trailObject.transform.parent = trusters.transform;
		}
		else if (!show && trailObject != null)
		{
			trailObject.GetComponent<TrailEffect>().StartFade();
			trailObject.transform.parent = GameController.Instance.transform;
			trailObject = null;
		}
	}

	private void animateAcceleration()
	{
		Vector3 position = trusters.transform.position;
		position.x += walker.Speed * 0.07f;
		RaycastHit hitInfo;
		if (Physics.Raycast(position, -base.transform.up, out hitInfo))
		{
			if (hitInfo.collider.gameObject.CompareTag("Sand") && hitInfo.distance < 2f)
			{
				boardSmoke.emissionRate = addMoreParticles;
				boardParticles.emissionRate = Mathf.Abs(easing);
				walker.OnSand(true);
				wasOnSand = true;
			}
			else
			{
				boardSmoke.emissionRate = 0f;
				boardParticles.emissionRate = 1f;
				walker.OnSand(false);
				wasOnSand = false;
			}
		}
		else
		{
			boardSmoke.emissionRate = 0f;
			boardParticles.emissionRate = 1f;
		}
	}
}
