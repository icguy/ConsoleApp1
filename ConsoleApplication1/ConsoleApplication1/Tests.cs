using System;
using System.Diagnostics;
using ConsoleApp1.Model;

namespace ConsoleApp1
{
	class WorkTimesBuilderTest : WorkTimesBuilder
	{
		public void RunTests()
		{
			Debug.Assert(this.T001());
			Console.WriteLine("Testing finished");
			Console.ReadLine();
		}

		bool T001()
		{
			WorkEvent[] events = new WorkEvent[] {
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 09, 12, 00),
					Type = EventType.Arrival
				},
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 12, 12, 00),
					Type = EventType.Departure
				},
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 12, 52, 00),
					Type = EventType.Arrival
				},
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 17, 52, 00),
					Type = EventType.Departure
				}
			};

			var dailywork = this.FromWorkEvents(events);
			return TSEquals(dailywork.Balance, new TimeSpan(0, 0, 0));
		}

		static bool TSEquals(TimeSpan ts1, TimeSpan ts2)
		{
			TimeSpan delta = TimeSpan.FromMilliseconds(10);
			return -delta < ts1 - ts2 && ts1 - ts2 < delta;
		}
	}
}
