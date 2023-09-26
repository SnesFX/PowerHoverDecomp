using UnityEngine;

public class Collectable : ResetObject
{
	public enum CollectableType
	{
		Normal = 0,
		Boost = 1,
		Letter = 2,
		Casette = 3,
		Heart = 4,
		Bolt = 5,
		SlowMotion = 6,
		Bomb = 7,
		GhostCoin = 8,
		SuperCollectable = 9,
		WireBomb = 10
	}

	public enum HoverLetter
	{
		H = 0,
		O = 1,
		V = 2,
		E = 3,
		R = 4
	}

	private const string COLLECTABLE_TAG = "PlayerCollectables";

	public static int LETTER_COUNT = 5;

	private static float MAGNET_DISTANCE = 7f;

	private static float MAGNET_MAX_FOLLOW_SPEED = 8f;

	public float rotationSpeed = 1f;

	public float movementSpeed = 1f;

	public float motionScale;

	public GameObject colletableVisual;

	public GameObject collectableShadow;

	public GameObject collectableNumber;

	public Material collectedMaterial;

	public Material oceanShadowMaterial;

	public bool hasBlobShadow;

	public float shadowOffsetY = 0.2f;

	[SerializeField]
	[HideInInspector]
	public CollectableType Type;

	[SerializeField]
	[HideInInspector]
	public float BoostForce;

	[SerializeField]
	[HideInInspector]
	public float BoostTime;

	[SerializeField]
	[HideInInspector]
	public int MusicID;

	[SerializeField]
	[HideInInspector]
	public HoverLetter hoverLetter;

	[SerializeField]
	[HideInInspector]
	public int distanceBoost;

	[SerializeField]
	[HideInInspector]
	public GameObject explosion;

	[SerializeField]
	[HideInInspector]
	public GameObject collectedLetterReplacementItem;

	[SerializeField]
	[HideInInspector]
	public int coinValue;

	[SerializeField]
	[HideInInspector]
	public AudioClip superCollected;

	private AudioSource collectSound;

	private bool collected;

	private Vector3 pos;

	private float startY;

	private Vector3 visualRotation;

	private float timeToFade;

	private float offset;

	private float movementAmount;

	private float startShadowScale;

	private Vector3 shadowScale;

	private Vector3 shadowStartScale;

	private Vector3 startScale;

	private SplineWalker walker;

	private float magnetSpeed;

	private Vector3 startPosition;

	private CollectEffect collectEffect;

	private bool initialized;

	private LayerMask workMask;

	private Animator batteryAnim;

	private float aliveTimer;

	public bool StopUpdate { get; set; }

	public bool DroppedOnCrash { get; set; }

	private void Awake()
	{
		collectSound = GetComponent<AudioSource>();
		walker = Object.FindObjectOfType<SplineWalker>();
		workMask = LayerMask.GetMask("Ground", "Level", "Default");
		startPosition = base.transform.localPosition;
		pos = Vector3.zero;
		if (colletableVisual != null)
		{
			startScale = colletableVisual.transform.localScale;
			startY = base.transform.localPosition.y + motionScale * 0.5f;
			movementAmount = motionScale;
			pos = colletableVisual.transform.localPosition;
		}
		if (collectableShadow != null)
		{
			shadowStartScale = collectableShadow.transform.localScale;
			if (!hasBlobShadow)
			{
				collectableShadow.GetComponent<Renderer>().enabled = false;
				return;
			}
			shadowScale = collectableShadow.transform.localScale;
			startShadowScale = shadowScale.y;
		}
	}

