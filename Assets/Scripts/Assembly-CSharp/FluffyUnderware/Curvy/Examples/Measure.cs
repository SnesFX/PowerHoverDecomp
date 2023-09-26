using System;
using System.Diagnostics;

namespace FluffyUnderware.Curvy.Examples
{
	public class Measure : IDisposable
	{
		private Stopwatch T;

		private Result R;

		public Measure(string name)
		{
			R = new Result
			{
				Name = "[" + Performance.Prefix + "] " + name
			};
			T = new Stopwatch();
		}

		public void Start()
		{
			T.Start();
		}

		public void StartNext()
		{
			T.Stop();
			R.Values.Add((double)T.ElapsedTicks / 10000.0 / (double)Performance.Runs);
			T.Reset();
			T.Start();
		}

		public void Stop()
		{
			T.Stop();
			R.Values.Add((double)T.ElapsedTicks / 10000.0 / (double)Performance.Runs);
		}

		public void AddLast()
		{
			R.Values.Add(R.Values[R.Values.Count - 1]);
		}

		public void Dispose()
		{
			Performance.Results.Add(R);
		}
	}
}
