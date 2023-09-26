using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneController : ControllerBase
{
	[Serializable]
	public enum CutSceneType
	{
		Normal = 0,
		First = 1,
		Last = 2
	}

	[Serializable]
	public class ExtraColorAnimate
	{
		public int extraCounterEffect;

		public ColorAnimate colorAnimate;

		public Color targetColor;

		public IEnumerator animRutine { get; set; }
	}

	[Serializable]
	public class CutScene
	{
		public GameObject rootObject;

		public Camera cutCamera;

		public float fadeAfterActionTime;

		public Animator AnimatorToActivate;

		public string AnimationToPlay;

		public bool fogEnabled;

		public float fogStart;

		public float fogEnd;

		public int ContinueOnExtraCounter;

		public ExtraColorAnimate[] extraColorAnimates;

		public int extraCounter { get; set; }
	}

	public AudioSource CasetteAudio;

	public List<CutScene> cutScenes;

	public int cutCounter;

	public CutSceneType cutSceneType;

	public GameObject SkipCanvas;

	private bool released;

	public MenuType currentMenu { get; private set; }

	public override void Awake()
	{
		foreach (CutScene cutScene in cutScenes)
		{
			if ((float)Screen.width / (float)Screen.height < 1.5f)
			{
				cutScene.cutCamera.fieldOfView *= 1.2f;
			}
			cutScene.cutCamera.enabled = false;
			cutScene.rootObject.SetActive(false);
		}
		if (Application.loadedLevelName.StartsWith("MenuCutScene"))
		{
			GetComponent<AudioListener>().enabled = true;
			if ((bool)debugEventSystem)
			{
				debugEventSystem.SetActive(true);
			}
			base.gameObject.SetActive(true);
		}
		else
		{
			cutCounter = 0;
			base.gameObject.SetActive(true);
		}
		UpdateLocalization();
	}

	public override void OnEnable()
	{
		if (renderSettingsInUse)
		{
			if (renderSettings == null)
			{
				renderSettings = GetComponent<SceneRenderSettings>();
			}
			renderSettings.LoadSettings();
		}
		if (GameDataController.Exists("FirstStartMusic") && SkipCanvas != null)
		{
			SkipCanvas.SetActive(false);
		}
		SetCutSceneValues();
	}

	private void Update()
	{
		if (DeviceSettings.Instance != null && DeviceSettings.Instance.EnableInputDevices)
		{
			if (Input.GetButtonDown("Fire"))
			{
				if (released)
				{
					Collider[] componentsInChildren = base.transform.GetComponentsInChildren<Collider>();
					if (componentsInChildren != null && componentsInChildren.Length > 0)
					{
						for (int i = 0; i < componentsInChildren.Length; i++)
						{
							if (!componentsInChildren[i].enabled)
							{
								continue;
							}
							if (componentsInChildren[i].CompareTag("CutsceneExtra"))
							{
								if (!ExtraEvent(componentsInChildren[i]))
								{
									MoveOn(componentsInChildren[i]);
								}
								break;
							}
							if (!componentsInChildren[i].CompareTag("GameController"))
							{
								MoveOn(componentsInChildren[i]);
								break;
							}
						}
					}
					released = false;
				}
			}
			else
			{
				released = true;
			}
		}
		RaycastHit hitInfo;
		if (Input.GetMouseButtonDown(0) && Physics.Raycast(cutScenes[cutCounter].cutCamera.ScreenPointToRay(Input.mousePosition), out hitInfo) && (!hitInfo.collider.gameObject.CompareTag("CutsceneExtra") || !ExtraEvent(hitInfo.collider)) && !hitInfo.collider.gameObject.CompareTag("GameController"))
		{
			MoveOn(hitInfo.collider);
		}
	}

	private bool ExtraEvent(Collider collider)
	{
		collider.enabled = false;
		if (collider.GetComponent<Animator>() != null)
		{
			collider.GetComponent<Animator>().Play("SmallLampOff", -1, 0f);
		}
		AudioSource component = collider.GetComponent<AudioSource>();
		if (component != null)
		{
			component.pitch = 0.8f - 0.04f * (float)cutScenes[cutCounter].extraCounter;
			component.Play();
		}
		cutScenes[cutCounter].extraCounter++;
		CheckExtraColorAnimators();
		if (cutScenes[cutCounter].extraCounter < cutScenes[cutCounter].ContinueOnExtraCounter)
		{
			return true;
		}
		return false;
	}

	private void MoveOn(Collider collider)
	{
		if (!CasetteAudio.isPlaying)
		{
			CasetteAudio.Play();
			if (GameDataController.Exists("FirstStartMusic"))
			{
				AudioController.Instance.SetMusicVolume(0.5f);
				AudioController.Instance.SwitchMusic(0, true);
				GameDataController.Delete("FirstStartMusic");
			}
		}
		collider.enabled = false;
		Collider[] componentsInChildren = cutScenes[cutCounter].rootObject.GetComponentsInChildren<Collider>();
		foreach (Collider collider2 in componentsInChildren)
		{
			collider2.enabled = false;
		}
		if (cutScenes[cutCounter].AnimatorToActivate != null && cutScenes[cutCounter].AnimationToPlay != null)
		{
			cutScenes[cutCounter].AnimatorToActivate.enabled = true;
			cutScenes[cutCounter].AnimatorToActivate.Play(cutScenes[cutCounter].AnimationToPlay, -1, 0f);
		}
		StartCoroutine(GoToNextScene());
	}

	public void MoveToNextScene()
	{
		if (cutScenes[cutCounter].AnimatorToActivate != null && cutScenes[cutCounter].AnimationToPlay != null)
		{
			cutScenes[cutCounter].AnimatorToActivate.enabled = true;
			cutScenes[cutCounter].AnimatorToActivate.Play(cutScenes[cutCounter].AnimationToPlay, -1, 0f);
		}
		StartCoroutine(GoToNextScene());
	}

	private IEnumerator AnimateColors(ColorAnimate anim, Color target)
	{
		float fTimeCounter = 0f;
		Color start = anim._Color;
		while (fTimeCounter < 1f)
		{
			fTimeCounter += Time.fixedDeltaTime * 1.5f;
			anim._Color = Color.Lerp(start, target, fTimeCounter);
			yield return new WaitForSeconds(0.02f);
		}
	}

	private void CheckExtraColorAnimators()
	{
		if (cutScenes[cutCounter].extraColorAnimates == null)
		{
			return;
		}
		for (int i = 0; i < cutScenes[cutCounter].extraColorAnimates.Length; i++)
		{
			if (cutScenes[cutCounter].extraColorAnimates[i].extraCounterEffect == cutScenes[cutCounter].extraCounter)
			{
				if (cutScenes[cutCounter].extraColorAnimates[i].animRutine != null)
				{
					StopCoroutine(cutScenes[cutCounter].extraColorAnimates[i].animRutine);
				}
				cutScenes[cutCounter].extraColorAnimates[i].animRutine = AnimateColors(cutScenes[cutCounter].extraColorAnimates[i].colorAnimate, cutScenes[cutCounter].extraColorAnimates[i].targetColor);
				StartCoroutine(cutScenes[cutCounter].extraColorAnimates[i].animRutine);
			}
		}
	}

	private IEnumerator GoToNextScene()
	{
		yield return new WaitForSeconds(cutScenes[cutCounter].fadeAfterActionTime);
		NextScene();
	}

	public void NextScene()
	{
		if (cutCounter + 1 >= cutScenes.Count)
		{
			if (SkipCanvas != null)
			{
				SkipCanvas.SetActive(false);
			}
			switch (cutSceneType)
			{
			case CutSceneType.Last:
				SceneLoader.Instance.LoadCredits();
				break;
			case CutSceneType.First:
				Main.Instance.SwapMenu(MenuType.Logo);
				break;
			default:
				Main.Instance.SwapMenu(MenuType.Main);
				break;
			}
			cutScenes[cutCounter].cutCamera.enabled = false;
			cutScenes[cutCounter].rootObject.SetActive(false);
			cutCounter = 0;
		}
		else if (Main.Instance != null)
		{
			StartCoroutine(SwapCutScene());
		}
		else
		{
			StartCoroutine(SwapCutScene());
		}
	}

	private IEnumerator SwapCutScene()
	{
		cutScenes[cutCounter].cutCamera.enabled = false;
		cutScenes[cutCounter].rootObject.SetActive(false);
		yield return new WaitForSeconds(0.6f);
		cutCounter++;
		SetCutSceneValues();
	}

	private void SetCutSceneValues()
	{
		cutScenes[cutCounter].cutCamera.enabled = true;
		cutScenes[cutCounter].rootObject.SetActive(true);
		RenderSettings.fog = cutScenes[cutCounter].fogEnabled;
		RenderSettings.fogStartDistance = cutScenes[cutCounter].fogStart;
		RenderSettings.fogEndDistance = cutScenes[cutCounter].fogEnd;
		CheckExtraColorAnimators();
	}

	private void UpdateLocalization()
	{
		TextMesh[] componentsInChildren = GetComponentsInChildren<TextMesh>(true);
		foreach (TextMesh textMesh in componentsInChildren)
		{
			if ((textMesh.gameObject.name.StartsWith("Cutscene") || textMesh.gameObject.name.StartsWith("MainMenu")) && LocalizationLoader.Instance != null)
			{
				LocalizationLoader.Instance.SetText(textMesh, textMesh.gameObject.name);
			}
		}
	}

	public void SkipCutScene()
	{
		if (Main.Instance != null)
		{
			Main.Instance.SwitchMenu(MenuType.Main);
		}
	}
}
