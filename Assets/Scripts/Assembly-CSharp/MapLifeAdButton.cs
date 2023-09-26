using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLifeAdButton : MonoBehaviour
{
	private AudioSource audioS;

	private bool AdRunning;

	private MapLifeGenerator mlg;

	private Animator anim;

	private void Start()
	{
		mlg = GetComponentInParent<MapLifeGenerator>();
		audioS = GetComponent<AudioSource>();
		anim = GetComponent<Animator>();
	}

	public void ShowAdButton(bool enable)
	{
		base.gameObject.SetActive(enable);
	}

	public void MakeItSmall()
	{
		anim.Play("AdWatchForLifePressToSmall");
	}

	public void MakeItBig()
	{
		audioS.Play();
		anim.Play("AdWatchForLifePressToBig");
	}

	public void AdForLife()
	{
		audioS.Play();
		anim.Play("AdWatchForLifePress");
		StartCoroutine(ShowAd());
	}

	private IEnumerator ShowAd()
	{
		yield return new WaitForSeconds(0.75f);
		ShowAdButton(false);
		bool wasStarted = UnityAdsIngetration.Instance.ShowSuperSonicAd(AdShown);
		if (wasStarted && UnityAnalyticsIntegration.Instance != null)
		{
			UnityAnalyticsIntegration.Instance.MakeEvent("Ad_ForLifeMenu", new Dictionary<string, object> { 
			{
				"scene",
				Main.Instance.CurrentScene
			} });
		}
		else if (!wasStarted)
		{
			mlg.CollectLife();
			GetComponentInParent<MapObject>().MakeUnlockEffect();
		}
	}

	public void AdShown(IronSourcePlacement placement)
	{
		IronSourceEvents.onRewardedVideoAdRewardedEvent -= AdShown;
		AdRunning = false;
		mlg.CollectLife();
		GetComponentInParent<MapObject>().MakeUnlockEffect();
	}
}
