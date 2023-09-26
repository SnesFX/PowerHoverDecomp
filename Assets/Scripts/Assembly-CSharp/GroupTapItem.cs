using UnityEngine;
using UnityEngine.UI;

public class GroupTapItem : MonoBehaviour
{
	public Color enabledColor = Color.black;

	public Color disabledColor;

	public Image tapImage;

	public float ScrollPosition { get; set; }

	public void ScrollingTo(float scroll)
	{
		if (Mathf.Abs(ScrollPosition - scroll) < 10f)
		{
			tapImage.color = enabledColor;
		}
		else
		{
			tapImage.color = disabledColor;
		}
	}
}
