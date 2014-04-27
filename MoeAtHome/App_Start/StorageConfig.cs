using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MoeAtHome
{
    public static class StorageConfig
    {
        private static CloudTableClient tableClient;
        public static CloudTableClient TableClient
        {
            get { return tableClient; }
        }

        public static void CreateTablesQueuesBlobContainerIfNotExists()
        {
            var connectionString = ConfigurationManager
                .ConnectionStrings["StorageConnectionString"].ConnectionString;
            var storageAccount = CloudStorageAccount.Parse(connectionString);

            CreateTablesIfNotExists(storageAccount);
        }

        private static void CreateTablesIfNotExists(CloudStorageAccount account)
        {
            tableClient = account.CreateCloudTableClient();
        }
    }
}