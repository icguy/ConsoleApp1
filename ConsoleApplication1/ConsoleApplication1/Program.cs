using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ConsoleApplication1.Model;
using Microsoft.Win32;
using ConsoleApplication1.IO;

namespace ConsoleApplication1
{
	public class Program
	{
		const string dataFile = @" D:\aspnethello\ConsoleApp1\ConsoleApplication1\ConsoleApplication1\bin\Release\times.json";
		static void Main(string[] args)
		{
			new WorkTimeApp(new FileIO(dataFile), new ConsoleInput()).Run(args);
			Console.ReadLine();
		}
	}
	public class WorkTimeApp
	{
		protected readonly IUserInput _input;
		protected readonly IFileIO _fileIO;

		public WorkTimeApp(IFileIO fileIO, IUserInput input)
		{
			_fileIO = fileIO;
			_input = input;
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

			if( args.Contains("/editday") )
			{
				this.EditDay(args);
				return;
			}

			if( args.Contains("/ignoreday") )
			{
				this.IgnoreDay(args);
				return;
			}

			List<EventLogEntry> eventList = LogReader.GetSecurityEvents();
			eventList.ForEach(e => e.PrintEvent());
			Console.WriteLine();

			var workTimes = _fileIO.ReadFromFile();
			this.BuildWorkTimes(workTimes, eventList);
			_fileIO.WriteToFile(workTimes);
			Console.WriteLine();

			var count = workTimes.DailyWorks.Count();
			var lastN = workTimes.DailyWorks.Skip(count - 5);
			foreach( var dailyWork in lastN )
			{
				Console.WriteLine(dailyWork);
			}
			Console.WriteLine("total:");
			Console.WriteLine(workTimes.Balance);

			var workEventsToday = eventList.Select(e => e.ToWorkEvent()).Where(e => DateTime.Now.Date < e.Time).ToList();
			var now = DateTime.Now;
			var nowSeconds = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Kind);
			workEventsToday.Add(new WorkEvent() { Time = nowSeconds, Type = EventType.Departure });
			var todayWork = Utils.CreateDailyWork(workEventsToday, 8);
			DateTime expectedDeparture = GetExpectedDeparture(todayWork);
			Console.WriteLine();
			Console.WriteLine("today:");
			Console.WriteLine(todayWork);
			Console.WriteLine($"expected departure at: {expectedDeparture}");
		}

		protected DateTime GetExpectedDeparture(DailyWork todayWork)
		{
			var lastSignin = todayWork.Events.OrderByDescending(e => e.Time).FirstOrDefault(/*e => e.Type == EventType.Arrival*/);
			var expectedDeparture = lastSignin.Time - todayWork.Balance;
			return expectedDeparture;
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

			var workTimes = _fileIO.ReadFromFile();

			DateTime date = new DateTime(year, month, day);
			var dailyWork = workTimes.DailyWorks.ToList().FirstOrDefault(dw => dw.Events.First().Time.Date == date);
			if( dailyWork == null )
			{
				Console.WriteLine("Could not find the specified date.");
				return;
			}

			var newDailyWorks = workTimes.DailyWorks.Where(dw => dw.Events.First().Time.Date != date);
			workTimes.DailyWorks = newDailyWorks.ToArray();

			_fileIO.WriteToFile(workTimes);
		}

		protected void EditDay(string[] args)
		{
			int year = -1;
			int month = -1;
			int day = -1;
			try
			{
				int argIdx = args.ToList().IndexOf("/editday");
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

			var workTimes = _fileIO.ReadFromFile();

			DateTime date = new DateTime(year, month, day);
			var dailyWork = workTimes.DailyWorks.ToList().FirstOrDefault(dw => dw.Events.First().Time.Date == date);
			if( dailyWork == null )
			{
				Console.WriteLine("Could not find the specified date.");
				return;
			}

			var newWorks = workTimes.DailyWorks.Where(dw => dw.Events.First().Time.Date != date).ToList();

			var newWork = new DailyWork();
			var newEvents = new List<WorkEvent>();
			foreach( var e in dailyWork.Events )
			{
				while( true )
				{
					Console.Clear();
					Console.WriteLine(dailyWork);
					Console.WriteLine();
					Console.WriteLine(e.ToString());
					Console.WriteLine("Keep event? (y/n)");
					string resp = _input.ReadLine();
					if( resp.ToLower() == "y" )
					{
						newEvents.Add(e);
						break;
					}
					else if( resp.ToLower() == "n" )
					{
						break;
					}
				}
			}
			while( true )
			{
				Console.Write("Hours to work today: ");
				var hoursString = _input.ReadLine();
				if( int.TryParse(hoursString, out int hours) )
				{
					newWork.HoursToWorkToday = hours;
					break;
				}
				Console.WriteLine("Please write a number.");
			}
			newWork.Events = newEvents.ToArray();
			newWorks.Add(newWork);
			workTimes.DailyWorks = newWorks.ToArray();
			workTimes.Recalculate();
			Console.WriteLine();
			Console.WriteLine("Editing finished. Press enter to exit.");

			_fileIO.WriteToFile(workTimes);
		}

		protected void IgnoreDay(string[] args)
		{
			int year = -1;
			int month = -1;
			int day = -1;
			try
			{
				int argIdx = args.ToList().IndexOf("/ignoreday");
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

			var workTimes = _fileIO.ReadFromFile();

			DateTime date = new DateTime(year, month, day);
			var dailyWorks = workTimes.DailyWorks.ToList().Where(dw => dw.Events.First().Time.Date == date);
			if( !dailyWorks.Any() )
			{
				Console.WriteLine("Could not find the specified date.");
				return;
			}

			var newWorks = workTimes.DailyWorks.Where(dw => dw.Events.First().Time.Date != date).ToList();

			var newWork = new DailyWork();
			var newEvents = new List<WorkEvent>();
			newEvents.Add(new WorkEvent { Time = new DateTime(year, month, day, 15, 50, 0), Type = EventType.Arrival });
			newEvents.Add(new WorkEvent { Time = new DateTime(year, month, day, 23, 50, 0), Type = EventType.Departure });
			newWork.Events = newEvents.ToArray();
			newWorks.Add(newWork);
			workTimes.DailyWorks = newWorks.ToArray();
			workTimes.Recalculate();
			Console.WriteLine();
			Console.WriteLine("Editing finished. Press enter to exit.");

			_fileIO.WriteToFile(workTimes);
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
			Console.WriteLine("/editday YYYY.MM.DD: edits the specified day, recalculates, then exists.");
		}
	}
}