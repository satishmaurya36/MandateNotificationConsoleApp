using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MandateNotificationConsoleApp
{
    public class MandateNotificationLogic
    {
        private readonly string _connectionString = "Data Source=192.168.1.55;Initial Catalog=PDLERP;User ID=BeetaUser;Password=BeetaUser@123;Connection Timeout=120;Trusted_Connection=False;MultipleActiveResultSets=True;Encrypt=false";

        public void ListMandateNotification()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Usp_MandateNotification", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Mode", "ListMandateNotification");
                    conn.Open();

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            foreach (DataRow row in dt.Rows)
            {
                string P_Phone = row["P_Phone"].ToString();
                string SmCode = row["SmCode"].ToString();
                float AMT = float.Parse(row["AMT"].ToString());

                DateTime dtValue = DateTime.Parse(row["PVN_RCP_DT"].ToString());
                DateOnly PVN_RCP_DT = DateOnly.FromDateTime(dtValue);
                SendSmsAsync(P_Phone, AMT, PVN_RCP_DT.ToString()).Wait();
            }
        }
        public async Task SendSmsAsync(string P_Phone, float amt, string PVN_RCP_DT)
        {
            var payload = new
            {
                contentId = "1007790270230967014",
                language = "Hindi",
                mobileNo = P_Phone.ToString(),
                data = new object[] {amt.ToString() }
            };

            string json = JsonSerializer.Serialize(payload);

            using (HttpClient client = new HttpClient())
            {
                string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjE1OSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJTQVRJU0ggTUFVUllBIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoiZG90bmV0ZGV2MUBwYWlzYWxvLmluIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZWlkZW50aWZpZXIiOiIxNTkiLCJDcmVhdG9yIjoiQWdyYSIsIkVtcENvZGUiOiJQRExBMDAwMTAxIiwidG9rZW5WZXJzaW9uIjoiMTgiLCJuYmYiOjE3NjczNTgwMDAsImV4cCI6MTc2NzM1OTgwMCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzE4OCIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjcxODgifQ.YVbWroLS-LAeWM3f9SWQ3lN2G5-5w-newitTMmLVXS8";
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    //HttpResponseMessage response = await client.PostAsync("http://localhost:5078/api/SMS/SendSMS", content);
                    HttpResponseMessage response = await client.PostAsync("https://apiuat.paisalo.in:4015/PDLadmin/api/SMS/SendSMS", content);
                    response.EnsureSuccessStatusCode();

                    string result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending SMS: {ex.Message}");
                }
            }
        }

    }

}