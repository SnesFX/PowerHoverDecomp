using System;
using System.Collections.Generic;
using UnityEngine;

namespace SmartLocalization
{
	public class LanguageManager : MonoBehaviour
	{
		private static LanguageManager instance;

		private static bool IsQuitting;

		private static bool DontDestroyOnLoadToggle;

		private static bool DidSetDontDestroyOnLoad;

		public ChangeLanguageEventHandler OnChangeLanguage;

		public string defaultLanguage = "en";

		private string loadedLanguage = "en";

		private SmartCultureInfo currentlyLoadedCulture;

		private SmartCultureInfoCollection availableLanguages;

		private SortedDictionary<string, LocalizedObject> languageDatabase;

		private bool verboseLogging;

		public static LanguageManager Instance
		{
			get
			{
				if (instance == null && !IsQuitting)
				{
					GameObject gameObject = new GameObject();
					instance = gameObject.AddComponent<LanguageManager>();
					gameObject.name = "LanguageManager";
					if (!DidSetDontDestroyOnLoad && DontDestroyOnLoadToggle)
					{
						UnityEngine.Object.DontDestroyOnLoad(instance);
						DidSetDontDestroyOnLoad = true;
					}
				}
				return instance;
			}
		}

		public static bool HasInstance
		{
			get
			{
				return instance != null;
			}
		}

		public SortedDictionary<string, LocalizedObject> LanguageDatabase
		{
			get
			{
				return languageDatabase;
			}
		}

