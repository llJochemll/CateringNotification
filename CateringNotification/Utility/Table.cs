using Microsoft.Azure.Cosmos.Table;
using System;
using System.Threading.Tasks;

namespace CateringNotification.Utility
{
    internal static class Table
    {
        internal static async Task<CloudTable> GetTableAsync(string tableName)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("STORAGE_CONNECTION"));
            var cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            var table = cloudTableClient.GetTableReference(tableName);

            await table.CreateIfNotExistsAsync();

            return table;
        }
    }
}
