using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
	private enum MusicPlayerState
	{
		None = 0,
		Play = 1,
		Stop = 2,
		Eject = 3,
		Close = 4
	}

	private const string PLAY_BUTTON = "Play";

	private const string STOP_BUTTON = "Stop";

	private const string EJECT_BUTTON = "Eject";

	private const string LID_BUTTON = "Lid";

	private const string CASETTE_BUTTON = "Casette";

	public Camera musicCam;

	public float damping = 0.9f;

	public float speed = 0.1f;

	public Animator playAnimator;

	public Animator stopAnimator;

	public Animator ejectAnimator;

	public Animator lidAnimator;

	private Vector3 vDown;

	private Vector3 vDrag;

	private bool dragging;

	private float angularVelocity;

	private Vector3 rotationAxis;

	private MusicPlayerState state;

	private float stateTimer;

	private bool lidOpen;

	private int downHash = Animator.StringToHash("MusicButtonDown");

	private int upHash = Animator.StringToHash("MusicButtonUp");

	private int lidOpenHash = Animator.StringToHash("MusicLidOpen");

	private int lidCloseHash = Animator.StringToHash("MusicLidClose");

	private MusicCasette[] casettes;

	private void Awake()
	{
		dragging = false;
		lidOpen = false;
		angularVelocity = 0f;
		rotationAxis = Vector3.zero;
		stopAnimator.Play(upHash);
		ejectAnimator.Play(upHash);
		playAnimator.Play(upHash);
		lidAnimator.Play(lidCloseHash);
		casettes = Object.FindObjectsOfType<MusicCasette>();
	}

	private void Start()
	{
		if ((bool)AudioController.Instance && !AudioController.Instance.isStopped)
		{
			state = MusicPlayerState.None;
			playAnimator.Play(downHash);
			stopAnimator.Play(upHash);
			ejectAnimator.Play(upHash);
			casettes[AudioController.Instance.GetMusicId()].InUse(true);
		}
	}

	private void Update()
	{
		switch (state)
		{
		case MusicPlayerState.None:
			UpdateRotator();
			return;
		case MusicPlayerState.Close:
			lidAnimator.GetComponent<BoxCollider>().enabled = false;
			playAnimator.GetComponent<BoxCollider>().enabled = true;
			stopAnimator.GetComponent<BoxCollider>().enabled = true;
			ejectAnimator.GetComponent<BoxCollider>().enabled = true;
			GetComponent<SphereCollider>().enabled = true;
			lidAnimator.Play(lidCloseHash);
			playAnimator.Play(upHash);
			stopAnimator.Play(upHash);
			ejectAnimator.Play(upHash);
			lidOpen = false;
			break;
		case MusicPlayerState.Play:
			playAnimator.Play(downHash);
			stopAnimator.Play(upHash);
			ejectAnimator.Play(upHash);
			if ((bool)AudioController.Instance)
			{
				AudioController.Instance.PlayMusic();
			}
			break;
		case MusicPlayerState.Eject:
			if ((bool)AudioController.Instance)
			{
				AudioController.Instance.Stop();
			}
			playAnimator.Play(upHash);
			stopAnimator.Play(upHash);
			ejectAnimator.Play(downHash);
			lidAnimator.Play(lidOpenHash);
			lidAnimator.GetComponent<BoxCollider>().enabled = true;
			playAnimator.GetComponent<BoxCollider>().enabled = false;
			stopAnimator.GetComponent<BoxCollider>().enabled = false;
			ejectAnimator.GetComponent<BoxCollider>().enabled = false;
			GetComponent<SphereCollider>().enabled = false;
			break;
		case MusicPlayerState.Stop:
			if ((bool)AudioController.Instance)
			{
				AudioController.Instance.Stop();
			}
			playAnimator.Play(upHash);
			stopAnimator.Play(downHash);
			ejectAnimator.Play(upHash);
			lidAnimator.Play(lidCloseHash);
			break;
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer < 0f)
		{
			state = MusicPlayerState.None;
		}
	}

	private void UpdateRotator()
	{
		if (Input.GetMouseButton(0))
		{
			Ray ray = musicCam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo))
			{
				if (!dragging)
				{
					vDown = hitInfo.point - base.transform.position;
					dragging = true;
				}
				else if (hitInfo.collider.gameObject.name.Equals("Play"))
				{
					ButtonPressed(MusicPlayerState.Play);
				}
				else if (hitInfo.collider.gameObject.name.Equals("Stop"))
				{
					ButtonPressed(MusicPlayerState.Stop);
				}
				else if (hitInfo.collider.gameObject.name.Equals("Eject"))
				{
					ButtonPressed(MusicPlayerState.Eject);
					lidOpen = true;
				}
				else if (hitInfo.collider.gameObject.name.Equals("Lid"))
				{
					ButtonPressed(MusicPlayerState.Close);
					lidOpen = true;
				}
				else if (hitInfo.collider.gameObject.name.StartsWith("Casette"))
				{
					if (lidOpen)
					{
						ButtonPressed(MusicPlayerState.Close);
						MusicCasette component = hitInfo.collider.GetComponent<MusicCasette>();
						if ((bool)AudioController.Instance && component != null)
						{
							for (int i = 0; i < casettes.Length; i++)
							{
								casettes[i].InUse(false);
							}
							component.InUse(true);
							AudioController.Instance.SetMusic(component.musicId);
						}
					}
				}
				else
				{
					vDrag = hitInfo.point - base.transform.position;
					rotationAxis = Vector3.Cross(vDown, vDrag);
					rotationAxis.x = (rotationAxis.z = 0f);
					angularVelocity = Vector3.Angle(vDown, vDrag) * speed;
				}
			}
			else
			{
				dragging = false;
			}
		}
		if (Input.GetMouseButtonUp(0))
		{
			dragging = false;
		}
		if (angularVelocity > 0f)
		{
			base.transform.Rotate(rotationAxis, angularVelocity * Time.deltaTime);
			angularVelocity = ((!(angularVelocity > 0.01f)) ? 0f : (angularVelocity * damping));
		}
	}

	private void ButtonPressed(MusicPlayerState toState, float stateTime = 0.2f)
	{
		state = toState;
		stateTimer = stateTime;
		dragging = false;
		GetComponent<AudioSource>().Play();
	}
}
