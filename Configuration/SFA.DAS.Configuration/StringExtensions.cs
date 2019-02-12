using System.IO;

namespace SFA.DAS.Configuration
{
    public static class StringExtensions
    {
        public static Stream ToStream(this string source)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            
            writer.Write(source);
            writer.Flush();
            stream.Position = 0;
            
            return stream;
        }
    }
}