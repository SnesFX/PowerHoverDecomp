using UnityEngine;

public class AIController : MonoBehaviour
{
	private enum DropType
	{
		Normal = 0,
		Ledge = 1,
		Pipe = 2,
		LedgeUp = 3
	}

	private const int ANGLE_OF_LANDING_WOBBLE = 45;

	private const float GROUND_DISTANCE = 0.8f;

	private const float IN_AIR_SIDE_RAY_DISTANCE = 1f;

	public const string TAG_JUMP = "Jump";

	public const string TAG_JUMP_SPECIAL = "JumpSpecial";

	public const string TAG_JUMP_SMALL = "JumpSmall";

	public const string TAG_JUMP_SPRING = "JumpSpring";

	public const string TAG_ROTATING_LAND = "RotatingLand";

	public const string TAG_ELEVATOR = "Elevator";

	public const string TAG_BOOST = "Boost";

	public const string TAG_GRIND = "Grind";

	public const string TAG_PIPE = "Pipe";

	private const int BaseGravity = 10;

	private const float JumpSpeed = 30f;

	public LayerMask LevelLayerMask;

	public Transform GraphicsTransform;

	public Transform leftRay;

	public Transform rightRay;

	public Transform leftRayInAir;

	public Transform rightRayInAir;

	private AIWalker walker;

	private AIAnimationController animationController;

	private bool inAirRayHit;

	private float startZ;

	private Vector3 forceLocalZ;

	private float Gravity = 10f;

	private bool makingJump;

	private DropType dropFromLedge;

	private int[] JumpGravityForce = new int[4] { -12, 5, 7, -4 };

	private string lastGameObjectTag = string.Empty;

	private float onGroundTimer;

	private float inAirTimer;

	private float rotateTimer;

	private float rotateTimer2;

	private float hitSmoothTimer;

	private float jumpAirTime;

	private float jumpRotation;

	private float easingWhenJumpStarts;

	private float easingInJump;

	private Vector3 jumpRayPositionLeft;

	private Vector3 jumpRayPositionRight;

	private Vector3 rotateGraphicsEuler;

	private bool wasFlipped;

	private bool specialJump;

	private float manualFlipEasing;

	private bool manualFlip;

	private PlayerState debugState;

	public PlayerState PlayerState { get; private set; }

	public JumpType JumpType { get; private set; }

	public bool isPlayerFlipped { get; private set; }

	public bool OffLimits { get; set; }

	private void Start()
	{
		startZ = base.transform.localPosition.z;
		walker = base.transform.parent.GetComponent<AIWalker>();
		animationController = GetComponent<AIAnimationController>();
		ResetPlayer();
	}

	private void ResetPlayer()
	{
		PlayerState = PlayerState.Idle;
		makingJump = false;
		lastGameObjectTag = string.Empty;
		dropFromLedge = DropType.Normal;
		isPlayerFlipped = false;
		specialJump = false;
		wasFlipped = false;
		Gravity = 10f;
		rotateGraphicsEuler = Vector3.zero;
		inAirTimer = (rotateTimer = (rotateTimer2 = (onGroundTimer = (easingInJump = (easingWhenJumpStarts = (jumpRotation = (jumpAirTime = (hitSmoothTimer = 0f))))))));
		OffLimits = false;
	}

	private void FixedUpdate()
	{
		switch (GameController.Instance.State)
		{
		case GameController.GameState.Reverse:
			GraphicsTransform.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
			break;
		case GameController.GameState.Kill:
			break;
		case GameController.GameState.Resume:
		case GameController.GameState.Running:
		case GameController.GameState.End:
			doUpdate();
			break;
		}
	}

