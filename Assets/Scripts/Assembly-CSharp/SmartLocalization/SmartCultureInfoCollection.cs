using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace SmartLocalization
{
	[Serializable]
	[XmlRoot("SmartCultureCollections")]
	public class SmartCultureInfoCollection
	{
		public const int LatestVersion = 4;

		[XmlElement(ElementName = "version")]
		public int version;

		[XmlArray("CultureInfos")]
		[XmlArrayItem("CultureInfo")]
		public List<SmartCultureInfo> cultureInfos = new List<SmartCultureInfo>();

		public void RemoveCultureInfo(SmartCultureInfo cultureInfo)
		{
			if (cultureInfo == null)
			{
				Debug.LogError("Cannot remove a SmartCultureInfo that's null!");
			}
			else if (!cultureInfos.Remove(cultureInfo))
			{
				Debug.LogError("Failed to remove cultureInfo!");
			}
		}

		public void AddCultureInfo(SmartCultureInfo cultureInfo)
		{
			if (cultureInfo == null)
			{
				Debug.LogError("Cannot add a SmartCultureInfo that's null!");
			}
			else
			{
				cultureInfos.Add(cultureInfo);
			}
		}

		public static SmartCultureInfoCollection Deserialize(TextAsset xmlFile)
		{
			return SmartCultureInfoCollectionDeserializer.Deserialize(xmlFile);
		}

		public SmartCultureInfo FindCulture(SmartCultureInfo cultureInfo)
		{
			if (cultureInfo == null)
			{
				return null;
			}
			return cultureInfos.Find((SmartCultureInfo c) => c.englishName.ToLower() == cultureInfo.englishName.ToLower() && c.languageCode.ToLower() == cultureInfo.languageCode.ToLower());
		}

		public SmartCultureInfo FindCulture(string languageCode)
		{
			if (string.IsNullOrEmpty(languageCode))
			{
				return null;
			}
			return cultureInfos.Find((SmartCultureInfo c) => c.languageCode.ToLower() == languageCode.ToLower());
		}

		public bool IsCultureInCollection(SmartCultureInfo cultureInfo)
		{
			return FindCulture(cultureInfo) != null;
		}
	}
}
