using System.IO;

namespace SFA.DAS.Configuration.Extensions
{
    public static class StringExtensions
    {
        /// <returns>Stream that contains the supplied source string. The caller is responsible for disposing the stream.</returns>
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