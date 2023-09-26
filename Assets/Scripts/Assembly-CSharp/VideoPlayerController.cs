using UnityEngine;

public class VideoPlayerController : ControllerBase
{
	public VideoPlayer player;

	public VideoType currentType;

	private bool skipping;

	public override void Awake()
	{
		type = MenuType.VideoPlayer;
		base.Awake();
		if (Application.loadedLevelName.Equals("MenuVideoPlayer"))
		{
			if ((bool)debugEventSystem)
			{
				debugEventSystem.SetActive(true);
			}
			base.gameObject.SetActive(true);
		}
		else if ((bool)debugEventSystem)
		{
			debugEventSystem.SetActive(false);
		}
	}

	public override void OnEnable()
	{
		base.OnEnable();
		player.PlayMovie(currentType, skipping);
		Main.Instance.SwitchMenu(MenuType.Main);
		base.gameObject.SetActive(false);
	}

	public void SetVideo(VideoType type, bool canSkip)
	{
		currentType = type;
		skipping = canSkip;
	}
}
