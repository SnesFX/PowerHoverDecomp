using UnityEngine;

namespace FluffyUnderware.Curvy.Examples
{
	public class CameraControl : MonoBehaviour
	{
		public Transform Character;

		public float Distance = 10f;

		public float Height = 2f;

		private Transform mTransform;

		private void Start()
		{
			mTransform = base.transform;
		}

		private void Update()
		{
			Vector3 vector = new Vector3(0f, Character.position.y, 0f);
			Vector3 position = Character.position;
			Vector3 vector2 = new Ray(vector, position - vector).GetPoint((position - vector).magnitude + Distance) + new Vector3(0f, Height, 0f);
			mTransform.position = new Vector3(Mathf.Lerp(mTransform.position.x, vector2.x, 0.08f), Mathf.Lerp(mTransform.position.y, vector2.y, 0.01f), Mathf.Lerp(mTransform.position.z, vector2.z, 0.08f));
			mTransform.LookAt(vector);
		}
	}
}
