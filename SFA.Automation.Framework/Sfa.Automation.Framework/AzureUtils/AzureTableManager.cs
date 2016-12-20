using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;
using Sfa.Automation.Framework.Constants;

namespace Sfa.Automation.Framework.AzureUtils
{
    public class AzureTableManager
    {
        private readonly CloudTable _cloudTable;

        /// <summary>
        /// Create a connection to the Azure Table storage and connects to the requested table.
        /// </summary>
        /// <param name="connectionString">The connection string to connect to Azure Table Storage</param>
        /// <param name="tableName">The Azure table to connect to</param>
        public AzureTableManager(string connectionString, string tableName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient cloudTableClient = storageAccount.CreateCloudTableClient();
            cloudTableClient.DefaultRequestOptions.RetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(5), 3);
            _cloudTable = cloudTableClient.GetTableReference(tableName);
        }

        /// <summary>
        /// Creates the table in the Azure server.
        /// </summary>
        /// <returns>The result of the creation of the Azure table</returns>
        public bool CreateTable()
        {
            bool success = false;
            int count = 0;
            if (!_cloudTable.Exists())
            {
                while (!success && (count != Timeouts.DefaultTimeOut))
                {
                    try
                    {
                        count = count + Timeouts.OneSecond;
                        success = _cloudTable.CreateIfNotExists();
                    }
                    catch
                    {
                        Thread.Sleep(Timeouts.OneSecond);
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// Deletes the table in the Azure server.
        /// </summary>
        /// <returns>The result of the deletion of the Azure table</returns>
        public bool DeleteTable()
        {
            return _cloudTable.DeleteIfExists();
        }

        /// <summary>
        /// Inserts the table entity into the Azure Table storage
        /// </summary>
        /// <param name="tableEntity">The entity to add to the Azure Table Storage</param>
        /// <returns>The table result from the Execute statement</returns>
        public TableResult Insert(TableEntity tableEntity)
        {
            return _cloudTable.Execute(TableOperation.Insert(tableEntity));
        }

        /// <summary>
        /// Updates the table entity into the Azure Table storage
        /// </summary>
        /// <param name="tableEntity">The entity to update to the Azure Table Storage</param>
        /// <returns>The table result from the Execute statement</returns>
        public TableResult Update(TableEntity tableEntity)
        {
            return _cloudTable.Execute(TableOperation.Replace(tableEntity));
        }

        /// <summary>
        /// Deletes the table entity from the Azure Table storage
        /// </summary>
        /// <param name="tableEntity">The entity to delete from the Azure Table Storage</param>
        /// <returns>The table result from the Execute statement</returns>
        public TableResult Delete(TableEntity tableEntity)
        {
            return _cloudTable.Execute(TableOperation.Delete(tableEntity));
        }

        /// <summary>
        /// Returns a single table entity from Table Storage
        /// </summary>
        /// <param name="partitionKey">The table entities partition key</param>
        /// <param name="rowKey">The table entities row key</param>
        /// <typeparam name="T">The TableEntity type</typeparam>
        /// <returns>THe table entity from Table Storage</returns>
        public T GetSingleTableEntity<T>(string partitionKey, string rowKey) where T : TableEntity
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            TableResult retrievedResult = _cloudTable.Execute(retrieveOperation);
            return retrievedResult.Result as T;
        }

        /// <summary>
        /// Return a list of entities from the Azure Table
        /// </summary>
        /// <returns>A List collection of all the Table Entities</returns>
        public List<DynamicTableEntity> GetAllTableEntities()
        {
            TableContinuationToken token = null;
            var entities = new List<DynamicTableEntity>();
            do
            {
                var queryResult = _cloudTable.ExecuteQuerySegmented(new TableQuery(), token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null);
            return entities;
        }

        /// <summary>
        /// Returns an enumerable list of entities from the Azure Table
        /// </summary>
        /// <typeparam name="T">Table Entity object</typeparam>
        /// <returns>Enumerable list of table entity object of T</returns>
        public IEnumerable<T> GetAllTableEntities<T>() where T : ITableEntity, new()
        {
            var allEntities = new List<T>();
            TableContinuationToken token = null;
            do
            {
                var queryResponse = _cloudTable.ExecuteQuerySegmented(new TableQuery<T>(), token);
                token = queryResponse.ContinuationToken;
                allEntities.AddRange(queryResponse.Results);
            }
            while (token != null);
            return allEntities;
        }

        /// <summary>
        /// Returns the number of entities from table storage
        /// </summary>
        /// <returns>A count of the number of entities in the storage area</returns>
        public int GetNumberOfEntities()
        {
            return _cloudTable.ExecuteQuery(new TableQuery()).Count();
        }

    }
}
