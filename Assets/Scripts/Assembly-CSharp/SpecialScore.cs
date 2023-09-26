public class SpecialScore : Score
{
	private string ScoreName;

	public SpecialScore(string name)
	{
		ScoreName = name;
	}

	public override float CalculateScore()
	{
		return 100f;
	}

	public override string ScoreText()
	{
		return ScoreName;
	}
}
