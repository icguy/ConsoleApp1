using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1.Model;

namespace ConsoleApp1
{
	class WorkTimesBuilder
	{
		public WorkTimes Build(WorkTimes workTimes, IEnumerable<EventLogEntry> events)
		{
			if (workTimes == null)
				workTimes = new WorkTimes();

			var workEvents = events.Select(e => LogEntryToWorkEvent(e));
			AddWorks(workTimes, workEvents);
			return workTimes;
		}

		protected virtual DailyWork FromWorkEvents(IEnumerable<WorkEvent> events)
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
				else if (e.Type == EventType.Departure)
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
		
		protected virtual WorkEvent LogEntryToWorkEvent(EventLogEntry entry)
		{
			WorkEvent workEvent = new WorkEvent();
			workEvent.Time = entry.TimeGenerated;
			workEvent.Type = GetEventType(entry.InstanceId);
			return workEvent;
		}
		
		public static EventType GetEventType(long id)
		{
			long[] loginIds = new[] { 4624L, 4648L, 4801L, };
			long[] logoutIds = new[] { 4634L, 4800L, 4647L, };
			if (loginIds.Contains(id))
				return EventType.Arrival;
			if (logoutIds.Contains(id))
				return EventType.Departure;
			return EventType.Unknown;
		}
		
		protected virtual void AddWorks(WorkTimes workTimes, IEnumerable<WorkEvent> events)
		{
			var lastWorkTime = workTimes.DailyWorks.LastOrDefault()?.Events?.LastOrDefault()?.Time ?? DateTime.MinValue;
			var filteredEvents = FilterSort(events, lastWorkTime, GetLastMidnight()).ToList();
			var newDailyWorks = new List<DailyWork>();
			newDailyWorks.AddRange(workTimes.DailyWorks);

			while (filteredEvents.Count != 0)
			{
				var firstEventTime = filteredEvents[0].Time;
				var currentDayEvents = filteredEvents.Where(e => (e.Time.DayOfYear == firstEventTime.DayOfYear)).ToList();
				var currentDailyWork = FromWorkEvents(currentDayEvents);
				newDailyWorks.Add(currentDailyWork);
				workTimes.Balance += currentDailyWork.Balance;
				foreach (var e in currentDayEvents)
				{
					filteredEvents.Remove(e);
				}
			}
			workTimes.DailyWorks = newDailyWorks.ToArray();
		}
		
		protected virtual DateTime GetLastMidnight()
		{
			DateTime result = DateTime.Now;
			result -= result.TimeOfDay;
			return result;
		}
		
		protected virtual IEnumerable<WorkEvent> FilterSort(IEnumerable<WorkEvent> events, DateTime from, DateTime to)
		{
			return events.Where(e => from < e.Time && e.Time < to).OrderBy(e => e.Time);
		}
	}
}
