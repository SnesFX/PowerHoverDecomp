using UnityEngine;
using UnityEngine.UI;

public class JumpLoader : MonoBehaviour
{
	private const float MaxLoadingTime = 1f;

	public Image JumpIndicator;

	private bool jumpEnabled;

	private float loadingTimer;

	public int JumpPower { get; set; }

	public int Jump()
	{
		JumpPower = 0;
		if (jumpEnabled && loadingTimer > 0f)
		{
			jumpEnabled = false;
			loadingTimer = 0f;
			if (JumpIndicator.fillAmount < 0.366f)
			{
				JumpPower = 0;
			}
			else if (JumpIndicator.fillAmount < 0.6f)
			{
				JumpPower = 1;
			}
			else if (JumpIndicator.fillAmount < 0.738f)
			{
				JumpPower = 2;
			}
			else if (JumpIndicator.fillAmount > 0.738f)
			{
				JumpPower = 2;
			}
		}
		return JumpPower;
	}

	private void Start()
	{
		jumpEnabled = true;
		JumpIndicator.fillAmount = 0f;
	}

	private void FixedUpdate()
	{
		if (jumpEnabled && UIController.Instance.leftPressed && UIController.Instance.rightPressed)
		{
			loadingTimer += Time.deltaTime;
			JumpIndicator.fillAmount = loadingTimer / 1f;
		}
		if (!UIController.Instance.leftPressed || !UIController.Instance.rightPressed)
		{
			jumpEnabled = true;
			JumpIndicator.fillAmount = 0f;
			loadingTimer = 0f;
		}
	}
}
