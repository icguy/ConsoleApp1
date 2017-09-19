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
			new WorkTimesBuilderTest().RunTests();

			List<EventLogEntry> eventList = LogReader.GetSecurityEvents();
			eventList.ForEach((e) => Console.WriteLine($"category: {e.Category}, type: {e.EntryType}, id: {e.InstanceId}, time1: {e.TimeGenerated}, type: {WorkTimesBuilder.GetEventType(e.InstanceId)}"));
			Console.WriteLine();
			var wtb = new WorkTimesBuilder();
			wtb.Build(null, eventList);

			Console.ReadLine();
		}
	}
}