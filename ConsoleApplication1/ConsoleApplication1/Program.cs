using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace ConsoleApp1
{
	class Program
	{
		static void Main(string[] args)
		{
			Tests.RunTests();

			EventLog securityLog = GetSecurityLog();
			List<EventLogEntry> eventList = GetEvents(securityLog);
			WriteToLog(eventList);
			eventList.ForEach((e) => Console.WriteLine($"category: {e.Category}, type: {e.EntryType}, id: {e.InstanceId}, time1: {e.TimeGenerated}, type: {GetEventType(e.InstanceId)}"));
			Console.ReadLine();
		}

		public static List<EventLogEntry> GetEvents(EventLog securityLog)
		{
			var q = securityLog.Entries.Cast<EventLogEntry>();
			q = FilterId(q);
			var eventlist = q.ToList();
			return eventlist;
		}

		public static EventLog GetSecurityLog()
		{
			var logs = EventLog.GetEventLogs();
			var securityLog = logs.Where(l => l.Log == "Security").FirstOrDefault();
			if (securityLog == null)
				throw new Exception("securitylog null");
			return securityLog;
		}

		public static IEnumerable<EventLogEntry> FilterId(IEnumerable<EventLogEntry> q)
		{
			long[] ids = new[] { 4647L, 4648L, 4800L, 4801L, /*4624L,*/ /*4634L*/ };
			q = q.Where(e => ids.Contains(e.InstanceId));
			return q;
		}

		const string dataFile = "times.json";
		public static void WriteToLog(IEnumerable<EventLogEntry> eventList)
		{

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
	}

	public class WorkTimes
	{
		public TimeSpan Balance { get; set; } = TimeSpan.Zero;
		public DailyWork[] DailyWorks { get; set; } = new DailyWork[0];
		public void AddWorks(IEnumerable<WorkEvent> events)
		{
			var lastWorkTime = DailyWorks.LastOrDefault()?.Events?.LastOrDefault()?.Time ?? DateTime.MinValue;
			var filteredEvents = FilterSort(events, lastWorkTime, GetLastMidnight()).ToList();
			var newDailyWorks = new List<DailyWork>();
			newDailyWorks.AddRange(DailyWorks);

			while (filteredEvents.Count != 0)
			{
				var firstEventTime = filteredEvents[0].Time;
				var currentDayEvents = filteredEvents.Where(e => (e.Time.DayOfYear == firstEventTime.DayOfYear)).ToList();
				newDailyWorks.Add(DailyWork.FromWorkEvents(currentDayEvents));
				foreach (var e in currentDayEvents)
				{
					filteredEvents.Remove(e);
				}
			}
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
	}

	public class WorkEvent
	{
		public EventType Type = EventType.Unknown;
		public DateTime Time = DateTime.MinValue;
		public static WorkEvent FromLogEntry(EventLogEntry entry)
		{
			WorkEvent workEvent = new WorkEvent();
			workEvent.Time = entry.TimeGenerated;
			workEvent.Type = Program.GetEventType(entry.InstanceId);
			return workEvent;
		}
	}

	public enum EventType
	{
		Arrival,
		Departure,
		Unknown
	}
}