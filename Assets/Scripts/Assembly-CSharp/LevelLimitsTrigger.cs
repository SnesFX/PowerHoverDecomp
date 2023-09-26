using UnityEngine;

public class LevelLimitsTrigger : ResetObject
{
	private const float BLINK_SPEED = 4.5f;

	public bool isOnPath;

	public MeshCollider otherCollider;

	private MeshCollider ownCollider;

	private Material fadeMaterialClone;

	private HoverController hoverController;

	private float fadingAmount;

	private Color fadeColor;

	private float blinkSpeed;

	private void Start()
	{
		hoverController = Object.FindObjectOfType<HoverController>();
		ownCollider = GetComponent<MeshCollider>();
		ownCollider.enabled = isOnPath;
		if (isOnPath)
		{
			fadeMaterialClone = Object.Instantiate(GetComponent<Renderer>().material);
			GetComponent<Renderer>().material = fadeMaterialClone;
			fadeColor = fadeMaterialClone.color;
			fadeColor.a = 0f;
			fadeMaterialClone.color = fadeColor;
		}
	}

	public override void Reset(bool onlyNonSaved)
	{
		ownCollider.enabled = isOnPath;
		if (isOnPath)
		{
			fadeColor.a = 0f;
			fadeMaterialClone.color = fadeColor;
		}
	}

	private void FixedUpdate()
	{
		if (isOnPath)
		{
			if (hoverController.OffLimits && GameController.Instance.State != GameController.GameState.Kill)
			{
				blinkSpeed += Time.fixedDeltaTime * 0.5f * blinkSpeed;
				fadeColor.a = Mathf.PingPong(blinkSpeed, 1f);
				fadeMaterialClone.color = fadeColor;
			}
			else if (fadeColor.a != 0f)
			{
				fadeColor.a = 0f;
				fadeMaterialClone.color = fadeColor;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		SetOnTrigger(other);
	}

	private void SetOnTrigger(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			blinkSpeed = 4.5f;
			if (isOnPath != hoverController.OffLimits)
			{
				hoverController.SetOffLimits();
				ownCollider.enabled = false;
				otherCollider.enabled = true;
			}
		}
	}
}
