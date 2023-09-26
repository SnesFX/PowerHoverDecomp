using System.Collections.Generic;
using UnityEngine;

public class VideoPlayer : MonoBehaviour
{
	public List<Video> Videos;

	public void PlayMovie(VideoType videoType, bool canSkip = false)
	{
		Video video = Videos.Find((Video x) => x.type == videoType);
	}
}
