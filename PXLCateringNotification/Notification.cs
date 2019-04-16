using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Linq;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace PXLCateringNotification
{
    public static class Notification
    {
        [FunctionName("SendNotification")]
        public static async Task SendNotificationAsync([TimerTrigger("0 */1 10-15 * * 1-5")]TimerInfo myTimer, ILogger log)
        {
            var subscriptionsTable = await Table.GetTableAsync("subscriptions");
            var queryResult = await subscriptionsTable.ExecuteQuerySegmentedAsync(new TableQuery<Subscription.UserSubscription>(), new TableContinuationToken());

            var subscriptions = queryResult.ToList();

            await Mail.SendMailAsync(
                subscriptions.Where(e =>
                {
                    if (!e.Verified)
                    {
                        return false;
                    }

                    switch (DateTime.Now.DayOfWeek)
                    {
                        case DayOfWeek.Monday:
                            return e.NotificationTimes.Monday.Hour == DateTime.Now.Hour && e.NotificationTimes.Monday.Minute == DateTime.Now.Minute;
                        case DayOfWeek.Tuesday:
                            return e.NotificationTimes.Tuesday.Hour == DateTime.Now.Hour && e.NotificationTimes.Tuesday.Minute == DateTime.Now.Minute;
                        case DayOfWeek.Wednesday:
                            return e.NotificationTimes.Wednesday.Hour == DateTime.Now.Hour && e.NotificationTimes.Wednesday.Minute == DateTime.Now.Minute;
                        case DayOfWeek.Thursday:
                            return e.NotificationTimes.Thursday.Hour == DateTime.Now.Hour && e.NotificationTimes.Thursday.Minute == DateTime.Now.Minute;
                        case DayOfWeek.Friday:
                            return e.NotificationTimes.Friday.Hour == DateTime.Now.Hour && e.NotificationTimes.Friday.Minute == DateTime.Now.Minute;
                    }

                    return false;
                }).Select(e => e.Email).ToList(), 
                $"PXL Menu {DateTime.Today.Day}-{DateTime.Today.Month}-{DateTime.Today.Year}",
                $"<h2>PXL Menu op: {DateTime.Today.Day}-{DateTime.Today.Month}-{DateTime.Today.Year} </h2>" +
                $"<br/>{(await GetMenuItemsAsync()).Aggregate((e1, e2) => e1 + "<br/>" + e2)}"
                );
        }

        static async Task<IEnumerable<string>> GetMenuItemsAsync()
        {
            var pageContent = await new HttpClient().GetStringAsync("https://www.pxl.be/Pub/Studenten/Voorzieningen-Student/Catering/Weekmenu-Campus-Elfde-Linie.html");

            var beginIndex = pageContent.IndexOf("<h3>Hoofdgerecht</h3>") + "<h3>Hoofdgerecht</h3>".Length;
            var endIndex = pageContent.IndexOf("<p><strong>", beginIndex);

            return pageContent.Substring(beginIndex, endIndex - beginIndex).Split("<p>")
                .AsSpan().Slice(1).ToArray()
                .ToList().Select(e => e.Replace("</p>", ""));
        }
    }
}
