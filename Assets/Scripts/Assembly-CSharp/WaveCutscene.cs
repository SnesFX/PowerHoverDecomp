using UnityEngine;

public class WaveCutscene : MonoBehaviour
{
	public float scale = 0.1f;

	public float speed = 1f;

	public float waveDistance = 1f;

	public float noiseStrength = 1f;

	public float noiseWalk = 1f;

	public bool forceOn;

	public bool IsActive;

	private Mesh waterMesh;

	private Vector3[] newVertices;

	private Vector3[] originalVertices;

	private void Start()
	{
		waterMesh = GetComponent<MeshFilter>().mesh;
		originalVertices = waterMesh.vertices;
	}

	private void Update()
	{
		if (forceOn || IsActive)
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
	}

	private float GetWaveYPos(float x_coord, float z_coord)
	{
		float num = 0f;
		num += Mathf.Sin((Time.time * speed + z_coord) / waveDistance) * scale;
		return num + Mathf.PerlinNoise(x_coord + noiseWalk, z_coord + Mathf.Sin(Time.time * 0.1f)) * noiseStrength;
	}
}
