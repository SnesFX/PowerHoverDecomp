using UnityEngine;

public class HoverBoardSound : MonoBehaviour
{
	public HoverController boardController;

	public AudioSource[] SoundLibrary;

	private float enginePitch;

	private float engineEasing;

	private float engineVolume;

	private void Start()
	{
		if ((bool)AudioController.Instance)
		{
			AudioController.Instance.SetListener(false);
		}
		GetComponent<AudioListener>().enabled = true;
	}

	public void DisableAudioListener()
	{
		GetComponent<AudioListener>().enabled = false;
	}

	private void LateUpdate()
	{
		if (GameController.Instance.State == GameController.GameState.End || GameController.Instance.State == GameController.GameState.Kill || GameController.Instance.State == GameController.GameState.Paused || boardController.walker.State == SplineWalker.LevelState.Start || boardController.walker.State == SplineWalker.LevelState.Zooming)
		{
			SoundLibrary[0].Stop();
			SoundLibrary[5].Stop();
		}
		else if (!SoundLibrary[0].isPlaying)
		{
			SoundLibrary[0].Play();
		}
		if (UIController.Instance.leftPressed || UIController.Instance.rightPressed)
		{
			if (SoundLibrary[0].pitch < 1f)
			{
				enginePitch = (SoundLibrary[0].pitch += Time.deltaTime * 5f);
				engineVolume = (SoundLibrary[0].volume += Time.deltaTime);
			}
			engineEasing = 0f;
		}
		else if (SoundLibrary[0].pitch < 0.2f || SoundLibrary[0].pitch > 0.2f)
		{
			engineEasing += Time.deltaTime * 2f;
			SoundLibrary[0].pitch = Mathf.Lerp(enginePitch, 0.3f, engineEasing);
			SoundLibrary[0].volume = Mathf.Lerp(engineVolume, 0.2f, engineEasing);
		}
		switch (boardController.PlayerState)
		{
		case PlayerState.Grinding:
			if (!SoundLibrary[5].isPlaying)
			{
				SoundLibrary[5].Play();
			}
			else
			{
				SoundLibrary[5].pitch = Mathf.Lerp(SoundLibrary[5].pitch, 4f, Time.deltaTime * 1.2f);
			}
			break;
		case PlayerState.Dying:
			break;
		case PlayerState.Boosting:
			break;
		case PlayerState.Idle:
		case PlayerState.Leaning:
			break;
		case PlayerState.InAir:
			switch (boardController.JumpType)
			{
			case JumpType.PreJump:
				break;
			case JumpType.Normal:
				break;
			case JumpType.BackFlip:
				break;
			case JumpType.FrontFlip:
				break;
			}
			break;
		case PlayerState.Wobble:
			break;
		}
	}

	public void PlayOnce(PlayerState state)
	{
		switch (state)
		{
		case PlayerState.MakeFlip:
			if (!SoundLibrary[6].isPlaying)
			{
				SoundLibrary[6].Play();
			}
			break;
		case PlayerState.MakeLanding:
			if (SoundLibrary[5].pitch > 2f)
			{
				SoundLibrary[5].pitch = 2f;
			}
			if (!SoundLibrary[4].isPlaying)
			{
				SoundLibrary[4].Play();
			}
			break;
		case PlayerState.MakeJump:
			if (!SoundLibrary[3].isPlaying)
			{
				SoundLibrary[3].Play();
			}
			SoundLibrary[5].Stop();
			break;
		case PlayerState.Dying:
			if (!SoundLibrary[7].isPlaying)
			{
				SoundLibrary[7].Play();
			}
			break;
		case PlayerState.Hit:
			if (!SoundLibrary[8].isPlaying)
			{
				SoundLibrary[8].Play();
			}
			break;
		case PlayerState.GrindStart:
			if (!SoundLibrary[17].isPlaying)
			{
				SoundLibrary[17].Play();
			}
			break;
		case PlayerState.MakeRare:
			if (!SoundLibrary[9].isPlaying)
			{
				SoundLibrary[9].Play();
			}
			break;
		case PlayerState.MakeRare2:
			if (!SoundLibrary[11].isPlaying)
			{
				SoundLibrary[11].Play();
			}
			break;
		case PlayerState.MakeRareMiss:
			if (!SoundLibrary[10].isPlaying)
			{
				SoundLibrary[10].Play();
			}
			break;
		case PlayerState.Wobble:
			if (!SoundLibrary[4].isPlaying)
			{
				SoundLibrary[4].Play();
			}
			break;
		case PlayerState.MakeBurst:
			if (!SoundLibrary[12].isPlaying)
			{
				SoundLibrary[12].Play();
			}
			break;
		case PlayerState.BurstEnd:
			if (!SoundLibrary[13].isPlaying)
			{
				SoundLibrary[13].Play();
			}
			break;
		case PlayerState.SlowMotion:
			SoundLibrary[14].Play();
			break;
		case PlayerState.Shoot:
			if (!SoundLibrary[15].isPlaying)
			{
				SoundLibrary[15].Play();
			}
			break;
		case PlayerState.Targetting:
			if (!SoundLibrary[16].isPlaying)
			{
				SoundLibrary[16].Play();
			}
			break;
		case PlayerState.Flickering:
			if (!SoundLibrary[18].isPlaying)
			{
				SoundLibrary[18].Play();
			}
			break;
		case PlayerState.Grinding:
			break;
		}
	}
}
