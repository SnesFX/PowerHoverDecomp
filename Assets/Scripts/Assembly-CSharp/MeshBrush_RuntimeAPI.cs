using UnityEngine;

public class MeshBrush_RuntimeAPI : MonoBehaviour
{
	private Transform thisTransform;

	private GameObject brushObj;

	private Transform brushTransform;

	private RaycastHit hit;

	private GameObject paintedMesh;

	private Transform paintedMeshTransform;

	private GameObject holder;

	private Transform holderTransform;

	private ushort _amount = 1;

	private float _delayBetweenPaintStrokes = 0.15f;

	private float _brushRadius = 1f;

	private float scatteringInsetThreshold;

	private float _slopeInfluence = 100f;

	private float _maxSlopeFilterAngle = 30f;

	private float slopeAngle;

	private float randomWidth;

	private float randomHeight;

	private Vector4 _randomScale = new Vector4(1f, 1f, 1f, 1f);

	public GameObject[] setOfMeshesToPaint { get; set; }

	public ushort amount
	{
		get
		{
			return _amount;
		}
		set
		{
			_amount = (ushort)Mathf.Clamp(value, 1, 100);
		}
	}

	public float delayBetweenPaintStrokes
	{
		get
		{
			return _delayBetweenPaintStrokes;
		}
		set
		{
			_delayBetweenPaintStrokes = Mathf.Clamp(value, 0.1f, 1f);
		}
	}

	public float brushRadius
	{
		get
		{
			return _brushRadius;
		}
		set
		{
			_brushRadius = value;
			if (_brushRadius <= 0.1f)
			{
				_brushRadius = 0.1f;
			}
		}
	}

	public float meshOffset { get; set; }

	public float scattering { get; set; }

	public bool yAxisIsTangent { get; set; }

	public float slopeInfluence
	{
		get
		{
			return _slopeInfluence;
		}
		set
		{
			_slopeInfluence = Mathf.Clamp(value, 0f, 100f);
		}
	}

	public bool activeSlopeFilter { get; set; }

	public float maxSlopeFilterAngle
	{
		get
		{
			return _maxSlopeFilterAngle;
		}
		set
		{
			_maxSlopeFilterAngle = Mathf.Clamp(value, 0f, 180f);
		}
	}

	public bool inverseSlopeFilter { get; set; }

	public bool manualRefVecSampling { get; set; }

	public Vector3 sampledSlopeRefVector { get; set; }

	public float randomRotation { get; set; }

	public Vector4 randomScale
	{
		get
		{
			return _randomScale;
		}
		set
		{
			_randomScale = new Vector4(Mathf.Clamp(value.x, 0.01f, value.x), Mathf.Clamp(value.y, 0.01f, value.y), Mathf.Clamp(value.z, 0.01f, value.z), Mathf.Clamp(value.w, 0.01f, value.w));
		}
	}

	public Vector3 additiveScale { get; set; }

	private void Start()
	{
		thisTransform = base.transform;
		scattering = 75f;
	}

	public void Paint_SingleMesh(RaycastHit paintHit)
	{
		if (!paintHit.collider.transform.Find("Holder"))
		{
			holder = new GameObject("Holder");
			holderTransform = holder.transform;
			holderTransform.position = paintHit.collider.transform.position;
			holderTransform.rotation = paintHit.collider.transform.rotation;
			holderTransform.parent = paintHit.collider.transform;
		}
		slopeAngle = (activeSlopeFilter ? Vector3.Angle(paintHit.normal, (!manualRefVecSampling) ? Vector3.up : sampledSlopeRefVector) : ((!inverseSlopeFilter) ? 0f : 180f));
		if ((!inverseSlopeFilter) ? (slopeAngle < maxSlopeFilterAngle) : (slopeAngle > maxSlopeFilterAngle))
		{
			paintedMesh = Object.Instantiate(setOfMeshesToPaint[Random.Range(0, setOfMeshesToPaint.Length)], paintHit.point, Quaternion.LookRotation(paintHit.normal));
			paintedMeshTransform = paintedMesh.transform;
			if (yAxisIsTangent)
			{
				paintedMeshTransform.up = Vector3.Lerp(paintedMeshTransform.up, paintedMeshTransform.forward, slopeInfluence * 0.01f);
			}
			else
			{
				paintedMeshTransform.up = Vector3.Lerp(Vector3.up, paintedMeshTransform.forward, slopeInfluence * 0.01f);
			}
			paintedMeshTransform.parent = holderTransform;
			ApplyRandomScale(paintedMesh);
			ApplyRandomRotation(paintedMesh);
			ApplyMeshOffset(paintedMesh, hit.normal);
		}
	}

