using UnityEngine;

public class ReplayKit : MonoBehaviour
{
	private static bool DEBUG;

	public static ReplayKit Instance { get; private set; }

	public bool IsAvailable { get; private set; }

	public bool IsRecordAvailable
	{
		get
		{
			return false;
		}
	}

	public bool IsRecording
	{
		get
		{
			return false;
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		IsAvailable = false;
	}

	private void OnDestroy()
	{
	}
}
