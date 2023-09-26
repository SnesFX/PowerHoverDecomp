using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TypeWriter : MonoBehaviour
{
	private const int LettersPerSecond = 26;

	private const int PlayAudioOnNewCharactersCount = 2;

	private const int CaretBlinkSpeed = 17;

	public Animator FaceAnimator;

	public AudioClip TextOverClip;

	private AudioClip defaultClip;

	private string fullText;

	private Text item;

	private bool blinkOn;

	private bool writingOn;

	private float timeElapsed;

	private float writeTimer;

	private float deltaTime;

	private float _timeAtLastFrame;

	private AudioSource audioS;

	private int AnimatorState = Animator.StringToHash("State");

	public bool WritingCompleted
	{
		get
		{
			return !blinkOn && !writingOn;
		}
	}

	private void Awake()
	{
		item = GetComponent<Text>();
		audioS = GetComponent<AudioSource>();
		defaultClip = audioS.clip;
	}

	public void StartWriting(string text, float blinkTime = 1f)
	{
		if (text != null && text.Length != 0)
		{
			fullText = text.Replace("\\n", "\n").ToUpper();
			timeElapsed = 0f - blinkTime;
			_timeAtLastFrame = Time.realtimeSinceStartup;
			blinkOn = true;
			item.text = string.Empty;
			if (FaceAnimator != null)
			{
				FaceAnimator.SetInteger(AnimatorState, 1);
			}
			audioS.clip = defaultClip;
		}
	}

	public void StopWriting()
	{
		writingOn = (blinkOn = false);
		fullText = string.Empty;
		item.text = fullText;
		if (audioS != null && audioS.isPlaying)
		{
			audioS.Stop();
		}
		if (FaceAnimator != null)
		{
			FaceAnimator.SetInteger(AnimatorState, 0);
		}
	}

	private void Update()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		deltaTime = realtimeSinceStartup - _timeAtLastFrame;
		_timeAtLastFrame = realtimeSinceStartup;
		if (blinkOn)
		{
			item.text = ((!(Mathf.Sin(Time.realtimeSinceStartup * 17f) >= 0f)) ? string.Empty : "_");
			timeElapsed += deltaTime;
			if (timeElapsed > 0f)
			{
				timeElapsed = 0f;
				writeTimer = 0.4f;
				_timeAtLastFrame = Time.realtimeSinceStartup;
				writingOn = true;
				blinkOn = false;
				if (audioS != null && !audioS.isPlaying)
				{
					audioS.loop = true;
					audioS.Play();
				}
				item.text = string.Empty;
			}
		}
		else if (writingOn)
		{
			timeElapsed += deltaTime * 26f;
			writeTimer += deltaTime;
			if (audioS != null && !audioS.isPlaying)
			{
				audioS.loop = true;
				audioS.Play();
			}
			if (!(writeTimer > 0.05f) || !(timeElapsed > 0.1f))
			{
				return;
			}
			timeElapsed += writeTimer;
			writeTimer = 0f;
			int num = Mathf.FloorToInt(timeElapsed);
			if (num < fullText.Length - 1)
			{
				item.text = fullText.Substring(0, num);
				return;
			}
			writingOn = false;
			if (FaceAnimator != null)
			{
				FaceAnimator.SetInteger(AnimatorState, 0);
			}
			if (TextOverClip != null)
			{
				audioS.loop = false;
				audioS.PlayOneShot(TextOverClip);
			}
		}
		else if (!item.text.Equals(fullText))
		{
			item.text = string.Format("{0}", fullText);
		}
	}
}
