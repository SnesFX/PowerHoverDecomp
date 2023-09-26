using UnityEngine;

public class FlyingAirplane : ObjectActivator
{
	public float speed = 40f;

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		if (base.IsActive)
		{
			base.transform.Translate(Vector3.forward * Time.fixedDeltaTime * speed);
		}
	}
}
