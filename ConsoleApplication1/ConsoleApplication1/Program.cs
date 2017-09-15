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
			EventLog securityLog = GetSecurityLog();
			List<EventLogEntry> eventList = GetEvents(securityLog);
			WriteToLog(eventList);
			eventList.ForEach((e) => Console.WriteLine($"category: {e.Category}, type: {e.EntryType}, id: {e.InstanceId}, time1: {e.TimeGenerated}, type: {GetEventType(e.InstanceId)}"));
			Console.ReadLine();
		}

		private static List<EventLogEntry> GetEvents(EventLog securityLog)
		{
			var q = securityLog.Entries.Cast<EventLogEntry>();
			q = FilterId(q);
			var eventlist = q.ToList();
			return eventlist;
		}

		private static EventLog GetSecurityLog()
		{
			var logs = EventLog.GetEventLogs();
			var securityLog = logs.Where(l => l.Log == "Security").FirstOrDefault();
			if( securityLog == null )
				throw new Exception("securitylog null");
			return securityLog;
		}

		private static IEnumerable<EventLogEntry> FilterId(IEnumerable<EventLogEntry> q)
		{
			long[] ids = new[] { 4647L, 4648L, 4800L, 4801L, /*4624L,*/ /*4634L*/ };
			q = q.Where(e => ids.Contains(e.InstanceId));
			return q;
		}

		const string dataFile = "times.json";
		private static void WriteToLog(IEnumerable<EventLogEntry> eventList)
		{

		}

		static EventType GetEventType(long id)
		{
			long[] loginIds = new[] { 4624L, 4648L, 4801L, };
			long[] logoutIds = new[] { 4634L, 4800L, 4647L, };
			if( loginIds.Contains(id) )
				return EventType.Arrival;
			if( logoutIds.Contains(id) )
				return EventType.Departure;
			return EventType.Unknown;
		}

		static DateTime GetLastMidnight()
		{
			DateTime result = DateTime.Now;
			result.AddHours(-result.Hour);
			result.AddMinutes(-result.Minute);
			return result;
		}
				
		class WorkTimes
		{
			public TimeSpan Balance;
			public List<DailyWork> DailyWorks;
			public void AddDailyWork(IEnumerable<WorkEvent> events)
			{
				TimeSpan balance = TimeSpan.FromHours(8);
				List<WorkEvent> filteredEvents = new List<WorkEvent>();
				
				DateTime? lastSignin = null;
				foreach( var e in events )
				{
					if( lastSignin == null )
					{
						if( e.Type == EventType.Arrival )
						{
							lastSignin = e.Time;
							filteredEvents.Add(e);
						}
					}
					else if( e.Type == EventType.Departure )
					{
						balance -= (e.Time - lastSignin.Value);
						lastSignin = null;
						filteredEvents.Add(e);
					}
				}

			}
		}

		class DailyWork
		{
			public TimeSpan Balance;
			public WorkEvent[] Events;
		}

		class WorkEvent
		{
			public EventType Type;
			public DateTime Time;
			public static WorkEvent FromLogEntry(EventLogEntry entry)
			{
				WorkEvent workEvent = new WorkEvent();
				workEvent.Time = entry.TimeGenerated;
				workEvent.Type = GetEventType(entry.InstanceId);
				return workEvent;
			}
		}

		enum EventType
		{
			Arrival,
			Departure,
			Unknown
		}
	}
}
