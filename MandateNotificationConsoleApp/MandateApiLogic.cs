using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace MandateNotificationConsoleApp
{
    public class MandateApiLogic
    {
        public async Task<bool> SendSmsAsync(string P_Phone, float amt, string PVN_RCP_DT)
        {
            var payload = new
            {
                contentId = "1007790270230967014",
                language = "Hindi",
                mobileNo = P_Phone.ToString(),
                data = new object[] { amt.ToString() }
            };

            string json = JsonSerializer.Serialize(payload);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", GlobalConfig.token);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    HttpResponseMessage response = await client.PostAsync(GlobalConfig.SendSmsUrl, content);
                    response.EnsureSuccessStatusCode();

                    if (!response.IsSuccessStatusCode)
                        return false;
                    return true;// SMS sent successfully
                }
                catch (Exception ex)
                {
                    return false; // SMS failed
                }

            }
        }
        public async Task<bool> CallExecuteMandateApi(string smCode)
        {
            using (HttpClient client = new HttpClient())
            {
                string encodedSmCode = Uri.EscapeDataString(smCode);
                string url = $"{GlobalConfig.ExecuteMandates}?SmCode={encodedSmCode}";

                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", GlobalConfig.token);
                var payload = new { SmCode = smCode };
                string json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    HttpResponseMessage response = await client.PostAsync(url, content);

                    if (!response.IsSuccessStatusCode)
                        return false;

                    string result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Response for {smCode}: {result}");

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"API Error for {smCode}: {ex.Message}");
                    return false;
                }
            }
        }

        public async Task<bool> SendMail(string toEmail)
        {
            bool isSuccess = false;
            string subject = "Mandate Notification Service Stopped";
                string bodyText = "Dear Team,\r\n\r\nThe Mandate Notification Scheduler has stopped at {time}.  \r\nNo further tasks will be executed until the service is restarted.\r\n\r\nRegards,  \r\nSystem Notification";
            try
            {
                // Create MimeMessage
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Paisalo Digital Limited", "noreply@paisalo.in"));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;
               
                // Add priority headers
                message.Headers.Add("X-Priority", "1"); // 1 = High, 3 = Normal, 5 = Low
                message.Headers.Add("Importance", "High"); // Outlook/Exchange
                message.Headers.Add("X-MSMail-Priority", "High"); // Some clients
                // Simple plain text body
                message.Body = new TextPart("plain")
                {
                    Text = bodyText
                };

                // Send the email using MailKit's SMTP client
                using (var client = new SmtpClient())
                {
                    client.Connect("email.paisalo.in", 465, SecureSocketOptions.SslOnConnect); // SSL
                    client.Authenticate("noreply1@paisalo.in", "Norep@34$w&"); // credentials

                    client.Send(message);
                    client.Disconnect(true);
                }

                isSuccess = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
            return isSuccess;
        }

    }
}
