using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ConsoleApplication1.Model;

namespace ConsoleApplication1
{
	class Program
	{
		const string dataFile = "times.json";
		static void Main(string[] args)
		{
			new Tests().RunTests();

			List<EventLogEntry> eventList = LogReader.GetSecurityEvents();
			eventList.ForEach(e => e.PrintEvent());
			Console.WriteLine();

			var workTimes = FileIO.ReadFromFile(dataFile);
			BuildWorkTimes(workTimes, eventList);
			FileIO.WriteToFile(dataFile, workTimes);
			Console.WriteLine();
			Console.WriteLine(workTimes.Balance);

			Console.ReadLine();
		}

		public static WorkTimes BuildWorkTimes(WorkTimes workTimes, IEnumerable<EventLogEntry> events)
		{
			if (workTimes == null)
				workTimes = new WorkTimes();

			var workEvents = events.Select(e => e.ToWorkEvent());
			workTimes.AddWorks(workEvents);
			return workTimes;
		}
	}
}