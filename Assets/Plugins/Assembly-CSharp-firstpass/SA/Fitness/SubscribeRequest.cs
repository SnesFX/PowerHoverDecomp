using System;
using SA.Common.Models;
using SA.Common.Util;

namespace SA.Fitness
{
	public class SubscribeRequest
	{
		public class Builder
		{
			private SubscribeRequest request = new SubscribeRequest();

			public Builder SetDataType(DataType dataType)
			{
				request.dataType = dataType;
				return this;
			}

			public SubscribeRequest Build()
			{
				return request;
			}
		}

		private int id = IdFactory.NextId;

		private DataType dataType;

		public int Id
		{
			get
			{
				return id;
			}
		}

		public DataType DataType
		{
			get
			{
				return dataType;
			}
		}

		public event Action<Result> OnSubscribeFinished = delegate
		{
		};

		private SubscribeRequest()
		{
		}

		public void DispatchResult(Result result)
		{
			this.OnSubscribeFinished(result);
		}
	}
}
