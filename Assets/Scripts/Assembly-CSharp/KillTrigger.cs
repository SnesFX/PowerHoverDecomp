using UnityEngine;

public class KillTrigger : GroundGizmos
{
	public enum SpecialKillType
	{
		None = 0,
		Worm = 1,
		ChallengeKill = 2
	}

	public SpecialKillType KillType;

	public Color gizmoColor = new Color(1f, 1f, 0f, 0.5f);

	public bool IsEnabled { get; set; }

	public override Color GizmoColor
	{
		get
		{
			return gizmoColor;
		}
	}

	private void Start()
	{
		IsEnabled = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!IsEnabled || !other.CompareTag("Player") || GameController.Instance.State != GameController.GameState.Running)
		{
			return;
		}
		CheckPointContoller component = other.transform.parent.GetComponent<CheckPointContoller>();
		if (component.hoverController.IsFlickering)
		{
			return;
		}
		if (KillType != SpecialKillType.ChallengeKill && UIController.Instance.challengeHits != null && UIController.Instance.challengeHits.HasLifes())
		{
			UIController.Instance.challengeHits.LooseLife();
			component.hoverController.FlickerPlayer();
			return;
		}
		SpecialKillType killType = KillType;
		if (killType == SpecialKillType.Worm)
		{
			LevelStats.Instance.WormFood++;
		}
		component.PlayerDie();
	}
}
