using System;
using SA.Common.Util;

namespace SA.Fitness
{
	public class StopSessionRequest
	{
		public class Builder
		{
			private StopSessionRequest request = new StopSessionRequest();

			public Builder SetIdentifier(string sessionId)
			{
				request.sessionId = sessionId;
				return this;
			}

			public StopSessionRequest Build()
			{
				return request;
			}
		}

		private int id = IdFactory.NextId;

		private string sessionId = string.Empty;

		public int Id
		{
			get
			{
				return id;
			}
		}

		public string SessionId
		{
			get
			{
				return sessionId;
			}
		}

		public event Action<StopSessionResult> OnSessionStopped = delegate
		{
		};

		private StopSessionRequest()
		{
		}

		public void DispatchResult(string[] bundle)
		{
			int num = int.Parse(bundle[1]);
			StopSessionResult stopSessionResult = ((num != 0) ? new StopSessionResult(id, num, bundle[2]) : new StopSessionResult(id));
			if (stopSessionResult.IsSucceeded)
			{
				for (int i = 2; i < bundle.Length; i++)
				{
					string[] array = bundle[i].Split(new string[1] { "~" }, StringSplitOptions.None);
					Session session = new Session();
					session.StartTime = long.Parse(array[0]);
					session.EndTime = long.Parse(array[1]);
					session.Name = array[2];
					session.Id = array[3];
					session.Description = array[4];
					session.Activity = new Activity(array[5]);
					session.AppPackageName = array[6];
					stopSessionResult.AddSession(session);
				}
			}
			this.OnSessionStopped(stopSessionResult);
		}
	}
}
