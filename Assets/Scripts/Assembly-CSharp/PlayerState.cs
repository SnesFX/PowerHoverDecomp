using System;

[Serializable]
public enum PlayerState
{
	Idle = 0,
	Leaning = 1,
	Boosting = 2,
	InAir = 3,
	Wobble = 4,
	Grinding = 5,
	Dying = 6,
	MakeJump = 7,
	MakeLanding = 8,
	GrindStart = 9,
	MakeRare = 10,
	MakeRare2 = 11,
	MakeRareMiss = 12,
	MakeFlip = 13,
	Hit = 14,
	MakeBurst = 15,
	BurstEnd = 16,
	SlowMotion = 17,
	Shoot = 18,
	Targetting = 19,
	Flickering = 20
}
