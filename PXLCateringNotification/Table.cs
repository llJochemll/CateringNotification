using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace PXLCateringNotification
{
    static class Table
    {
        public static async Task<CloudTable> GetTableAsync(string table)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("STORAGE_CONNECTION"));
            var cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            var subscriptionsTable = cloudTableClient.GetTableReference(table);

            await subscriptionsTable.CreateIfNotExistsAsync();

            return subscriptionsTable;
        }
    }
}
