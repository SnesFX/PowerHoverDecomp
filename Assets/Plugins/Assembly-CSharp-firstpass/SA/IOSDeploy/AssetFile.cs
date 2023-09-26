using System;
using System.IO;
using UnityEngine;

namespace SA.IOSDeploy
{
	[Serializable]
	public class AssetFile
	{
		public bool IsOpen;

		public string XCodePath = string.Empty;

		public UnityEngine.Object Asset;

		public string FileName
		{
			get
			{
				return string.Empty;
			}
		}

		public string FilePath
		{
			get
			{
				return string.Empty;
			}
		}

		public string XCodeRelativePath
		{
			get
			{
				return XCodePath + FileName;
			}
		}

		public bool IsDirectory
		{
			get
			{
				FileAttributes attributes = File.GetAttributes(FilePath);
				if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
				{
					return true;
				}
				return false;
			}
		}
	}
}
