public class FB_Permission
{
	private string _Name = string.Empty;

	private FB_PermissionStatus _Status = FB_PermissionStatus.Declined;

	public string Name
	{
		get
		{
			return _Name;
		}
	}

	public FB_PermissionStatus Status
	{
		get
		{
			return _Status;
		}
	}

	public FB_Permission(string permission, FB_PermissionStatus status)
	{
		_Name = permission;
		_Status = status;
	}
}
