using SmartLocalization;

public class SloppyLanding : Score
{
	public override float CalculateScore()
	{
		return 0f;
	}

	public override string ScoreText()
	{
		return LanguageManager.Instance.GetTextValue("Ingame.Sloppy");
	}
}
