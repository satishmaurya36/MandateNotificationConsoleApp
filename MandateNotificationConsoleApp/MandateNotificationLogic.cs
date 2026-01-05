using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;
        private readonly MandateApiLogic _mandateApiLogic;
        public MandateNotificationLogic(IConfiguration configuration, MandateApiLogic mandateApiLogic)
        {
            _configuration = configuration;
            _mandateApiLogic = mandateApiLogic;
        }
        public async void ListMandateNotification()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(GlobalConfig.ConnectionString))
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
                    bool isSent = await _mandateApiLogic.SendSmsAsync(P_Phone, AMT, PVN_RCP_DT.ToString("dd/MM/yyyy"));
                    if (isSent)
                    {
                        int res = InsertMandateNotificationLog(P_Phone, SmCode, AMT, dtValue);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.InsertLogException(ex, _configuration, "ListMandateNotification_MandateNotificationLogic");
            }
        }

        public int InsertMandateNotificationLog(string P_Phone, string SmCode, float AMT, DateTime PVN_RCP_DT)
        {
            int insertedId = 0;

            try
            {
                using (SqlConnection con = new SqlConnection(GlobalConfig.ConnectionString))
                {
                    string query = "Usp_MandateNotification";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Mode", "InsertMandateNotificationLog");
                        cmd.Parameters.AddWithValue("@P_Phone", P_Phone);
                        cmd.Parameters.AddWithValue("@SmCode", SmCode);
                        cmd.Parameters.AddWithValue("@AMT", Convert.ToDecimal(AMT));
                        cmd.Parameters.AddWithValue("@PVN_RCP_DT", PVN_RCP_DT);

                        con.Open();
                        insertedId = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.InsertLogException(ex, _configuration, "InsertMandateNotificationLog_MandateNotificationLogic");
            }

            return insertedId;
        }
        public List<string> GetSmCodeList()
        {
            List<string> smCodeList = new List<string>();
            string query = "Usp_MandateNotification";

            using (SqlConnection conn = new SqlConnection(GlobalConfig.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Mode", "GetSmCodeList");
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            smCodeList.Add(reader["SmCode"].ToString());
                        }
                    }
                }
            }

            return smCodeList;
        }

        public async Task ExecuteMandatesForPendingSmCodes()
        {
            List<string> smCodes = GetSmCodeList();
            try
            {
                foreach (var smCode in smCodes)
                {
                    bool success = await _mandateApiLogic.CallExecuteMandateApi(smCode);

                    if (success)
                    {
                        int res = UpdateExecuteMandate(smCode);
                        Console.WriteLine($"ExecuteMandate success for {smCode}");
                        // Optional: update DB to mark ExecuteMandate = 1
                    }
                    else
                    {
                        Console.WriteLine($"ExecuteMandate failed for {smCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.InsertLogException(ex, _configuration, "ExecuteMandatesForPendingSmCodes_MandateNotificationLogic");
                Console.WriteLine("All pending SmCodes processed.");
            }
        }
        public int UpdateExecuteMandate(string smCode)
        {
            int updatedId = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(GlobalConfig.ConnectionString))
                {
                    string query = "Usp_MandateNotification";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Mode", "UpdateExecuteMandate");
                        cmd.Parameters.AddWithValue("@SmCode", smCode);
                        con.Open();
                        updatedId = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.InsertLogException(ex, _configuration, "UpdateExecuteMandate_MandateNotificationLogic");
            }
            return updatedId;
        }

    }
}