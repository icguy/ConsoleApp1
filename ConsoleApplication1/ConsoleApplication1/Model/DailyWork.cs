using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication1.Model
{
	public class DailyWork
	{
		public TimeSpan Balance = TimeSpan.Zero;
		public WorkEvent[] Events = new WorkEvent[0];
	}
}
