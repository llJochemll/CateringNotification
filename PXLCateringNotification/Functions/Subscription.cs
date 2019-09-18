using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using PXLCateringNotification.Utility;
using static PXLCateringNotification.Data.Subscription;
using PXLCateringNotification.Data;

namespace PXLCateringNotification.Functions
{
    public static class SubscriptionFunctions
    {
        [FunctionName("Subscribe")]
        public static async Task<IActionResult> Subscribe(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "subscribe")] HttpRequest req,
            ILogger log)
        {
            Subscription subscription;
            UserSubscription newSubscription;


            using (var reader = new StreamReader(req.Body, Encoding.UTF8))
            {
                var body = reader.ReadToEnd();
                newSubscription = JsonConvert.DeserializeObject<UserSubscription>(body);
            }

            

            try
            {
                var subscriptionsTable = await Table.GetTableAsync("subscriptions");

                subscription = subscriptionsTable.ExecuteAsync(TableOperation.Retrieve<Subscription>("sub", )
                await subscriptionsTable.ExecuteAsync(TableOperation.InsertOrReplace(subscription));
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());

                return new BadRequestResult();
            }
            

            await Mail.SendMailAsync(new List<string>(new string[] { subscription.Email }), "Catering Notification - Bevestiging inschrijving", $"Gebruik de code {subscription.Token} om je inschrijving op de dagelijkse email van de catering van de Elfde Linie te updaten of te activeren. Deze kan je ingeven op https://cateringstorage.z6.web.core.windows.net/verify?email={subscription.Email} .");

            return new OkResult();
        }

        [FunctionName("UnSubscribe")]
        public static async Task<IActionResult> UnSubscribe(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "unsubscribe")] HttpRequest req,
            ILogger log)
        {
            return new OkResult();
        }

        [FunctionName("Verify")]
        public static async Task<IActionResult> Verify(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "verify")] HttpRequest req,
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

                if (verification.Token != subscription.Token)
                {
                    return new UnauthorizedResult();
                }

                subscription.Verified = true;

                await subscriptionsTable.ExecuteAsync(TableOperation.InsertOrReplace(subscription));
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());

                return new BadRequestResult();
            }

            await Mail.SendMailAsync(new List<string>(new string[] { verification.Email }), "Catering Notification - Bevestiging inschrijving", "Je hebt je inschrijving succesvol bevestigd! Indien je de uren van de emails wilt aanpassen, kan je altijd opnieuw inschrijven op https://cateringstorage.z6.web.core.windows.net/ .");

            return new OkResult();
        }
    }
}
