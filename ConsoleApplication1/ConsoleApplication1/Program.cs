using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ConsoleApp1
{
	class Program
	{
		static void Main(string[] args)
		{
			//Tests.RunTests();

			EventLog securityLog = GetSecurityLog();
			List<EventLogEntry> eventList = GetEvents(securityLog);
			WriteToLog(eventList);
			eventList.ForEach((e) => Console.WriteLine($"category: {e.Category}, type: {e.EntryType}, id: {e.InstanceId}, time1: {e.TimeGenerated}, type: {GetEventType(e.InstanceId)}"));
			Console.WriteLine();
			var wt = new WorkTimes();
			wt.AddWorks(eventList.Select(e => WorkEvent.FromLogEntry(e)).ToList());

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
			if( securityLog == null )
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
			if( loginIds.Contains(id) )
				return EventType.Arrival;
			if( logoutIds.Contains(id) )
				return EventType.Departure;
			return EventType.Unknown;
		}
	}
}