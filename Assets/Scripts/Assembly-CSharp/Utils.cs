using System;

public static class Utils
{
	public static string FormatTime(float time)
	{
		int num = (int)time;
		int num2 = num / 60;
		int num3 = num % 60;
		int num4 = (int)(time * 100f) % 100;
		return string.Format("{0:0}:{1:00}:{2:0}", num2, num3, (num4 >= 10) ? string.Format("{0}", num4) : string.Format("0{0}", num4));
	}

	public static string FormatTimeLeft(double time)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(time);
		return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
	}
}