	private void Start()
	{
		switch (Type)
		{
		case CollectableType.Bolt:
			if (GameDataController.Exists(string.Format("BOLT_{0}", startPosition.ToString())))
			{
				Object.Destroy(base.gameObject);
				return;
			}
			collectEffect = (Object.Instantiate(Resources.Load("CollectEffect")) as GameObject).GetComponent<CollectEffect>();
			collectEffect.transform.parent = base.transform.parent;
			collectEffect.transform.localPosition = base.transform.localPosition;
			collectEffect.gameObject.SetActive(false);
			break;
		case CollectableType.SlowMotion:
		case CollectableType.SuperCollectable:
			collectEffect = (Object.Instantiate(Resources.Load("CollectEffect")) as GameObject).GetComponent<CollectEffect>();
			collectEffect.transform.parent = base.transform.parent;
			collectEffect.transform.localPosition = base.transform.localPosition;
			collectEffect.gameObject.SetActive(false);
			break;
		case CollectableType.Letter:
			if (SceneLoader.Instance.Current != null && SceneLoader.Instance.Current.Storage.CollectableLetters[(int)hoverLetter] != 0)
			{
				GameObject gameObject = Object.Instantiate(collectedLetterReplacementItem);
				gameObject.transform.parent = base.transform.parent;
				gameObject.transform.localPosition = base.transform.localPosition;
				gameObject.gameObject.SetActive(true);
				Object.Destroy(base.gameObject);
			}
			break;
		case CollectableType.Normal:
		case CollectableType.GhostCoin:
			if (base.transform.GetComponentInChildren<Animator>() != null)
			{
				batteryAnim = base.transform.GetComponentInChildren<Animator>();
			}
			if (!DeviceSettings.Instance.EnableBatteryRotation)
			{
				ContinuosRotation[] componentsInChildren = base.transform.GetComponentsInChildren<ContinuosRotation>();
				foreach (ContinuosRotation continuosRotation in componentsInChildren)
				{
					continuosRotation.enabled = false;
				}
			}
			break;
		}
		initialized = true;
	}

	private void FixedUpdate()
	{
		if (StopUpdate)
		{
			return;
		}
		Vector3 vector = Camera.main.WorldToViewportPoint(base.transform.position);
		if (vector.x < 0f || vector.x > 1f || vector.y < 0f || vector.y > 1f)
		{
			return;
		}
		if (GameController.Instance.State == GameController.GameState.Running && Type != CollectableType.Bomb && Type != CollectableType.WireBomb)
		{
			UpdateMagnet();
		}
		if (DeviceSettings.Instance.EnableBatteryRotation)
		{
			colletableVisual.transform.Rotate(0f, (Type == CollectableType.Heart) ? 0f : rotationSpeed, (Type == CollectableType.Heart) ? 1 : 0);
		}
		if (collected)
		{
			timeToFade += Time.fixedDeltaTime;
			if (timeToFade > 0.1f)
			{
				StopUpdate = true;
				if (Type != CollectableType.GhostCoin && Type != 0)
				{
					ViewCoinVisuals(false);
				}
			}
			else if (!colletableVisual.GetComponent<Renderer>().enabled)
			{
				ViewCoinVisuals(true);
			}
			return;
		}
		if (collectableNumber != null)
		{
			Vector3 position = Camera.main.transform.position;
			collectableNumber.transform.LookAt(position);
			collectableNumber.transform.Rotate(Vector3.up, 90f);
		}
		if (motionScale != 0f)
		{
			float num = Mathf.PingPong(offset + Time.time * movementSpeed, movementAmount);
			pos.y = startY - num;
			if (!collected && hasBlobShadow && startShadowScale - num * 0.5f > 0f)
			{
				shadowScale.z = (shadowScale.x = startShadowScale + num * 0.5f);
				collectableShadow.transform.localScale = shadowScale;
			}
		}
	}

	private void OnEnable()
	{
		UpdateWithRaycast();
	}

	private void OnDestroy()
	{
		if (Type == CollectableType.Bomb && walker != null && walker.hoverController != null)
		{
			explosion.transform.parent = GameController.Instance.transform;
			explosion.GetComponent<GhostBomb>().enabled = true;
		}
	}

