public class SplineCarActivator : ObjectActivator
{
	private CarFollowSpline[] cars;

	private bool activateChanged;

	public override void Start()
	{
		base.Start();
		cars = base.transform.GetComponentsInChildren<CarFollowSpline>();
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		if (base.IsActive && !activateChanged && cars != null)
		{
			activateChanged = true;
			for (int i = 0; i < cars.Length; i++)
			{
				cars[i].IsActive = true;
			}
		}
		else if (!base.IsActive && activateChanged && cars != null)
		{
			activateChanged = false;
			for (int j = 0; j < cars.Length; j++)
			{
				cars[j].IsActive = false;
			}
		}
	}

	public override void Reset(bool isRewind)
	{
		base.Reset(isRewind);
		if (cars != null)
		{
			for (int i = 0; i < cars.Length; i++)
			{
				cars[i].Reset();
			}
		}
	}
}
