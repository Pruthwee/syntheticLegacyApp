// =============================================================================
// RULE ID   : cr-dotnet-0056
// RULE NAME : SMTP Local Server
// CATEGORY  : ExternalServices
// DESCRIPTION: Application assumes a local SMTP server installation for email
//              functionality through System.Net.Mail.SmtpClient with localhost
//              configuration. Local SMTP servers don't exist in cloud environments.
// =============================================================================

using System;
using System.Net;
using System.Net.Mail;
using System.Collections.Generic;

namespace SyntheticLegacyApp.ExternalServices
{
    public class EmailNotificationService
    {
        // VIOLATION cr-dotnet-0056: SmtpClient configured to localhost — assumes local IIS SMTP relay
        private readonly SmtpClient _smtpClient = new SmtpClient("localhost", 25)
        {
            // VIOLATION cr-dotnet-0056: No credentials — relies on anonymous local SMTP server
            UseDefaultCredentials = false,
            EnableSsl = false
        };

        // VIOLATION cr-dotnet-0056: Hard-coded localhost SMTP — will fail in cloud (no local relay)
        public void SendOrderConfirmation(string toEmail, string orderId)
        {
            var mail = new MailMessage
            {
                From    = new MailAddress("orders@legacyapp.corp"),
                Subject = $"Order Confirmation — {orderId}",
                Body    = $"Your order {orderId} has been received and is being processed.",
                IsBodyHtml = false
            };
            mail.To.Add(toEmail);

            _smtpClient.Send(mail); // VIOLATION cr-dotnet-0056: Synchronous send to localhost SMTP
        }

        // VIOLATION cr-dotnet-0056: Sending via 127.0.0.1 — explicit loopback SMTP reference
        public void SendAlertEmail(string subject, string body, IEnumerable<string> recipients)
        {
            using (var client = new SmtpClient("127.0.0.1", 25))
            {
                var mail = new MailMessage
                {
                    From       = new MailAddress("alerts@legacyapp.corp"),
                    Subject    = subject,
                    Body       = body,
                    IsBodyHtml = true
                };

                foreach (var r in recipients) mail.To.Add(r);

                client.Send(mail);
                Console.WriteLine($"Alert sent to {mail.To.Count} recipients via localhost SMTP.");
            }
        }

        // VIOLATION cr-dotnet-0056: PickupDirectoryFromIis — IIS SMTP pickup folder (local disk)
        public void SendViaPickupFolder(string to, string subject, string body)
        {
            using (var client = new SmtpClient
            {
                // VIOLATION cr-dotnet-0056: SmtpDeliveryMethod.PickupDirectoryFromIis — IIS-only
                DeliveryMethod       = SmtpDeliveryMethod.PickupDirectoryFromIis,
                PickupDirectoryLocation = @"C:\inetpub\mailroot\Pickup"
            })
            {
                var mail = new MailMessage("noreply@legacyapp.corp", to, subject, body);
                client.Send(mail);
                Console.WriteLine($"Email dropped to IIS SMTP pickup folder for: {to}");
            }
        }

        // VIOLATION cr-dotnet-0056: SendAsync with localhost — non-blocking but still local SMTP
        public void SendAsync(string to, string subject, string body)
        {
            var client = new SmtpClient("localhost", 25);
            var mail   = new MailMessage("noreply@legacyapp.corp", to, subject, body);

            client.SendCompleted += (s, e) =>
            {
                if (e.Error != null) Console.WriteLine($"SMTP error: {e.Error.Message}");
                client.Dispose();
                mail.Dispose();
            };

            client.SendAsync(mail, userToken: null);
        }
    }
}
