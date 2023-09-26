using System.Collections.Generic;
using UnityEngine;

public class MissileShooter : ResetObject
{
	public GameObject missle;

	public float randomSpeedMultiplier = 1f;

	public int missleCount = 15;

	public float serieAngleDiff;

	public float serieEndDiff = 5f;

	public bool randomScaling = true;

	private HoverController player;

	private float shootTimer;

	private Stack<GameObject> missles;

	private void Awake()
	{
		player = Object.FindObjectOfType<HoverController>();
		missles = new Stack<GameObject>(missleCount);
		CreateMissiles();
	}

	private void OnEnable()
	{
		shootTimer = 0.01f;
	}

	private void Update()
	{
		if ((GameController.Instance != null && GameController.Instance.State == GameController.GameState.Start) || !(shootTimer > 0f))
		{
			return;
		}
		shootTimer -= Time.deltaTime;
		if (shootTimer <= 0f)
		{
			if (missles.Count > 1)
			{
				shootTimer = Random.Range(0.5f, 2.5f) * randomSpeedMultiplier;
			}
			GameObject gameObject = missles.Pop();
			gameObject.SetActive(true);
			gameObject.transform.parent = GameController.Instance.transform;
			gameObject.GetComponent<Missile>().anim.enabled = true;
		}
	}

	public override void Reset(bool isRewind)
	{
		base.Reset(isRewind);
		CreateMissiles();
		shootTimer = 0.01f;
		base.gameObject.SetActive(false);
	}

	private void CreateMissiles()
	{
		int count = missles.Count;
		for (int i = count; i < missleCount; i++)
		{
			GameObject gameObject = Object.Instantiate(missle);
			gameObject.name = string.Format("{0}_{1}", "Missile", i);
			gameObject.transform.parent = player.walker.transform;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localPosition = Vector3.zero;
			if (randomScaling)
			{
				float num = Random.Range(0.75f, 1.5f);
				gameObject.transform.localScale = new Vector3(num, num, num);
			}
			if (serieAngleDiff == 0f)
			{
				gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 180), 0f));
				gameObject.transform.localPosition = new Vector3(0f, -2f, Random.Range(100, 180));
			}
			else
			{
				gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0f, Random.Range(100, 180), 0f));
				gameObject.transform.localPosition = new Vector3((float)(missleCount / 2) * (0f - serieAngleDiff) + (float)i * serieAngleDiff, -2f, 110f + (float)i * serieEndDiff);
			}
			gameObject.SetActive(false);
			missles.Push(gameObject);
		}
	}
}
