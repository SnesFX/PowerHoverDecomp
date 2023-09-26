using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItemUnlock : MonoBehaviour
{
	[Serializable]
	public class EffectItem
	{
		public PlayerStatType type;

		public GameObject innerObject;
	}

	private const string UnlockAnimationName = "itemUnlock";

	public Animator UnlockAnimator;

	public List<EffectItem> ObjectToActivate;

	public PlayerStatType currentType { get; set; }

	public void StartEffect()
	{
		base.gameObject.SetActive(true);
		for (int i = 0; i < ObjectToActivate.Count; i++)
		{
			if (ObjectToActivate[i].type == currentType)
			{
				ObjectToActivate[i].innerObject.SetActive(true);
				break;
			}
		}
		UnlockAnimator.enabled = true;
		UnlockAnimator.Play("itemUnlock", -1, 0f);
		StartCoroutine(DisableMe());
	}

	private IEnumerator DisableMe()
	{
		yield return new WaitForSeconds(2.5f);
		base.gameObject.SetActive(false);
	}
}
