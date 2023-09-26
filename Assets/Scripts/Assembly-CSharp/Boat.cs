using UnityEngine;

public class Boat : ObjectActivator
{
	private float speed;

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		if (base.IsActive)
		{
			speed = Mathf.Min(5f, speed + Time.fixedDeltaTime * 3f);
			base.transform.Translate(Vector3.forward * Time.fixedDeltaTime * speed);
		}
		else if (speed > 0f)
		{
			speed = Mathf.Max(0f, speed - Time.fixedDeltaTime * 3f);
			base.transform.Translate(Vector3.forward * Time.fixedDeltaTime * speed);
		}
	}
}
