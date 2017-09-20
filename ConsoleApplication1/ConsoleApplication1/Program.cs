using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ConsoleApplication1.Model;
using Microsoft.Win32;

namespace ConsoleApplication1
{
	class Program
	{
		const string dataFile = "times.json";
		static void Main(string[] args)
		{
			if( args.Contains("/help") )
			{
				PrintHelp();
				return;
			}

			if( args.Contains("/tests") )
				new Tests().RunTests();

			RegistryKey rk = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
			if( args.Contains("/addregistry") )
			{
				rk.SetValue("WorkTime", System.Reflection.Assembly.GetExecutingAssembly().Location);
				Console.WriteLine("Now running on startup");
			}
			else if( args.Contains("/removeregistry") )
			{
				rk.DeleteValue("WorkTime");
				Console.WriteLine("Disabled running on startup");
				Console.ReadLine();
				return;
			}

			List<EventLogEntry> eventList = LogReader.GetSecurityEvents();
			eventList.ForEach(e => e.PrintEvent());
			Console.WriteLine();

			var workTimes = FileIO.ReadFromFile(dataFile);
			BuildWorkTimes(workTimes, eventList);
			FileIO.WriteToFile(dataFile, workTimes);
			Console.WriteLine();

			var count = workTimes.DailyWorks.Count();
			var lastN = workTimes.DailyWorks.Skip(count - 5);
			foreach( var dailyWork in lastN )
			{
				Console.WriteLine(dailyWork);
			}
			Console.WriteLine("total:");
			Console.WriteLine(workTimes.Balance);

			Console.ReadLine();
		}

		public static WorkTimes BuildWorkTimes(WorkTimes workTimes, IEnumerable<EventLogEntry> events)
		{
			if( workTimes == null )
				workTimes = new WorkTimes();

			var workEvents = events.Select(e => e.ToWorkEvent());
			workTimes.AddWorks(workEvents);
			return workTimes;
		}

		public static void PrintHelp()
		{
			Console.WriteLine("Switches:");
			Console.WriteLine("/help: this help page");
			Console.WriteLine("/tests: run tests at startup");
			Console.WriteLine("/addregistry: run at startup");
			Console.WriteLine("/removeregistry: disables run at startup, then exits");
		}
	}
}