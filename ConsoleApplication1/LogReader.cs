﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ConsoleApplication1
{
	class LogReader
	{
		public static List<EventLogEntry> GetSecurityEvents()
		{
			EventLog securityLog = GetSecurityLog();
			List<EventLogEntry> eventList = GetEvents(securityLog);
			return eventList;
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
	}
}
