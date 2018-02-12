using ConsoleApplication1.Model;

namespace ConsoleApplication1.IO
{
	public interface IFileIO
	{
		WorkTimes ReadFromFile();

		void WriteToFile(WorkTimes workTimes);
	}
}
