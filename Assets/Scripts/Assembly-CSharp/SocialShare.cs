using System.Collections;
using UnityEngine;

public class SocialShare : MonoBehaviour
{
	public UIController uiController;

	public Camera UICamera;

	public Animator anim;

	public GameObject content;

	public GameObject buttonGrid;

	public GameObject replayButton;

	public GameObject replaySaveButton;

	private const string gamename = "Power Hover";

	private bool shown;

	private void Start()
	{
		replayButton.SetActive(false);
		replaySaveButton.SetActive(false);
	}

	private void OnDestroy()
	{
	}

	public void ShowSharing(bool show)
	{
	}

	private IEnumerator PostTwitterScreenshot()
	{
		yield return new WaitForEndOfFrame();
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
		tex.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
		tex.Apply();
		AndroidSocialGate.StartShareIntent("Share", GetShareText(true), tex, "twi");
		Object.Destroy(tex);
	}

	private IEnumerator PostFBScreenshot()
	{
		yield return new WaitForEndOfFrame();
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
		tex.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
		tex.Apply();
		AndroidSocialGate.StartShareIntent("Share", GetShareText(true), tex, "facebook.katana");
		Object.Destroy(tex);
	}

	private string GetShareText(bool hashtags)
	{
		string text = "Get it here https://itunes.apple.com/app/id1034773723";
		text = string.Empty;
		return string.Format("{0} {1}", "#powerhover", text);
	}

	public void ShareToTwitter()
	{
		ShowHideOnSharing(false);
		StartCoroutine(PostTwitterScreenshot());
	}

	public void ShareToFacebook()
	{
		ShowHideOnSharing(false);
		StartCoroutine(PostFBScreenshot());
	}

	public void UseReplayKit(bool start)
	{
	}

	private void ShowHideOnSharing(bool show)
	{
		if (show)
		{
			buttonGrid.SetActive(true);
			UICamera.enabled = true;
			ShowSharing(true);
			return;
		}
		UICamera.enabled = uiController.menuState != UIController.IngameMenuState.Paused;
		if (UICamera.enabled)
		{
			ShowSharing(false);
			buttonGrid.SetActive(false);
		}
	}
}
