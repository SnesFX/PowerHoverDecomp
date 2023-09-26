using UnityEngine;

public class ApparatusLazer : ResetObject
{
	public enum LaserType
	{
		Basic = 0,
		Probel = 1,
		Tunnel = 2
	}

	public enum RandomizeType
	{
		Normal = 0,
		Common = 1,
		Rare = 2
	}

	public RandomizeType RandomType;

	public Apparatus.DroppingState[] BlacklistedDropTypes;

	public bool CanBeRotated;

	public LaserType Type;

	public bool ChangeActivatedMaterial = true;

	public string ANIM_APPARATUS_DISABLED = "ApparatusLazerDisabled";

	public string ANIM_APPARATUS_OFF = "ApparatusLazerOff";

	public string ANIM_APPARATUS_ON = "ApparatusLazerOn";

	public override void Reset(bool isRewind)
	{
		if (GetComponent<Animator>() == null)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
