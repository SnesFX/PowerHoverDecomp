using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossPromoButton : MonoBehaviour
{
	[Serializable]
	public class PromoDetails
	{
		public string url;

		public Sprite img;
	}

	private const int PromoVisibleFrequency = 1;

	public string ButtonStoragePrefixx = "CPButtonCounter";

	public GameObject promoObject;

	public Button openButton;

	public Material promoImage;

	private CrossPromo.PromoDetails current;

	private int promoEnableCounter;

	private MainController mainController;

	private bool loaded;

	private void Awake()
	{
		EnableItems(false);
		mainController = UnityEngine.Object.FindObjectOfType<MainController>();
	}

	private void Update()
	{
		if (mainController != null && loaded)
		{
			if (mainController.currentMenu == MenuType.Main && !promoObject.activeSelf)
			{
				ShowPromo();
			}
			else if (mainController.currentMenu != 0 && promoObject.activeSelf)
			{
				EnableItems(false);
			}
		}
	}

	private void OnEnable()
	{
		promoEnableCounter = (GameDataController.Exists(ButtonStoragePrefixx) ? GameDataController.Load<int>(ButtonStoragePrefixx) : 0);
		if (promoEnableCounter > 0)
		{
			ShowPromo();
		}
		else
		{
			EnableItems(false);
		}
		promoEnableCounter++;
		GameDataController.Save(promoEnableCounter, ButtonStoragePrefixx);
	}

	private void ShowPromo()
	{
		if (!(CrossPromo.Instance == null))
		{
			List<CrossPromo.PromoDetails> crossPromos = CrossPromo.Instance.GetCrossPromos();
			if (crossPromos != null && crossPromos.Count > 0)
			{
				int index = UnityEngine.Random.Range(0, crossPromos.Count);
				current = crossPromos[index];
				promoImage.mainTexture = current.Image;
				EnableItems(true);
			}
		}
	}

	public void OpenPromo()
	{
		Debug.Log("promo open " + current.StoreURL);
		CrossPromo.Instance.OpenPromoFromButton(current);
		EnableItems(false);
	}

	private void EnableItems(bool enable)
	{
		openButton.interactable = enable;
		promoObject.SetActive(enable);
	}

	public void ShowWhenLoaded()
	{
		ShowPromo();
		loaded = true;
	}
}
