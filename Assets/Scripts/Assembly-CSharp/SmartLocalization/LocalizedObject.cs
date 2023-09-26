using System;
using UnityEngine;

namespace SmartLocalization
{
	[Serializable]
	public class LocalizedObject
	{
		public static readonly string keyTypeIdentifier = "[type=";

		private static readonly string endBracket = "]";

		[SerializeField]
		private LocalizedObjectType objectType;

		[SerializeField]
		private string textValue;

		[SerializeField]
		private GameObject thisGameObject;

		[SerializeField]
		private AudioClip thisAudioClip;

		[SerializeField]
		private Texture thisTexture;

		[SerializeField]
		private TextAsset thisTextAsset;

		[SerializeField]
		private Font font;

		[SerializeField]
		private bool overrideLocalizedObject;

		[SerializeField]
		private string overrideObjectLanguageCode;

		public LocalizedObjectType ObjectType
		{
			get
			{
				return objectType;
			}
			set
			{
				objectType = value;
			}
		}

		public string TextValue
		{
			get
			{
				return textValue;
			}
			set
			{
				textValue = value;
			}
		}

		public GameObject ThisGameObject
		{
			get
			{
				return thisGameObject;
			}
			set
			{
				thisGameObject = value;
			}
		}

		public AudioClip ThisAudioClip
		{
			get
			{
				return thisAudioClip;
			}
			set
			{
				thisAudioClip = value;
			}
		}

		public Texture ThisTexture
		{
			get
			{
				return thisTexture;
			}
			set
			{
				thisTexture = value;
			}
		}

		public bool OverrideLocalizedObject
		{
			get
			{
				return overrideLocalizedObject;
			}
			set
			{
				overrideLocalizedObject = value;
				if (!overrideLocalizedObject)
				{
					overrideObjectLanguageCode = null;
					return;
				}
				ThisAudioClip = null;
				ThisTexture = null;
				ThisGameObject = null;
				ThisTexture = null;
				ThisTextAsset = null;
				Font = null;
			}
		}

		public string OverrideObjectLanguageCode
		{
			get
			{
				return overrideObjectLanguageCode;
			}
			set
			{
				overrideObjectLanguageCode = value;
				if (overrideLocalizedObject)
				{
					textValue = "override=" + overrideObjectLanguageCode;
				}
			}
		}

		public TextAsset ThisTextAsset
		{
			get
			{
				return thisTextAsset;
			}
			set
			{
				thisTextAsset = value;
			}
		}

		public Font Font
		{
			get
			{
				return font;
			}
			set
			{
				font = value;
			}
		}

		public LocalizedObject()
		{
		}

		public LocalizedObject(LocalizedObject other)
		{
			if (other != null)
			{
				objectType = other.ObjectType;
				textValue = other.TextValue;
				overrideLocalizedObject = other.OverrideLocalizedObject;
			}
		}

		public static LocalizedObjectType GetLocalizedObjectType(string key)
		{
			if (key.StartsWith(keyTypeIdentifier))
			{
				if (key.StartsWith(keyTypeIdentifier + "AUDIO" + endBracket))
				{
					return LocalizedObjectType.AUDIO;
				}
				if (key.StartsWith(keyTypeIdentifier + "GAME_OBJECT" + endBracket))
				{
					return LocalizedObjectType.GAME_OBJECT;
				}
				if (key.StartsWith(keyTypeIdentifier + "TEXTURE" + endBracket))
				{
					return LocalizedObjectType.TEXTURE;
				}
				if (key.StartsWith(keyTypeIdentifier + "TEXT_ASSET" + endBracket))
				{
					return LocalizedObjectType.TEXT_ASSET;
				}
				if (key.StartsWith(keyTypeIdentifier + "FONT" + endBracket))
				{
					return LocalizedObjectType.FONT;
				}
				Debug.LogError("LocalizedObject.cs: ERROR IN SYNTAX of key:" + key + ", setting object type to STRING");
				return LocalizedObjectType.STRING;
			}
			return LocalizedObjectType.STRING;
		}

		public static string GetCleanKey(string key, LocalizedObjectType objectType)
		{
			int length = (keyTypeIdentifier + GetLocalizedObjectTypeStringValue(objectType) + endBracket).Length;
			switch (objectType)
			{
			case LocalizedObjectType.STRING:
				return key;
			case LocalizedObjectType.GAME_OBJECT:
			case LocalizedObjectType.AUDIO:
			case LocalizedObjectType.TEXTURE:
				return key.Substring(length);
			case LocalizedObjectType.TEXT_ASSET:
			case LocalizedObjectType.FONT:
				return key.Substring(length);
			default:
				Debug.LogError("LocalizedObject.GetCleanKey(key) error!, object type is unknown! objectType:" + (int)objectType);
				return key;
			}
		}

		public static string GetCleanKey(string key)
		{
			LocalizedObjectType localizedObjectType = GetLocalizedObjectType(key);
			return GetCleanKey(key, localizedObjectType);
		}

		public string GetFullKey(string parsedKey)
		{
			if (objectType == LocalizedObjectType.STRING)
			{
				return parsedKey;
			}
			return keyTypeIdentifier + GetLocalizedObjectTypeStringValue(objectType) + endBracket + parsedKey;
		}

		public static string GetFullKey(string parsedKey, LocalizedObjectType objectType)
		{
			if (objectType == LocalizedObjectType.STRING)
			{
				return parsedKey;
			}
			return keyTypeIdentifier + GetLocalizedObjectTypeStringValue(objectType) + endBracket + parsedKey;
		}

		public static string GetLocalizedObjectTypeStringValue(LocalizedObjectType objectType)
		{
			switch (objectType)
			{
			case LocalizedObjectType.AUDIO:
				return "AUDIO";
			case LocalizedObjectType.GAME_OBJECT:
				return "GAME_OBJECT";
			case LocalizedObjectType.STRING:
				return "STRING";
			case LocalizedObjectType.TEXTURE:
				return "TEXTURE";
			case LocalizedObjectType.TEXT_ASSET:
				return "TEXT_ASSET";
			case LocalizedObjectType.FONT:
				return "FONT";
			default:
				return "STRING";
			}
		}
	}
}
