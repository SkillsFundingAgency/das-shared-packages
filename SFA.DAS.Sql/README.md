### Example for using the MappedQueryAsync method:

connection is a `SqlConnection`
```C#
var result = await connection.MappedQueryAsync<Parent, Child>(
              query.RawSql,
              query.Parameters,
              splitOn: c => c.UniqueChildId,
              parentIdentifier: p => p.UniqueParentId,
              childCollection: p => p.CollectionOfChildren
          ).ConfigureAwait(false);
```
