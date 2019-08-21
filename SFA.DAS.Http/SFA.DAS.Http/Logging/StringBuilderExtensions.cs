using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SFA.DAS.Http.Logging
{
    internal static class StringBuilderExtensions
    {
        public static StringBuilder AppendJoin(this StringBuilder stringBuilder, string separator, IEnumerable<string> values)
        {
            return values.Aggregate(stringBuilder, (b, v) => b.Append(v).Append(separator))
                .Remove(stringBuilder.Length - separator.Length, separator.Length);
        }
    }
}