	private void UpdateWithRaycast()
	{
		if (Type == CollectableType.SlowMotion)
		{
			RaycastHit hitInfo;
			if (Physics.Raycast(base.transform.position, -base.transform.up, out hitInfo, 999f, workMask.value))
			{
				Vector3 point = hitInfo.point;
				point += hitInfo.normal * 2.2f;
				base.transform.position = point;
			}
		}
		else
		{
			RaycastHit hitInfo2;
			if ((Type != 0 && Type != CollectableType.GhostCoin && Type != CollectableType.Bomb) || !(collectableShadow != null) || !Physics.Raycast(base.transform.position, -base.transform.up, out hitInfo2, 999f, workMask.value))
			{
				return;
			}
			if (hitInfo2.distance > 5f)
			{
				collectableShadow.GetComponent<Renderer>().enabled = false;
				return;
			}
			collectableShadow.GetComponent<Renderer>().enabled = true;
			Vector3 point2 = hitInfo2.point;
			point2 += hitInfo2.normal * shadowOffsetY;
			collectableShadow.transform.up = hitInfo2.normal;
			collectableShadow.transform.position = point2;
			if (hitInfo2.collider.CompareTag("Water"))
			{
				collectableShadow.GetComponent<Renderer>().material = oceanShadowMaterial;
				if (DeviceSettings.Instance != null && DeviceSettings.Instance.EnableFloatingBatteryShadows && hitInfo2.collider.transform.GetComponent<Wave>() != null)
				{
					FloatingObject floatingObject = base.gameObject.AddComponent<FloatingObject>();
					floatingObject.YOffSet = 0.1f;
					floatingObject.surfaceObject = base.transform;
				}
			}
		}
	}

	private void ViewCoinVisuals(bool enable)
	{
		if (collectableShadow != null)
		{
			collectableShadow.GetComponent<Renderer>().enabled = enable;
		}
		if (collectableNumber != null)
		{
			if (collectableNumber.GetComponent<Renderer>() != null)
			{
				collectableNumber.GetComponent<Renderer>().enabled = enable;
			}
			else if (enable)
			{
				collectableNumber.SetActive(enable);
			}
		}
		colletableVisual.GetComponent<Renderer>().enabled = enable;
	}

	private void UpdateMagnet()
	{
		int num = ((!SceneLoader.Instance.Current.IsChallenge) ? PlayerStats.Instance.GetMagnetLevel() : (TrickController.Instance.ChallengeCharacter.Magnet * 2));
		if (num > 0)
		{
			if (!collected && Vector3.Distance(base.transform.position, walker.hoverController.transform.position) < MAGNET_DISTANCE + (float)(SceneLoader.Instance.Current.IsChallenge ? num : 0))
			{
				magnetSpeed = Mathf.Min(magnetSpeed + Time.fixedDeltaTime * 40f, MAGNET_MAX_FOLLOW_SPEED + (float)num * 0.5f);
				base.transform.position = Vector3.MoveTowards(base.transform.position, walker.hoverController.transform.position, Time.fixedDeltaTime * magnetSpeed);
			}
			else if (magnetSpeed > 0f)
			{
				magnetSpeed = 0f;
			}
		}
	}

