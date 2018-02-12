using System;

namespace ConsoleApplication1.IO
{
	class ConsoleInput : IUserInput
	{
		public string ReadLine()
		{
			return Console.ReadLine();
		}
	}
}
