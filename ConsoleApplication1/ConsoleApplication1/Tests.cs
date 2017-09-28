using System;
using System.Diagnostics;
using System.Linq;
using ConsoleApplication1.Model;
using ConsoleApplication1.IO;

namespace ConsoleApplication1
{
	class Tests : WorkTimeApp
	{
		public Tests() : base(new TestFileIO(), new TestInput())
		{

		}

		public void RunTests()
		{
			Debug.Assert(this.T001_DailyWork_FromEvents(), "T001");
			Debug.Assert(this.T002_DailyWork_FromEvents(), "T002");
			Debug.Assert(this.T003_DailyWork_FromEvents(), "T003");
			Debug.Assert(this.T004_DailyWork_FromEvents(), "T004");
			Debug.Assert(this.T005_DailyWork_FromEvents(), "T005");
			Debug.Assert(this.T006_DailyWork_FromEvents(), "T006");
			Debug.Assert(this.T007_FileIO(), "T007");
			Debug.Assert(this.T008_DeleteDay(), "T008");
			Debug.Assert(this.T009_Recalculate(), "T009");
			Console.WriteLine("Testing finished");
			Console.ReadLine();
		}
		
		bool T001_DailyWork_FromEvents()
		{
			WorkEvent[] events = new WorkEvent[] {
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 09, 12, 00),
					Type = EventType.Arrival
				},
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 12, 12, 00),
					Type = EventType.Departure
				},
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 12, 52, 00),
					Type = EventType.Arrival
				},
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 17, 52, 00),
					Type = EventType.Departure
				}
			};

			var dailywork = Utils.CreateDailyWork(events);
			return TSEquals(dailywork.Balance, new TimeSpan(0, 0, 0));
		}
		bool T002_DailyWork_FromEvents()
		{
			WorkEvent[] events = new WorkEvent[] {
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 09, 12, 00),
					Type = EventType.Departure
				},
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 12, 12, 00),
					Type = EventType.Departure
				},
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 12, 52, 00),
					Type = EventType.Departure
				},
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 17, 52, 00),
					Type = EventType.Departure
				}
			};

			var dailywork = Utils.CreateDailyWork(events);
			return TSEquals(dailywork.Balance, new TimeSpan(-8, 0, 0));
		}
		bool T003_DailyWork_FromEvents()
		{
			WorkEvent[] events = new WorkEvent[] {
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 09, 12, 00),
					Type = EventType.Arrival
				},
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 12, 12, 00),
					Type = EventType.Arrival
				},
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 12, 52, 00),
					Type = EventType.Arrival
				},
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 17, 52, 00),
					Type = EventType.Arrival
				}
			};

			var dailywork = Utils.CreateDailyWork(events);
			return TSEquals(dailywork.Balance, new TimeSpan(-8, 0, 0));
		}
		bool T004_DailyWork_FromEvents()
		{
			WorkEvent[] events = new WorkEvent[] {
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 09, 12, 00),
					Type = EventType.Arrival
				},
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 12, 12, 00),
					Type = EventType.Departure
				},
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 12, 52, 00),
					Type = EventType.Departure
				},
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 17, 52, 00),
					Type = EventType.Arrival
				}
			};

			var dailywork = Utils.CreateDailyWork(events);
			return TSEquals(dailywork.Balance, new TimeSpan(-5, 0, 0));
		}
		bool T005_DailyWork_FromEvents()
		{
			WorkEvent[] events = new WorkEvent[] {
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 09, 12, 00),
					Type = EventType.Departure
				},
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 12, 12, 00),
					Type = EventType.Arrival
				},
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 12, 42, 00),
					Type = EventType.Arrival
				},
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 17, 42, 00),
					Type = EventType.Departure
				}
			};

			var dailywork = Utils.CreateDailyWork(events);
			return TSEquals(dailywork.Balance, new TimeSpan(-2, -30, 0));
		}
		bool T006_DailyWork_FromEvents()
		{
			WorkEvent[] events = new WorkEvent[] {
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 09, 12, 00),
					Type = EventType.Departure
				},
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 12, 12, 00),
					Type = EventType.Arrival
				},
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 12, 42, 00),
					Type = EventType.Departure
				},
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 17, 52, 00),
					Type = EventType.Arrival
				}
			};

			var dailywork = Utils.CreateDailyWork(events);
			return TSEquals(dailywork.Balance, new TimeSpan(-7, -30, 0));
		}
		bool T007_FileIO()
		{
			WorkEvent[] events = new WorkEvent[] {
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 09, 12, 00),
					Type = EventType.Arrival
				},
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 12, 12, 00),
					Type = EventType.Departure
				},
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 02, 12, 42, 00),
					Type = EventType.Arrival
				},
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 02, 17, 52, 00),
					Type = EventType.Departure
				}
			};

			WorkTimes wt = new WorkTimes();
			wt.AddWorks(events);

			_fileIO.WriteToFile(wt);
			var wt2 = _fileIO.ReadFromFile();
			if( !TSEquals(wt2.Balance, wt.Balance) )
				return false;
			if( wt2.DailyWorks.Length != wt.DailyWorks.Length )
				return false;
			for( int i = 0; i < wt.DailyWorks.Length; i++ )
			{
				if( !TSEquals(wt.DailyWorks[i].Balance, wt2.DailyWorks[i].Balance) )
					return false;
			}
			return true;
		}
		bool T008_DeleteDay()
		{
			WorkEvent[] events = new WorkEvent[] {
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 09, 12, 00),
					Type = EventType.Arrival
				},
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 01, 12, 12, 00),
					Type = EventType.Departure
				},
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 02, 12, 42, 00),
					Type = EventType.Arrival
				},
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 02, 17, 52, 00),
					Type = EventType.Departure
				},
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 03, 17, 52, 00),
					Type = EventType.Arrival
				},
				new WorkEvent()
				{
					Time = new DateTime(2017, 06, 03, 17, 55, 00),
					Type = EventType.Departure
				}
			};
			var workTimes = new WorkTimes();
			workTimes.AddWorks(events);
			_fileIO.WriteToFile(workTimes);
			this.DeleteDay(new[] { "/deleteday", "2017.06.02" });
			var newWorkTimes = _fileIO.ReadFromFile();
			if (newWorkTimes.DailyWorks.FirstOrDefault(dw => dw.Events.First().Time.Day == 2) != null)
				return false;
			if (newWorkTimes.DailyWorks.FirstOrDefault(dw => dw.Events.First().Time.Day == 1).Events.Count() != 2)
				return false;
			if (newWorkTimes.DailyWorks.FirstOrDefault(dw => dw.Events.First().Time.Day == 3).Events.Count() != 2)
				return false;
			return true;
		}
		bool T009_Recalculate()
		{
			WorkTimes workTimes = new WorkTimes()
			{
				DailyWorks = new DailyWork[] {
					new DailyWork() {
						 Events = new WorkEvent[] {
							new WorkEvent() { Time = new DateTime(2017, 06, 20, 9, 00, 00), Type = EventType.Arrival },
							new WorkEvent() { Time = new DateTime(2017, 06, 20, 18, 00, 00), Type = EventType.Departure }
						 }
					},
					new DailyWork() {
						 Events = new WorkEvent[] {
							new WorkEvent() { Time = new DateTime(2017, 06, 21, 9, 00, 00), Type = EventType.Arrival },
							new WorkEvent() { Time = new DateTime(2017, 06, 21, 18, 00, 00), Type = EventType.Departure }
						 }

					},
					new DailyWork() {
						 Events = new WorkEvent[] {
							new WorkEvent() { Time = new DateTime(2017, 06, 22, 9, 00, 00), Type = EventType.Arrival },
							new WorkEvent() { Time = new DateTime(2017, 06, 22, 18, 00, 00), Type = EventType.Departure }
						 }

					},
					new DailyWork() {
						 Events = new WorkEvent[] {
							new WorkEvent() { Time = new DateTime(2017, 06, 23, 9, 00, 00), Type = EventType.Arrival },
							new WorkEvent() { Time = new DateTime(2017, 06, 23, 18, 00, 00), Type = EventType.Departure }
						 }

					},
				}
			};

			workTimes.Recalculate();

			if ( !TSEquals(workTimes.Balance, new TimeSpan(4, 0, 0)) )
				return false;
			foreach ( var dailyWork in workTimes.DailyWorks )
			{
				if ( !TSEquals(dailyWork.Balance, new TimeSpan(1, 0, 0)) )
					return false;
			}
			return true;
		}
		bool T010_EditDay()
		{
			WorkTimes workTimes = new WorkTimes()
			{
				DailyWorks = new DailyWork[] {
					new DailyWork() {
						 Events = new WorkEvent[] {
							new WorkEvent() { Time = new DateTime(2017, 06, 20, 9, 00, 00), Type = EventType.Arrival },
							new WorkEvent() { Time = new DateTime(2017, 06, 20, 18, 00, 00), Type = EventType.Departure }
						 }
					},
					new DailyWork() {
						 Events = new WorkEvent[] {
							new WorkEvent() { Time = new DateTime(2017, 06, 21, 9, 00, 00), Type = EventType.Arrival },
							new WorkEvent() { Time = new DateTime(2017, 06, 21, 18, 00, 00), Type = EventType.Departure }
						 }

					},
					new DailyWork() {
						 Events = new WorkEvent[] {
							new WorkEvent() { Time = new DateTime(2017, 06, 22, 9, 00, 00), Type = EventType.Arrival },
							new WorkEvent() { Time = new DateTime(2017, 06, 22, 18, 00, 00), Type = EventType.Departure }
						 }

					},
					new DailyWork() {
						 Events = new WorkEvent[] {
							new WorkEvent() { Time = new DateTime(2017, 06, 23, 9, 00, 00), Type = EventType.Arrival },
							new WorkEvent() { Time = new DateTime(2017, 06, 23, 18, 00, 00), Type = EventType.Departure }
						 }

					},
				}
			};

			workTimes.Recalculate();

			if ( !TSEquals(workTimes.Balance, new TimeSpan(4, 0, 0)) )
				return false;
			foreach ( var dailyWork in workTimes.DailyWorks )
			{
				if ( !TSEquals(dailyWork.Balance, new TimeSpan(1, 0, 0)) )
					return false;
			}
			return true;
		}

		static bool TSEquals(TimeSpan ts1, TimeSpan ts2)
		{
			TimeSpan delta = TimeSpan.FromMilliseconds(10);
			return -delta < ts1 - ts2 && ts1 - ts2 < delta;
		}
	}

	class TestInput : IUserInput
	{
		public Func<string> OnReadline = () => { throw new NotImplementedException(); };
		public string ReadLine()
		{
			return OnReadline();
		}
	}

	class TestFileIO : IFileIO
	{
		public WorkTimes WorkTimes { get; set; }

		public TestFileIO()
		{

		}

		public WorkTimes ReadFromFile()
		{
			return this.WorkTimes;
		}

		public void WriteToFile(WorkTimes workTimes)
		{
			this.WorkTimes = workTimes;
		}
	}
}
