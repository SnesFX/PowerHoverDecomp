using System;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonRetryTimer : MonoBehaviour
{
	public Text waitTimer;

	private float timer;

	private double timeLeft;

	private bool challenge;

	private void OnEnable()
	{
		if (SceneLoader.Instance == null || SceneLoader.Instance.Current == null || SceneLoader.Instance.Current.IsChallenge)
		{
			challenge = true;
			SetTime((DateTime.Today.AddDays(1.0) - DateTime.Now).TotalSeconds);
		}
		else
		{
			challenge = false;
			SetTime(SceneLoader.Instance.UnlockTimeLeft(SceneLoader.Instance.Current));
		}
	}

	public void SetTime(double time)
	{
		timeLeft = time;
		if (challenge)
		{
			waitTimer.text = Utils.FormatTimeLeft(timeLeft);
		}
		else
		{
			waitTimer.text = Utils.FormatTimeLeft(28800.0 - timeLeft);
		}
		timer = 1f;
	}

	private void Update()
	{
		if (!(timer > 0f))
		{
			return;
		}
		timer -= Time.deltaTime;
		if (timer <= 0f)
		{
			timer = 1f;
			if (challenge)
			{
				timeLeft -= 1.0;
				waitTimer.text = Utils.FormatTimeLeft((!(timeLeft < 0.0)) ? timeLeft : 0.0);
			}
			else
			{
				timeLeft += 1.0;
				waitTimer.text = Utils.FormatTimeLeft((!(28800.0 - timeLeft < 0.0)) ? (28800.0 - timeLeft) : 0.0);
			}
		}
	}
}
