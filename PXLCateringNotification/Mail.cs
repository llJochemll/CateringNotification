using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PXLCateringNotification
{
    public static class Mail
    {
        public static async Task SendMailAsync(List<string> to, string subject, string body)
        {
            var mail = new MailMessage("cateringnotification@gmail.com", "cateringnotification@gmail.com")
            {
                Subject = subject,//$"PXL Menu {DateTime.Today.Year}-{DateTime.Today.Month}-{DateTime.Today.Day}",
                Body = body,//$"<h2>PXL Menu on: {DateTime.Today.Year}-{DateTime.Today.Month}-{DateTime.Today.Day} </h2><br/>{(await GetMenuItemsAsync()).Aggregate((e1, e2) => e1 + "<br/>" + e2)}",
                IsBodyHtml = true
            };

            to.ForEach(e => mail.Bcc.Add(e));

            var client = new SmtpClient
            {
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = "smtp.gmail.com",
                Credentials = new NetworkCredential("cateringnotification@gmail.com", "Test123?")
            };

            await client.SendMailAsync(mail);
        }
    }
}
