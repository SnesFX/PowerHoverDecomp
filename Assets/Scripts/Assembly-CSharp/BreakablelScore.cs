public class BreakablelScore : Score
{
	private int PartCount;

	public BreakablelScore(int partCount)
	{
		PartCount = partCount;
	}

	public override float CalculateScore()
	{
		return PartCount * 100;
	}

	public override string ScoreText()
	{
		return "BREAK-IT";
	}
}
