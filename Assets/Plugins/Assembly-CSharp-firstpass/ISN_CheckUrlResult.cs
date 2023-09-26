using SA.Common.Models;

public class ISN_CheckUrlResult : Result
{
	private string _url;

	public string url
	{
		get
		{
			return _url;
		}
	}

	public ISN_CheckUrlResult(string checkedUrl)
	{
		_url = checkedUrl;
	}

	public ISN_CheckUrlResult(string checkedUrl, Error error)
		: base(error)
	{
		_url = checkedUrl;
	}
}
