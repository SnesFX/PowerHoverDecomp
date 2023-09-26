using UnityEngine;

public class RespawnController : MonoBehaviour
{
	public float StartingMinHeight = 20f;

	public float StartingMaxHeight = 40f;

	public Rect StartingRange = new Rect(-5f, -5f, 10f, 10f);

	public Collider RespawnCollider;

	private void Start()
	{
		Spawn();
	}

	private void OnCollisionEnter(Collision col)
	{
		if (col.collider == RespawnCollider)
		{
			Spawn();
		}
	}

	private void Spawn()
	{
		base.transform.position = new Vector3(Random.Range(StartingRange.xMin, StartingRange.xMax), Random.Range(StartingMinHeight, StartingMaxHeight), Random.Range(StartingRange.yMin, StartingRange.yMax));
	}
}
