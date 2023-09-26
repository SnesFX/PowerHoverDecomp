using UnityEngine;

public class MenuButton : MonoBehaviour
{
	public void Show(bool enable)
	{
		base.gameObject.SetActive(enable);
	}
}
