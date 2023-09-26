using UnityEngine;

public class AIAnimationController : ResetObject
{
	public const int SIDE_CONTROL_SPEED = 20;

	public const int MAX_EASING = 3;

	public Animator animator;

	public float easing;

	public GameObject trusters;

	public GameObject trustersA;

	private GameObject trailObject;

	public GameObject trailPrefab;

	private AIController hoverController;

	private Vector3 trusterScale = new Vector3(1f, 0.5f, 1f);

	private float stateClearTimer;

	private float wobbleClearTimer;

	public Projector shadowBlob;

	[HideInInspector]
	public int leftTurnHash = Animator.StringToHash("leftTurn");

	[HideInInspector]
	public int rightTurnHash = Animator.StringToHash("rightTurn");

	private int wobbleHash = Animator.StringToHash("wobble");

	private int breakingHash = Animator.StringToHash("breaking");

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

	public JumpAnimationState currentState { get; private set; }

	private void Awake()
	{
		jumpingLayerIndex = animator.GetLayerIndex("Jumping");
		hoverController = GetComponent<AIController>();
	}

	public override void Reset(bool onlyNonSaved)
	{
		ShowTrails(false);
		easing = 0f;
	}

	private void Update()
	{
		switch (GameController.Instance.State)
		{
		case GameController.GameState.Kill:
		case GameController.GameState.End:
			ShowTrails(false);
			animator.SetBool(wobbleHash, false);
			wobbleClearTimer = 0f;
			break;
		case GameController.GameState.Resume:
			ResetAnimator();
			break;
		case GameController.GameState.Running:
			doUpdate();
			break;
		case GameController.GameState.Reverse:
			break;
		}
	}

	private void doUpdate()
	{
		if (stateClearTimer > 0f)
		{
			stateClearTimer -= Time.deltaTime;
			if (stateClearTimer <= 0f)
			{
				SetJumpState(JumpAnimationState.None);
			}
		}
		float num = Mathf.Sin(Time.time * 2f);
		bool flag = num < -0.3f;
		bool flag2 = num > 0.3f;
		if (currentState == JumpAnimationState.Jump || flag || flag2)
		{
			ShowTrails(true);
		}
		else
		{
			ShowTrails(false);
		}
		if (wobbleClearTimer > 0f)
		{
			wobbleClearTimer -= Time.deltaTime;
			if (wobbleClearTimer <= 0f)
			{
				animator.SetBool(wobbleHash, false);
			}
		}
		if (currentState != JumpAnimationState.Jump && (flag || flag2) && (!flag || !flag2))
		{
			if (!trustersA.activeSelf)
			{
				trustersA.SetActive(true);
			}
			if (trusterScale.y < 1f)
			{
				trusterScale.y += Time.deltaTime * 2.5f;
			}
			bool flag3 = ((!hoverController.isPlayerFlipped) ? flag2 : flag);
			bool flag4 = ((!hoverController.isPlayerFlipped) ? flag : flag2);
			animator.SetBool(leftTurnHash, flag4);
			animator.SetBool(rightTurnHash, flag3);
			float num2 = 20f;
			if (flag3)
			{
				if (easing < 3f)
				{
					easing += Time.deltaTime * num2;
				}
			}
			else if (flag4 && easing > -3f)
			{
				easing -= Time.deltaTime * num2;
			}
		}
		else
		{
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
				easing = Mathf.Max(0f, easing - Time.deltaTime * 10f);
			}
			else if (easing < 0f)
			{
				easing += Time.deltaTime * 10f;
			}
			animator.SetBool(leftTurnHash, false);
			animator.SetBool(rightTurnHash, false);
		}
	}

	public void ResetAnimator()
	{
		easing = 0f;
		animator.SetBool(leftTurnHash, false);
		animator.SetBool(rightTurnHash, false);
		animator.SetBool(breakingHash, false);
		animator.SetBool(backwardHash, false);
		animator.SetFloat(speedHash, 0f);
		animator.SetBool(wobbleHash, false);
		animator.SetBool(grindingHash, false);
		SetJumpState(JumpAnimationState.None);
	}

	public void PlayerHitWall()
	{
		animator.SetBool(wobbleHash, true);
		wobbleClearTimer = 0.5f;
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
			animator.SetInteger(jumpHash, extraValue);
			animator.SetBool(prejumpHash, false);
			animator.SetBool(landHash, false);
			animator.SetBool(grindingHash, false);
			stateClearTimer = 0f;
			break;
		case JumpAnimationState.Landing:
			animator.SetInteger(jumpHash, 0);
			animator.SetBool(prejumpHash, false);
			animator.SetBool(landHash, true);
			animator.SetBool(grindingHash, false);
			animator.SetBool(backwardHash, extraValue == 1);
			stateClearTimer = 0.6f;
			break;
		case JumpAnimationState.PreJump:
			animator.SetLayerWeight(jumpingLayerIndex, 1f);
			animator.SetInteger(jumpHash, 0);
			animator.SetBool(prejumpHash, true);
			animator.SetBool(landHash, false);
			animator.SetBool(backwardHash, extraValue == 1);
			stateClearTimer = 3f;
			break;
		case JumpAnimationState.Sliding:
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
			animator.SetInteger(jumpHash, 0);
			animator.SetBool(prejumpHash, false);
			animator.SetBool(landHash, false);
			stateClearTimer = 0f;
			break;
		}
		currentState = jumpState;
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
}
