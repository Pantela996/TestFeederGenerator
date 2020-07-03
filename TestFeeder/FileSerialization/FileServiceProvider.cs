using GraphX.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAXLib;

namespace TestFeeder.FileSerialization
{
    public static class FileServiceProvider { 


        //opening stream and writing
        public static void SerializeDataToFile(string filename, List<GraphSerializationData> modelsList)
        {
            using (FileStream stream = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                SerializeDataToStream(stream, modelsList);
            }
        }


        //opening stream and reading
        public static List<GraphSerializationData> DeserializeDataFromFile(string filename)
        {
            using (FileStream stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return DeserializeDataFromStream(stream);
            }
        }


        public static void SerializeDataToStream(Stream stream, List<GraphSerializationData> modelsList)
        {
            var serializer = new YAXSerializer(typeof(List<GraphSerializationData>));
            using (var textWriter = new StreamWriter(stream))
            {
                serializer.Serialize(modelsList, textWriter);
                textWriter.Flush();
            }
        }

        public static List<GraphSerializationData> DeserializeDataFromStream(Stream stream)
        {
            var deserializer = new YAXSerializer(typeof(List<GraphSerializationData>));
            using (var textReader = new StreamReader(stream))
            {
                return (List<GraphSerializationData>)deserializer.Deserialize(textReader);
            }
        }
}
}
