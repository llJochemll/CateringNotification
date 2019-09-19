using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;

namespace CateringNotification.Utility
{
    internal static class Verification
    {
        private static readonly Random random = new Random();

        internal static async Task<string> SetVerify(string email)
        {
            var verificationToken = new VerificationToken
            {
                RowKey = email,
                PartitionKey = "verification",
                Token = random.Next(100000, 999999).ToString()
            };

            var table = await Table.GetTableAsync("catering");
            await table.ExecuteAsync(TableOperation.InsertOrReplace(verificationToken));

            return verificationToken.Token;
        }

        internal static async Task<bool> Verify(string email, string token)
        {
            var verificationsTable = await Table.GetTableAsync("catering");
            var result = (await verificationsTable.ExecuteAsync(TableOperation.Retrieve<VerificationToken>("ver", email))).Result as VerificationToken;

            return result?.Token == token;
        }

        private class VerificationToken : TableEntity
        {
            internal string Token { get; set; }
        }
    }
}
