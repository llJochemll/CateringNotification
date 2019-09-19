using CateringNotification.Content.Campus;
using CateringNotification.Data;
using CateringNotification.Utility;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CateringNotification.Functions
{
    public static class NotificationFunctions
    {
        [FunctionName("SendNotification")]
        public static async Task SendNotificationAsync([TimerTrigger("0 */1 10-23 * * 1-5")] TimerInfo myTimer,
            ILogger log)
        {
            var currentHour = DateTime.Now.Hour;
            var currentMinute = DateTime.Now.Minute;

            var table = await Table.GetTableAsync("subscriptions");

            var subscriptions = await table.ExecuteQuerySegmentedAsync(new TableQuery<Subscription>(),
                new TableContinuationToken());

            var notificationSettings = subscriptions
                .Where(s => s.CurrentSubscription != null)
                .Where(s => s.CurrentSubscription.NotificationSettings.ContainsKey(DateTime.Now.DayOfWeek))
                .Select(s => new { Settings = s.CurrentSubscription.NotificationSettings[DateTime.Now.DayOfWeek], s.Email })
                .Where(ns => ns.Settings.Value.Hour == currentHour && ns.Settings.Value.Minute == currentMinute)
                .GroupBy(ns => ns.Settings.Key)
                .ToDictionary(g => g.Key, g => g.AsEnumerable());

            if (notificationSettings.ContainsKey(Subscription.UserSubscription.Campus.ElfdeLinie))
            {
                var settings = notificationSettings[Subscription.UserSubscription.Campus.ElfdeLinie];

                await Mail.SendMailAsync(settings.Select(ns => ns.Email).ToList(),
                    $"PXL Menu {DateTime.Today.Day}-{DateTime.Today.Month}-{DateTime.Today.Year}",
                    await Campus.GetMenuAsync(
                        "https://www.pxl.be/Pub/Studenten/Voorzieningen-Student/Catering/Weekmenu-Campus-Elfde-Linie.html"));
            }
        }
    }
}
