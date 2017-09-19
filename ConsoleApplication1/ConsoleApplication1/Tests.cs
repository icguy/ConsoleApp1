﻿using System;
using System.Diagnostics;
using ConsoleApplication1.Model;

namespace ConsoleApplication1
{
	class Tests
	{
		public void RunTests()
		{
			Debug.Assert(this.T001_DailyWork_FromEvents(), "T001");
			Debug.Assert(this.T002_DailyWork_FromEvents(), "T002");
			Debug.Assert(this.T003_DailyWork_FromEvents(), "T003");
			Debug.Assert(this.T004_DailyWork_FromEvents(), "T004");
			Debug.Assert(this.T005_DailyWork_FromEvents(), "T005");
			Debug.Assert(this.T006_DailyWork_FromEvents(), "T006");
			Debug.Assert(this.T007_FileIO(), "T007");
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

			var dailywork = DailyWork.FromWorkEvents(events);
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

			var dailywork = DailyWork.FromWorkEvents(events);
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

			var dailywork = DailyWork.FromWorkEvents(events);
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

			var dailywork = DailyWork.FromWorkEvents(events);
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

			var dailywork = DailyWork.FromWorkEvents(events);
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

			var dailywork = DailyWork.FromWorkEvents(events);
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

			FileIO.WriteToFile("testfile.json", wt);
			var wt2 = FileIO.ReadFromFile("testfile.json");
			if (!TSEquals(wt2.Balance, wt.Balance))
				return false;
			if (wt2.DailyWorks.Length != wt.DailyWorks.Length)
				return false;
			for (int i = 0; i < wt.DailyWorks.Length; i++)
			{
				if (!TSEquals(wt.DailyWorks[i].Balance, wt2.DailyWorks[i].Balance))
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
}
