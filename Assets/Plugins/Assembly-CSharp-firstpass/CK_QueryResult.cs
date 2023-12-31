using System.Collections.Generic;
using SA.Common.Models;

public class CK_QueryResult : Result
{
	private CK_Database _Database;

	private List<CK_Record> _Records = new List<CK_Record>();

	public CK_Database Database
	{
		get
		{
			return _Database;
		}
	}

	public List<CK_Record> Records
	{
		get
		{
			return _Records;
		}
	}

	public CK_QueryResult(List<CK_Record> records)
	{
		_Records = records;
	}

	public CK_QueryResult(string errorData)
		: base(new Error(errorData))
	{
	}

	public void SetDatabase(CK_Database database)
	{
		_Database = database;
	}
}
