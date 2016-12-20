using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Sfa.Automation.Framework.AzureUtils
{
    internal class AzureTableStorage
    {
        private string ConnectionString { get; set; }
        private List<string> TablesToExclude { get; set; }

        internal AzureTableStorage(string connectionString, List<string> tablesToExclude)
        {
            ConnectionString = connectionString;
            TablesToExclude = tablesToExclude;
        }

        internal List<CloudTable> GetAllTableNamesForAzureAccount(bool removeWhiteList)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);
            CloudTableClient tableClient = new CloudTableClient(storageAccount.TableStorageUri, storageAccount.Credentials);
            IEnumerable<CloudTable> tables = tableClient.ListTables().Where(a => !a.Name.StartsWith("WAD"));
            if (removeWhiteList)
            {
                return RemoveWhiteListTables(tables, TablesToExclude).ToList();
            }
            return tables.ToList();
        }

        private IEnumerable<CloudTable> RemoveWhiteListTables(IEnumerable<CloudTable> tables, List<string> tablesToExclude)
        {
            var whiteList = new List<CloudTable>();
            foreach (var table in tables.ToArray())
            {
                if (!tablesToExclude.Contains(table.Name))
                {
                    whiteList.Add(table);
                }
            }
            return whiteList;
        }

    }
}
