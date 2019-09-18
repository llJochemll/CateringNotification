using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PXLCateringNotification.Utility
{
    static class Verification
    {
        static readonly Random random = new Random();

        public static async Task SetVerify(string email)
        {
            var verificationToken = new VerificationToken
            {
                RowKey = email,
                PartitionKey = "ver",
                Token = random.Next(100000, 999999).ToString()
            };

            var verificationsTable = await Table.GetTableAsync("subscriptions");
            await verificationsTable.ExecuteAsync(TableOperation.InsertOrReplace(verificationToken));
        }

        public static async Task<bool> Verify(string email, string token)
        {
            var verificationsTable = await Table.GetTableAsync("subscriptions");
            var result = (await verificationsTable.ExecuteAsync(TableOperation.Retrieve<VerificationToken>("ver", email))).Result as VerificationToken;

            return result.Token == token;
        }

        private class VerificationToken : TableEntity
        {
            public string Token { get; set; }
        }
    }
}
