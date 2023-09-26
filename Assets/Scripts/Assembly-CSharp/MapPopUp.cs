using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MapPopUp : MonoBehaviour
{
	public Animator popUpAnimator;

	public Text popUpText;

	public Text popUpTextHeader;

	public Button close;

	public GameObject HD;

	public GameObject Magnet;

	public GameObject Life;

	public GameObject Character;

	public bool IsActive { get; private set; }

	private void Start()
	{
		close.gameObject.SetActive(false);
	}

	public void Show(bool enable, MapObject mapObject = null)
	{
		popUpAnimator.Play((!enable) ? "MapPopUpFadeOut" : "MapPopUpFadeIn", -1, 0f);
		IsActive = enable;
		if (enable)
		{
			LocalizationLoader.Instance.SetText(popUpTextHeader, mapObject.title);
			LocalizationLoader.Instance.SetText(popUpText, mapObject.description);
			if (mapObject.notification != null && mapObject.notification.gameObject.activeSelf)
			{
				mapObject.notification.ShowInfo();
			}
			switch (mapObject.StatToGet)
			{
			case PlayerStatType.BackUps:
				HD.SetActive(true);
				Magnet.SetActive(false);
				Life.SetActive(false);
				Character.SetActive(false);
				break;
			case PlayerStatType.Lives:
			case PlayerStatType.StartLives:
				HD.SetActive(false);
				Magnet.SetActive(false);
				Life.SetActive(true);
				Character.SetActive(false);
				break;
			case PlayerStatType.Magnet:
				HD.SetActive(false);
				Magnet.SetActive(true);
				Life.SetActive(false);
				Character.SetActive(false);
				break;
			case PlayerStatType.Character:
				HD.SetActive(false);
				Magnet.SetActive(false);
				Life.SetActive(false);
				Character.SetActive(true);
				break;
			default:
				HD.SetActive(false);
				Magnet.SetActive(false);
				Life.SetActive(false);
				Character.SetActive(false);
				break;
			}
		}
		StartCoroutine(EnableClose(enable));
	}

	private IEnumerator EnableClose(bool enable)
	{
		yield return new WaitForSeconds(0.3f);
		close.gameObject.SetActive(enable);
	}

	private void OnEnable()
	{
		popUpAnimator.Play("MapPopUpOut", -1, 0f);
	}
}
