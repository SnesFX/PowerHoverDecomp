using UnityEngine;

public class MapEnergyIcon : MonoBehaviour
{
	public TextMesh costText;

	public Color fontNotEnoughMoneyColor;

	public AudioSource selectAudio;

	private Vector3 startScale;

	private Color defaultFontColor;

	private Vector3 selectedPosition;

	private Vector3 startPosition;

	private float zoomTimer;

	private int currentTotal;

	public Camera Cam { get; set; }

	public MapObject mapObject { get; private set; }

	public bool IsActivated { get; private set; }

	private void Start()
	{
		startScale = base.transform.localScale;
		base.transform.rotation = Quaternion.Euler(new Vector3(332f, 165f, 320f));
		startPosition = base.transform.localPosition;
		selectedPosition = base.transform.localPosition + Vector3.up * base.transform.localPosition.y * 0.5f;
		defaultFontColor = costText.color;
	}

	public void SetMapObject(MapObject obj)
	{
		mapObject = obj;
		SetCost(mapObject.BatteryCost);
	}

	private void SetCost(int cost)
	{
		if (currentTotal != cost)
		{
			costText.text = string.Format("{0}", cost);
			currentTotal = cost;
		}
		if (GameStats.Instance != null)
		{
			if (mapObject.BatteryCost > GameStats.Instance.TotalBattery && costText.color == defaultFontColor)
			{
				costText.color = fontNotEnoughMoneyColor;
			}
			else if (mapObject.BatteryCost <= GameStats.Instance.TotalBattery && costText.color == fontNotEnoughMoneyColor)
			{
				costText.color = defaultFontColor;
			}
		}
	}

	private void FixedUpdate()
	{
		if (Cam.enabled)
		{
			if (IsActivated)
			{
				base.transform.localScale = Vector3.Lerp(base.transform.localScale, startScale * 2.5f, Time.fixedDeltaTime * 10f);
				base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, selectedPosition, Time.fixedDeltaTime * 10f);
			}
			else if (zoomTimer > 0f)
			{
				zoomTimer -= Time.fixedDeltaTime;
				base.transform.localScale = Vector3.Lerp(base.transform.localScale, startScale, Time.fixedDeltaTime * 5f);
				base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, startPosition, Time.fixedDeltaTime * 10f);
			}
			if (mapObject != null)
			{
				SetCost(mapObject.BatteryCost);
			}
		}
	}

	public void SelectFromMap(bool enable)
	{
		zoomTimer = 2f;
		IsActivated = enable;
		if (enable)
		{
			selectAudio.Play();
		}
	}
}
