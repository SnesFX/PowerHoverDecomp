using System;
using SA.Common.Models;
using UnityEngine;

namespace SA.Common.Util
{
	public static class Screen
	{
		public static void TakeScreenshot(Action<Texture2D> callback)
		{
			ScreenshotMaker screenshotMaker = ScreenshotMaker.Create();
			screenshotMaker.OnScreenshotReady = (Action<Texture2D>)Delegate.Combine(screenshotMaker.OnScreenshotReady, callback);
			screenshotMaker.GetScreenshot();
		}
	}
}