		public Dictionary<string, string> RawTextDatabase
		{
			get
			{
				if (languageDatabase == null)
				{
					return null;
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				foreach (KeyValuePair<string, LocalizedObject> item in languageDatabase)
				{
					dictionary.Add(item.Key, item.Value.TextValue);
				}
				return dictionary;
			}
		}

		public int NumberOfSupportedLanguages
		{
			get
			{
				if (availableLanguages == null)
				{
					return 0;
				}
				return availableLanguages.cultureInfos.Count;
			}
		}

		public string LoadedLanguage
		{
			get
			{
				return loadedLanguage;
			}
		}

		public SmartCultureInfo CurrentlyLoadedCulture
		{
			get
			{
				return currentlyLoadedCulture;
			}
		}

		public bool VerboseLogging
		{
			get
			{
				return verboseLogging;
			}
			set
			{
				verboseLogging = value;
			}
		}

		public static void SetDontDestroyOnLoad()
		{
			DontDestroyOnLoadToggle = true;
			if (instance != null && !DidSetDontDestroyOnLoad)
			{
				UnityEngine.Object.DontDestroyOnLoad(instance);
				DidSetDontDestroyOnLoad = true;
			}
		}

		private void Awake()
		{
			SetDontDestroyOnLoad();
			if (instance == null)
			{
				instance = this;
			}
			else if (instance != this)
			{
				if (VerboseLogging)
				{
					Debug.LogError("Found duplicate LanguageManagers! Removing one of them");
				}
				UnityEngine.Object.Destroy(this);
				return;
			}
			if (LoadAvailableLanguages())
			{
				if (VerboseLogging)
				{
					Debug.Log("LanguageManager.cs: Waking up");
				}
				if (availableLanguages.cultureInfos.Count > 0)
				{
					SmartCultureInfo smartCultureInfo = availableLanguages.cultureInfos.Find((SmartCultureInfo info) => info.languageCode == defaultLanguage);
					if (smartCultureInfo != null)
					{
						ChangeLanguage(smartCultureInfo);
						return;
					}
					ChangeLanguage(availableLanguages.cultureInfos[0]);
					defaultLanguage = availableLanguages.cultureInfos[0].languageCode;
				}
				else
				{
					Debug.LogError("LanguageManager.cs: No language is available! Use Window->Smart Localization tool to create a language");
				}
			}
			else
			{
				Debug.LogError("LanguageManager.cs: No localization workspace is created! Use Window->Smart Localization tool to create one");
			}
		}

		private void OnDestroy()
		{
			OnChangeLanguage = null;
		}

		private void OnApplicationQuit()
		{
			IsQuitting = true;
		}

		private bool LoadAvailableLanguages()
		{
			TextAsset textAsset = Resources.Load(LanguageRuntimeData.AvailableCulturesFilePath()) as TextAsset;
			if (textAsset == null)
			{
				Debug.LogError("Could not load available languages! No such file!");
				return false;
			}
			availableLanguages = SmartCultureInfoCollection.Deserialize(textAsset);
			return true;
		}

		public void ChangeLanguage(SmartCultureInfo cultureInfo)
		{
			ChangeLanguage(cultureInfo.languageCode);
		}

		public void ChangeLanguage(string languageCode)
		{
			TextAsset textAsset = Resources.Load(LanguageRuntimeData.LanguageFilePath(languageCode)) as TextAsset;
			if (textAsset == null)
			{
				Debug.LogError("Failed to load language: " + languageCode);
				return;
			}
			LoadLanguage(textAsset.text, languageCode);
			if (OnChangeLanguage != null)
			{
				OnChangeLanguage(this);
			}
		}

		public void ChangeLanguageWithData(string languageDataInResX, string languageCode)
		{
			if (LoadLanguage(languageDataInResX, languageCode) && OnChangeLanguage != null)
			{
				OnChangeLanguage(this);
			}
		}

		public bool AppendLanguageWithTextData(string languageDataInResX)
		{
			if (string.IsNullOrEmpty(languageDataInResX))
			{
				Debug.LogError("Failed to append data to the currently loaded language. Data was null or empty");
				return false;
			}
			if (languageDatabase == null)
			{
				Debug.LogError("Failed to append data to the currently loaded language. There is no language loaded.");
				return false;
			}
			SortedDictionary<string, LocalizedObject> sortedDictionary = LanguageParser.LoadLanguage(languageDataInResX);
			foreach (KeyValuePair<string, LocalizedObject> item in sortedDictionary)
			{
				if (item.Value.ObjectType == LocalizedObjectType.STRING)
				{
					if (languageDatabase.ContainsKey(item.Key))
					{
						languageDatabase[item.Key] = item.Value;
					}
					else
					{
						languageDatabase.Add(item.Key, item.Value);
					}
				}
			}
			if (verboseLogging)
			{
				Debug.Log("Appended or updated language:" + currentlyLoadedCulture.englishName + " with " + sortedDictionary.Count + " values");
			}
			return true;
		}

		private bool LoadLanguage(string languageData, string languageCode)
		{
			if (string.IsNullOrEmpty(languageData))
			{
				Debug.LogError("Failed to load language with ISO-639 code. Data was null or empty");
				return false;
			}
			if (languageDatabase != null)
			{
				languageDatabase.Clear();
				languageDatabase = null;
			}
			languageDatabase = LanguageParser.LoadLanguage(languageData);
			loadedLanguage = languageCode;
			currentlyLoadedCulture = GetCultureInfo(loadedLanguage);
			return true;
		}

		public bool IsLanguageSupported(string languageCode)
		{
			if (availableLanguages == null)
			{
				Debug.LogError("LanguageManager is not initialized properly!");
				return false;
			}
			return availableLanguages.cultureInfos.Find((SmartCultureInfo info) => info.languageCode == languageCode) != null;
		}

		public bool IsLanguageSupportedEnglishName(string englishName)
		{
			if (availableLanguages == null)
			{
				Debug.LogError("LanguageManager is not initialized properly!");
				return false;
			}
			return availableLanguages.cultureInfos.Find((SmartCultureInfo info) => info.englishName.ToLower() == englishName.ToLower()) != null;
		}

		public bool IsLanguageSupported(SmartCultureInfo cultureInfo)
		{
			return IsLanguageSupported(cultureInfo.languageCode);
		}

		public SmartCultureInfo GetCultureInfo(string languageCode)
		{
			return availableLanguages.cultureInfos.Find((SmartCultureInfo info) => info.languageCode == languageCode);
		}

		public string GetSystemLanguageEnglishName()
		{
			return ApplicationExtensions.GetSystemLanguage();
		}

		public SmartCultureInfo GetSupportedSystemLanguage()
		{
			if (availableLanguages == null)
			{
				return null;
			}
			string englishName = GetSystemLanguageEnglishName();
			return availableLanguages.cultureInfos.Find((SmartCultureInfo info) => info.englishName.ToLower() == englishName.ToLower());
		}

		public string GetSupportedSystemLanguageCode()
		{
			SmartCultureInfo supportedSystemLanguage = GetSupportedSystemLanguage();
			if (supportedSystemLanguage == null)
			{
				return string.Empty;
			}
			return supportedSystemLanguage.languageCode;
		}

		public List<SmartCultureInfo> GetSupportedLanguages()
		{
			if (availableLanguages == null)
			{
				Debug.LogError("LanguageManager is not initialized properly!");
				return null;
			}
			return availableLanguages.cultureInfos;
		}

		public List<string> GetKeysWithinCategory(string category)
		{
			List<string> list = new List<string>();
			if (string.IsNullOrEmpty(category) || languageDatabase == null)
			{
				return list;
			}
			foreach (KeyValuePair<string, LocalizedObject> item in languageDatabase)
			{
				if (item.Key.StartsWith(category))
				{
					list.Add(item.Key);
				}
			}
			return list;
		}

		public string GetTextValue(string key)
		{
			LocalizedObject localizedObject = GetLocalizedObject(key);
			if (localizedObject != null)
			{
				return localizedObject.TextValue;
			}
			if (verboseLogging)
			{
				Debug.LogError("LanguageManager.cs: Invalid Key:" + key + "for language: " + loadedLanguage);
			}
			return null;
		}

		public string GetTextValue(string key, int count)
		{
			return GetTextValue(key + "_" + PluralForms.Languages[loadedLanguage](count));
		}

		public TextAsset GetTextAsset(string key, int count)
		{
			return GetTextAsset(key + "_" + PluralForms.Languages[loadedLanguage](count));
		}

		public AudioClip GetAudioClip(string key, int count)
		{
			return GetAudioClip(key + "_" + PluralForms.Languages[loadedLanguage](count));
		}

		public GameObject GetPrefab(string key, int count)
		{
			return GetPrefab(key + "_" + PluralForms.Languages[loadedLanguage](count));
		}

		public Texture GetTexture(string key, int count)
		{
			return GetTexture(key + "_" + PluralForms.Languages[loadedLanguage](count));
		}

		public string GetTextValue(string key, int count, Func<int, int> pluralForm)
		{
			return GetTextValue(key + "_" + pluralForm(count));
		}

		public TextAsset GetTextAsset(string key, int count, Func<int, int> pluralForm)
		{
			return GetTextAsset(key + "_" + pluralForm(count));
		}

		public Font GetFont(string key, int count, Func<int, int> pluralForm)
		{
			return GetFont(key + "_" + pluralForm(count));
		}

		public AudioClip GetAudioClip(string key, int count, Func<int, int> pluralForm)
		{
			return GetAudioClip(key + "_" + pluralForm(count));
		}

		public GameObject GetPrefab(string key, int count, Func<int, int> pluralForm)
		{
			return GetPrefab(key + "_" + pluralForm(count));
		}

		public Texture GetTexture(string key, int count, Func<int, int> pluralForm)
		{
			return GetTexture(key + "_" + pluralForm(count));
		}

		public TextAsset GetTextAsset(string key)
		{
			LocalizedObject localizedObject = GetLocalizedObject(key);
			if (localizedObject != null)
			{
				return Resources.Load(LanguageRuntimeData.TextAssetsFolderPath(CheckLanguageOverrideCode(localizedObject)) + "/" + key) as TextAsset;
			}
			if (verboseLogging)
			{
				Debug.LogError("Could not get text asset with key: " + key);
			}
			return null;
		}

		public string GetFontResourcePath(string key)
		{
			LocalizedObject localizedObject = GetLocalizedObject(key);
			if (localizedObject != null)
			{
				return string.Format("{0}/{1}", LanguageRuntimeData.FontsFolderPath(CheckLanguageOverrideCode(localizedObject)), key);
			}
			return null;
		}

		public Font GetFont(string key)
		{
			LocalizedObject localizedObject = GetLocalizedObject(key);
			if (localizedObject != null)
			{
				return Resources.Load(LanguageRuntimeData.FontsFolderPath(CheckLanguageOverrideCode(localizedObject)) + "/" + key) as Font;
			}
			if (verboseLogging)
			{
				Debug.LogError("Could not get font asset with key: " + key);
			}
			return null;
		}

		public AudioClip GetAudioClip(string key)
		{
			LocalizedObject localizedObject = GetLocalizedObject(key);
			if (localizedObject != null)
			{
				return Resources.Load(LanguageRuntimeData.AudioFilesFolderPath(CheckLanguageOverrideCode(localizedObject)) + "/" + key) as AudioClip;
			}
			if (verboseLogging)
			{
				Debug.LogError("Could not get audio clip with key: " + key);
			}
			return null;
		}

		public GameObject GetPrefab(string key)
		{
			LocalizedObject localizedObject = GetLocalizedObject(key);
			if (localizedObject != null)
			{
				return Resources.Load(LanguageRuntimeData.PrefabsFolderPath(CheckLanguageOverrideCode(localizedObject)) + "/" + key) as GameObject;
			}
			if (verboseLogging)
			{
				Debug.LogError("Could not get prefab with key: " + key);
			}
			return null;
		}

		public Texture GetTexture(string key)
		{
			LocalizedObject localizedObject = GetLocalizedObject(key);
			if (localizedObject != null)
			{
				return Resources.Load(LanguageRuntimeData.TexturesFolderPath(CheckLanguageOverrideCode(localizedObject)) + "/" + key) as Texture;
			}
			if (verboseLogging)
			{
				Debug.LogError("Could not get texture with key: " + key);
			}
			return null;
		}

		private string CheckLanguageOverrideCode(LocalizedObject localizedObject)
		{
			if (localizedObject == null)
			{
				return loadedLanguage;
			}
			string text = ((!localizedObject.OverrideLocalizedObject) ? loadedLanguage : localizedObject.OverrideObjectLanguageCode);
			if (string.IsNullOrEmpty(text))
			{
				text = loadedLanguage;
			}
			return text;
		}

		public bool HasKey(string key)
		{
			return GetLocalizedObject(key) != null;
		}

		private LocalizedObject GetLocalizedObject(string key)
		{
			LocalizedObject value;
			languageDatabase.TryGetValue(key, out value);
			return value;
		}
	}
}
