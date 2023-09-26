using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MiniJSON;
using UnityEngine;
using UnityEngine.Networking;

public class CrossPromo : MonoBehaviour
{
	[Serializable]
	public class PromoDetails
	{
		public string Id;

		public string StoreURL;

		public Texture2D Image;
	}

	private const bool DEBUG = false;

	private const string PromoJsonFileName = "promos";

	private const string PromoUrl = "http://www.oddrok.com/crosspromo_small/";

	private const string PromoJsonFileEnd = ".json";

	private const string LastModified = "Last-Modified";

	private const string CrossPromoStorage = "CrossPrmos";

	private const string PromoOpenedStorage = "CRSS_Open";

	private List<PromoDetails> PromoList;

	private int downloading = -1;

	private PromoDetails current;

	private float loadAfterStartTimer;

	public static CrossPromo Instance { get; private set; }

	public bool IsReady { get; private set; }

	private void Start()
	{
		Instance = this;
		IsReady = false;
		if (!GameDataController.Exists("CrossPrmos"))
		{
			TextAsset textAsset = Resources.Load("promos") as TextAsset;
			if (textAsset != null && textAsset.text.Length > 0)
			{
				GameDataController.Save(textAsset.text, "CrossPrmos");
			}
		}
		if (Application.internetReachability != 0)
		{
			loadAfterStartTimer = 2f;
		}
	}

	private void LateUpdate()
	{
		if (loadAfterStartTimer > 0f)
		{
			loadAfterStartTimer -= Time.deltaTime;
			if (loadAfterStartTimer <= 0f)
			{
				try
				{
					StartCoroutine(CheckCrossPromoConfigs());
				}
				catch (Exception)
				{
					downloading = -1;
					IsReady = false;
				}
			}
		}
		else
		{
			if (IsReady || downloading != 0)
			{
				return;
			}
			IsReady = true;
			List<PromoDetails> crossPromos = GetCrossPromos();
			if (crossPromos != null && crossPromos.Count > 0)
			{
				CrossPromoButton crossPromoButton = UnityEngine.Object.FindObjectOfType<CrossPromoButton>();
				if (crossPromoButton != null)
				{
					crossPromoButton.ShowWhenLoaded();
				}
			}
		}
	}

	private IEnumerator CheckCrossPromoConfigs()
	{
		string loadingUrl = string.Format("{0}{1}{2}{3}", "http://www.oddrok.com/crosspromo_small/", "android/", "promos", ".json");
		UnityWebRequest ww = new UnityWebRequest(loadingUrl, "HEAD");
		yield return ww.Send();
		if (!ww.isNetworkError && ww.isDone && ww.GetResponseHeaders() != null && ww.GetResponseHeaders().Count > 0)
		{
			string storage = string.Format("{0}{1}", "Last-Modified", "promos");
			if (!GameDataController.Exists(storage) || !GameDataController.Load<string>(storage).Equals(ww.GetResponseHeader("Last-Modified")))
			{
				WWW www = new WWW(loadingUrl);
				yield return www;
				if (www.error == null || www.error.Length <= 0)
				{
					string @string = Encoding.UTF8.GetString(www.bytes);
					GameDataController.Save(@string, "CrossPrmos");
					GameDataController.Save(ww.GetResponseHeader("Last-Modified"), storage);
				}
			}
		}
		string json = GameDataController.Load<string>("CrossPrmos");
		if (json.Length > 0)
		{
			Dictionary<string, object> dictionary = Json.Deserialize(json) as Dictionary<string, object>;
			List<object> list = (List<object>)dictionary["promos"];
			if (list != null && list.Count > 0)
			{
				PromoList = new List<PromoDetails>();
				foreach (Dictionary<string, object> item in list)
				{
					if (Application.identifier.Equals((string)item["id"]))
					{
						continue;
					}
					if (item.ContainsKey("enabled") && !Convert.ToBoolean(item["enabled"]))
					{
						Debug.Log("app " + (string)item["id"] + " set disabled, skipping");
						continue;
					}
					string text = (string)item["id"];
					PromoDetails promoDetails = new PromoDetails();
					promoDetails.Id = text;
					promoDetails.StoreURL = (string)item["storeurl"];
					if (ES2.Exists(Application.persistentDataPath + "/" + text + "_sml.png"))
					{
						promoDetails.Image = ES2.LoadImage(Application.persistentDataPath + "/" + text + "_sml.png");
					}
					else
					{
						downloading = ((downloading == -1) ? 1 : (downloading + 1));
						StartCoroutine(LoadImage((string)item["img"], text));
					}
					PromoList.Add(promoDetails);
				}
			}
			else
			{
				downloading = -2;
				IsReady = false;
			}
		}
		if (downloading == -1)
		{
			downloading = 0;
		}
	}

	private IEnumerator LoadImage(string loadedURL, string id)
	{
		WWW www = new WWW(loadedURL);
		yield return www;
		ES2.SaveImage(www.texture, Application.persistentDataPath + "/" + id + "_sml.png");
		for (int i = 0; i < PromoList.Count; i++)
		{
			if (PromoList[i].Id.Equals(id))
			{
				PromoList[i].Image = www.texture;
				break;
			}
		}
		downloading--;
	}

	public void OpenPromoFromButton(PromoDetails details)
	{
		Application.OpenURL(details.StoreURL);
		GameDataController.Save(true, "CRSS_Open" + details.Id);
		UnityAnalyticsIntegration.Instance.MakeEvent("CrossPromoNewOpen", new Dictionary<string, object> { { "PromoURL", details.StoreURL } });
	}

	public List<PromoDetails> GetCrossPromos()
	{
		if (PromoList != null && PromoList.Count > 0)
		{
			List<PromoDetails> list = new List<PromoDetails>();
			for (int i = 0; i < PromoList.Count; i++)
			{
				if (!GameDataController.Exists("CRSS_Open" + PromoList[i].Id))
				{
					list.Add(PromoList[i]);
				}
			}
			return list;
		}
		return null;
	}
}
