using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication1.Model;

namespace ConsoleApplication1.IO
{
	public interface IFileIO
	{
		WorkTimes ReadFromFile();

		void WriteToFile(WorkTimes workTimes);
	}
}
