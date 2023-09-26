using UnityEngine;
using UnityEngine.UI;

public class UIItem : MonoBehaviour
{
	public enum ItemType
	{
		None = 0,
		Boost = 1
	}

	public Sprite enabledImage;

	public ItemType itemType;

	public Text itemCount;

	private Image itemImage;

	private Sprite disabledImage;

	public bool IsAvailable { get; private set; }

	private void Awake()
	{
		itemImage = GetComponent<Image>();
		disabledImage = itemImage.sprite;
	}

	public void Collect()
	{
		itemImage.sprite = enabledImage;
		IsAvailable = true;
	}

	public void Use()
	{
		itemImage.sprite = disabledImage;
		IsAvailable = false;
	}
}
