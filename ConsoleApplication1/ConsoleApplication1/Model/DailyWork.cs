using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication1.Model
{
	public class DailyWork
	{
		public TimeSpan Balance = TimeSpan.Zero;
		public WorkEvent[] Events = new WorkEvent[0];

		public static DailyWork FromWorkEvents(IEnumerable<WorkEvent> events)
		{
			var work = new DailyWork();
			TimeSpan balance = TimeSpan.FromHours(-8);
			List<WorkEvent> filteredEvents = new List<WorkEvent>();

			DateTime? lastSignin = null;
			foreach (var e in events)
			{
				if (lastSignin == null && e.Type == EventType.Arrival)
				{
					lastSignin = e.Time;
					filteredEvents.Add(e);
				}
				else if (e.Type == EventType.Departure && lastSignin != null)
				{
					balance += (e.Time - lastSignin.Value);
					lastSignin = null;
					filteredEvents.Add(e);
				}
			}
			if (lastSignin != null)
			{
				filteredEvents.Remove(filteredEvents.Last());
			}

			return new DailyWork()
			{
				Balance = balance,
				Events = filteredEvents.ToArray()
			};
		}
	}
}
