using System;
using System.Collections.Generic;
using System.Text;
using SA.Common.Pattern;
using UnityEngine;

public class AndroidSocialGate : MonoBehaviour
{
	private static AndroidSocialGate _Instance = null;

	private static string s_message;

	private static string s_caption;

	public static event Action<bool, string> OnShareIntentCallback;

	public static void StartGooglePlusShare(string text, Texture2D texture = null)
	{
		CheckAndCreateInstance();
		AN_SocialSharingProxy.StartGooglePlusShareIntent(text, (!(texture == null)) ? Convert.ToBase64String(texture.EncodeToPNG()) : string.Empty);
	}

	public static void StartVideoPickerAndShareIntent(string message, string caption)
	{
		s_message = message;
		s_caption = caption;
		Singleton<AndroidCamera>.Instance.OnVideoPicked += OnVideoPickedHandler;
		Singleton<AndroidCamera>.Instance.GetVideoFromGallery();
	}

	private static void OnVideoPickedHandler(AndroidVideoPickResult result)
	{
		Singleton<AndroidCamera>.Instance.OnVideoPicked -= OnVideoPickedHandler;
		if (result.IsSucceeded)
		{
			StartVideoShareIntent(result.VideoPath, s_message, s_caption);
		}
		else
		{
			Debug.Log("Failed to choose video file.Result code: " + result.code);
		}
	}

	public static void StartVideoShareIntent(string videoFilePath, string message, string caption)
	{
		CheckAndCreateInstance();
		AN_SocialSharingProxy.StartVideoShareIntent(videoFilePath, message, string.Empty, caption);
	}

	public static void StartShareIntent(string caption, string message, string packageNamePattern = "")
	{
		CheckAndCreateInstance();
		StartShareIntentWithSubject(caption, message, string.Empty, packageNamePattern);
	}

	public static void StartShareIntent(string caption, string message, Texture2D texture, string packageNamePattern = "")
	{
		CheckAndCreateInstance();
		StartShareIntentWithSubject(caption, message, string.Empty, texture, packageNamePattern);
	}

	public static void StartShareIntent(string caption, string message, Texture2D[] textures, string packageNamePattern = "")
	{
		CheckAndCreateInstance();
		StartShareIntentWithSubject(caption, message, string.Empty, textures, packageNamePattern = string.Empty);
	}

	public static void StartShareIntentWithSubject(string caption, string message, string subject, Texture2D[] textures, string packageNamePattern = "")
	{
		CheckAndCreateInstance();
		if (textures == null)
		{
			StartShareIntentWithSubject(caption, message, subject, packageNamePattern);
			return;
		}
		if (textures.Length == 0)
		{
			StartShareIntentWithSubject(caption, message, subject, packageNamePattern);
			return;
		}
		if (textures.Length == 1)
		{
			StartShareIntent(caption, message, textures[0], packageNamePattern);
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < textures.Length - 1; i++)
		{
			stringBuilder.Append(Convert.ToBase64String(textures[i].EncodeToPNG()));
			stringBuilder.Append("|");
		}
		stringBuilder.Append(Convert.ToBase64String(textures[textures.Length - 1].EncodeToPNG()));
		AN_SocialSharingProxy.StartShareCollectionIntent(caption, message, subject, stringBuilder.ToString(), packageNamePattern, (int)AndroidNativeSettings.Instance.ImageFormat, AndroidNativeSettings.Instance.SaveCameraImageToGallery);
	}

	public static void StartShareIntentWithSubject(string caption, string message, string subject, string packageNamePattern = "")
	{
		CheckAndCreateInstance();
		AN_SocialSharingProxy.StartShareIntent(caption, message, subject, packageNamePattern);
	}

	public static void StartShareIntentWithSubject(string caption, string message, string subject, Texture2D texture, string packageNamePattern = "")
	{
		CheckAndCreateInstance();
		byte[] inArray = texture.EncodeToPNG();
		string media = Convert.ToBase64String(inArray);
		AN_SocialSharingProxy.StartShareIntent(caption, message, subject, media, packageNamePattern, (int)AndroidNativeSettings.Instance.ImageFormat, AndroidNativeSettings.Instance.SaveCameraImageToGallery);
	}

	public static void SendMail(string caption, string message, string subject, string recipients, Texture2D[] textures)
	{
		if (textures == null)
		{
			AN_SocialSharingProxy.SendMail(caption, message, subject, recipients);
			return;
		}
		if (textures.Length == 0)
		{
			AN_SocialSharingProxy.SendMail(caption, message, subject, recipients);
			return;
		}
		if (textures.Length == 1)
		{
			SendMail(caption, message, subject, recipients, textures[0]);
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < textures.Length - 1; i++)
		{
			stringBuilder.Append(Convert.ToBase64String(textures[i].EncodeToPNG()));
			stringBuilder.Append("|");
		}
		stringBuilder.Append(Convert.ToBase64String(textures[textures.Length - 1].EncodeToPNG()));
		AN_SocialSharingProxy.SendMailWithImages(caption, message, subject, recipients, stringBuilder.ToString(), (int)AndroidNativeSettings.Instance.ImageFormat, AndroidNativeSettings.Instance.SaveCameraImageToGallery);
	}

	public static void SendMail(string caption, string message, string subject, string recipients, Texture2D texture = null)
	{
		CheckAndCreateInstance();
		if (texture != null)
		{
			byte[] inArray = texture.EncodeToPNG();
			string media = Convert.ToBase64String(inArray);
			AN_SocialSharingProxy.SendMailWithImage(caption, message, subject, recipients, media, (int)AndroidNativeSettings.Instance.ImageFormat, AndroidNativeSettings.Instance.SaveCameraImageToGallery);
		}
		else
		{
			AN_SocialSharingProxy.SendMail(caption, message, subject, recipients);
		}
	}

	public static void ShareTwitterGif(string gifPath, string message)
	{
		AN_SocialSharingProxy.ShareTwitterGif(gifPath, message);
	}

	public static void SendTextMessage(string body, string recepient)
	{
		List<string> list = new List<string>();
		list.Add(recepient);
		SendTextMessage(body, list);
	}

	public static void SendTextMessage(string body, List<string> recipients)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string recipient in recipients)
		{
			stringBuilder.Append(recipient);
			stringBuilder.Append("|");
		}
		AN_SocialSharingProxy.SendTextMessage(body, stringBuilder.ToString());
	}

	private static void CheckAndCreateInstance()
	{
		if (_Instance == null)
		{
			_Instance = UnityEngine.Object.FindObjectOfType(typeof(AndroidSocialGate)) as AndroidSocialGate;
			if (_Instance == null)
			{
				_Instance = new GameObject().AddComponent<AndroidSocialGate>();
				_Instance.gameObject.name = _Instance.GetType().Name;
			}
		}
	}

	private void ShareCallback(string data)
	{
		string[] array = data.Split(new string[1] { "|" }, StringSplitOptions.None);
		bool flag = int.Parse(array[1]) == -1;
		AndroidSocialGate.OnShareIntentCallback(flag, array[0]);
		Debug.Log("[AndroidSocialGate]ShareCallback Posted:" + flag + " Package:" + array[0]);
	}

	static AndroidSocialGate()
	{
		AndroidSocialGate.OnShareIntentCallback = delegate
		{
		};
		s_message = string.Empty;
		s_caption = string.Empty;
	}
}
