using System;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace SFA.DAS.EntityFramework
{
    public static class QueryFutureEnumerableExtensions
    {
        public static async Task<T> SingleOrDefaultAsync<T>(this QueryFutureEnumerable<T> source)
        {
            if (!source.HasValue)
            {
                await source.OwnerBatch.ExecuteQueriesAsync().ConfigureAwait(false);
            }

            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    return default(T);
                }

                var current = enumerator.Current;

                if (!enumerator.MoveNext())
                {
                    return current;
                }
            }

            throw new InvalidOperationException("Sequence contains more than one element");
        }

        public static Task<T> ValueAsync<T>(this QueryFutureEnumerable<T> source) where T : struct
        {
            return source.SingleOrDefaultAsync();
        }
    }
}
