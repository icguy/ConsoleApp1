using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1.Model
{
	public class WorkTimes
	{
		public TimeSpan Balance { get; set; } = TimeSpan.Zero;
		public DailyWork[] DailyWorks { get; set; } = new DailyWork[0];
	}
}
