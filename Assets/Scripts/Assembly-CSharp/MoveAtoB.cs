using UnityEngine;

public class MoveAtoB : ObjectActivator
{
	public GameObject BpositionObject;

	public float AnimationSpeed = 1f;

	private Vector3 Bposition;

	private float animationLerp;

	public bool Shake;

	private Vector3 move;

	private Vector3 startPos;

	private bool ShakeStartValue;

	public override void Start()
	{
		Bposition = BpositionObject.transform.position;
		startPos = base.transform.localPosition;
		ShakeStartValue = Shake;
		base.Start();
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		if (base.IsActive && animationLerp < 1f)
		{
			if (Shake && animationLerp < 0.5f)
			{
				if (childObject.gameObject != null)
				{
					childObject.SetActive(false);
				}
				move.x = Mathf.PingPong(Time.time * 100f * animationLerp, 0.01f + animationLerp);
				base.transform.localPosition = startPos + move;
				animationLerp += Time.fixedDeltaTime;
				if (animationLerp >= 0.4f)
				{
					Shake = false;
					animationLerp = 0f;
					if (childObject.gameObject != null)
					{
						childObject.SetActive(true);
					}
				}
			}
			else
			{
				animationLerp += Time.fixedDeltaTime * AnimationSpeed;
				base.transform.position = Vector3.Lerp(base.transform.position, Bposition, animationLerp);
			}
		}
		else if (!base.IsActive)
		{
			animationLerp = 0f;
			Shake = ShakeStartValue;
		}
	}
}
