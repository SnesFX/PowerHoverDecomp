using UnityEngine;

public class PlatformSpecificEnabler : MonoBehaviour
{
	public bool isOnAndroid;

	public bool isOnIOS;

	public bool isOnTvOS;

	public bool isOnOthers;

	public bool isOnTVOnly;

	private void Start()
	{
		base.gameObject.SetActive(isOnAndroid);
		if (isOnTVOnly && DeviceSettings.Instance != null && !DeviceSettings.Instance.RunningOnTV)
		{
			base.gameObject.SetActive(false);
		}
	}
}
