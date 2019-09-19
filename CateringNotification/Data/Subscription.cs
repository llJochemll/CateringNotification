using CateringNotification.Utility;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CateringNotification.Data
{
    internal class Subscription : TableEntity
    {
        public class UserSubscription
        {
            public enum Campus
            {
                DiepenBeek,
                ElfdeLinie,
                VilderStraat
            }

            public class NotificationSetting
            {
                internal int Hour { get; set; }
                internal int Minute { get; set; }
            }

            public Dictionary<DayOfWeek, KeyValuePair<Campus, NotificationSetting>> NotificationSettings { get; set; }
        }

        public string Email { get; set; }

        internal UserSubscription CurrentSubscription { get; set; }
        public string CurrentSubscriptionSerialized {
            get => JsonConvert.SerializeObject(CurrentSubscription);
            set => CurrentSubscription = JsonConvert.DeserializeObject<UserSubscription>(value);
        }
        public UserSubscription RequestedSubscription { get; set; }
        public string RequestedSubscriptionSerialized {
            get => JsonConvert.SerializeObject(RequestedSubscription);
            set => RequestedSubscription = JsonConvert.DeserializeObject<UserSubscription>(value);
        }

        internal async Task InsertOrReplaceAsync()
        {
            PartitionKey = "subscription";
            RowKey = Email;

            var table = await Table.GetTableAsync("catering");

            await table.ExecuteAsync(TableOperation.InsertOrReplace(this));
        }

        internal static async Task<Subscription> RetrieveAsync(string email)
        {
            var table = await Table.GetTableAsync("catering");
            return (await table.ExecuteAsync(TableOperation.Retrieve<Subscription>("subscription", email))).Result as Subscription;
        }
    }
}
