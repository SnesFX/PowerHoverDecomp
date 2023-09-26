using SmartLocalization;

public class CollectableScore : Score
{
	private Collectable.CollectableType Type;

	public CollectableScore(Collectable.CollectableType type)
	{
		Type = type;
	}

	public override float CalculateScore()
	{
		return (int)(Type + 1) * 100;
	}

	public override string ScoreText()
	{
		switch (Type)
		{
		case Collectable.CollectableType.Boost:
			return LanguageManager.Instance.GetTextValue("Ingame.Boost");
		case Collectable.CollectableType.Casette:
			return LanguageManager.Instance.GetTextValue("Ingame.Music");
		case Collectable.CollectableType.Heart:
			return LanguageManager.Instance.GetTextValue("Ingame.Life");
		case Collectable.CollectableType.Letter:
			return LanguageManager.Instance.GetTextValue("Ingame.Grammar");
		default:
			return LanguageManager.Instance.GetTextValue("Ingame.Bling");
		}
	}
}
