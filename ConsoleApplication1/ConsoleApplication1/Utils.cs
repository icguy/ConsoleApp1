using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication1.Model;

namespace ConsoleApplication1
{
	class Utils
	{
		public static DailyWork CreateDailyWork(IEnumerable<WorkEvent> events)
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
