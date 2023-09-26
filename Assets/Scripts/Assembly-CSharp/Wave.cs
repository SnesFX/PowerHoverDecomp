using UnityEngine;

public class Wave : ObjectActivator
{
	public float scale = 0.1f;

	public float speed = 1f;

	public float waveDistance = 1f;

	public float noiseStrength = 1f;

	public float noiseWalk = 1f;

	public bool forceOn;

	private Mesh waterMesh;

	private Vector3[] newVertices;

	private Vector3[] originalVertices;

	private MeshCollider waterCollider;

	public override void Start()
	{
		base.Start();
		waterMesh = GetComponent<MeshFilter>().mesh;
		waterCollider = GetComponent<MeshCollider>();
		originalVertices = waterMesh.vertices;
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
	}

	private void Update()
	{
		if ((forceOn || base.IsActive) && DeviceSettings.Instance != null && DeviceSettings.Instance.EnableWaves)
		{
			MoveSea();
		}
	}

	private void MoveSea()
	{
		newVertices = new Vector3[originalVertices.Length];
		for (int i = 0; i < originalVertices.Length; i++)
		{
			Vector3 position = originalVertices[i];
			position = base.transform.TransformPoint(position);
			position.y += GetWaveYPos(position.x, position.z);
			newVertices[i] = base.transform.InverseTransformPoint(position);
		}
		waterMesh.vertices = newVertices;
		waterMesh.RecalculateNormals();
		waterCollider.sharedMesh = null;
		waterCollider.sharedMesh = waterMesh;
	}

	private float GetWaveYPos(float x_coord, float z_coord)
	{
		float num = 0f;
		num += Mathf.Sin((Time.time * speed + z_coord) / waveDistance) * scale;
		return num + Mathf.PerlinNoise(x_coord + noiseWalk, z_coord + Mathf.Sin(Time.time * 0.1f)) * noiseStrength;
	}
}
