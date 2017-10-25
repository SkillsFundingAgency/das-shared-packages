using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;

namespace SFA.DAS.Sql.Dapper
{
    public static class ConnectionExtensions
    {
        public static async Task<List<TParent>> MappedQueryAsync<TParent, TChild>(
            this SqlConnection connection,
            string sql, object parameters,
            Expression<Func<TChild, object>> splitOn,
            Func<TParent, object> parentIdentifier,
            Expression<Func<TParent, IList<TChild>>> childCollection) where TParent : class
        {
            var mapper = new ParentChildrenMapper<TParent, TChild>();
            var lookup = new Dictionary<object, TParent>();

            MemberExpression member;
            if (splitOn.Body is UnaryExpression)
            {
                member = ((UnaryExpression)splitOn.Body).Operand as MemberExpression;
            }
            else
            {
                member = splitOn.Body as MemberExpression;
            }
            var splitOnName = member?.Member.Name;

            await connection.QueryAsync(
                    sql,
                    param: parameters,
                    splitOn: splitOnName,
                    map: mapper.Map(lookup, parentIdentifier, childCollection))
                .ConfigureAwait(false);

            return lookup.Values.ToList();
        }
    }
}
