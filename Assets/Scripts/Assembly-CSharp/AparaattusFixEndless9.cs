using UnityEngine;

public class AparaattusFixEndless9 : MonoBehaviour
{
	public Transform lampStartBG;

	private void Start()
	{
		if (SceneLoader.Instance.Current.SceneName.Equals("Endless9"))
		{
			base.transform.localPosition = new Vector3(base.transform.localPosition.x - 20f, base.transform.localPosition.y, base.transform.localPosition.z);
			lampStartBG.localPosition = new Vector3(0.597f, -2.692f, 1.15f);
		}
	}
}
