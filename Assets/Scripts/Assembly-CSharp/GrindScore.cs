using SmartLocalization;
using UnityEngine;

public class GrindScore : Score
{
	public const float TIMER_ADD = 10f;

	private const float COMPLETION_ADD = 500f;

	public float Timer;

	public float Percentage;

	public GrindScore(float timer, float percentage)
	{
		Timer = timer;
		Percentage = percentage * 100f;
	}

	public override float CalculateScore()
	{
		float num = 0f;
		num += Timer;
		num += ((!(Percentage > 80f)) ? 0f : 500f);
		return Mathf.Max(0f, num);
	}

	public override string ScoreText()
	{
		return LanguageManager.Instance.GetTextValue((!(Percentage > 80f)) ? "Ingame.Slide" : "Ingame.PerfectSlide");
	}
}
