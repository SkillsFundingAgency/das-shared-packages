using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Sfa.Automation.Framework.Constants;

namespace Sfa.Automation.Framework.AzureUtils
{
    public class AzureBlobManager
    {
        private readonly CloudBlobContainer _cloudBlobContainer;

        /// <summary>
        /// Create a connection to the Azure Blob storage and connects to the requested blob container.
        /// </summary>
        /// <param name="connectionString">The connection string to connect to Azure Blob Storage</param>
        /// <param name="blobContainer">The Azure Blob to connect to</param>
        public AzureBlobManager(string connectionString, string blobContainer)
        {
            string blobContainer1 = blobContainer.ToLower();
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
            cloudBlobClient.DefaultRequestOptions.RetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(5), 3);
            _cloudBlobContainer = cloudBlobClient.GetContainerReference(blobContainer1);
        }

        /// <summary>
        /// Creates the Blob container if it does not already exist
        /// </summary>
        /// <returns>Returns the result of the Blob Creation</returns>
        public bool CreateBlobContainer()
        {
            bool success = false;
            int count = 0;
            while (!success && (count != Timeouts.AzureTimeOut))
            {
                try
                {
                    count = count + Timeouts.OneSecond;
                    success = _cloudBlobContainer.CreateIfNotExists();
                }
                catch
                {
                    Thread.Sleep(Timeouts.OneSecond);
                }
            }
            return success;
        }

        /// <summary>
        /// Deletes teh Blob container if it exists
        /// </summary>
        /// <returns></returns>
        public bool DeleteBlobContainer()
        {
            return _cloudBlobContainer.DeleteIfExists();
        }

        /// <summary>
        /// Gets the URL of the Blob container
        /// </summary>
        /// <returns>Returns the URL</returns>
        public Uri GetBlobContainerUri()
        {
            return _cloudBlobContainer.Uri;
        }

        /// <summary>
        /// Uploads the Blob to the Blob container
        /// </summary>
        /// <param name="memoryStream">Memory Stream to upload</param>
        /// <param name="blobName">Name of the Blob</param>
        /// <returns>The Url of the uploaded Blob</returns>
        public Uri UpLoadBlobIntoContainer(MemoryStream memoryStream, string blobName)
        {
            CloudBlockBlob blockBlob = _cloudBlobContainer.GetBlockBlobReference(blobName);
            memoryStream.Position = 0;
            blockBlob.UploadFromStream(memoryStream);
            return blockBlob.Uri;
        }

        /// <summary>
        /// Downloads the Blob from the Blob container
        /// </summary>
        /// <param name="blobName">Name of the Blo</param>
        /// <returns>MemoryStream of the Blob</returns>
        public MemoryStream DownloadBlobFromContainer(string blobName)
        {
            CloudBlockBlob blockBlob = _cloudBlobContainer.GetBlockBlobReference(blobName);
            using (var memoryStream = new MemoryStream())
            {
                blockBlob.DownloadToStream(memoryStream);
                return memoryStream;
            }
        }

        /// <summary>
        /// Returns a list of Blobs stored in the container
        /// </summary>
        /// <returns>Collection of Blob's in a list</returns>
        public IEnumerable<IListBlobItem> GetListBlobsInContainer()
        {
            return _cloudBlobContainer.ListBlobs(null, true, BlobListingDetails.All);
        }

        /// <summary>
        /// Deletes the requested Blob from the container
        /// </summary>
        /// <param name="blobName">Name of the Blob</param>
        public void DeleteBlobItem(string blobName)
        {
            CloudBlockBlob blockBlob = _cloudBlobContainer.GetBlockBlobReference(blobName);
            blockBlob.Delete();
        }

        /// <summary>
        /// Counts the number of Blob's in the container
        /// </summary>
        /// <returns>Total number of items in the selected container</returns>
        public int Count()
        {
            return GetListBlobsInContainer().Count();
        }

        /// <summary>
        /// Deletes all the Blob's in the container
        /// </summary>
        public void DeleteAllBlobItems()
        {
            foreach (var blockBlob in GetListBlobsInContainer().Where(blob => blob is CloudBlockBlob).Cast<CloudBlockBlob>())
            {
                blockBlob.Delete();
            }
        }
    }
}
