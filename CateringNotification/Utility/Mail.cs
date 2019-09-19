using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CateringNotification.Utility
{
    internal static class Mail
    {
        internal static async Task SendMailAsync(IEnumerable<string> to, string subject, string body)
        {
            using (var client = new SmtpClient
            {
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = "smtp.gmail.com",
                Credentials = new NetworkCredential(Environment.GetEnvironmentVariable("MAIL_USERNAME"),
                    Environment.GetEnvironmentVariable("MAIL_PASSWORD"))
            })
            {
                using (var mail = new MailMessage("cateringnotification@gmail.com", "cateringnotification@gmail.com")
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    foreach (var address in to)
                    {
                        mail.Bcc.Add(address);
                    }

                    await client.SendMailAsync(mail);
                }
            }
        }
    }
}
