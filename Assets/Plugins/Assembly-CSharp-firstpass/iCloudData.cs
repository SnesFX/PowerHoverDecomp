using System;
using System.Collections.Generic;
using SA.Common.Data;

public class iCloudData
{
	private string m_key;

	private string m_val;

	private bool m_IsEmpty;

	[Obsolete("use Key instead")]
	public string key
	{
		get
		{
			return Key;
		}
	}

	public string Key
	{
		get
		{
			return m_key;
		}
	}

	public bool IsEmpty
	{
		get
		{
			return m_IsEmpty;
		}
	}

	[Obsolete("use StringValue instead")]
	public string stringValue
	{
		get
		{
			return StringValue;
		}
	}

	public string StringValue
	{
		get
		{
			if (m_IsEmpty)
			{
				return null;
			}
			return m_val;
		}
	}

	[Obsolete("use FloatValue instead")]
	public float floatValue
	{
		get
		{
			return FloatValue;
		}
	}

	public float FloatValue
	{
		get
		{
			if (m_IsEmpty)
			{
				return 0f;
			}
			return Convert.ToSingle(m_val);
		}
	}

	[Obsolete("use BytesValue instead")]
	public byte[] bytesValue
	{
		get
		{
			return BytesValue;
		}
	}

	public byte[] BytesValue
	{
		get
		{
			if (m_IsEmpty)
			{
				return null;
			}
			return Convert.FromBase64String(m_val);
		}
	}

	public List<object> ListValue
	{
		get
		{
			if (m_IsEmpty)
			{
				return new List<object>();
			}
			return (List<object>)Json.Deserialize(m_val);
		}
	}

	public Dictionary<string, object> DictionaryValue
	{
		get
		{
			if (m_IsEmpty)
			{
				return new Dictionary<string, object>();
			}
			return (Dictionary<string, object>)Json.Deserialize(m_val);
		}
	}

	public int IntValue
	{
		get
		{
			if (m_IsEmpty)
			{
				return 0;
			}
			return Convert.ToInt32(m_val);
		}
	}

	public long LongValue
	{
		get
		{
			if (m_IsEmpty)
			{
				return 0L;
			}
			return Convert.ToInt64(m_val);
		}
	}

	public ulong UlongValue
	{
		get
		{
			if (m_IsEmpty)
			{
				return 0uL;
			}
			return Convert.ToUInt64(m_val);
		}
	}

	public iCloudData(string k, string v)
	{
		m_key = k;
		m_val = v;
		if (m_val.Equals("null"))
		{
			if (!IOSNativeSettings.Instance.DisablePluginLogs)
			{
				ISN_Logger.Log("ISN iCloud Empty set");
			}
			m_IsEmpty = true;
		}
	}

	public T GetObject<T>()
	{
		return default(T);
	}
}
