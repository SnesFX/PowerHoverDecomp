using System;
using SA.Common.Util;

namespace SA.Fitness
{
	public class SensorListener
	{
		public class Builder
		{
			private SensorListener listener = new SensorListener();

			public Builder SetDataType(DataType dataType)
			{
				listener.dataType = dataType;
				return this;
			}

			public Builder SetSamplingRate(long amount, TimeUnit unit)
			{
				listener.rateAmount = amount;
				listener.rateTimeUnit = unit;
				return this;
			}

			public SensorListener Build()
			{
				return listener;
			}
		}

		private int id;

		private DataType dataType;

		private long rateAmount;

		private TimeUnit rateTimeUnit;

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

		public long RateAmount
		{
			get
			{
				return rateAmount;
			}
		}

		public TimeUnit RateTimeUnit
		{
			get
			{
				return rateTimeUnit;
			}
		}

		public event Action<int> OnRegisterSuccess = delegate
		{
		};

		public event Action<int> OnRegisterFail = delegate
		{
		};

		public event Action<int, DataPoint> OnDataPointReceived = delegate
		{
		};

		private SensorListener()
		{
			id = IdFactory.NextId;
		}

		public void DispatchRegisterSuccess()
		{
			this.OnRegisterSuccess(id);
		}

		public void DispatchRegisterFail()
		{
			this.OnRegisterFail(id);
		}

		public void DispatchDataPointEvent(string[] bundle)
		{
			this.OnDataPointReceived(id, new DataPoint(dataType, bundle, "|"));
		}
	}
}
