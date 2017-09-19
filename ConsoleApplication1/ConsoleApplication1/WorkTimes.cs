using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
	public class WorkTimes
	{
		public TimeSpan Balance { get; set; } = TimeSpan.Zero;

		public DailyWork[] DailyWorks { get; set; } = new DailyWork[0];

		public void AddWorks(IEnumerable<WorkEvent> events)
		{
			var lastWorkTime = DailyWorks.LastOrDefault()?.Events?.LastOrDefault()?.Time ?? DateTime.MinValue;
			var filteredEvents = FilterSort(events, lastWorkTime, GetLastMidnight()).ToList();
			var newDailyWorks = new List<DailyWork>();
			newDailyWorks.AddRange(this.DailyWorks);

			while( filteredEvents.Count != 0 )
			{
				var firstEventTime = filteredEvents[0].Time;
				var currentDayEvents = filteredEvents.Where(e => (e.Time.DayOfYear == firstEventTime.DayOfYear)).ToList();
				var currentDailyWork = DailyWork.FromWorkEvents(currentDayEvents);
				newDailyWorks.Add(currentDailyWork);
				this.Balance += currentDailyWork.Balance;
				foreach( var e in currentDayEvents )
				{
					filteredEvents.Remove(e);
				}
			}
			this.DailyWorks = newDailyWorks.ToArray();
		}

		public static DateTime GetLastMidnight()
		{
			DateTime result = DateTime.Now;
			result -= result.TimeOfDay;
			return result;
		}

		public static IEnumerable<WorkEvent> FilterSort(IEnumerable<WorkEvent> events, DateTime from, DateTime to)
		{
			return events.Where(e => from < e.Time && e.Time < to).OrderBy(e => e.Time);
		}
	}
}
