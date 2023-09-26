using UnityEngine;

public class SplineWormActivator : ObjectActivator
{
	public WormFollowSpline headPart;

	public WormFollowSpline tailPart;

	public WormFollowSpline middlePart;

	public int middlePartCount;

	public int tailEndPart = 5;

	public GameObject GroundParticle1;

	private WormFollowSpline[] wormParts;

	private bool activateChanged;

	public override void Start()
	{
		base.Start();
		wormParts = new WormFollowSpline[1 + middlePartCount];
		wormParts[0] = headPart;
		float num = (headPart.Initial.Position - tailPart.Initial.Position) / (float)middlePartCount;
		float num2 = 0f;
		for (int num3 = middlePartCount; num3 > 0; num3--)
		{
			GameObject gameObject = Object.Instantiate(middlePart.gameObject, middlePart.transform.position, middlePart.transform.rotation);
			gameObject.transform.parent = middlePart.transform.parent;
			gameObject.name = string.Format("{0}_{1}", gameObject.name, num3);
			if (num3 < tailEndPart + 1)
			{
				float num4 = 1f / (float)(tailEndPart + 1) * (float)num3;
				num2 += num * (1f - num4) * 0.75f;
				gameObject.transform.localScale *= num4;
			}
			gameObject.GetComponentInChildren<ContinuosRotation>().rotationVector = new Vector3(0f, 0f, 1f + (float)num3 * 0.1f);
			WormFollowSpline component = gameObject.GetComponent<WormFollowSpline>();
			component.Initial.Position = tailPart.Initial.Position + (float)(num3 - 1) * num + num2;
			component.IsTail = num3 == 1;
			wormParts[num3] = component;
		}
		middlePart.Spline.enabled = false;
		middlePart.gameObject.SetActive(false);
		tailPart.gameObject.SetActive(false);
		activateChanged = true;
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		if (base.IsActive && !activateChanged && wormParts != null)
		{
			activateChanged = true;
			for (int i = 0; i < wormParts.Length; i++)
			{
				if (!wormParts[i].Spline.enabled)
				{
					wormParts[i].Spline.enabled = true;
				}
				wormParts[i].IsActive = true;
				wormParts[i].gameObject.SetActive(true);
				if (GroundParticle1 != null)
				{
					GroundParticle1.SetActive(true);
				}
			}
		}
		else
		{
			if (base.IsActive || !activateChanged || wormParts == null)
			{
				return;
			}
			activateChanged = false;
			for (int j = 0; j < wormParts.Length; j++)
			{
				middlePart.Spline.enabled = false;
				wormParts[j].IsActive = false;
				wormParts[j].gameObject.SetActive(false);
				if (GroundParticle1 != null)
				{
					GroundParticle1.SetActive(false);
				}
			}
		}
	}

	public override void Reset(bool isRewind)
	{
		base.Reset(isRewind);
		if (wormParts != null)
		{
			for (int i = 0; i < wormParts.Length; i++)
			{
				wormParts[i].Reset();
				wormParts[i].IsActive = false;
				wormParts[i].gameObject.SetActive(false);
				GroundParticle1.SetActive(false);
			}
			middlePart.Spline.enabled = false;
		}
	}
}
