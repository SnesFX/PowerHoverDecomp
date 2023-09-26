using UnityEngine;

public class RandomMeshBuilderPosition : ResetObject
{
	public float Lenght;

	public float start;

	public float end;

	public SplinePathMeshBuilder colliderObject;

	private SplinePathMeshBuilder builder;

	private MeshRenderer renderMesh;

	private void Start()
	{
		renderMesh = GetComponent<MeshRenderer>();
		builder = GetComponent<SplinePathMeshBuilder>();
		Reset(false);
	}

	public override void Reset(bool isRewind)
	{
		base.Reset(isRewind);
		builder.FromTF = Random.Range(start, end - Lenght);
		builder.ToTF = builder.FromTF + Lenght;
		builder.Refresh();
		colliderObject.FromTF = builder.FromTF;
		colliderObject.ToTF = builder.ToTF;
		colliderObject.Refresh();
	}

	private void FixedUpdate()
	{
		renderMesh.enabled = Time.timeScale < 1f;
	}
}
