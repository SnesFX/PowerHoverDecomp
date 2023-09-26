using UnityEngine;

public class MeshBrush : MonoBehaviour
{
	[HideInInspector]
	public bool isActive = true;

	[HideInInspector]
	public GameObject brush;

	[HideInInspector]
	public Transform holderObj;

	[HideInInspector]
	public Transform brushTransform;

	[HideInInspector]
	public string groupName = "<group name>";

	public GameObject[] setOfMeshesToPaint = new GameObject[1];

	[HideInInspector]
	public KeyCode paint = KeyCode.P;

	[HideInInspector]
	public KeyCode increaseRadius = KeyCode.O;

	[HideInInspector]
	public KeyCode decreaseRadius = KeyCode.I;

	[HideInInspector]
	public float hRadius = 0.3f;

	[HideInInspector]
	public Color hColor = Color.white;

	[HideInInspector]
	public int meshCount = 1;

	[HideInInspector]
	public float delay = 0.25f;

	[HideInInspector]
	public float meshOffset;

	[HideInInspector]
	public float slopeInfluence = 100f;

	[HideInInspector]
	public bool activeSlopeFilter;

	[HideInInspector]
	public float maxSlopeFilterAngle = 30f;

	[HideInInspector]
	public bool inverseSlopeFilter;

	[HideInInspector]
	public bool manualRefVecSampling;

	[HideInInspector]
	public bool showRefVecInSceneGUI = true;

	[HideInInspector]
	public Vector3 slopeRefVec = Vector3.up;

	[HideInInspector]
	public Vector3 slopeRefVec_HandleLocation = Vector3.zero;

	[HideInInspector]
	public bool yAxisIsTangent;

	[HideInInspector]
	public bool invertY;

	[HideInInspector]
	public float inset = 60f;

	[HideInInspector]
	public bool autoStatic = true;

	[HideInInspector]
	public bool uniformScale = true;

	[HideInInspector]
	public bool constUniformScale = true;

	[HideInInspector]
	public bool rWithinRange;

	[HideInInspector]
	public bool b_CustomKeys;

	[HideInInspector]
	public bool b_Slopes = true;

	[HideInInspector]
	public bool b_Randomizers = true;

	[HideInInspector]
	public bool b_AdditiveScale = true;

	[HideInInspector]
	public bool b_opt = true;

	[HideInInspector]
	public float rScaleW;

	[HideInInspector]
	public float rScaleH;

	[HideInInspector]
	public float rScale;

	[HideInInspector]
	public Vector2 rUniformRange = Vector2.zero;

	[HideInInspector]
	public Vector4 rNonUniformRange = Vector4.zero;

	[HideInInspector]
	public float cScale;

	[HideInInspector]
	public Vector3 cScaleXYZ = Vector3.zero;

	[HideInInspector]
	public float rRot;

	public void ResetSlopeSettings()
	{
		slopeInfluence = 100f;
		maxSlopeFilterAngle = 30f;
		activeSlopeFilter = false;
		inverseSlopeFilter = false;
		manualRefVecSampling = false;
		showRefVecInSceneGUI = true;
	}

	public void ResetRandomizers()
	{
		rScale = 0f;
		rScaleW = 0f;
		rScaleH = 0f;
		rRot = 0f;
		rUniformRange = Vector2.zero;
		rNonUniformRange = Vector4.zero;
	}
}
