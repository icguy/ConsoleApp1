using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication1.Model;

namespace ConsoleApplication1
{
	class WorkTimesBuilder
	{
		public WorkTimes Build(WorkTimes workTimes, IEnumerable<EventLogEntry> events)
		{
			if (workTimes == null)
				workTimes = new WorkTimes();

			var workEvents = events.Select(e => e.ToWorkEvent());
			workTimes.AddWorks(workEvents);
			return workTimes;
		}
	}
}
