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

			List<EventLogEntry> eventList = LogReader.GetSecurityEvents();
			WriteToLog(eventList);
			eventList.ForEach((e) => Console.WriteLine($"category: {e.Category}, type: {e.EntryType}, id: {e.InstanceId}, time1: {e.TimeGenerated}, type: {GetEventType(e.InstanceId)}"));
			Console.WriteLine();
			var wt = new WorkTimes();
			wt.AddWorks(eventList.Select(e => WorkEvent.FromLogEntry(e)).ToList());

			Console.ReadLine();
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