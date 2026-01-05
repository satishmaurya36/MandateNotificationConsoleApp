using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandateNotificationConsoleApp
{
    public class MandateNotification: BackgroundService
    {
        private readonly ILogger<MandateNotification> _logger;
        private Timer _timer;
        private readonly IConfiguration _configuration;
        private readonly MandateApiLogic _mandateApiLogic;


        #region  ---- Mandate Notification ----- created by Satish Maurya ----
        
        public MandateNotification(ILogger<MandateNotification> logger,IConfiguration configuration, MandateApiLogic mandateApiLogic)
        {
            _logger = logger;
            _configuration = configuration;
            _mandateApiLogic = mandateApiLogic;
        }
        //protected override Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    _logger.LogInformation("MandateNotification service started at: {time}", DateTime.Now);

        //    ScheduleDailyTask(stoppingToken);

        //    return Task.CompletedTask;
        //}
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("MandateNotification started at: {time}", DateTime.Now);

            _timer = new Timer(async state => await RunTask(), null, TimeSpan.Zero, TimeSpan.FromMinutes(2));

            return Task.CompletedTask;
        }
        private void ScheduleDailyTask(CancellationToken stoppingToken)
        {
            // Calculate time until next 10 AM
            var now = DateTime.Now;
            var nextRun = new DateTime(now.Year, now.Month, now.Day, 10, 0, 0);

            if (now > nextRun)
            {
                // If it's already past 10 AM today, schedule for tomorrow
                nextRun = nextRun.AddDays(1);
            }

            var initialDelay = nextRun - now;

            _timer = new Timer(async state =>
            {
                await RunTask();

                // After running, reschedule for next day
                ScheduleDailyTask(stoppingToken);

            }, null, initialDelay, Timeout.InfiniteTimeSpan);
        }

        private async Task RunTask()
        {
            WriteToFile("Task started at " + DateTime.Now);
            MandateNotificationLogic logic = new MandateNotificationLogic(_configuration,_mandateApiLogic);
            try
            {
                logic.ListMandateNotification();

               await logic.ExecuteMandatesForPendingSmCodes();

                _logger.LogInformation("Task executed at {time}", DateTime.Now);
            }
            catch (Exception ex)
            {
                ExceptionLog.InsertLogException(ex, _configuration, "RunTask_MandateNotificationLogic");

                _logger.LogError(ex, "Error running task");
                WriteToFile("Error: " + ex.ToString());
            }

            WriteToFile("Task finished at " + DateTime.Now);
            await Task.CompletedTask;
        }
        private void WriteToFile(string message)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LOSDOC");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string filepath = Path.Combine(path, "ServiceLog_" + DateTime.Now.ToString("yyyy_MM_dd") + ".txt");

            using (StreamWriter sw = File.Exists(filepath) ? File.AppendText(filepath) : File.CreateText(filepath))
            {
                sw.WriteLine(message);
            }
        }
        #endregion
    }
}
