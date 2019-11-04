using Microsoft.Azure.Cosmos.Table;
using System;
using System.Threading.Tasks;

namespace CateringNotification.Utility
{
    internal static class Verification
    {
        private static readonly Random random = new Random();

        internal static async Task<string> SetVerify(string email)
        {
            var token = random.Next(100000, 999999).ToString();

            await SetToken(email, token);

            return token;
        }

        internal static async Task<bool> Verify(string email, string token)
        {
            var verificationsTable = await Table.GetTableAsync("catering");
            var result = (await verificationsTable.ExecuteAsync(TableOperation.Retrieve<VerificationToken>("verification", email))).Result as VerificationToken;

            await SetToken(email, null);

            return result?.Token == token;
        }

        private class VerificationToken : TableEntity
        {
            public string Token { get; set; }
        }

        private static async Task SetToken(string email, string token)
        {
            var verificationToken = new VerificationToken
            {
                RowKey = email,
                PartitionKey = "verification",
                Token = token
            };

            var table = await Table.GetTableAsync("catering");
            await table.ExecuteAsync(TableOperation.InsertOrReplace(verificationToken));
        }
    }
}
