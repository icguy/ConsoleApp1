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
				string typeChar; ;
				switch( workEvent.Type )
				{
					case EventType.Arrival:
						typeChar = "A";
						break;
					case EventType.Departure:
						typeChar = "D";
						break;
					case EventType.Unknown:
					default:
						typeChar = "U";
						break;
				}
				sb.AppendLine($"  {typeChar} {workEvent.Time.TimeOfDay}");
			}
			return sb.ToString();
		}
	}
}