	public void Paint_MultipleMeshes(RaycastHit paintHit)
	{
		scatteringInsetThreshold = brushRadius * 0.01f * scattering;
		if (brushObj == null)
		{
			brushObj = new GameObject("Brush");
			brushTransform = brushObj.transform;
			brushTransform.position = thisTransform.position;
			brushTransform.parent = paintHit.collider.transform;
		}
		if (!paintHit.collider.transform.Find("Holder"))
		{
			holder = new GameObject("Holder");
			holderTransform = holder.transform;
			holderTransform.position = paintHit.collider.transform.position;
			holderTransform.rotation = paintHit.collider.transform.rotation;
			holderTransform.parent = paintHit.collider.transform;
		}
		for (int num = amount; num > 0; num--)
		{
			brushTransform.position = paintHit.point + paintHit.normal * 0.5f;
			brushTransform.rotation = Quaternion.LookRotation(paintHit.normal);
			brushTransform.up = brushTransform.forward;
			brushTransform.Translate(Random.Range((0f - Random.insideUnitCircle.x) * scatteringInsetThreshold, Random.insideUnitCircle.x * scatteringInsetThreshold), 0f, Random.Range((0f - Random.insideUnitCircle.y) * scatteringInsetThreshold, Random.insideUnitCircle.y * scatteringInsetThreshold), Space.Self);
			if (Physics.Raycast(brushTransform.position, -paintHit.normal, out hit, 2.5f))
			{
				slopeAngle = (activeSlopeFilter ? Vector3.Angle(hit.normal, (!manualRefVecSampling) ? Vector3.up : sampledSlopeRefVector) : ((!inverseSlopeFilter) ? 0f : 180f));
				if ((!inverseSlopeFilter) ? (slopeAngle < maxSlopeFilterAngle) : (slopeAngle > maxSlopeFilterAngle))
				{
					paintedMesh = Object.Instantiate(setOfMeshesToPaint[Random.Range(0, setOfMeshesToPaint.Length)], hit.point, Quaternion.LookRotation(hit.normal));
					paintedMeshTransform = paintedMesh.transform;
					if (yAxisIsTangent)
					{
						paintedMeshTransform.up = Vector3.Lerp(paintedMeshTransform.up, paintedMeshTransform.forward, slopeInfluence * 0.01f);
					}
					else
					{
						paintedMeshTransform.up = Vector3.Lerp(Vector3.up, paintedMeshTransform.forward, slopeInfluence * 0.01f);
					}
					paintedMeshTransform.parent = holderTransform;
				}
				ApplyRandomScale(paintedMesh);
				ApplyRandomRotation(paintedMesh);
				ApplyMeshOffset(paintedMesh, hit.normal);
			}
		}
	}

	private void ApplyRandomScale(GameObject sMesh)
	{
		randomWidth = Random.Range(randomScale.x, randomScale.y);
		randomHeight = Random.Range(randomScale.z, randomScale.w);
		sMesh.transform.localScale = new Vector3(randomWidth, randomHeight, randomWidth);
	}

	private void AddConstantScale(GameObject sMesh)
	{
		sMesh.transform.localScale += new Vector3(Mathf.Clamp(additiveScale.x, -0.9f, additiveScale.x), Mathf.Clamp(additiveScale.y, -0.9f, additiveScale.y), Mathf.Clamp(additiveScale.z, -0.9f, additiveScale.z));
	}

	private void ApplyRandomRotation(GameObject rMesh)
	{
		rMesh.transform.Rotate(new Vector3(0f, Random.Range(0f, 3.6f * Mathf.Clamp(randomRotation, 0f, 100f)), 0f));
	}

	private void ApplyMeshOffset(GameObject oMesh, Vector3 offsetDirection)
	{
		oMesh.transform.Translate(offsetDirection.normalized * meshOffset * 0.01f, Space.World);
	}
}
