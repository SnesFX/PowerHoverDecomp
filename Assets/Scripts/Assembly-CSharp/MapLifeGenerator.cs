using System;
using UnityEngine;

public class MapLifeGenerator : MonoBehaviour
{
	private const string PREFIX_TIMER = "LifeTimer";

	private float GenerateTimeInMillis = 180000f;

	public GameObject HeartItem;

	public AudioSource HeartGeneratedSound;

	public AudioSource HeartActivateSound;

	public Animator HeartDoor;

	public Animator HeartAnimator;

	private string prefix;

	private long storeTime;

	private float timer;

	private bool checkEnabled;

	private bool started;

	private MapLifeAdButton mlab;

	private Camera roadCam;

	private float doubleTap;

	private void Awake()
	{
		timer = GenerateTimeInMillis / 1000f;
		prefix = string.Format("{0}{1}", "LifeTimer", base.transform.position.ToString());
		roadCam = GetComponentInParent<MapObject>().roadCamera;
		mlab = GetComponentInChildren<MapLifeAdButton>();
		if (mlab != null)
		{
			mlab.ShowAdButton(false);
		}
	}

	private void Start()
	{
		started = true;
		CalculateTime(false);
	}

	private void OnApplicationPause(bool paused)
	{
		if (paused)
		{
			SaveTime();
		}
		else
		{
			CalculateTime(true);
		}
	}

	private void OnDestroy()
	{
		SaveTime();
	}

	private void OnEnable()
	{
		CalculateTime(false);
	}

	private void FixedUpdate()
	{
		if (!checkEnabled || !(timer > 0f))
		{
			return;
		}
		timer -= Time.fixedDeltaTime;
		if (timer < 0f)
		{
			CalculateTime(true);
		}
		if (!(doubleTap > 0f))
		{
			return;
		}
		if (roadCam != null && Input.GetMouseButtonDown(0))
		{
			RaycastHit hitInfo;
			if (Physics.Raycast(roadCam.ScreenPointToRay(Input.mousePosition), out hitInfo))
			{
				if (!hitInfo.collider.CompareTag("ExtraLife"))
				{
					doubleTap = 0f;
				}
			}
			else
			{
				doubleTap = 0f;
			}
		}
		doubleTap -= Time.fixedDeltaTime;
		if (doubleTap <= 0f)
		{
			mlab.MakeItSmall();
		}
	}

	public void EnableLifeGenerator()
	{
		checkEnabled = true;
		CalculateTime(false);
	}

	public bool CollectLife()
	{
		if (mlab != null && mlab.gameObject.activeSelf)
		{
			if (doubleTap > 0f)
			{
				mlab.AdForLife();
			}
			else
			{
				mlab.MakeItBig();
				doubleTap = 4f;
			}
			return false;
		}
		GetComponentInParent<MapObject>().MakeUnlockEffect();
		GetComponentInParent<MapObject>().EnableTip(true);
		timer = GenerateTimeInMillis / 1000f;
		storeTime = Convert.ToInt64(DateTime.Now.ToBinary().ToString());
		HeartActivateSound.Play();
		HeartItem.SetActive(false);
		HeartDoor.Play("HeartDoorIdle");
		GameStats.Instance.RegenLifesCollected++;
		PlayerStats.Instance.UpdateStat(PlayerStatType.Lives);
		SaveTime();
		return true;
	}

	private void SaveTime()
	{
		if (checkEnabled)
		{
			PlayerPrefs.SetString(prefix, DateTime.FromBinary(storeTime).ToBinary().ToString());
			PlayerPrefs.Save();
		}
	}

	private void CalculateTime(bool makeASound)
	{
		if (!checkEnabled || !started)
		{
			return;
		}
		long dateData = Convert.ToInt64(PlayerPrefs.GetString(prefix, DateTime.Now.ToBinary().ToString()));
		DateTime dateTime = DateTime.FromBinary(dateData);
		DateTime now = DateTime.Now;
		bool flag = false;
		if ((now - dateTime).TotalMilliseconds > (double)GenerateTimeInMillis)
		{
			flag = true;
			dateTime = now;
		}
		storeTime = Convert.ToInt64(dateTime.ToBinary().ToString());
		double totalSeconds = (now - dateTime).TotalSeconds;
		timer = GenerateTimeInMillis / 1000f - Convert.ToSingle(totalSeconds);
		if ((!UnityAdsIngetration.Instance.IsAdsActivated || !(mlab != null)) && !flag)
		{
			return;
		}
		if (LifeController.Instance.LifeCount < 10 && !HeartItem.activeSelf)
		{
			if (HeartDoor.enabled)
			{
				HeartDoor.Play("OpenHeartDoor");
			}
			HeartItem.SetActive(true);
			if (UnityAdsIngetration.Instance.IsAdsActivated && mlab != null)
			{
				HeartItem.GetComponent<Collider>().enabled = false;
				mlab.ShowAdButton(true);
				GetComponentInParent<MapObject>().EnableTip(false);
			}
			else
			{
				GetComponentInParent<MapObject>().EnableTip(true);
				HeartItem.GetComponent<Collider>().enabled = true;
			}
			if (makeASound)
			{
				HeartGeneratedSound.Play();
			}
		}
		SaveTime();
	}
}
