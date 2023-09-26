using System;
using SA.Common.Pattern;
using SA.Common.Util;
using UnityEngine;

public class AndroidCamera : Singleton<AndroidCamera>
{
	private static string _lastImageName = string.Empty;

	public event Action<AndroidImagePickResult> OnImagePicked = delegate
	{
	};

	public event Action<AndroidImagesPickResult> OnImagesPicked = delegate
	{
	};

	public event Action<AndroidVideoPickResult> OnVideoPicked = delegate
	{
	};

	public event Action<GallerySaveResult> OnImageSaved = delegate
	{
	};

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		AndroidNative.InitCameraAPI(AndroidNativeSettings.Instance.GalleryFolderName, AndroidNativeSettings.Instance.MaxImageLoadSize, (int)AndroidNativeSettings.Instance.CameraCaptureMode, (int)AndroidNativeSettings.Instance.ImageFormat);
	}

	[Obsolete("SaveImageToGalalry is deprecated, please use SaveImageToGallery instead.")]
	public void SaveImageToGalalry(Texture2D image, string name = "Screenshot")
	{
		SaveImageToGallery(image, name);
	}

	public void SaveImageToGallery(Texture2D image, string name = "Screenshot")
	{
		if (image != null)
		{
			byte[] inArray = image.EncodeToPNG();
			string imageData = Convert.ToBase64String(inArray);
			AndroidNative.SaveToGalalry(imageData, name);
		}
		else
		{
			Debug.LogWarning("AndroidCamera::SaveToGalalry:  image is null");
		}
	}

	public void SaveScreenshotToGallery(string name = "Screenshot")
	{
		_lastImageName = name;
		SA.Common.Util.Screen.TakeScreenshot(OnScreenshotReady);
	}

	public void GetVideoFromGallery()
	{
		AndroidNative.GetVideoFromGallery();
	}

	public void GetImageFromGallery()
	{
		AndroidNative.GetImageFromGallery();
	}

	public void GetImagesFromGallery()
	{
		AndroidNative.GetImagesFromGallery();
	}

	public void GetImageFromCamera()
	{
		AndroidNative.GetImageFromCamera(AndroidNativeSettings.Instance.SaveCameraImageToGallery);
	}

	private void OnVideoPickedCallback(string data)
	{
		string[] array = data.Split(new string[1] { "|" }, StringSplitOptions.None);
		AndroidVideoPickResult obj = new AndroidVideoPickResult(array[0], array[1]);
		this.OnVideoPicked(obj);
	}

	private void OnImagePickedEvent(string data)
	{
		Debug.Log("OnImagePickedEvent");
		string[] array = data.Split("|"[0]);
		string codeString = array[0];
		string imagePathInfo = array[1];
		string imageData = array[2];
		AndroidImagePickResult obj = new AndroidImagePickResult(codeString, imageData, imagePathInfo);
		this.OnImagePicked(obj);
	}

	private void ImagesPickedCallback(string data)
	{
		Debug.Log("[OnImagesPickedEvent]");
		string[] array = data.Split(new string[1] { "|%|" }, StringSplitOptions.None);
		string resultCode = array[0];
		string imagesData = array[1];
		AndroidImagesPickResult obj = new AndroidImagesPickResult(resultCode, imagesData);
		this.OnImagesPicked(obj);
	}

	private void OnImageSavedEvent(string data)
	{
		GallerySaveResult obj = new GallerySaveResult(data);
		this.OnImageSaved(obj);
	}

	private void OnImageSaveFailedEvent(string data)
	{
		GallerySaveResult obj = new GallerySaveResult();
		this.OnImageSaved(obj);
	}

	private void OnScreenshotReady(Texture2D tex)
	{
		SaveImageToGallery(tex, _lastImageName);
	}

	public static string GetRandomString()
	{
		string text = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
		text = text.Replace("=", string.Empty);
		text = text.Replace("+", string.Empty);
		return text.Replace("/", string.Empty);
	}
}
