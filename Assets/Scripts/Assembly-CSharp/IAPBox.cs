using UnityEngine;
using UnityEngine.UI;

public class IAPBox : MonoBehaviour
{
	public Text waitTimer;

	private float timer;

	private double timeLeft;

	private void OnEnable()
	{
		if (UnityAdsIngetration.Instance.IsInitialized)
		{
			UnityAdsIngetration.Instance.BannerShow();
		}
	}

	private void OnDisable()
	{
		if (UnityAdsIngetration.Instance.IsInitialized)
		{
			UnityAdsIngetration.Instance.BannerHide();
		}
	}

	public void SetTime(double time)
	{
		timeLeft = time;
		waitTimer.text = Utils.FormatTimeLeft(28800.0 - timeLeft);
		timer = 0f;
	}

	private void Update()
	{
		timer += Time.deltaTime;
		if (timer >= 1f)
		{
			timer = 0f;
			timeLeft += 1.0;
			waitTimer.text = Utils.FormatTimeLeft(28800.0 - timeLeft);
		}
	}
}
