using UnityEngine;

public class BreakWhenHit : MonoBehaviour
{
	public GameObject[] breakingPiecesLeftOver;

	public GameObject breakEffect;

	public ParticleSystem breakParticles;

	public BoxCollider killerCollider;

	private Color partColor = new Color(1f, 1f, 1f, 1f);

	private Material partMaterial;

	private AudioSource breakSound;

	private bool active;

	private float timeToDestroyPieces = 1f;

	private void Awake()
	{
		breakSound = breakEffect.GetComponent<AudioSource>();
	}

	private void Start()
	{
		Reset();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!active && other.CompareTag("Player") && GameController.Instance.State == GameController.GameState.Running)
		{
			BreakStuff();
			if (LevelStats.Instance != null)
			{
				LevelStats.Instance.Collect();
			}
		}
	}

	private void BreakStuff()
	{
		active = true;
		base.transform.GetComponent<MeshRenderer>().enabled = false;
		base.transform.GetComponent<BoxCollider>().enabled = false;
		killerCollider.enabled = false;
		breakEffect.SetActive(true);
		breakSound.Play();
		int num = Random.Range(0, 4);
		breakingPiecesLeftOver[num].SetActive(true);
	}

	public void Reset()
	{
		breakEffect.SetActive(false);
		if (breakSound != null)
		{
			breakSound.Stop();
			breakSound.pitch = Random.Range(0.9f, 1.2f);
		}
		base.transform.GetComponent<MeshRenderer>().enabled = true;
		base.transform.GetComponent<BoxCollider>().enabled = true;
		killerCollider.enabled = true;
		GameObject[] array = breakingPiecesLeftOver;
		foreach (GameObject gameObject in array)
		{
			gameObject.gameObject.SetActive(false);
		}
		active = false;
	}
}
