public abstract class Score
{
	public const string SCORE_TEXT_DEFAULT = "";

	public abstract float CalculateScore();

	public abstract string ScoreText();
}
