using UnityEngine;

public class LineGraphics : MonoBehaviour
{
	private float setPositionTimer;

	private BlobShadowSimple shadow;

	private Transform moveTransform;

	public void SetLineGraphics(CurvySpline spline)
	{
		Vector3 position = spline.ControlPoints[spline.ControlPointCount - 1].transform.position;
		Quaternion orientationFast = spline.GetOrientationFast(1f);
		GameObject gameObject = Object.Instantiate(position: new Vector3(position.x + 1f, position.y + 10f, position.z), original: Resources.Load("FinishLine"), rotation: orientationFast) as GameObject;
		gameObject.transform.parent = base.transform.parent;
		shadow = gameObject.GetComponentInChildren<BlobShadowSimple>();
		gameObject.transform.position += gameObject.transform.forward * -10f;
		gameObject.transform.position += gameObject.transform.up * 10f;
		if ((SceneLoader.Instance != null && SceneLoader.Instance.Current != null && SceneLoader.Instance.Current.SceneName.Equals("Hover29")) || Application.loadedLevelName.Equals("Hover29"))
		{
			BoxCollider componentInChildren = gameObject.GetComponentInChildren<BoxCollider>();
			Vector3 localScale = componentInChildren.transform.localScale;
			localScale.x *= 0.25f;
			componentInChildren.transform.localScale = localScale;
		}
		else if ((SceneLoader.Instance != null && SceneLoader.Instance.Current != null && SceneLoader.Instance.Current.SceneName.Equals("Hover28")) || Application.loadedLevelName.Equals("Hover28"))
		{
			gameObject.GetComponentInChildren<GameLine>().CPlineAnimator.transform.localScale = new Vector3(0.37f, 0.37f, 0.37f);
			gameObject.GetComponentInChildren<GameLine>().CPlineAnimator.transform.localPosition = new Vector3(80f, -170f, 50f);
		}
		moveTransform = gameObject.transform;
		setPositionTimer = 1f;
	}

	private void Update()
	{
		if (!(setPositionTimer > 0f))
		{
			return;
		}
		setPositionTimer -= Time.deltaTime;
		if (setPositionTimer <= 0f)
		{
			moveTransform.position = new Vector3(moveTransform.position.x, moveTransform.position.y + 1f, moveTransform.position.z);
			if (shadow != null)
			{
				shadow.ShadowPosition();
			}
		}
	}
}
