using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

public class ThrowCoins : MonoBehaviour
{
	private const int MAX_COINS = 10;

	private const float RANDOM_POSITION_OFFSET_X = 2.5f;

	private const float RANDOM_POSITION_OFFSET_Z = 3f;

	public GameObject coin1;

	private Stack<GameObject> coins;

	private HoverController hover;

	private Vector3 endPos;

	private void Start()
	{
		hover = Object.FindObjectOfType<HoverController>();
		coins = new Stack<GameObject>(10);
		CreateCoins();
	}

	private void LateUpdate()
	{
		if (GameController.Instance.State == GameController.GameState.Running)
		{
			endPos = hover.transform.position;
		}
	}

	private void CreateCoins()
	{
		for (int i = coins.Count; i < 10; i++)
		{
			GameObject gameObject = Object.Instantiate(coin1);
			gameObject.transform.parent = hover.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localScale = coin1.transform.localScale;
			Collectable component = gameObject.GetComponent<Collectable>();
			component.collectableShadow.GetComponent<Renderer>().enabled = true;
			component.StopUpdate = true;
			component.DroppedOnCrash = true;
			gameObject.SetActive(false);
			coins.Push(gameObject);
		}
	}

	public int Throw(bool debug = false)
	{
		int num = ((LevelStats.Instance.CollectebleCollectCount <= 10) ? LevelStats.Instance.CollectebleCollectCount : 10);
		if (debug)
		{
			num = 1;
		}
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = coins.Pop();
			gameObject.transform.parent = GameController.Instance.transform;
			Vector3 vector = gameObject.transform.position + (endPos - gameObject.transform.position).normalized * 10f;
			vector.x += hover.transform.right.x * Random.Range(-2.5f, 2.5f);
			vector.y += hover.transform.up.y * 1.1f;
			vector.z += Random.Range(-3f, 3f);
			HOTween.To(gameObject.transform, 0.45f, new TweenParms().Prop("position", vector).UpdateType(UpdateType.TimeScaleIndependentUpdate).Ease(EaseType.EaseOutBack)
				.OnComplete(EnableCoin, gameObject));
			gameObject.SetActive(true);
		}
		CreateCoins();
		return num;
	}

	private void EnableCoin(TweenEvent e)
	{
		(e.parms[0] as GameObject).GetComponent<Collectable>().StopUpdate = false;
	}
}
