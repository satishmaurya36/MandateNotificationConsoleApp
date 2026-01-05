using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandateNotificationConsoleApp
{
    public class ExceptionLog
    {
        public static void InsertLogException(Exception exc, IConfiguration iConfig, string source = null)
        {
            LosCodeExceptionLogVM losCodeExceptionLog = new LosCodeExceptionLogVM();
            if (exc.InnerException != null)
            {
                losCodeExceptionLog.InnerExeType = exc.InnerException.GetType().ToString().Replace("'", "_");
                losCodeExceptionLog.InnerExeMessage = exc.InnerException.Message.Replace("'", "_");
                losCodeExceptionLog.InnerExeSource = exc.InnerException.Source.Replace("'", "_");
                losCodeExceptionLog.InnerExeStackTrace = exc.InnerException.StackTrace.Replace("'", "_");
            }
            losCodeExceptionLog.ExeType = exc.GetType().ToString().Replace("'", "_");
            losCodeExceptionLog.ExeMessage = exc.Message.Replace("'", "_");
            losCodeExceptionLog.ExeStackTrace = exc.StackTrace != null ? exc.StackTrace.Replace("'", "_") : null;
            losCodeExceptionLog.ExeSource = source;

            string query = @"INSERT INTO LosCodeExceptionLogs (ExeSource,ExeType,ExeMessage,ExeStackTrace,InnerExeSource,InnerExeType,InnerExeMessage,InnerExeStackTrace,CreationDate)
                             VALUES  ('" + losCodeExceptionLog.ExeSource + "','" + losCodeExceptionLog.ExeType + "','" + losCodeExceptionLog.ExeMessage + "','" + losCodeExceptionLog.ExeStackTrace + "','" + losCodeExceptionLog.InnerExeSource + "','" + losCodeExceptionLog.InnerExeType + "','" + losCodeExceptionLog.InnerExeMessage + "','" + losCodeExceptionLog.InnerExeStackTrace + "',GETDATE())";

            using (var con = new SqlConnection(GlobalConfig.ConnectionString))
            {
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.CommandType = CommandType.Text;
                    if (con.State == ConnectionState.Closed) con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

        }
        public class LosCodeExceptionLogVM
        {
            public string? ExeSource { get; set; }
            public string? ExeType { get; set; }
            public string? ExeMessage { get; set; }
            public string? ExeStackTrace { get; set; }
            public string? InnerExeSource { get; set; }
            public string? InnerExeType { get; set; }
            public string? InnerExeMessage { get; set; }
            public string? InnerExeStackTrace { get; set; }
            public DateTime? CreationDate { get; set; }
        }
    }
}
