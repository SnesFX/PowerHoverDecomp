using UnityEngine;

public class PingPongMove : MonoBehaviour
{
	public float speed = 1f;

	public Vector3 moveLength;

	public bool randomStart;

	private Vector3 randomStartPosition = Vector3.zero;

	private Vector3 startPos;

	private Vector3 move = Vector3.zero;

	private float timer;

	private void Start()
	{
		startPos = base.transform.localPosition;
		if (randomStart)
		{
			randomStartPosition = new Vector3(Random.Range(1f, 5f), Random.Range(1f, 5f), Random.Range(1f, 5f));
		}
	}

	private void Update()
	{
		timer += Time.deltaTime;
		move.x = ((moveLength.x != 0f) ? Mathf.PingPong(timer * speed + randomStartPosition.x, moveLength.x) : 0f);
		move.y = ((moveLength.y != 0f) ? Mathf.PingPong(timer * speed + randomStartPosition.y, moveLength.y) : 0f);
		move.z = ((moveLength.z != 0f) ? Mathf.PingPong(timer * speed + randomStartPosition.z, moveLength.z) : 0f);
		move.x *= ((!(moveLength.x < 0f)) ? 1 : (-1));
		move.y *= ((!(moveLength.y < 0f)) ? 1 : (-1));
		move.z *= ((!(moveLength.z < 0f)) ? 1 : (-1));
		base.transform.localPosition = startPos + move;
	}

	public void SetMove(float newSpeed, Vector3 move)
	{
		if (randomStart)
		{
			randomStartPosition = new Vector3(Random.Range(1f, 5f), Random.Range(1f, 5f), Random.Range(1f, 5f));
		}
		moveLength = move;
		timer = 0f;
		speed = newSpeed;
	}
}
