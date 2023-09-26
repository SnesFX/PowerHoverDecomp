using UnityEngine;
using UnityEngine.UI;

public class LifeUI : MonoBehaviour
{
	public Animator LifeAnimator;

	public Text LifeCount;

	public Text LifeChanged;

	private int lifeCountRef;

	private float lifeChangedTimer;

	private bool resetAnim;

	private void Update()
	{
		if (LifeController.Instance != null && lifeCountRef != LifeController.Instance.LifeCount)
		{
			UpdateLifeCount();
		}
		if (resetAnim && GameController.Instance.State != GameController.GameState.Kill)
		{
			LifeAnimator.Play("LifesNormal", -1, 0f);
			resetAnim = false;
		}
		if (lifeChangedTimer > 0f)
		{
			lifeChangedTimer -= Time.unscaledDeltaTime;
			if (lifeChangedTimer <= 0f)
			{
				LifeChanged.gameObject.SetActive(false);
			}
		}
	}

	public void UpdateLifeCount()
	{
		int num = LifeController.Instance.LifeCount - lifeCountRef;
		lifeCountRef = LifeController.Instance.LifeCount;
		LifeCount.text = string.Format("{0}", lifeCountRef);
		if (GameController.Instance.State == GameController.GameState.Running || GameController.Instance.State == GameController.GameState.Kill || GameController.Instance.State == GameController.GameState.Paused)
		{
			LifeChanged.text = ((num <= 0) ? string.Format("{0}", num) : string.Format("+{0}", num));
			LifeChanged.gameObject.SetActive(true);
			lifeChangedTimer = 1f;
			if (GameController.Instance.State == GameController.GameState.Kill)
			{
				LifeAnimator.Play("LifesOnKill", -1, 0f);
				resetAnim = true;
			}
		}
	}
}
