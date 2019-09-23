using CateringNotification.Data;
using CateringNotification.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CateringNotification.Functions
{
    public static class SubscriptionFunctions
    {
        [FunctionName("Subscribe")]
        public static async Task<IActionResult> Subscribe(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "subscribe/{email}")] HttpRequest req, 
            string email, ILogger log)
        {
            var subscription = await Subscription.RetrieveAsync(email) ?? new Subscription
            {
                Email = email
            };

            using (var reader = new StreamReader(req.Body, Encoding.UTF8))
            {
                var body = reader.ReadToEnd();
                subscription.RequestedSubscription = JsonConvert.DeserializeObject<Subscription.UserSubscription>(body);
            }

            await subscription.InsertOrReplaceAsync();

            var verificationToken = await Verification.SetVerify(email);

            await Mail.SendMailAsync(new List<string>(new[] {subscription.Email}),
                "Catering Notification - Bevestiging inschrijving",
                $"<p>Gebruik de code <strong>{verificationToken}</strong> om je inschrijving op de dagelijkse e-mail van de catering te updaten of te activeren. Deze kan je ingeven op<a href=\"https://www.pxlfood.be/verify?email={subscription.Email} \">pxlfood.be</a>.</p>");

            return new OkResult();
        }

        [FunctionName("UnSubscribe")]
        public static async Task<IActionResult> UnSubscribe(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "unsubscribe/{email}")] HttpRequest req,
            string email, ILogger log)
        {
            var subscription = await Subscription.RetrieveAsync(email);

            if (subscription == null)
            {
                return new NotFoundResult();
            }

            var verificationToken = await Verification.SetVerify(email);

            subscription.RequestedSubscription = null;

            await subscription.InsertOrReplaceAsync();

            await Mail.SendMailAsync(new List<string>(new[] {subscription.Email}),
                "Catering Notification - Bevestig uitschrijving",
                $"Gebruik de code {verificationToken} om je inschrijving op de dagelijkse email van de catering op te zeggen. Deze kan je ingeven op https://www.pxlfood.be/verify?email={subscription.Email} .");

            return new OkResult();
        }

        [FunctionName("Verify")]
        public static async Task<IActionResult> Verify(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "verify/{email}/{token}")] HttpRequest req,
            string email, string token, ILogger log)
        {
            var subscription = await Subscription.RetrieveAsync(email);

            if (subscription == null)
            {
                return new NotFoundResult();
            }

            if (!await Verification.Verify(email, token))
            {
                await Verification.SetVerify(email);
                return new UnauthorizedResult();
            }

            subscription.CurrentSubscription = subscription.RequestedSubscription;

            await subscription.InsertOrReplaceAsync();

            await Mail.SendMailAsync(new List<string>(new[] {subscription.Email}), 
                "Catering Notification - Bevestiging verandering",
                "Je hebt je inschrijving succesvol veranderd! Indien je de uren van de emails wilt aanpassen, kan je altijd opnieuw inschrijven op <a href=\"https://www.pxlfood.be/ \">pxlfood.be</a>.");

            return new OkResult();
        }
    }
}
