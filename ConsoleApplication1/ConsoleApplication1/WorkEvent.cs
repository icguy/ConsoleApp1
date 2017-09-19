using System;
using System.Diagnostics;

namespace ConsoleApp1
{
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
}
