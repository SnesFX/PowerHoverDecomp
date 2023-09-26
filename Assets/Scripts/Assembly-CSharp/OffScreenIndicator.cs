using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class OffScreenIndicator : MonoBehaviour
{
	public GameObject objectToFollow;

	public Text offScreenDistance;

	public bool isPlayer;

	private Image onScreenImage;

	private Vector3 screenCenter;

	private Vector3 screenBounds;

	private Color enabledColor;

	private Color disabledColor;

	private float colorLerp;

	private HoverController hoverController;

	private float screenOffset = 0.07f;

	private void Start()
	{
		onScreenImage = GetComponent<Image>();
		screenCenter = new Vector3(Screen.width, Screen.height, 0f) * 0.5f;
		screenBounds = screenCenter * 0.95f;
		enabledColor = onScreenImage.color;
		disabledColor = enabledColor;
		disabledColor.a = 0f;
		onScreenImage.color = disabledColor;
		onScreenImage.enabled = false;
		offScreenDistance.color = enabledColor;
		if (isPlayer && objectToFollow == null)
		{
			hoverController = UnityEngine.Object.FindObjectOfType<HoverController>();
			objectToFollow = hoverController.gameObject;
		}
	}

	private void Update()
	{
		if (objectToFollow == null || !objectToFollow.activeSelf)
		{
			return;
		}
		Vector3 vector = Camera.main.WorldToViewportPoint(objectToFollow.transform.position);
		if (vector.x < 0f - screenOffset || vector.x > 1f + screenOffset || vector.y < 0f - screenOffset || vector.y > 1f + screenOffset)
		{
			Vector3 vector2 = Camera.main.WorldToScreenPoint(objectToFollow.transform.position);
			if (!isPlayer && (vector2.y > (float)Screen.height * 0.5f || vector2.y < (float)(-Screen.height) || vector2.x > (float)(Screen.width * 2) || vector2.x < (float)(-Screen.width)))
			{
				onScreenImage.enabled = false;
				if ((bool)offScreenDistance)
				{
					offScreenDistance.text = string.Empty;
				}
				return;
			}
			onScreenImage.enabled = true;
			vector2 -= screenCenter;
			float num = Mathf.Atan2(vector2.y, vector2.x);
			num -= (float)Math.PI / 2f;
			float num2 = Mathf.Cos(num);
			float num3 = 0f - Mathf.Sin(num);
			vector2 = screenCenter;
			vector2.x = num3 * 150f;
			vector2.y = num2 * 150f;
			vector2.z = 0f;
			float num4 = num2 / num3;
			if (num2 > 0f)
			{
				vector2.x = screenBounds.y / num4;
				vector2.y = screenBounds.y;
			}
			else
			{
				vector2.x = (0f - screenBounds.y) / num4;
				vector2.y = 0f - screenBounds.y;
			}
			if (vector2.x > screenBounds.x)
			{
				vector2.x = screenBounds.x;
				vector2.y = screenBounds.x * num4;
			}
			else if (vector2.x < 0f - screenBounds.x)
			{
				vector2.x = 0f - screenBounds.x;
				vector2.y = (0f - screenBounds.x) * num4;
			}
			vector2 += screenCenter;
			if ((bool)offScreenDistance)
			{
				if (vector.x < 0f || vector.x > 1f)
				{
					if (vector.x > 1f)
					{
						vector.x -= 1f;
					}
					offScreenDistance.text = string.Format("{0:F0}m", Mathf.Abs(vector.x) * 100f);
				}
				else if (vector.y < 0f || vector.y > 1f)
				{
					if (vector.y > 1f)
					{
						vector.y -= 1f;
					}
					offScreenDistance.text = string.Format("{0:F0}m", Mathf.Abs(vector.y) * 100f);
				}
				else
				{
					offScreenDistance.text = string.Empty;
				}
			}
			base.transform.position = vector2;
			base.transform.localRotation = Quaternion.Euler(0f, 0f, num * 57.29578f);
			if (colorLerp < 1f)
			{
				onScreenImage.color = Color.Lerp(onScreenImage.color, enabledColor, colorLerp += Time.deltaTime * 4f);
			}
			else if (isPlayer && !hoverController.OffLimits)
			{
				hoverController.SetOffLimits();
			}
		}
		else if (colorLerp > 0f && onScreenImage.enabled)
		{
			if (isPlayer && hoverController.OffLimits)
			{
				hoverController.SetOffLimits();
			}
			onScreenImage.color = Color.Lerp(onScreenImage.color, disabledColor, 1f - (colorLerp -= Time.deltaTime * 5f));
			if ((bool)offScreenDistance && offScreenDistance.text != string.Empty)
			{
				offScreenDistance.text = string.Empty;
			}
		}
	}
}
