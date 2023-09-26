using UnityEngine;

public class ElevatorTrigger : MonoBehaviour
{
	private const string textureName = "_MainTex";

	public Vector2 uvAnimationRate = new Vector2(1f, 0f);

	public float force;

	private Vector2 uvOffset = Vector2.zero;

	private void LateUpdate()
	{
		uvOffset += uvAnimationRate * Time.deltaTime;
		if (GetComponent<Renderer>().enabled)
		{
			GetComponent<Renderer>().material.SetTextureOffset("_MainTex", uvOffset);
		}
	}
}
