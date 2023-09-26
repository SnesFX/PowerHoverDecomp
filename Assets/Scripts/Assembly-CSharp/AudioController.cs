using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
	[Serializable]
	public enum Effect
	{
		None = 0,
		Music = 1
	}

	[Serializable]
	public enum MusicType
	{
		Menu = 0,
		Normal = 1,
		Boss = 2,
		Slow = 3
	}

	[Serializable]
	public class AudioItem
	{
		public Effect type;

		public string description;

		public AudioClip clip;

		[Range(0f, 1f)]
		public float volume;

		[Range(-3f, 3f)]
		public float pitch;

		public AudioMixerGroup mixer;

		public bool availableForPlay;

		[HideInInspector]
		public AudioSource audioSource;
	}

	public const string CASETTE_PREFIX = "SSS_CASETTE_";

	private const float MUSIC_FADE_SPEED = 5f;

	public AudioListener listener;

	public AudioMixer audioMixer;

	public AudioMixerGroup SFXMixerGroup;

	public AudioItem[] BackgroundMusics;

	public AudioItem[] AudioItems;

	public bool SfxEnabled = true;

	private Dictionary<Effect, AudioItem> Audio;

	private AudioSource _currentMusicSource;

	private int _currentMusicId;

	private float volumeTimer;

	private int _nextMusicId;

	public static AudioController Instance { get; private set; }

	public float MusicVolume { get; private set; }

	public bool isStopped { get; private set; }

	private void Awake()
	{
		if ((bool)Instance)
		{
			base.gameObject.SetActive(false);
			return;
		}
		Instance = this;
		Audio = new Dictionary<Effect, AudioItem>(AudioItems.Length);
		AudioItem[] audioItems = AudioItems;
		foreach (AudioItem audioItem in audioItems)
		{
			Audio.Add(audioItem.type, audioItem);
		}
		AudioItems = null;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
		isStopped = (GameDataController.Exists("AudioStop") ? true : false);
		if (BackgroundMusics != null)
		{
			_currentMusicId = (GameDataController.Exists("LastMap") ? ((GameDataController.Load<int>("LastMap") == 1) ? 3 : 0) : 0);
			BackgroundMusics[0].availableForPlay = true;
			for (int i = 0; i < BackgroundMusics.Length; i++)
			{
				AudioItem audioItem = BackgroundMusics[i];
				GameObject gameObject = new GameObject(string.Format("{0}_{1}", "M", audioItem.clip.name));
				AudioSource audioSource = gameObject.AddComponent<AudioSource>();
				audioSource.outputAudioMixerGroup = audioItem.mixer;
				audioSource.clip = audioItem.clip;
				audioSource.pitch = audioItem.pitch;
				audioSource.volume = audioItem.volume * MusicVolume;
				audioSource.playOnAwake = false;
				audioSource.loop = true;
				gameObject.transform.parent = base.transform;
				if (i == _currentMusicId)
				{
					_currentMusicSource = audioSource;
				}
				if (GameDataController.Exists(string.Format("{0}{1}", "SSS_CASETTE_", i)))
				{
					BackgroundMusics[i].availableForPlay = true;
				}
				BackgroundMusics[i].audioSource = audioSource;
			}
			SwitchState(false);
		}
		PlayMusic();
	}

	private void Update()
	{
		if (_currentMusicSource.isPlaying)
		{
			_currentMusicSource.volume = Mathf.Lerp(_currentMusicSource.volume, BackgroundMusics[_currentMusicId].volume * MusicVolume, volumeTimer += 5f * Time.deltaTime);
		}
	}

	public void SetListener(bool enable)
	{
		listener.enabled = enable;
	}

	public void EnableMusic(int musicId)
	{
		if (musicId >= 0 || musicId <= BackgroundMusics.Length - 1)
		{
			BackgroundMusics[musicId].availableForPlay = true;
			GameDataController.Save(1, string.Format("{0}{1}", "SSS_CASETTE_", musicId));
		}
	}

	public void Stop()
	{
		_currentMusicSource.Stop();
		isStopped = true;
		GameDataController.Save(true, "AudioStop");
	}

	public void PlayMusic()
	{
		isStopped = false;
		GameDataController.Delete("AudioStop");
		SwitchMusic(_currentMusicId);
	}

	public void SetMusic(int musicId)
	{
		if (musicId >= 0 || musicId <= BackgroundMusics.Length - 1)
		{
			_currentMusicId = musicId;
		}
	}

	public int GetMusicId()
	{
		return _currentMusicId;
	}

	public string PlayNext()
	{
		int musicId = FindNextAvailableMusic(true);
		SwitchMusic(musicId);
		return GetCurrentMusicTitle();
	}

	public string PlayPrevious()
	{
		int musicId = FindNextAvailableMusic(false);
		SwitchMusic(musicId);
		return GetCurrentMusicTitle();
	}

	public string GetCurrentMusicTitle()
	{
		return string.Format("{0}. {1}", _currentMusicId + 1, BackgroundMusics[_currentMusicId].description);
	}

	public void SwitchMusic(int musicId, bool firstRun = false)
	{
		if (firstRun || _currentMusicId != musicId)
		{
			_nextMusicId = musicId;
			_currentMusicSource = BackgroundMusics[_nextMusicId].audioSource;
			_currentMusicSource.volume = 0f;
			_currentMusicSource.Play();
			StartCoroutine("ChangeMusic");
		}
	}

	private IEnumerator ChangeMusic()
	{
		float fTimeCounter = 0f;
		while (!Mathf.Approximately(fTimeCounter, 1f))
		{
			fTimeCounter = Mathf.Clamp01(fTimeCounter + Time.deltaTime * 0.85f);
			if (_currentMusicId != _nextMusicId)
			{
				BackgroundMusics[_currentMusicId].audioSource.volume = BackgroundMusics[_currentMusicId].volume * MusicVolume * (1f - fTimeCounter);
			}
			_currentMusicSource.volume = BackgroundMusics[_nextMusicId].volume * MusicVolume * fTimeCounter;
			yield return new WaitForSeconds(0.03f);
		}
		if (_currentMusicId != _nextMusicId)
		{
			BackgroundMusics[_currentMusicId].audioSource.Stop();
			_currentMusicId = _nextMusicId;
		}
		StopCoroutine("ChangeMusic");
	}

	private int FindNextAvailableMusic(bool forward)
	{
		int num = _currentMusicId;
		bool flag = false;
		while (!flag)
		{
			num += (forward ? 1 : (-1));
			if (num > BackgroundMusics.Length - 1)
			{
				num = 0;
			}
			else if (num < 0)
			{
				num = BackgroundMusics.Length - 1;
			}
			if (BackgroundMusics[num].availableForPlay)
			{
				flag = true;
			}
		}
		return num;
	}

	public void SetSFXVolume(float vol)
	{
		audioMixer.SetFloat("EffectsVolume", (!(vol > 0f)) ? (-80f) : 0f);
	}

	public void SetMusicVolume(float vol)
	{
		MusicVolume = vol;
		volumeTimer = 0f;
	}

	public AudioItem GetAudio(Effect type)
	{
		if (Audio.ContainsKey(type))
		{
			return Audio[type];
		}
		return null;
	}

	public void Play(Effect effect, float overridePitch = -1f, float overrideVolume = -1f)
	{
		if (effect != 0 && SfxEnabled && Audio.ContainsKey(effect))
		{
			GetComponent<AudioSource>().pitch = ((overridePitch == -1f) ? Audio[effect].pitch : overridePitch);
			GetComponent<AudioSource>().PlayOneShot(Audio[effect].clip, (overrideVolume == -1f) ? Audio[effect].volume : overrideVolume);
		}
	}

	public void SwitchState(bool pause)
	{
		if (!isStopped)
		{
			if (pause && _currentMusicSource != null)
			{
				_currentMusicSource.Pause();
			}
			else if (_currentMusicSource != null && !_currentMusicSource.isPlaying)
			{
				_currentMusicSource.Play();
			}
		}
	}
}
