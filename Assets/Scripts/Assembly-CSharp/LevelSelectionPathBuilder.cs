using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectionPathBuilder : MonoBehaviour
{
	private const int MAXCLONES = 2000;

	public SceneLoader Scenes;

	public int group;

	public CurvySplineBase Spline;

	public GameObject[] Source = new GameObject[0];

	public float Gap;

	public float StartOffset;

	private bool AutoRefresh = true;

	private Transform mTransform;

	private float mLastRefresh;

	private bool lastOpenLevelFound;

	private bool doitOnce;

	private bool RefreshDone;

	public CurvySpline TargetSpline
	{
		get
		{
			return Spline as CurvySpline;
		}
	}

	public CurvySplineGroup TargetSplineGroup
	{
		get
		{
			return Spline as CurvySplineGroup;
		}
	}

	public int ObjectCount
	{
		get
		{
			return mTransform.childCount;
		}
	}

	public SceneDetails lastOpenLevel { get; set; }

	public Vector3 lastOpenLevelPosition { get; private set; }

	private void OnEnable()
	{
		mTransform = base.transform;
	}

	private void OnDisable()
	{
		if ((bool)Spline)
		{
			Spline.OnRefresh -= OnSplineRefresh;
		}
	}

	private void OnDestroy()
	{
		if (Application.isPlaying)
		{
			Clear();
		}
	}

	private void Update()
	{
		if ((bool)Spline && Spline.IsInitialized)
		{
			Spline.OnRefresh -= OnSplineRefresh;
			if (AutoRefresh)
			{
				Spline.OnRefresh += OnSplineRefresh;
			}
			if (Application.isPlaying && !doitOnce)
			{
				doitOnce = true;
				Refresh(true);
			}
		}
	}

	public void Refresh(bool force)
	{
		if (Spline == null || !Spline.IsInitialized)
		{
			return;
		}
		if (Spline.Length == 0f)
		{
			Spline.Refresh(true, false, false);
		}
		checkSources();
		if (Source.Length == 0)
		{
			Clear();
			return;
		}
		float total;
		float[] sourceDepths = getSourceDepths(out total);
		int index = 0;
		if (force)
		{
			Clear();
		}
		else
		{
			Clear(index);
		}
		int num = 0;
		float num2 = StartOffset;
		int num3 = -1;
		int objectCount = ObjectCount;
		Transform transform = base.transform;
		bool lastBossSet = false;
		string text = ((!GameDataController.Exists("LastScene")) ? string.Empty : GameDataController.Load<string>("LastScene"));
		string text2 = ((!text.Equals("Tutorial")) ? text : string.Empty);
		List<SceneDetails> list = Scenes.Scenes.FindAll((SceneDetails x) => x.Group == group);
		foreach (SceneDetails item in list)
		{
			if (!item.Storage.IsOpen)
			{
				lastOpenLevelFound = true;
			}
			num = (item.IsEndless ? 1 : 0);
			num3++;
			float tf = Spline.DistanceToTF(num2 + sourceDepths[num] / 2f);
			if (num3 < objectCount)
			{
				Transform child = mTransform.GetChild(num3);
				child.position = Spline.Interpolate(tf);
				child.rotation = Spline.GetOrientationFast(tf) * Source[num].transform.rotation;
				if (Application.isPlaying)
				{
					child.GetComponentInChildren<MapLevelSelector>().SetState(item, num3 + 1, child.position);
					if (CheckEndlessLimit(lastBossSet, item))
					{
						lastBossSet = true;
						base.transform.GetComponentInParent<Map>().cameraSpline.SetScrollLimit(child.position);
					}
				}
				if (text2.Equals(item.SceneName))
				{
					lastOpenLevelPosition = child.position;
					lastOpenLevel = item;
				}
				else if (text2.Equals(string.Empty) && !lastOpenLevelFound)
				{
					lastOpenLevelPosition = child.position;
					lastOpenLevel = item;
				}
			}
			else
			{
				GameObject gameObject = CloneObject(Source[num]);
				if ((bool)gameObject)
				{
					Transform transform2 = gameObject.transform;
					transform2.parent = base.transform;
					gameObject.name = string.Format("{0:000}{1}", num3, item.VisibleName);
					transform2.position = Spline.Interpolate(tf);
					transform2.rotation = Spline.GetOrientationFast(tf) * Source[num].transform.rotation;
					if (num3 > 0 && num3 < Scenes.Scenes.Count)
					{
						transform.GetComponentInChildren<MapLineConnector>().SetLinePosition(transform2.GetComponentInChildren<MapLineConnector>().transform.position);
					}
					if (num3 == Scenes.Scenes.Count - 1)
					{
						transform2.GetComponentInChildren<MapLineConnector>().gameObject.SetActive(false);
					}
					transform = transform2;
					if (Application.isPlaying)
					{
						gameObject.GetComponentInChildren<MapLevelSelector>().SetState(item, num3 + 1, transform2.position);
						if (CheckEndlessLimit(lastBossSet, item))
						{
							lastBossSet = true;
							base.transform.GetComponentInParent<Map>().cameraSpline.SetScrollLimit(transform2.position);
						}
					}
					if (text2.Equals(item.SceneName))
					{
						lastOpenLevelPosition = transform2.position;
						lastOpenLevel = item;
					}
					else if (text2.Equals(string.Empty) && !lastOpenLevelFound)
					{
						lastOpenLevelPosition = transform2.position;
						lastOpenLevel = item;
					}
				}
			}
			num2 += sourceDepths[num] + Gap;
		}
		RefreshDone = true;
	}

	private bool CheckEndlessLimit(bool lastBossSet, SceneDetails sd)
	{
		bool flag = false;
		SceneDetails nextLevel = SceneLoader.Instance.GetNextLevel(sd.SceneName, group);
		if (nextLevel != null && nextLevel.Storage.IsOpen)
		{
			flag = true;
		}
		if (!lastBossSet && !flag && sd.IsEndless && ((sd.SceneName.Equals("Endless9") && sd.Storage.BestDistance < 1000f) || (!sd.SceneName.Equals("Endless9") && sd.Storage.BestDistance < 1500f)))
		{
			return true;
		}
		return false;
	}

	public void SetLastLevelOpened(MapLevelSelector mls)
	{
		lastOpenLevelPosition = mls.positionOnMap;
		lastOpenLevel = mls.sceneDetails;
	}

	public MapLevelSelector GetLevelSelector(string sceneName)
	{
		MapLevelSelector[] componentsInChildren = base.transform.GetComponentsInChildren<MapLevelSelector>();
		foreach (MapLevelSelector mapLevelSelector in componentsInChildren)
		{
			if (mapLevelSelector.sceneDetails.SceneName.Equals(sceneName))
			{
				return mapLevelSelector;
			}
		}
		return null;
	}

	public bool UnlockLevel(SceneDetails unlock)
	{
		if (!RefreshDone)
		{
			Debug.Log("Unlocking before path created! exit ");
			return false;
		}
		MapLevelSelector[] componentsInChildren = base.transform.GetComponentsInChildren<MapLevelSelector>();
		foreach (MapLevelSelector mapLevelSelector in componentsInChildren)
		{
			if (mapLevelSelector.sceneDetails.SceneName.Equals(unlock.SceneName))
			{
				mapLevelSelector.Unlock();
				lastOpenLevelPosition = mapLevelSelector.positionOnMap;
				return true;
			}
		}
		return false;
	}

	public void UpdateBossLimits()
	{
		if (!lastOpenLevelFound)
		{
			return;
		}
		bool flag = false;
		MapLevelSelector[] componentsInChildren = base.transform.GetComponentsInChildren<MapLevelSelector>();
		foreach (MapLevelSelector mapLevelSelector in componentsInChildren)
		{
			if (mapLevelSelector.sceneDetails.IsEndless && ((mapLevelSelector.sceneDetails.SceneName.Equals("Endless9") && mapLevelSelector.sceneDetails.Storage.BestDistance < 1000f) || (!mapLevelSelector.sceneDetails.SceneName.Equals("Endless9") && mapLevelSelector.sceneDetails.Storage.BestDistance < 1500f)))
			{
				base.transform.GetComponentInParent<Map>().cameraSpline.SetScrollLimit(mapLevelSelector.positionOnMap);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			base.transform.GetComponentInParent<Map>().cameraSpline.ClearScrollLimit();
		}
	}

	public void UpdateLevels()
	{
		if (doitOnce)
		{
			MapLevelSelector[] componentsInChildren = base.transform.GetComponentsInChildren<MapLevelSelector>();
			foreach (MapLevelSelector mapLevelSelector in componentsInChildren)
			{
				mapLevelSelector.UpdateState();
			}
		}
	}

	public void Clear()
	{
		Transform[] componentsInChildren = base.transform.GetComponentsInChildren<Transform>();
		for (int num = componentsInChildren.Length - 1; num > 0; num--)
		{
			Object.DestroyImmediate(componentsInChildren[num].gameObject);
		}
	}

	public void Clear(int index)
	{
		int childCount = mTransform.childCount;
		for (int num = childCount - 1; num >= index; num--)
		{
			Object.DestroyImmediate(mTransform.GetChild(num).gameObject);
		}
	}

	private GameObject CloneObject(GameObject source)
	{
		return (!(source != null)) ? null : Object.Instantiate(source);
	}

	private void DestroyObject(GameObject obj)
	{
		if (Application.isPlaying)
		{
			Object.Destroy(obj);
		}
		else
		{
			Object.DestroyImmediate(obj);
		}
	}

	private void checkSources()
	{
		ArrayList arrayList = new ArrayList();
		for (int i = 0; i < Source.Length; i++)
		{
			if (Source[i] != null)
			{
				arrayList.Add(Source[i]);
			}
		}
		Source = ((arrayList.Count != 0) ? ((GameObject[])arrayList.ToArray(typeof(GameObject))) : new GameObject[0]);
	}

	private float getDepth(GameObject o)
	{
		if (!o)
		{
			return 0f;
		}
		Bounds bounds = new Bounds(o.transform.position, Vector3.zero);
		Collider[] componentsInChildren = o.GetComponentsInChildren<Collider>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			bounds.Encapsulate(componentsInChildren[i].bounds);
		}
		return bounds.size.z;
	}

	private float[] getSourceDepths(out float total)
	{
		float[] array = new float[Source.Length];
		total = 0f;
		for (int i = 0; i < Source.Length; i++)
		{
			array[i] = getDepth(Source[i]);
			total += array[i];
		}
		total += (float)array.Length * Gap;
		return array;
	}

	private void OnSplineRefresh(CurvySplineBase sender)
	{
		if (Time.realtimeSinceStartup - mLastRefresh > 0.01f)
		{
			mLastRefresh = Time.realtimeSinceStartup;
			Refresh(false);
		}
	}
}
