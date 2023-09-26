using UnityEngine;

public class OddrokController : MonoBehaviour
{
	public AudioListener audioListener;

	public GameObject splash;

	public Animator fader;

	public AudioSource audioSplash;

	private float waitTimer;

	private int state;

	private bool kill;

	private void Start()
	{
		waitTimer = 0f;
		state = 0;
	}

	private void FixedUpdate()
	{
		switch (state)
		{
		case 0:
			waitTimer += Time.fixedDeltaTime;
			if (waitTimer > 1.5f)
			{
				state = 1;
				waitTimer = 0f;
				fader.Play("oddrokfadein", -1, 0f);
				audioSplash.Play();
			}
			break;
		case 1:
			waitTimer += Time.fixedDeltaTime;
			if (waitTimer > 6f || (waitTimer > 1.5f && (Input.GetMouseButton(0) || Input.GetButtonDown("Fire"))))
			{
				audioSplash.Stop();
				state = 2;
				waitTimer = 0f;
				if (GameDataController.Exists("LastScene"))
				{
					fader.Play("oddrokfadeout", -1, 0f);
				}
				else if (Main.Instance != null)
				{
					Main.Instance.FakeFadeIn();
					waitTimer = -0.25f;
				}
			}
			break;
		case 2:
			waitTimer += Time.fixedDeltaTime;
			if (waitTimer > 1f)
			{
				state = 3;
				audioListener.enabled = false;
				Application.LoadLevelAdditive("MenuMain");
				kill = true;
			}
			break;
		case 3:
			waitTimer += Time.fixedDeltaTime;
			if (kill && waitTimer > 0.5f && SceneLoader.Instance != null && Main.Instance != null && Main.Instance.MainMenuLoaded)
			{
				state = 4;
				if (!GameDataController.Exists("LastScene"))
				{
					SceneLoader.Instance.LoadTutorial();
				}
				else
				{
					Main.Instance.SwapMenu(MenuType.Logo, true);
				}
				Object.Destroy(splash);
			}
			break;
		case 4:
			break;
		}
	}
}
