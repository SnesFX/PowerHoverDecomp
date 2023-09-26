using System;
using System.Collections.Generic;

[Serializable]
public class AudioEffectPlayRules
{
	public enum EventType
	{
		OnTriggerEnter = 0,
		OnTriggerExit = 1,
		OnDestroy = 2,
		OnStart = 3
	}

	public EventType Type;

	public List<string> Tags;
}
