using System;
using System.Collections.Generic;
using SA.Common.Util;

namespace SA.Fitness
{
	public class DeleteDataRequest
	{
		public class Builder
		{
			private DeleteDataRequest request = new DeleteDataRequest();

			public Builder SetTimeInterval(long startTime, long endTime, TimeUnit unit)
			{
				request.startTime = startTime;
				request.endTime = endTime;
				request.timeUnit = unit;
				return this;
			}

			public Builder AddDataType(DataType dataType)
			{
				if (!request.dataTypes.Contains(dataType))
				{
					request.dataTypes.Add(dataType);
				}
				return this;
			}

			public Builder AddSession(string sessionId)
			{
				if (!request.sessions.Contains(sessionId))
				{
					request.sessions.Add(sessionId);
				}
				return this;
			}

			public Builder DeleteAllSessions()
			{
				request.sessions.Clear();
				return this;
			}

			public DeleteDataRequest Build()
			{
				return request;
			}
		}

		private int id = IdFactory.NextId;

		private long startTime;

		private long endTime = 1L;

		private TimeUnit timeUnit;

		private List<DataType> dataTypes = new List<DataType>();

		private List<string> sessions = new List<string>();

		public int Id
		{
			get
			{
				return id;
			}
		}

		public long StartTime
		{
			get
			{
				return startTime;
			}
		}

		public long EndTime
		{
			get
			{
				return endTime;
			}
		}

		public TimeUnit TimeUnit
		{
			get
			{
				return timeUnit;
			}
		}

		public List<DataType> DataTypes
		{
			get
			{
				return dataTypes;
			}
		}

		public List<string> Sessions
		{
			get
			{
				return sessions;
			}
		}

		public event Action OnRequestFinished = delegate
		{
		};

		public void DispatchRequestResult()
		{
			this.OnRequestFinished();
		}
	}
}
