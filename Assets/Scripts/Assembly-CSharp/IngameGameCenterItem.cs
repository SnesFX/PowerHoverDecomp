using UnityEngine;

public class IngameGameCenterItem : MonoBehaviour
{
	public TextMesh GameCenterRanking;

	private void Start()
	{
		base.gameObject.SetActive(true);
	}

	private void Update()
	{
		UpdateRankingText();
	}

	public void UIButtonGameCenter()
	{
		GameCenter.Instance.ShowLeaderboardsUI(Main.Instance.CurrentScene);
	}

	private void UpdateRankingText()
	{
		int leaderboardRanking = GameCenter.Instance.GetLeaderboardRanking(Main.Instance.CurrentScene);
		if (leaderboardRanking > 0)
		{
			GameCenterRanking.text = string.Format("#{0}", leaderboardRanking.ToString());
		}
		else
		{
			GameCenterRanking.text = "-";
		}
	}
}
