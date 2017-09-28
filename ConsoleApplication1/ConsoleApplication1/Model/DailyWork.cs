using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1.Model
{
	public class DailyWork
	{
		public TimeSpan Balance { get; set; } = TimeSpan.Zero;
		public WorkEvent[] Events { get; set; } = new WorkEvent[0];
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine($"{this.Events.FirstOrDefault().Time.Date.ToShortDateString()} | {this.Balance}");
			foreach( var workEvent in this.Events )
			{
				sb.AppendLine($"  {workEvent.ToString()}");
			}
			return sb.ToString();
		}
	}
}