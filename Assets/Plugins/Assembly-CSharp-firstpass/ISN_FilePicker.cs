using System;
using SA.Common.Pattern;
using UnityEngine;

public class ISN_FilePicker : Singleton<ISN_FilePicker>
{
	public static event Action<ISN_FilePickerResult> MediaPickFinished;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void PickFromCameraRoll(int maxItemsCount = 0)
	{
	}

	private void OnSelectImagesComplete(string data)
	{
		string[] array = data.Split(new string[1] { "|%|" }, StringSplitOptions.None);
		ISN_FilePickerResult iSN_FilePickerResult = new ISN_FilePickerResult();
		if (data.Equals(string.Empty))
		{
			ISN_FilePicker.MediaPickFinished(iSN_FilePickerResult);
			return;
		}
		for (int i = 0; i < array.Length && !(array[i] == "endofline"); i++)
		{
			string s = array[i];
			byte[] data2 = Convert.FromBase64String(s);
			Texture2D texture2D = new Texture2D(1, 1);
			texture2D.LoadImage(data2);
			texture2D.hideFlags = HideFlags.DontSave;
			iSN_FilePickerResult.PickedImages.Add(texture2D);
		}
		ISN_FilePicker.MediaPickFinished(iSN_FilePickerResult);
	}

	static ISN_FilePicker()
	{
		ISN_FilePicker.MediaPickFinished = delegate
		{
		};
	}
}
