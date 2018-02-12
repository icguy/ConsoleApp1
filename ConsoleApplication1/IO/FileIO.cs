using System;
using System.IO;
using System.Runtime.Serialization.Json;
using ConsoleApplication1.Model;

namespace ConsoleApplication1.IO
{
	public class FileIO : IFileIO
	{
		private readonly string _filePath;
		public FileIO(string filePath)
		{
			_filePath = filePath;
		}

		public WorkTimes ReadFromFile()
		{
			using (var fileStream = File.Open(_filePath, FileMode.OpenOrCreate, FileAccess.Read))
			{
				var serializer = new DataContractJsonSerializer(typeof(WorkTimes));
				WorkTimes deserializedObject = null;
				try
				{
					deserializedObject = serializer.ReadObject(fileStream) as WorkTimes;
				}
				catch (Exception)
				{
				}
				return deserializedObject ?? new WorkTimes();
			}
		}

		public void WriteToFile(WorkTimes workTimes)
		{
			using (var fileStream = File.Open(_filePath, FileMode.Create, FileAccess.Write))
			{
				var serializer = new DataContractJsonSerializer(typeof(WorkTimes));
				serializer.WriteObject(fileStream, workTimes);
			}
		}
	}
}
