using UnityEngine;
using UnityEngine.UI;

public class ActivationRing : MonoBehaviour
{
	private enum ActivateState
	{
		None = 0,
		Zooming = 1,
		Rotating = 2,
		Completed = 3,
		ZoomOut = 4
	}

	public TextMesh counterText;

	public Image fillImage;

	public MeshRenderer icon;

	public AudioSource ringElectric;

	public AudioSource cannotUse;

	public AudioSource startAndComplete;

	private float pressTimer;

	private Vector3 maxSize;

	private MapObject mapObj;

	private float activatingSpeed;

	private int startCost;

	private int takeOutCounter;

	private ActivateState State;

	public bool IsActive
	{
		get
		{
			return State != ActivateState.None;
		}
	}

	private void Start()
	{
		maxSize = base.transform.localScale;
		Active(false);
	}

	private void Update()
	{
		if (State == ActivateState.None)
		{
			return;
		}
		pressTimer += Time.deltaTime;
		if (!(pressTimer > 0f))
		{
			return;
		}
		switch (State)
		{
		case ActivateState.Zooming:
			base.transform.localScale = Vector3.Lerp(Vector3.zero, maxSize, pressTimer *= 2f);
			if (pressTimer > 1f)
			{
				pressTimer = 0f;
				takeOutCounter = 0;
				if (GameStats.Instance.TotalBattery == 0)
				{
					cannotUse.Play();
					State = ActivateState.ZoomOut;
				}
				else
				{
					State = ActivateState.Rotating;
				}
			}
			break;
		case ActivateState.Rotating:
			ringElectric.pitch = 0.7f + pressTimer * 0.5f;
			pressTimer += Time.deltaTime * activatingSpeed;
			fillImage.fillAmount = pressTimer;
			mapObj.BatteryCost = Mathf.CeilToInt(Mathf.Lerp(startCost, 0f, fillImage.fillAmount));
			counterText.text = string.Format("{0}", mapObj.BatteryCost);
			takeOutCounter = startCost - mapObj.BatteryCost;
			if (GameStats.Instance.TotalBattery - takeOutCounter <= 0)
			{
				mapObj.BatteryCost = startCost - GameStats.Instance.TotalBattery;
				GameStats.Instance.TotalBattery = 0;
				takeOutCounter = 0;
				pressTimer = 2f;
			}
			if (pressTimer > 1f)
			{
				pressTimer = 0f;
				State = ActivateState.Completed;
			}
			break;
		case ActivateState.Completed:
			if (mapObj.BatteryCost == 0)
			{
				startAndComplete.Play();
				mapObj.Unlock();
			}
			State = ActivateState.ZoomOut;
			pressTimer = 0f;
			GameStats.Instance.TotalBattery -= takeOutCounter;
			takeOutCounter = 0;
			break;
		case ActivateState.ZoomOut:
			base.transform.localScale = Vector3.Lerp(maxSize, Vector3.zero, pressTimer *= 2f);
			if (pressTimer > 1f)
			{
				pressTimer = 0f;
				Active(false);
			}
			break;
		}
	}

	public void Active(bool enable, MapObject mapOb = null)
	{
		if (mapObj != null && mapObj.energyIcon.IsActivated)
		{
			mapObj.energyIcon.SelectFromMap(false);
		}
		if (enable)
		{
			mapObj = mapOb;
			counterText.text = string.Format("{0}", mapObj.BatteryCost);
			startCost = mapObj.BatteryCost;
			activatingSpeed = 0.5f / (float)mapObj.BatteryCost;
			takeOutCounter = 0;
			State = ActivateState.Zooming;
			ringElectric.Play();
			startAndComplete.Play();
			pressTimer = -0.05f;
			base.transform.localScale = Vector3.zero;
			icon.enabled = true;
			counterText.gameObject.SetActive(true);
			fillImage.gameObject.SetActive(true);
		}
		else
		{
			ringElectric.Stop();
			State = ActivateState.None;
			base.transform.localScale = Vector3.zero;
			icon.enabled = false;
			counterText.gameObject.SetActive(false);
			fillImage.gameObject.SetActive(false);
			if (takeOutCounter > 0)
			{
				GameStats.Instance.TotalBattery -= takeOutCounter;
				takeOutCounter = 0;
			}
		}
	}
}
