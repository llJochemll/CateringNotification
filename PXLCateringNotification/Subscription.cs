using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;

namespace PXLCateringNotification
{
    public static class Subscription
    {
        static Random random = new Random();

        public class NotificationTime
        {
            public int Hour { get; set; }
            public int Minute { get; set; }
        }

        public class NotificationTimes
        {
            public NotificationTime Monday { get; set; }
            public NotificationTime Tuesday { get; set; }
            public NotificationTime Wednesday { get; set; }
            public NotificationTime Thursday { get; set; }
            public NotificationTime Friday { get; set; }
        }

        public class UserSubscription: TableEntity
        {
            public string Email { get; set; }
            public NotificationTimes NotificationTimes { get; set; }
            public string NotificationTimesSerialized { get => JsonConvert.SerializeObject(NotificationTimes); set => NotificationTimes = JsonConvert.DeserializeObject<NotificationTimes>(value); }
            public string Token { get; set; }
            public bool Verified { get; set; }
        }

        class UserSubscriptionVerification
        {
            public string Email { get; set; }
            public string Token { get; set; }
        }

        [FunctionName("Subscribe")]
        public static async Task<IActionResult> Subscribe(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "subscribe")] HttpRequest req,
            ILogger log)
        {
            UserSubscription subscription;

            using (var reader = new StreamReader(req.Body, Encoding.UTF8))
            {
                var body = reader.ReadToEnd();
                subscription = JsonConvert.DeserializeObject<UserSubscription>(body);
            }

            subscription.RowKey = subscription.Email;
            subscription.PartitionKey = "sub";
            subscription.Verified = false;
            subscription.Token = random.Next(100000, 999999).ToString();

            try
            {
                var subscriptionsTable = await Table.GetTableAsync("subscriptions");
                await subscriptionsTable.ExecuteAsync(TableOperation.InsertOrReplace(subscription));
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());

                return new BadRequestResult();
            }
            

            await Mail.SendMailAsync(new List<string>(new string[] { subscription.Email }), "Confirm subscription", $"Use the code {subscription.Token} to confirm your subscription");

            return new OkResult();
        }

        [FunctionName("UnSubscribe")]
        public static async Task<IActionResult> UnSubscribe(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "unsubscribe")] HttpRequest req,
            ILogger log)
        {
            return new OkResult();
        }

        [FunctionName("Verify")]
        public static async Task<IActionResult> Verify(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "verify")] HttpRequest req,
            ILogger log)
        {
            UserSubscriptionVerification verification;

            using (var reader = new StreamReader(req.Body, Encoding.UTF8))
            {
                var body = reader.ReadToEnd();
                verification = JsonConvert.DeserializeObject<UserSubscriptionVerification>(body);
            }

            try
            {
                var subscriptionsTable = await Table.GetTableAsync("subscriptions");
                var subscription = (await subscriptionsTable.ExecuteAsync(TableOperation.Retrieve<UserSubscription>("sub", verification.Email))).Result as UserSubscription;
                subscription.Verified = true;

                await subscriptionsTable.ExecuteAsync(TableOperation.InsertOrReplace(subscription));
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());

                return new BadRequestResult();
            }

            await Mail.SendMailAsync(new List<string>(new string[] { verification.Email }), "Confirm subscription", "Subscription confirmed");

            return new OkResult();
        }
    }
}
