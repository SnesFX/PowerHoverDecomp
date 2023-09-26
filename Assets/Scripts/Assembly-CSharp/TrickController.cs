using System.Collections.Generic;
using UnityEngine;

public class TrickController : MonoBehaviour
{
	private const string lockPrefix = "_lock";

	public Trick TrickFlip;

	public Trick TrickFail;

	public List<Trick> Tricks;

	public CharacterUpgrade CurrentCharacter;

	public CharacterUpgrade ChallengeCharacter;

	public int debug = -1;

	private float totalCounter;

	private int totalCount;

	private Trick currentTrick;

	private float rotateInAirTimer;

	public static TrickController Instance { get; private set; }

	public int TargetingLevel { get; set; }

	public int Total
	{
		get
		{
			return Tricks.Count;
		}
	}

	public int TrickLevel { get; private set; }

	private void Awake()
	{
		Instance = this;
		foreach (Trick trick in Tricks)
		{
			totalCounter += trick.Probability;
		}
	}

	private void Start()
	{
		totalCount = Tricks.Count;
		currentTrick = Tricks[0];
	}

	private void FixedUpdate()
	{
		if (rotateInAirTimer > 1f)
		{
			rotateInAirTimer -= Time.fixedDeltaTime;
		}
	}

	public int GetUfoTrick()
	{
		currentTrick = Tricks[(!(Random.value <= 0.5f)) ? 6 : 5];
		if (currentTrick.Rotate > 0)
		{
			rotateInAirTimer = 1f + 0.1f * (float)(540 / currentTrick.Rotate);
		}
		return currentTrick.ID;
	}

	private bool IsUfo()
	{
		return SceneLoader.Instance != null && SceneLoader.Instance.Current != null && SceneLoader.Instance.Current.IsChallenge && ChallengeCharacter.CharacterName.Contains("UFO");
	}

	public int BasicTrick()
	{
		if (IsUfo())
		{
			return GetUfoTrick();
		}
		currentTrick = Tricks[0];
		if (currentTrick.Rotate > 0)
		{
			rotateInAirTimer = 1f + 0.1f * (float)(540 / currentTrick.Rotate);
		}
		return currentTrick.ID;
	}

	public int FlipTrick()
	{
		if (IsUfo())
		{
			return GetUfoTrick();
		}
		currentTrick = TrickFlip;
		if (currentTrick.Rotate > 0)
		{
			rotateInAirTimer = 0.9f;
		}
		return currentTrick.ID;
	}

	public int FailTrick()
	{
		if (IsUfo())
		{
			return GetUfoTrick();
		}
		currentTrick = TrickFail;
		rotateInAirTimer = -1f;
		return currentTrick.ID;
	}

	public int RandomizeTrick()
	{
		if (IsUfo())
		{
			return GetUfoTrick();
		}
		float num = Random.Range(0f, totalCounter);
		if (debug > -1)
		{
			currentTrick = Tricks[debug];
			if (currentTrick.Rotate > 0)
			{
				rotateInAirTimer = 1f + 0.1f * (float)(540 / currentTrick.Rotate);
			}
			return currentTrick.ID;
		}
		float num2 = 0f;
		for (int i = 0; i < totalCount; i++)
		{
			num2 += Tricks[i].Probability;
			if (num <= num2)
			{
				Tricks[i].UsedCounter++;
				currentTrick = Tricks[i];
				if (currentTrick.Rotate > 0)
				{
					rotateInAirTimer = 1f + 0.1f * (float)(540 / currentTrick.Rotate);
				}
				return currentTrick.ID;
			}
		}
		currentTrick = Tricks[0];
		return currentTrick.ID;
	}

	public string GetName()
	{
		return (currentTrick != null) ? currentTrick.Name : string.Empty;
	}

	public int GetRotationSpeed()
	{
		return (rotateInAirTimer > 0f && rotateInAirTimer < 1f) ? Mathf.FloorToInt(720f * currentTrick.RotationSpeedMultiplier) : 0;
	}

	public int GetRotation()
	{
		return (rotateInAirTimer > 0f && rotateInAirTimer < 1f) ? currentTrick.Rotate : 0;
	}

	public int GetTrickExtra()
	{
		return currentTrick.ExtraScore;
	}
}
