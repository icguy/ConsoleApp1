using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ConsoleApplication1.Model;
using Microsoft.Win32;

namespace ConsoleApplication1
{
	public class Program
	{
		const string dataFile = "times.json";
		static void Main(string[] args)
		{
			new WorkTimeApp(dataFile).Run(args);
			Console.ReadLine();
		}
	}
	public class WorkTimeApp
	{
		protected readonly string _dataFile = "times.json";

		public WorkTimeApp(string dataFile)
		{
			_dataFile = dataFile;
		}

		public void Run(string[] args)
		{
			if( args.Contains("/help") )
			{
				this.PrintHelp();
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
				return;
			}

			if( args.Contains("/deleteday") )
			{
				this.DeleteDay(args);
				return;
			}

			List<EventLogEntry> eventList = LogReader.GetSecurityEvents();
			eventList.ForEach(e => e.PrintEvent());
			Console.WriteLine();

			var workTimes = FileIO.ReadFromFile(_dataFile);
			this.BuildWorkTimes(workTimes, eventList);
			FileIO.WriteToFile(_dataFile, workTimes);
			Console.WriteLine();

			var count = workTimes.DailyWorks.Count();
			var lastN = workTimes.DailyWorks.Skip(count - 5);
			foreach( var dailyWork in lastN )
			{
				Console.WriteLine(dailyWork);
			}
			Console.WriteLine("total:");
			Console.WriteLine(workTimes.Balance);
		}

		protected void DeleteDay(string[] args)
		{
			int year = -1;
			int month = -1;
			int day = -1;
			try
			{
				int argIdx = args.ToList().IndexOf("/deleteday");
				string dateString = args[argIdx + 1];
				var parts = dateString.Split(new char[] { '.' });
				year = int.Parse(parts[0]);
				month = int.Parse(parts[1]);
				day = int.Parse(parts[2]);
			}
			catch( Exception )
			{
				Console.WriteLine("please specify a date in YYYY.MM.DD format");
				return;
			}

			var workTimes = FileIO.ReadFromFile(_dataFile);

			DateTime date = new DateTime(year, month, day);
			var dailyWork = workTimes.DailyWorks.ToList().FirstOrDefault(dw => dw.Events.First().Time.Date == date);
			if( dailyWork == null )
			{
				Console.WriteLine("Could not find the specified date.");
				return;
			}

			var newDailyWorks = workTimes.DailyWorks.ToList();
			newDailyWorks.Remove(dailyWork);
			workTimes.DailyWorks = newDailyWorks.ToArray();

			FileIO.WriteToFile(_dataFile, workTimes);
		}

		public WorkTimes BuildWorkTimes(WorkTimes workTimes, IEnumerable<EventLogEntry> events)
		{
			if( workTimes == null )
				workTimes = new WorkTimes();

			var workEvents = events.Select(e => e.ToWorkEvent());
			workTimes.AddWorks(workEvents);
			return workTimes;
		}

		void PrintHelp()
		{
			Console.WriteLine("Switches:");
			Console.WriteLine("/help: this help page");
			Console.WriteLine("/tests: run tests at startup");
			Console.WriteLine("/addregistry: run at startup");
			Console.WriteLine("/removeregistry: disables run at startup, then exits");
			Console.WriteLine("/deleteday YYYY.MM.DD: deletes the specified day");
		}
	}
}