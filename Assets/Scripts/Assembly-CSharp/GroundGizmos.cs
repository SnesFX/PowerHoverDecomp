using UnityEngine;

public class GroundGizmos : MonoBehaviour
{
	public virtual Color GizmoColor
	{
		get
		{
			return new Color(0f, 0f, 1f, 0.25f);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = GizmoColor;
		if (GetComponent<BoxCollider>() != null)
		{
			BoxCollider component = GetComponent<BoxCollider>();
			if (component.enabled)
			{
				Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, base.transform.lossyScale);
				Gizmos.DrawCube(component.center, component.size);
			}
		}
		else if (GetComponent<MeshCollider>() != null)
		{
			MeshCollider component2 = GetComponent<MeshCollider>();
			if (component2.enabled)
			{
				Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, base.transform.localScale);
				Gizmos.DrawMesh(component2.sharedMesh);
			}
		}
		else
		{
			if (!(GetComponent<CapsuleCollider>() != null))
			{
				return;
			}
			CapsuleCollider component3 = GetComponent<CapsuleCollider>();
			if (!component3.enabled)
			{
				return;
			}
			Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one);
			float num = ((component3.direction == 1) ? base.transform.lossyScale.y : ((component3.direction != 0) ? base.transform.lossyScale.z : base.transform.lossyScale.x)) * component3.height;
			float num2 = ((!(base.transform.lossyScale.x > base.transform.lossyScale.z)) ? base.transform.lossyScale.z : base.transform.lossyScale.x) * component3.radius;
			if (Mathf.CeilToInt(num / num2) > 2)
			{
				for (int i = 1; i < Mathf.CeilToInt(num / num2); i++)
				{
					Vector3 center = Vector3.Scale(base.transform.lossyScale, component3.center);
					if (component3.direction == 1)
					{
						center.y += num * 0.5f;
						center.y -= (float)i * num2;
					}
					else if (component3.direction == 0)
					{
						center.x += num * 0.5f;
						center.x -= (float)i * num2;
					}
					else if (component3.direction == 2)
					{
						center.z += num * 0.5f;
						center.z -= (float)i * num2;
					}
					Gizmos.DrawSphere(center, num2);
				}
			}
			else
			{
				Gizmos.DrawSphere(Vector3.Scale(base.transform.lossyScale, component3.center), num2);
			}
		}
	}
}
