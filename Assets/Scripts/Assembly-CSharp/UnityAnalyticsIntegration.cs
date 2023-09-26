using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class UnityAnalyticsIntegration : MonoBehaviour
{
	public static UnityAnalyticsIntegration Instance { get; private set; }

	private void Start()
	{
		Instance = this;
	}

	public void MakeEvent(string name, Dictionary<string, object> parameters)
	{
		Analytics.CustomEvent(name, parameters);
	}
}
