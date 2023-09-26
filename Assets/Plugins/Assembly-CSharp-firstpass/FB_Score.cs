using System;
using System.Collections.Generic;
using SA.Common.Util;
using UnityEngine;

public class FB_Score
{
	public string UserId;

	public string UserName;

	public string AppId;

	public string AppName;

	public int value;

	private Dictionary<FB_ProfileImageSize, Texture2D> profileImages = new Dictionary<FB_ProfileImageSize, Texture2D>();

	public event Action<FB_Score> OnProfileImageLoaded = delegate
	{
	};

	public string GetProfileUrl(FB_ProfileImageSize size)
	{
		return "https://graph.facebook.com/" + UserId + "/picture?type=" + size;
	}

	public Texture2D GetProfileImage(FB_ProfileImageSize size)
	{
		if (profileImages.ContainsKey(size))
		{
			return profileImages[size];
		}
		return null;
	}

	public void LoadProfileImage(FB_ProfileImageSize size)
	{
		if (GetProfileImage(size) != null)
		{
			Debug.LogWarning("Profile image already loaded, size: " + size);
			this.OnProfileImageLoaded(this);
		}
		switch (size)
		{
		case FB_ProfileImageSize.large:
			Loader.LoadWebTexture(GetProfileUrl(size), OnLargeImageLoaded);
			break;
		case FB_ProfileImageSize.normal:
			Loader.LoadWebTexture(GetProfileUrl(size), OnNormalImageLoaded);
			break;
		case FB_ProfileImageSize.small:
			Loader.LoadWebTexture(GetProfileUrl(size), OnSmallImageLoaded);
			break;
		case FB_ProfileImageSize.square:
			Loader.LoadWebTexture(GetProfileUrl(size), OnSquareImageLoaded);
			break;
		}
		Debug.Log("LOAD IMAGE URL: " + GetProfileUrl(size));
	}

	private void OnSquareImageLoaded(Texture2D image)
	{
		if (this != null)
		{
			if (image != null && !profileImages.ContainsKey(FB_ProfileImageSize.square))
			{
				profileImages.Add(FB_ProfileImageSize.square, image);
			}
			this.OnProfileImageLoaded(this);
		}
	}

	private void OnLargeImageLoaded(Texture2D image)
	{
		if (this != null)
		{
			if (image != null && !profileImages.ContainsKey(FB_ProfileImageSize.large))
			{
				profileImages.Add(FB_ProfileImageSize.large, image);
			}
			this.OnProfileImageLoaded(this);
		}
	}

	private void OnNormalImageLoaded(Texture2D image)
	{
		if (this != null)
		{
			if (image != null && !profileImages.ContainsKey(FB_ProfileImageSize.normal))
			{
				profileImages.Add(FB_ProfileImageSize.normal, image);
			}
			this.OnProfileImageLoaded(this);
		}
	}

	private void OnSmallImageLoaded(Texture2D image)
	{
		if (this != null)
		{
			if (image != null && !profileImages.ContainsKey(FB_ProfileImageSize.small))
			{
				profileImages.Add(FB_ProfileImageSize.small, image);
			}
			this.OnProfileImageLoaded(this);
		}
	}
}
