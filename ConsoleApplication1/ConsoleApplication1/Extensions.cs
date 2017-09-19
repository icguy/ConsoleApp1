using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ConsoleApplication1.Model;

namespace ConsoleApplication1
{
	public static class Extensions
	{
		public static EventType GetEventType(this EventLogEntry logEntry)
		{
			var id = logEntry.InstanceId;
			long[] loginIds = new[] { 4624L, 4648L, 4801L, };
			long[] logoutIds = new[] { 4634L, 4800L, 4647L, };
			if (loginIds.Contains(id))
				return EventType.Arrival;
			if (logoutIds.Contains(id))
				return EventType.Departure;
			return EventType.Unknown;
		}

		public static WorkEvent ToWorkEvent(this EventLogEntry entry)
		{
			WorkEvent workEvent = new WorkEvent();
			workEvent.Time = entry.TimeGenerated;
			workEvent.Type = entry.GetEventType();
			return workEvent;
		}

		public static void AddWorks(this WorkTimes workTimes, IEnumerable<WorkEvent> events)
		{
			var lastWorkTime = workTimes.DailyWorks.LastOrDefault()?.Events?.LastOrDefault()?.Time ?? DateTime.MinValue;
			var lastMidnight = DateTime.Now;
			lastMidnight -= lastMidnight.TimeOfDay;
			var filteredEvents = events
				.Where(e => lastWorkTime < e.Time && e.Time < lastMidnight)
				.OrderBy(e => e.Time)
				.ToList();
				
			var newDailyWorks = new List<DailyWork>();
			newDailyWorks.AddRange(workTimes.DailyWorks);

			while (filteredEvents.Count != 0)
			{
				var firstEventTime = filteredEvents[0].Time;
				var currentDayEvents = filteredEvents.Where(e => (e.Time.DayOfYear == firstEventTime.DayOfYear)).ToList();
				var currentDailyWork = DailyWork.FromWorkEvents(currentDayEvents);
				newDailyWorks.Add(currentDailyWork);
				workTimes.Balance += currentDailyWork.Balance;
				foreach (var e in currentDayEvents)
				{
					filteredEvents.Remove(e);
				}
			}
			workTimes.DailyWorks = newDailyWorks.ToArray();
		}

		public static void PrintEvent(this EventLogEntry entry)
		{
			Console.Write($"category: {entry.Category}, ");
			Console.Write($"type: {entry.EntryType}, ");
			Console.Write($"id: {entry.InstanceId}, ");
			Console.Write($"time: {entry.TimeGenerated}, ");
			Console.WriteLine($"type: {entry.GetEventType()}");
		}
	}
}
