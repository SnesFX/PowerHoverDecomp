using System;
using SA.Common.Models;
using SA.Common.Util;

namespace SA.Fitness
{
	public class UnsubscribeRequest
	{
		public class Builder
		{
			private UnsubscribeRequest request = new UnsubscribeRequest();

			public Builder SetDataType(DataType dataType)
			{
				request.dataType = dataType;
				return this;
			}

			public UnsubscribeRequest Build()
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

		public event Action<Result> OnUnsubscribeFinished = delegate
		{
		};

		private UnsubscribeRequest()
		{
		}

		public void DispatchUnsubscribeResult(Result result)
		{
			this.OnUnsubscribeFinished(result);
		}
	}
}
