using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PXLCateringNotification.Utility
{
    public static class Mail
    {
        public static async Task SendMailAsync(List<string> to, string subject, string body)
        {
            var mail = new MailMessage("cateringnotification@gmail.com", "cateringnotification@gmail.com")
            {
                Subject = subject,
                Body = body,
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
