using System;
using System.Diagnostics;
using System.Linq;

namespace ConsoleApplication1.Model
{
	public class WorkEvent
	{
		public EventType Type = EventType.Unknown;
		public DateTime Time = DateTime.MinValue;
	}
}
