using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Collections.Generic;

namespace PXLCateringNotification
{
    public static class Notification
    {
        [FunctionName("SendNotification")]
        public static void SendNotification([TimerTrigger("0 45 11 * * 1-5")]TimerInfo myTimer, ILogger log)
        {
            var emails = new string[] {
                "jochem.dejaeghere@student.pxl.be",
                "ward.poel@student.pxl.be",
                "ian.angillis@student.pxl.be",
                "joachim.veulemans@student.pxl.be",
                "toon.lodts@student.pxl.be",
                "toon.vanengeland@student.pxl.be"
            };
            emails.ToList().ForEach(async e => await SendMailAsync(e));
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

        static async Task SendMailAsync(string email)
        {
            var mail = new MailMessage("cateringnotification@gmail.com", email);
            var client = new SmtpClient
            {
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = "smtp.gmail.com",
                Credentials = new NetworkCredential("cateringnotification@gmail.com", "Test123?")
            };
            mail.Subject = "PXL Menu " + DateTime.Today.ToShortDateString();
            mail.Body = "<h2>PXL Menu on: " + DateTime.Today.ToShortDateString() + "</h2><br/>" + (await GetMenuItemsAsync()).Aggregate((e1, e2) => e1 + "<br/>" + e2);
            mail.IsBodyHtml = true;
            client.Send(mail);
        }
    }
}
