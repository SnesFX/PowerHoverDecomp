using UnityEngine;

public class TrailEffect : MonoBehaviour
{
	private const string COLOR_PREFIX = "_TintColor";

	public float fadeSpeed;

	public Material fadingMaterial;

	public Material fadingMaterialGrounded;

	public GameObject trail1;

	public GameObject trail2;

	public GameObject trailGrounded;

	private Material fadeMaterialCenterClone;

	private Material fadeMaterialClone;

	private float fadingStartAmount;

	private float fadingAmount;

	private Color fadeColor;

	private Color fadeColorCenter;

	private float fadingAmountCenter;

	private bool startFade;

	private bool grounded;

	private void Awake()
	{
		fadeMaterialClone = Object.Instantiate(fadingMaterial);
		fadeColor = fadeMaterialClone.GetColor("_TintColor");
		fadingAmount = fadeColor.a;
		if (fadingMaterialGrounded != null)
		{
			fadeMaterialCenterClone = Object.Instantiate(fadingMaterialGrounded);
			fadeColorCenter = fadeMaterialCenterClone.GetColor("_TintColor");
			fadingAmountCenter = fadeColorCenter.a;
		}
		if (trail1 != null)
		{
			trail1.GetComponent<Renderer>().material = fadeMaterialClone;
		}
		if (trail2 != null)
		{
			trail2.GetComponent<Renderer>().material = fadeMaterialClone;
		}
		if (trailGrounded != null)
		{
			trailGrounded.GetComponent<Renderer>().material = fadeMaterialCenterClone;
		}
	}

	public void StartTrail(bool onGround)
	{
		grounded = onGround;
		if (trail1 != null)
		{
			trail1.SetActive(!grounded);
		}
		if (trail2 != null)
		{
			trail2.SetActive(!grounded);
		}
		if (trailGrounded != null)
		{
			trailGrounded.SetActive(grounded);
		}
	}

	public void StartFade()
	{
		startFade = true;
	}

	private void Update()
	{
		if (startFade)
		{
			FadeColor();
		}
	}

	private void FadeColor()
	{
		if (grounded)
		{
			fadeColorCenter.a = fadingAmountCenter;
			fadeMaterialCenterClone.SetColor("_TintColor", fadeColorCenter);
			if (fadingAmountCenter > 0f)
			{
				fadingAmountCenter -= Time.deltaTime * fadeSpeed;
			}
			else
			{
				Reset();
			}
		}
		else
		{
			fadeColor.a = fadingAmount;
			fadeMaterialClone.SetColor("_TintColor", fadeColor);
			if (fadingAmount > 0f)
			{
				fadingAmount -= Time.deltaTime * fadeSpeed;
			}
			else
			{
				Reset();
			}
		}
	}

	public void Reset()
	{
		startFade = false;
		Object.Destroy(base.gameObject);
	}
}
