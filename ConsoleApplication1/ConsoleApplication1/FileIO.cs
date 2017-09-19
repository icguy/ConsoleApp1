using System;
using System.IO;
using System.Runtime.Serialization.Json;
using ConsoleApplication1.Model;

namespace ConsoleApplication1
{
	static class FileIO
	{
		public static WorkTimes ReadFromFile(string filePath)
		{
			using (var fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Read))
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

		public static void WriteToFile(string filePath, WorkTimes workTimes)
		{
			using (var fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write))
			{
				var serializer = new DataContractJsonSerializer(typeof(WorkTimes));
				serializer.WriteObject(fileStream, workTimes);
			}
		}
	}
}
