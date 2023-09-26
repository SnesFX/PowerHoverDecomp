using System.Collections;
using UnityEngine;

public class UploadDownloadTexture : MonoBehaviour
{
	public enum Mode
	{
		Upload = 0,
		Download = 1
	}

	public Mode mode;

	public string url = "http://www.server.com/ES2.php";

	public string filename = "textureFile.txt";

	public string textureTag = "textureTag";

	public string webUsername = "ES2";

	public string webPassword = "65w84e4p994z3Oq";

	private void Start()
	{
		if (mode == Mode.Upload)
		{
			Texture2D texture = GetTexture();
			if (texture == null)
			{
				Debug.LogError("There is no texture attached to this object.");
			}
			else
			{
				StartCoroutine(Upload(texture));
			}
		}
		else
		{
			StartCoroutine(Download());
		}
	}

	private IEnumerator Upload(Texture2D texture)
	{
		ES2Web web = new ES2Web(url, CreateSettings());
		yield return StartCoroutine(web.Upload(texture));
		if (web.isError)
		{
			Debug.LogError(web.errorCode + ":" + web.error);
		}
		else
		{
			Debug.Log("Uploaded Successfully. Reload scene to load texture into blank object.");
		}
	}

	private IEnumerator Download()
	{
		ES2Web web = new ES2Web(url, CreateSettings());
		yield return StartCoroutine(web.Download());
		if (web.isError)
		{
			if (!(web.errorCode == "05"))
			{
				Debug.LogError(web.errorCode + ":" + web.error);
			}
		}
		else
		{
			SetTexture(web.Load<Texture2D>(textureTag));
			yield return StartCoroutine(Delete());
			Debug.Log("Texture successfully downloaded and applied to blank object.");
		}
	}

	private IEnumerator Delete()
	{
		ES2Web web = new ES2Web(url, CreateSettings());
		yield return StartCoroutine(web.Delete());
		if (web.isError)
		{
			Debug.LogError(web.errorCode + ":" + web.error);
		}
	}

	private ES2Settings CreateSettings()
	{
		ES2Settings eS2Settings = new ES2Settings();
		eS2Settings.webFilename = filename;
		eS2Settings.tag = textureTag;
		eS2Settings.webUsername = webUsername;
		eS2Settings.webPassword = webPassword;
		return eS2Settings;
	}

	private Texture2D GetTexture()
	{
		Renderer component = GetComponent<Renderer>();
		if (component.material != null && component.material.mainTexture != null)
		{
			return component.material.mainTexture as Texture2D;
		}
		return null;
	}

	private void SetTexture(Texture2D texture)
	{
		Renderer component = GetComponent<Renderer>();
		if (component.material != null)
		{
			component.material.mainTexture = texture;
		}
		else
		{
			Debug.LogError("There is no material attached to this object.");
		}
	}
}
