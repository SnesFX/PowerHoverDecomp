using SmartLocalization;
using UnityEngine;

public class JumpScore : Score
{
	private const float ROTATION_ADD = 1f;

	private const float AIR_ADD = 500f;

	public float Rotation;

	public float DiffToGround;

	public bool Special;

	public bool BreakCombo;

	public float AirTime;

	public float AirScore;

	public float RotateScore;

	public JumpScore(float rotation, float airTime, bool special, bool breakCombo)
	{
		Rotation = rotation;
		AirTime = Mathf.Round(airTime * 10f) / 10f;
		Special = special;
		BreakCombo = breakCombo;
	}

	public override float CalculateScore()
	{
		float num = 0f;
		num = ((!Special) ? (num + Rotation * 1f) : (num + ((Rotation % 360f != 0f) ? 0f : (20f * (Mathf.Round(Rotation / 360f) * 360f)))));
		RotateScore = Mathf.Max(0f, num);
		AirScore = ((!(AirTime > 0.2f)) ? 0f : (500f * AirTime));
		num += AirScore;
		return Mathf.Max(0f, num);
	}

	public override string ScoreText()
	{
		return string.Format("{0}{1}", (!(RotateScore > 0f)) ? string.Empty : string.Format("{0} {1}", Rotation, LanguageManager.Instance.GetTextValue((!Special) ? "Ingame.Roll" : "Ingame.Flip")), (!(AirScore > 0f)) ? string.Empty : string.Format("{0} {1}", LanguageManager.Instance.GetTextValue((!(AirScore > 1000f)) ? "Ingame.AirBonus" : "Ingame.BigAir"), AirScore));
	}
}
