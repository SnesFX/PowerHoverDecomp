using UnityEngine;

public class CameraSingleton : MonoBehaviour
{
	public static CameraSingleton Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}
}