	public override void Reset(bool isRewind)
	{
		if (!initialized)
		{
			return;
		}
		if (Type == CollectableType.Bomb || Type == CollectableType.GhostCoin || (!isRewind && DroppedOnCrash))
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			if (DroppedOnCrash || Type == CollectableType.Casette)
			{
				return;
			}
			if (Type == CollectableType.WireBomb)
			{
				Animator component = explosion.GetComponent<Animator>();
				component.enabled = true;
				component.Play("EmpBombExplosionStart", -1, 0f);
				collected = false;
				GetComponent<Collider>().enabled = true;
				ViewCoinVisuals(true);
				StopUpdate = false;
				return;
			}
			if (Type == CollectableType.Normal)
			{
				if (batteryAnim.enabled)
				{
					batteryAnim.Play("CollectBatteryReset", -1, 0f);
				}
				else
				{
					collectableNumber.transform.localScale = Vector3.one * 0.5f;
				}
			}
			else if (collected && isRewind && Type != CollectableType.Heart)
			{
				GetComponent<Collider>().enabled = false;
				ViewCoinVisuals(false);
				return;
			}
			base.transform.localPosition = startPosition;
			if (Type == CollectableType.Letter || (Type == CollectableType.Bolt && collected))
			{
				Object.Destroy(base.gameObject);
			}
			else if (!isRewind || Type != CollectableType.SuperCollectable || !collected)
			{
				collected = false;
				GetComponent<Collider>().enabled = true;
				colletableVisual.transform.localScale = startScale;
				if (collectableShadow != null)
				{
					collectableShadow.transform.localScale = shadowStartScale;
					shadowScale = shadowStartScale;
				}
				ViewCoinVisuals(true);
				UpdateWithRaycast();
				StopUpdate = false;
				timeToFade = 0f;
				rotationSpeed = 1f;
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.gameObject.CompareTag("PlayerCollectables") || StopUpdate || GameController.Instance.State != GameController.GameState.Running)
		{
			return;
		}
		GetComponent<Collider>().enabled = false;
		if (!collected)
		{
			switch (Type)
			{
			case CollectableType.Normal:
			case CollectableType.GhostCoin:
				if (batteryAnim != null)
				{
					batteryAnim.enabled = true;
					batteryAnim.Play("CollectBattery", -1, 0f);
				}
				if (collectableShadow != null)
				{
					collectableShadow.GetComponent<Renderer>().enabled = false;
				}
				if (collectSound != null && !collectSound.isPlaying)
				{
					collectSound.pitch = 1f + 0.1f * LevelStats.Instance.CollectablePitch;
					collectSound.Play();
				}
				LevelStats.Instance.Collect();
				collected = true;
				return;
			case CollectableType.WireBomb:
			{
				Animator component = explosion.GetComponent<Animator>();
				component.speed = 1.5f;
				component.enabled = true;
				explosion.GetComponent<GhostBomb>().SetCheckDistance();
				component.Play("EmpBombExplosion", -1, 0f);
				break;
			}
			case CollectableType.Bomb:
			{
				Animator component2 = explosion.GetComponent<Animator>();
				component2.enabled = true;
				component2.Play("EmpBombExplosion", -1, 0f);
				return;
			}
			case CollectableType.Casette:
				Main.Instance.IngameOff();
				CutSceneLoader.Instance.StartCutScene("MenuCutSceneIntro");
				break;
			case CollectableType.Heart:
				LifeController.Instance.ChangeLifes(true);
				break;
			case CollectableType.Bolt:
				GameStats.Instance.TotalBattery += coinValue;
				if (collectEffect != null)
				{
					collectEffect.GetComponent<CollectEffect>().Show(base.transform.localPosition, walker.transform.rotation);
				}
				GameDataController.Save(true, string.Format("BOLT_{0}", startPosition.ToString()));
				break;
			case CollectableType.Boost:
				walker.SetBoost(BoostTime, BoostForce);
				break;
			case CollectableType.SlowMotion:
				if (collectEffect != null)
				{
					collectEffect.GetComponent<CollectEffect>().Show(base.transform.localPosition, walker.transform.rotation);
				}
				walker.hoverController.SetSlowMotion(true);
				break;
			case CollectableType.SuperCollectable:
				if (LevelStats.Instance.SuperCollectableCounter == 5 && superCollected != null)
				{
					collectSound.pitch = 1f;
					collectSound.PlayOneShot(superCollected);
				}
				break;
			}
			if (collectEffect != null)
			{
				collectEffect.GetComponent<CollectEffect>().Show(base.transform.localPosition, walker.transform.rotation);
			}
			rotationSpeed *= 5f;
			if (collectSound != null && !collectSound.isPlaying)
			{
				collectSound.pitch = 0.9f + 0.1f * ((Type != CollectableType.SuperCollectable) ? LevelStats.Instance.CollectablePitch : ((float)LevelStats.Instance.SuperCollectableCounter));
				collectSound.Play();
			}
		}
		collected = true;
	}
}