	private void doUpdate()
	{
		if (PlayerState != PlayerState.Grinding)
		{
			Vector3 position = base.transform.position;
			RaycastHit hitInfo;
			inAirRayHit = !Physics.Raycast(base.transform.position, -base.transform.up, out hitInfo, 15f, LevelLayerMask.value);
			if (inAirRayHit || (!inAirRayHit && (hitInfo.distance > 1f || !isJump(hitInfo.collider.gameObject.tag)) && isJump(lastGameObjectTag)))
			{
				inAirTimer = (rotateTimer = (rotateTimer2 = 0f));
				UpdateAirControl();
				ForcePosition();
			}
			else if (makingJump)
			{
				UpdateLanding(hitInfo);
			}
			else
			{
				UpdateGroundControl(hitInfo, position);
			}
			ForcePosition();
		}
	}

	private void DropOffFromWall()
	{
		if (!Physics.Raycast(base.transform.position, Vector3.down, 0.8f, LevelLayerMask.value))
		{
			base.transform.position += Vector3.up * (-10f + onGroundTimer * 4f) * Time.deltaTime;
		}
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation((!isPlayerFlipped) ? walker.transform.forward : (-walker.transform.forward), Vector3.up), onGroundTimer);
		onGroundTimer += Time.deltaTime * 5f;
		if ((onGroundTimer > 0.7f || base.transform.up == Vector3.up) && lastGameObjectTag.Equals("Pipe"))
		{
			lastGameObjectTag = "Jump";
			dropFromLedge = DropType.Pipe;
		}
		ForcePosition();
	}

	private Vector3 CheckSides(Vector3 direction, Vector3 moveToPosition, out bool gotHit)
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(moveToPosition + base.transform.up * 0.8f, direction, out hitInfo, 1.4f, LevelLayerMask.value) && Vector3.Angle(base.transform.up, hitInfo.normal) >= 88f)
		{
			moveToPosition += (moveToPosition + base.transform.up * 0.8f - hitInfo.point).normalized * 1.4f;
			gotHit = true;
			hitSmoothTimer = 0.1f;
			return moveToPosition;
		}
		gotHit = false;
		return moveToPosition;
	}

	private bool isRotationObject(string tag)
	{
		return isJump(tag) || tag.Equals("RotatingLand");
	}

	private bool isJump(string tag)
	{
		return tag.Equals("Jump") || tag.Equals("JumpSpecial") || tag.Equals("JumpSmall");
	}

	private Vector3 UpdateSideMovement(int dir, Vector3 moveToPosition)
	{
		Vector3 checkPosition = ((dir <= 0) ? rightRay.position : leftRay.position);
		if (CheckBorder(checkPosition, true))
		{
			moveToPosition += base.transform.right * Time.deltaTime * animationController.easing;
		}
		return moveToPosition;
	}

	private bool CheckBorder(Vector3 checkPosition, bool checkSides = false)
	{
		Debug.DrawRay(checkPosition, -base.transform.up, Color.black, 4f);
		RaycastHit hitInfo;
		RaycastHit hitInfo2;
		if (Physics.Raycast(checkPosition, -base.transform.up, out hitInfo, 40f, LevelLayerMask.value) && Physics.Raycast(base.transform.position + base.transform.forward * 0.5f, -base.transform.up, out hitInfo2, 40f, LevelLayerMask.value))
		{
			if (checkSides)
			{
				Vector3 direction = ((!(animationController.easing > 0f)) ? (-base.transform.right) : base.transform.right);
				RaycastHit hitInfo3;
				if (Physics.Raycast(checkPosition, direction, out hitInfo3, 0.8f, LevelLayerMask.value) && Vector3.Angle(base.transform.up, hitInfo3.normal) >= 88f)
				{
					animationController.easing = 0f;
					return false;
				}
			}
			if (!isJump(lastGameObjectTag) && (hitInfo.distance > 2f || hitInfo2.distance > 2f))
			{
				dropFromLedge = ((onGroundTimer != 0f) ? DropType.Ledge : DropType.LedgeUp);
				lastGameObjectTag = "Jump";
				if (hitInfo.distance > 2f && Mathf.Abs(animationController.easing) < 7f)
				{
					animationController.easing = ((!(animationController.easing < 0f)) ? 7 : (-7));
				}
			}
			return true;
		}
		animationController.easing = 0f;
		return false;
	}

	private void UpdateSpecials(GameObject tagObj)
	{
		string text = tagObj.tag;
		if (text != null && text.Length > 1)
		{
			switch (text)
			{
			case "Elevator":
				animationController.easing += Time.deltaTime * tagObj.GetComponent<ElevatorTrigger>().force * (float)((!isPlayerFlipped) ? 1 : (-1));
				break;
			case "Boost":
				PlayerState = PlayerState.Boosting;
				break;
			case "Jump":
			case "JumpSpecial":
				JumpType = JumpType.PreJump;
				animationController.SetJumpState(JumpAnimationState.PreJump, isPlayerFlipped ? 1 : 0);
				break;
			}
		}
	}

	public void ForcePosition(int lerpMultiplier = 4)
	{
		forceLocalZ = base.transform.localPosition;
		forceLocalZ.z = Mathf.Lerp(forceLocalZ.z, startZ, Time.deltaTime * (float)lerpMultiplier);
		base.transform.localPosition = forceLocalZ;
	}

	private void UpdateLanding(RaycastHit hit)
	{
		PlayerState = PlayerState.MakeLanding;
		MakeLanding();
	}

	private void UpdateGroundControl(RaycastHit hit, Vector3 moveToPosition)
	{
		if (onGroundTimer > 0f && onGroundTimer < 1f)
		{
			DropOffFromWall();
			return;
		}
		if (hit.distance > 0.35000002f)
		{
			inAirTimer = Vector3.Angle(base.transform.up, hit.normal) * 0.01f;
		}
		lastGameObjectTag = hit.collider.gameObject.tag;
		UpdateSpecials(hit.collider.gameObject);
		if (hit.normal != rotateGraphicsEuler)
		{
			rotateTimer2 = 0.05f;
			rotateTimer = 0.15f;
			rotateGraphicsEuler = hit.normal;
		}
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.LookRotation((!isPlayerFlipped) ? walker.transform.forward : (-walker.transform.forward), hit.normal), rotateTimer += Time.deltaTime * 2f);
		if (isRotationObject(lastGameObjectTag))
		{
			GraphicsTransform.rotation = Quaternion.Lerp(GraphicsTransform.rotation, Quaternion.LookRotation((!isPlayerFlipped) ? hit.collider.transform.right : (-hit.collider.transform.right), hit.normal), rotateTimer2 += Time.deltaTime * 3f);
		}
		else
		{
			GraphicsTransform.rotation = base.transform.rotation;
		}
		moveToPosition = ((!(inAirTimer < 1f)) ? ((base.transform.position - hit.point).normalized * 0.8f + hit.point) : Vector3.Slerp(moveToPosition, (base.transform.position - hit.point).normalized * 0.8f + hit.point, inAirTimer += 3f * Time.deltaTime));
		if (animationController.easing != 0f)
		{
			moveToPosition = UpdateSideMovement((animationController.easing > 0f) ? 1 : (-1), moveToPosition);
			PlayerState = PlayerState.Leaning;
		}
		else
		{
			CheckBorder(base.transform.position);
			PlayerState = PlayerState.Idle;
		}
		if (isJump(lastGameObjectTag) && dropFromLedge != 0)
		{
			return;
		}
		bool gotHit = false;
		moveToPosition = CheckSides(-base.transform.right, moveToPosition, out gotHit);
		if (!gotHit)
		{
			moveToPosition = CheckSides(base.transform.right, moveToPosition, out gotHit);
		}
		base.transform.position = ((!gotHit) ? moveToPosition : Vector3.Lerp(base.transform.position, moveToPosition, hitSmoothTimer += Time.deltaTime * 6f));
		if (gotHit)
		{
			animationController.PlayerHitWall();
			PlayerState = PlayerState.Wobble;
		}
		if (manualFlip)
		{
			animationController.easing = manualFlipEasing * rotateTimer;
			if (rotateTimer >= 1f)
			{
				PlayerState = ((animationController.easing != 0f) ? PlayerState.Leaning : PlayerState.Idle);
				manualFlip = false;
			}
		}
	}

	public bool FlipPlayer()
	{
		if (manualFlip)
		{
			return false;
		}
		PlayerState = PlayerState.MakeFlip;
		manualFlip = true;
		rotateTimer = (rotateTimer2 = 0f);
		manualFlipEasing = 0f - animationController.easing;
		isPlayerFlipped = !isPlayerFlipped;
		return true;
	}

	private void UpdateAirControl()
	{
		if ((!inAirRayHit && !makingJump) || (inAirRayHit && !makingJump && isJump(lastGameObjectTag)))
		{
			Jump();
		}
		else if (makingJump)
		{
			jumpAirTime += Time.deltaTime;
			jumpRayPositionLeft = base.transform.position;
			jumpRayPositionLeft.z += 1f;
			jumpRayPositionRight = base.transform.position;
			jumpRayPositionRight.z -= 1f;
			leftRayInAir.position = jumpRayPositionLeft;
			rightRayInAir.position = jumpRayPositionRight;
			RaycastHit hitInfo;
			bool flag = Physics.Raycast(base.transform.position, -base.transform.up, out hitInfo, (!specialJump) ? 25f : 3f, LevelLayerMask.value);
			Vector3 zero = Vector3.zero;
			zero = base.transform.up * (0f - Gravity) * Time.deltaTime;
			if (Gravity < 15f)
			{
				Gravity += Time.deltaTime * 30f;
			}
			bool flag2 = false;
			if ((Gravity <= 3f || (Gravity > 3f && RotateOnGroundInAir())) && (specialJump || hitInfo.distance >= 2f))
			{
				RotateInAir();
				flag2 = true;
			}
			bool flag3 = !UIController.Instance.leftPressed && !UIController.Instance.rightPressed;
			if ((!specialJump && flag && hitInfo.distance < 3f && Gravity > 3f) || flag3)
			{
				if (Mathf.Abs(Vector3.Angle(base.transform.forward, -walker.transform.forward)) < Mathf.Abs(Vector3.Angle(base.transform.forward, walker.transform.forward)))
				{
					isPlayerFlipped = true;
				}
				else
				{
					isPlayerFlipped = false;
				}
				float y = base.transform.rotation.eulerAngles.y;
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation((!isPlayerFlipped) ? walker.transform.forward : (-walker.transform.forward), hitInfo.normal), rotateTimer += Time.deltaTime * 2.5f * (float)((!flag3) ? 1 : 4));
				y = Mathf.DeltaAngle(y, base.transform.rotation.eulerAngles.y);
				jumpRotation += y;
			}
			if (hitInfo.distance < 1f && flag && Gravity > 3f)
			{
				lastGameObjectTag = string.Empty;
			}
			RaycastHit hitInfo2;
			if (Gravity > -3f && !specialJump && Physics.Raycast(base.transform.position, -walker.transform.up, out hitInfo2, 30f, LevelLayerMask.value))
			{
				GraphicsTransform.rotation = Quaternion.Lerp(GraphicsTransform.rotation, base.transform.rotation, rotateTimer2 += Time.deltaTime * 5f);
			}
			if (flag2 && dropFromLedge == DropType.Normal)
			{
				if (UIController.Instance.leftPressed && easingInJump < 12f)
				{
					easingInJump += Time.deltaTime * 25f * 0.5f;
				}
				else if (UIController.Instance.rightPressed && easingInJump > -12f)
				{
					easingInJump -= Time.deltaTime * 25f * 0.5f;
				}
			}
			if (easingInJump != 0f && (hitInfo.distance > 1f || Gravity <= 3f || specialJump))
			{
				Vector3 position = base.transform.position;
				float num = Time.deltaTime * easingInJump * ((!specialJump) ? 1f : 0.5f);
				if (Gravity < 15f)
				{
					Gravity += Mathf.Abs(num) * ((!specialJump) ? 1.3f : 1f);
				}
				position += zero;
				Vector3 origin = ((!(easingInJump > 0f)) ? leftRayInAir.position : rightRayInAir.position);
				if (Physics.Raycast(origin, -base.transform.up, 40f, LevelLayerMask.value))
				{
					Vector3 direction = ((!(easingInJump > 0f)) ? (-rightRayInAir.right) : rightRayInAir.right);
					if (!Physics.Raycast(origin, direction, 1f, LevelLayerMask.value) && !Physics.Raycast(position, direction, 1f, LevelLayerMask.value))
					{
						position += base.transform.parent.transform.right * num;
					}
					else
					{
						easingInJump = 0f;
					}
				}
				else
				{
					easingInJump = 0f;
				}
				base.transform.position = position;
			}
			else
			{
				base.transform.position += zero;
			}
			PlayerState = PlayerState.InAir;
		}
		else if (inAirRayHit && base.transform.up != Vector3.up)
		{
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation((!isPlayerFlipped) ? walker.transform.forward : (-walker.transform.forward), Vector3.up), onGroundTimer += Time.deltaTime * 2f);
		}
	}

	private void RotateInAir()
	{
		if (UIController.Instance.leftPressed || UIController.Instance.rightPressed)
		{
			int num = (UIController.Instance.leftPressed ? 1 : (-1));
			float num2 = (float)(num * 720) * Time.smoothDeltaTime;
			if (specialJump)
			{
				GraphicsTransform.Rotate(0f, 0f, num2 *= 0.5f);
			}
			else
			{
				base.transform.Rotate(0f, num2, 0f);
			}
			jumpRotation += num2;
		}
	}

	private bool RotateOnGroundInAir()
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(base.transform.position + Vector3.right, -base.transform.up, out hitInfo, 2f, LevelLayerMask.value) && hitInfo.normal.x > -0.9f)
		{
			animationController.easing = easingInJump;
			return false;
		}
		return true;
	}

	public void Jump(string tag)
	{
		lastGameObjectTag = tag;
		Jump();
	}

	private void Jump()
	{
		PlayerState = PlayerState.MakeJump;
		Gravity = JumpGravityForce[(int)dropFromLedge];
		if (lastGameObjectTag.Equals("JumpSmall"))
		{
			Gravity = -4f;
		}
		makingJump = true;
		easingWhenJumpStarts = ((onGroundTimer != 0f) ? 0f : animationController.easing);
		easingInJump = ((!isPlayerFlipped) ? easingWhenJumpStarts : (0f - easingWhenJumpStarts));
		animationController.easing = 0f;
		jumpRotation = (jumpAirTime = 0f);
		rotateTimer = (rotateTimer2 = 0f);
		specialJump = lastGameObjectTag.Equals("JumpSpecial");
		JumpType = ((!specialJump) ? JumpType.Normal : ((!(easingWhenJumpStarts > 0f)) ? JumpType.BackFlip : JumpType.FrontFlip));
		animationController.SetJumpState(JumpAnimationState.Jump, (!specialJump) ? 1 : ((!(easingWhenJumpStarts > 0f)) ? 2 : 3));
	}

	private void MakeLanding()
	{
		animationController.easing *= ((!isPlayerFlipped) ? 1 : (-1));
		dropFromLedge = DropType.Normal;
		float num = Mathf.Abs(Mathf.DeltaAngle(jumpRotation, (wasFlipped == isPlayerFlipped) ? 360 : 180));
		jumpRotation = Mathf.Round(jumpRotation / 180f) * 180f;
		jumpRotation = (jumpAirTime = 0f);
		if (PlayerState != PlayerState.Grinding)
		{
			animationController.SetJumpState(JumpAnimationState.Landing, isPlayerFlipped ? 1 : 0);
			if (num > 45f)
			{
				animationController.PlayerHitWall();
			}
		}
		onGroundTimer = 0f;
		makingJump = (specialJump = false);
		wasFlipped = isPlayerFlipped;
	}
}
