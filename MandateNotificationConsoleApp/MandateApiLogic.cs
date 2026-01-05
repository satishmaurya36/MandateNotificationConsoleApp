using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
    }
}
