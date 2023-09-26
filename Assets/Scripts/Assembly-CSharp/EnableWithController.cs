using UnityEngine;

public class EnableWithController : MonoBehaviour
{
	public bool OnlyOnTouch;

	public bool OnlyWithAndroidTV;

	public bool OnlyWithAppleTV;

	private void Start()
	{
		if (OnlyOnTouch)
		{
			if (DeviceSettings.Instance != null && !DeviceSettings.Instance.EnableInputDevices && !DeviceSettings.Instance.RunningOnTV)
			{
				base.gameObject.SetActive(true);
			}
			else
			{
				base.gameObject.SetActive(false);
			}
		}
		else if (OnlyWithAppleTV)
		{
			base.gameObject.SetActive(false);
		}
		else if (DeviceSettings.Instance != null && ((OnlyWithAndroidTV && DeviceSettings.Instance.RunningOnTV) || DeviceSettings.Instance.EnableInputDevices))
		{
			base.gameObject.SetActive(true);
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}
}
