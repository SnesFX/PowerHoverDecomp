using UnityEngine;

public class HoverController : MonoBehaviour
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

	public const string TAG_JUMP_GRIND = "JumpGrind";

	public const string TAG_JUMP_SPRING = "JumpSpring";

	public const string TAG_ROTATING_LAND = "RotatingLand";

	public const string TAG_ELEVATOR = "Elevator";

	public const string TAG_BOOST = "Boost";

	public const string TAG_GRIND = "Grind";

	public const string TAG_PIPE = "Pipe";

	private const int BaseGravity = 10;

	private const float JumpSpeed = 30f;

	private const float HitFlickerSpeed = 20f;

	public LayerMask LevelLayerMask;

	public SplineWalker walker;

	public LeanAnimationsTest animationController;

	public GrindControl grindControl;

	public CheckPointContoller checkpointController;

	public Transform GraphicsTransform;

	public HoverBoardSound soundController;

	public GameObject slowMotionParticle;

	private GameObject slowMotionOverLay;

	private float slowMotionTimer = 4f;

	private float slowMotionSmooth;

	private bool inSlowMotion;

	public Transform leftRay;

	public Transform rightRay;

	public Transform leftRayInAir;

	public Transform rightRayInAir;

	private bool inAirRayHit;

	private float startZ;

	private Vector3 forceLocalZ;

	private float Gravity = 10f;

	private DropType dropFromLedge;

	private int[] JumpGravityForce = new int[4] { -12, 5, 7, -4 };

	private string lastGameObjectTag = string.Empty;

	private Vector3 GfxNormalPosition;

	private float onGroundTimer;

	private float inAirTimer;

	private float rotateTimer;

	private float rotateTimer2;

	private float hitSmoothTimer;

	private float jumpAirTime;

	private float jumpRotation;

	private float easingInJump;

	private Vector3 jumpRayPositionLeft;

	private Vector3 jumpRayPositionRight;

	private Vector3 rotateGraphicsEuler;

	private bool wasFlipped;

	private PlayerState debugState;

	private float hitTimer;

	private SkinnedMeshRenderer hoverRenderer;

	private GameObject LandingEffect;

	private GameObject[] LandingEffects;

	public GameObject BurstEffect;

	private GameObject RailGunEffect;

	public Material flickerMaterial;

	private Material normalMaterial;

	private bool burstOn;

	private bool jumpFromGrind;

	public PlayerState PlayerState { get; private set; }

	public JumpType JumpType { get; private set; }

	public bool isPlayerFlipped { get; private set; }

	public bool OffLimits { get; set; }

	public bool IsFlickering
	{
		get
		{
			return hitTimer > 0f;
		}
	}

	public bool makingJump { get; private set; }

	public TargetObject targetObject { get; private set; }

	public TargetObject targetActiveObject { get; private set; }

	private void Start()
	{
		slowMotionOverLay = Object.FindObjectOfType<SlowMotionEffect>().gameObject;
		if (slowMotionOverLay != null)
		{
			slowMotionOverLay.GetComponent<MeshRenderer>().enabled = true;
		}
		SetSlowMotion(false);
		RailGunEffect = Object.Instantiate(Resources.Load("BurstEffect")) as GameObject;
		RailGunEffect.transform.parent = GameController.Instance.transform;
		RailGunEffect.SetActive(false);
		BurstEffect.SetActive(false);
		startZ = base.transform.localPosition.z;
		hoverRenderer = GraphicsTransform.GetComponentInChildren<SkinnedMeshRenderer>();
		LandingEffects = new GameObject[2];
		LandingEffects[0] = Object.Instantiate(Resources.Load("LandingEffects/LandingEffect") as GameObject);
		LandingEffects[1] = Object.Instantiate(Resources.Load("LandingEffects/LandingEffectWater") as GameObject);
		LandingEffects[0].SetActive(false);
		LandingEffects[1].SetActive(false);
		LandingEffect = LandingEffects[0];
		GfxNormalPosition = GraphicsTransform.localPosition;
		normalMaterial = hoverRenderer.material;
		ResetPlayer();
	}

	private void ResetPlayer()
	{
		hoverRenderer.material = normalMaterial;
		Time.timeScale = 1f;
		SetSlowMotion(false);
		PlayerState = PlayerState.Idle;
		makingJump = false;
		lastGameObjectTag = string.Empty;
		dropFromLedge = DropType.Normal;
		isPlayerFlipped = false;
		wasFlipped = false;
		Gravity = 10f;
		rotateGraphicsEuler = Vector3.zero;
		inAirTimer = (rotateTimer = (rotateTimer2 = (onGroundTimer = (easingInJump = (jumpRotation = (jumpAirTime = (hitSmoothTimer = (hitTimer = 0f))))))));
		animationController.ResetAnimator();
		grindControl.Reset();
		OffLimits = false;
		TargetObject targetObject2 = (this.targetObject = null);
		targetActiveObject = targetObject2;
		walker.SlowdownEnabled = OffLimits;
		GraphicsTransform.localPosition = GfxNormalPosition;
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, GameController.Instance.PlayerLocalStartY, base.transform.localPosition.z);
	}

	private void FixedUpdate()
	{
		switch (GameController.Instance.State)
		{
		case GameController.GameState.Reverse:
			hoverRenderer.enabled = true;
			StayOnGround();
			break;
		case GameController.GameState.Kill:
			base.transform.localPosition = new Vector3(base.transform.localPosition.x, GameController.Instance.PlayerLocalStartY, base.transform.localPosition.z);
			break;
		case GameController.GameState.Start:
			hoverRenderer.enabled = true;
			StayOnGround();
			break;
		case GameController.GameState.End:
		case GameController.GameState.Ending:
			StayOnGround();
			break;
		case GameController.GameState.Resume:
		case GameController.GameState.Running:
			doUpdate();
			break;
		case GameController.GameState.Paused:
			break;
		}
	}

	private void doUpdate()
	{
		UpdateFlickering();
		if (inSlowMotion)
		{
			SlowMotionEnabled();
		}
		if (PlayerState == PlayerState.Grinding)
		{
			return;
		}
		if (PlayerState == PlayerState.MakeBurst)
		{
			if (burstOn && UIController.Instance.leftPressed && UIController.Instance.rightPressed)
			{
				inAirTimer += Time.fixedDeltaTime;
				Time.timeScale = Mathf.Lerp(1f, 0.15f, inAirTimer * 25f);
			}
			else
			{
				if (burstOn)
				{
					if (targetActiveObject.indicator.IsOnTarget)
					{
						targetActiveObject.SetKillers(false);
					}
					RailGunEffect.SetActive(true);
					soundController.PlayOnce(PlayerState.Shoot);
					RailGunEffect.transform.position = base.transform.position;
					BurstEffect.SetActive(true);
					burstOn = false;
					Time.timeScale = 1f;
					soundController.PlayOnce(PlayerState.BurstEnd);
					return;
				}
				Vector3 vector = ((!targetActiveObject.indicator.IsOnTarget) ? targetActiveObject.indicator.targetObject.transform.position : targetActiveObject.transform.position);
				RailGunEffect.transform.LookAt(vector, Vector3.forward);
				base.transform.position = Vector3.Lerp(base.transform.position, vector, 6f * Time.deltaTime);
				walker.SetPositionOnSpline(Vector3.Lerp(walker.transform.position, vector, 5f * Time.deltaTime));
				Camera.main.transform.GetComponentInParent<CameraFollowAnimation>().ClearSmoothDamp(1f);
			}
			animationController.easing = 0f;
			ForcePosition(2);
			return;
		}
		if (targetObject != null && UIController.Instance.leftPressed && UIController.Instance.rightPressed)
		{
			if (targetActiveObject != targetObject)
			{
				Camera.main.transform.GetComponentInParent<CameraFollowAnimation>().ClearSmoothDamp();
				walker.SetBoost(1f);
				inAirTimer = 0f;
				soundController.PlayOnce(PlayerState.Targetting);
				soundController.PlayOnce(PlayerState.MakeBurst);
			}
			RailGunEffect.SetActive(false);
			BurstEffect.SetActive(false);
			burstOn = true;
			targetActiveObject = targetObject;
			PlayerState = PlayerState.MakeBurst;
			return;
		}
		Vector3 position = base.transform.position;
		RaycastHit hitInfo;
		inAirRayHit = !Physics.Raycast(base.transform.position, -base.transform.up, out hitInfo, 20f, LevelLayerMask.value);
		if (inAirRayHit || (!inAirRayHit && (hitInfo.distance > 1f || !isJump(hitInfo.collider.gameObject.tag)) && isJump(lastGameObjectTag)))
		{
			inAirTimer = (rotateTimer = (rotateTimer2 = 0f));
			UpdateAirControl();
			ForcePosition();
		}
		else
		{
			if (makingJump && (!lastGameObjectTag.Equals("Jump") || jumpAirTime > 0.2f))
			{
				UpdateLanding(hitInfo);
			}
			UpdateGroundControl(hitInfo, position);
		}
		ForcePosition();
	}

	private void UpdateFlickering()
	{
		if (hitTimer > 0f)
		{
			hitTimer -= Time.fixedDeltaTime;
			hoverRenderer.material = ((!(Mathf.Sin(Time.time * 20f) >= 0f) && !(hitTimer <= 0f)) ? flickerMaterial : normalMaterial);
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

	private Vector3 CheckSides(Vector3 direction, Vector3 moveToPosition, float distance, out bool gotHit)
	{
		Debug.DrawRay(moveToPosition + base.transform.up * 0.5f, direction, Color.blue, 4f);
		RaycastHit hitInfo;
		if (Physics.Raycast(moveToPosition + base.transform.up * 0.5f, direction, out hitInfo, distance, LevelLayerMask.value) && Vector3.Angle(base.transform.up, hitInfo.normal) >= 87f)
		{
			gotHit = true;
			hitSmoothTimer = 0.1f;
			if (animationController.easing == 0f)
			{
				animationController.easing = 3f * base.transform.localPosition.normalized.x;
			}
			else
			{
				animationController.easing = 0f;
			}
			if (direction == base.transform.forward)
			{
				moveToPosition += new Vector3(hitInfo.normal.x * 2f, 0f, hitInfo.normal.z * 0.25f) * distance * 0.5f;
				animationController.easing *= 1.4f;
			}
			else
			{
				moveToPosition = base.transform.position + (moveToPosition + base.transform.up - hitInfo.point).normalized * 0.1f;
			}
			return moveToPosition;
		}
		gotHit = false;
		return moveToPosition;
	}

	private void StayOnGround()
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(base.transform.position, -walker.transform.up, out hitInfo, 20f, LevelLayerMask.value))
		{
			base.transform.rotation = walker.transform.rotation;
			base.transform.position = (base.transform.position - hitInfo.point).normalized * 0.8f + hitInfo.point;
		}
	}

	private bool isRotationObject(string tag)
	{
		return isJump(tag) || tag.Equals("RotatingLand");
	}

	private bool isJump(string tag)
	{
		return tag.Equals("Jump") || tag.Equals("JumpSpecial") || tag.Equals("JumpSmall") || tag.Equals("JumpGrind");
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
				walker.SetBoost(1f);
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
		soundController.PlayOnce(PlayerState);
		if (hit.collider.tag.Equals("Water"))
		{
			LandingEffect = LandingEffects[1];
		}
		else
		{
			LandingEffect = LandingEffects[0];
		}
		LandingEffect.transform.rotation = Quaternion.Euler(hit.normal);
		LandingEffect.transform.position = hit.normal * 0.2f + hit.point;
		LandingEffect.SetActive(true);
		MakeLanding();
	}

	private void UpdateGroundControl(RaycastHit hit, Vector3 moveToPosition)
	{
		if (onGroundTimer > 0f && onGroundTimer < 1f)
		{
			DropOffFromWall();
			return;
		}
		if (hit.distance > 0.90000004f)
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
		if (!isJump(lastGameObjectTag) || dropFromLedge == DropType.Normal)
		{
			bool gotHit = false;
			moveToPosition = CheckSides(-base.transform.right, moveToPosition, 1.3f, out gotHit);
			if (!gotHit)
			{
				moveToPosition = CheckSides(base.transform.right, moveToPosition, 1.3f, out gotHit);
			}
			if (!gotHit)
			{
				moveToPosition = CheckSides(base.transform.forward, moveToPosition, 2.5f, out gotHit);
			}
			base.transform.position = ((!gotHit) ? moveToPosition : Vector3.Lerp(base.transform.position, moveToPosition, hitSmoothTimer += Time.deltaTime * 6f));
			if (gotHit)
			{
				animationController.PlayerHitWall();
				PlayerState = PlayerState.Wobble;
			}
		}
	}

	public void FlickerPlayer()
	{
		hitTimer = 2f;
		soundController.PlayOnce(PlayerState.Flickering);
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
			bool flag = Physics.Raycast(base.transform.position, -base.transform.up, out hitInfo, 99f, LevelLayerMask.value);
			Vector3 zero = Vector3.zero;
			zero = base.transform.up * (0f - Gravity) * Time.deltaTime;
			if (Gravity < 15f)
			{
				Gravity += Time.deltaTime * 30f;
			}
			if ((Gravity <= 3f || (Gravity > 3f && RotateOnGroundInAir())) && hitInfo.distance >= 1f && TrickController.Instance.GetRotation() > 0)
			{
				RotateInAir();
			}
			if (flag && hitInfo.distance < 3f && Gravity > 3f)
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
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation((!isPlayerFlipped) ? walker.transform.forward : (-walker.transform.forward), hitInfo.normal), rotateTimer += Time.deltaTime * 2.5f);
				y = Mathf.DeltaAngle(y, base.transform.rotation.eulerAngles.y);
				jumpRotation += y;
			}
			if (hitInfo.distance < 1f && flag && Gravity > 3f)
			{
				lastGameObjectTag = string.Empty;
			}
			RaycastHit hitInfo2;
			if (Gravity > -3f && Physics.Raycast(base.transform.position, -walker.transform.up, out hitInfo2, 30f, LevelLayerMask.value))
			{
				GraphicsTransform.rotation = Quaternion.Lerp(GraphicsTransform.rotation, base.transform.rotation, rotateTimer2 += Time.deltaTime * 5f);
			}
			if (UIController.Instance.leftPressed && easingInJump < 12f)
			{
				easingInJump += Time.deltaTime * 25f * 0.5f;
			}
			else if (UIController.Instance.rightPressed && easingInJump > -12f)
			{
				easingInJump -= Time.deltaTime * 25f * 0.5f;
			}
			if (easingInJump != 0f && (hitInfo.distance > 1f || Gravity <= 3f))
			{
				Vector3 position = base.transform.position;
				float num = Time.deltaTime * easingInJump;
				if (Gravity < 15f)
				{
					Gravity += Mathf.Abs(num);
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
		if (Mathf.Abs(jumpRotation) < (float)TrickController.Instance.GetRotation())
		{
			float num = (float)(((!isPlayerFlipped) ? 1 : (-1)) * TrickController.Instance.GetRotationSpeed()) * Time.deltaTime;
			jumpRotation += num;
			base.transform.Rotate(0f, num, 0f);
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
		soundController.PlayOnce(PlayerState);
		Gravity = JumpGravityForce[(int)dropFromLedge];
		if (lastGameObjectTag.Equals("JumpSmall"))
		{
			Gravity = -4f;
		}
		makingJump = true;
		easingInJump = 0.5f * ((onGroundTimer != 0f) ? 0f : ((!isPlayerFlipped) ? animationController.easing : (0f - animationController.easing)));
		animationController.easing = 0f;
		jumpRotation = (jumpAirTime = (rotateTimer = (rotateTimer2 = 0f)));
		if (lastGameObjectTag.Equals("JumpGrind") || jumpFromGrind)
		{
			if (grindControl.completion > 0.9f)
			{
				animationController.SetJumpState(JumpAnimationState.Jump, TrickController.Instance.RandomizeTrick());
			}
			else
			{
				animationController.SetJumpState(JumpAnimationState.Jump, TrickController.Instance.BasicTrick());
			}
		}
		else if (lastGameObjectTag.Equals("Jump"))
		{
			animationController.SetJumpState(JumpAnimationState.Jump, TrickController.Instance.RandomizeTrick());
		}
		else
		{
			Gravity = -7f;
			animationController.SetJumpState(JumpAnimationState.Jump, TrickController.Instance.FlipTrick());
		}
		jumpFromGrind = false;
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
			if (TrickController.Instance.GetTrickExtra() > 0)
			{
				walker.SetBoost(0.75f, walker.NormalSpeed * 1.15f);
			}
			animationController.SetJumpState(JumpAnimationState.Landing, isPlayerFlipped ? 1 : 0);
			if (num > 45f)
			{
				animationController.PlayerHitWall();
			}
		}
		onGroundTimer = 0f;
		makingJump = false;
		wasFlipped = isPlayerFlipped;
	}

	public void SetOffLimits()
	{
		OffLimits = !OffLimits;
		walker.SlowdownEnabled = OffLimits;
	}

	public bool isOnPipe()
	{
		return lastGameObjectTag.Equals("Pipe");
	}

	public void ResetFromUI()
	{
		grindControl.Reset();
		ResetPlayer();
	}

	public void SlowMotionEnabled()
	{
		slowMotionTimer -= Time.fixedDeltaTime;
		slowMotionParticle.GetComponent<ParticleSystem>().emissionRate = slowMotionTimer * 100f;
		if (slowMotionTimer >= 0f)
		{
			if (Time.timeScale > 0.51f)
			{
				slowMotionSmooth += Time.fixedDeltaTime;
				Time.timeScale = Mathf.Lerp(1f, 0.5f, slowMotionSmooth * 10f);
			}
			else
			{
				Time.timeScale = 0.5f;
				slowMotionSmooth = 0f;
			}
			return;
		}
		slowMotionSmooth += Time.fixedDeltaTime;
		Time.timeScale = Mathf.Lerp(0.5f, 1f, slowMotionSmooth);
		if (Time.timeScale >= 0.99f)
		{
			SetSlowMotion(false);
			slowMotionOverLay.SetActive(false);
		}
	}

	public void SetSlowMotion(bool enabled)
	{
		if (enabled)
		{
			inSlowMotion = true;
			soundController.PlayOnce(PlayerState.SlowMotion);
			slowMotionTimer = 4f;
		}
		else
		{
			inSlowMotion = false;
			slowMotionTimer = 4f;
			Time.timeScale = 1f;
			slowMotionSmooth = 0f;
		}
		if (slowMotionOverLay != null)
		{
			slowMotionOverLay.SetActive(enabled);
		}
		slowMotionParticle.SetActive(enabled);
	}

	public void SetTargetObject(TargetObject target, bool breakDone = false)
	{
		targetObject = target;
		targetActiveObject = null;
		PlayerState = PlayerState.Leaning;
		if (breakDone)
		{
			Time.timeScale = 1f;
		}
	}

	public void SetOnRail(bool enable, CurvySpline path, GrindTrigger trigger)
	{
		if ((PlayerState == PlayerState.Grinding && enable) || PlayerState == PlayerState.Dying)
		{
			return;
		}
		if (enable)
		{
			if (grindControl.CanGrind(path, trigger))
			{
				Camera.main.transform.GetComponentInParent<CameraFollowAnimation>().ClearSmoothDamp();
				soundController.PlayOnce(PlayerState.GrindStart);
				animationController.easing = 0f;
				PlayerState = PlayerState.Grinding;
				if (makingJump)
				{
					MakeLanding();
				}
				makingJump = false;
				onGroundTimer = (inAirTimer = (rotateTimer = (rotateTimer2 = 0f)));
				grindControl.SetGrindEnabled(path, trigger, walker.Speed);
				isPlayerFlipped = false;
				animationController.SetJumpState(JumpAnimationState.Sliding);
			}
		}
		else
		{
			dropFromLedge = DropType.Normal;
			GraphicsTransform.localPosition = GfxNormalPosition;
			int num = (UIController.Instance.leftPressed ? 16 : (UIController.Instance.rightPressed ? (-16) : 0)) + grindControl.jumpOffDiff;
			animationController.easing = num;
			PlayerState = PlayerState.InAir;
			walker.SetPositionOnSpline(base.transform.position);
			ForcePosition();
			lastGameObjectTag = "JumpGrind";
			jumpFromGrind = true;
		}
	}

	public void KillPlayer()
	{
		if (PlayerState != PlayerState.Dying)
		{
			PlayerState = PlayerState.Dying;
			hoverRenderer.enabled = false;
			soundController.PlayOnce(PlayerState);
			animationController.animator.gameObject.SetActive(false);
		}
	}
}
