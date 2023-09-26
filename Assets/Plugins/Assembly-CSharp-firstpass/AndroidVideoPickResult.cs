public class AndroidVideoPickResult : AndroidActivityResult
{
	private string m_videoPath = string.Empty;

	public string VideoPath
	{
		get
		{
			return m_videoPath;
		}
		set
		{
			m_videoPath = value;
		}
	}

	public AndroidVideoPickResult(string resultCode, string videoData)
		: base("0", resultCode)
	{
		if (base.IsSucceeded)
		{
			m_videoPath = videoData;
		}
	}
}
