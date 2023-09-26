using System;

namespace SA.IOSDeploy
{
	[Serializable]
	public class VariableListed
	{
		public bool IsOpen = true;

		public string DictKey = string.Empty;

		public string StringValue = string.Empty;

		public int IntegerValue;

		public float FloatValue;

		public bool BooleanValue = true;

		public PlistValueTypes Type;
	}
}
