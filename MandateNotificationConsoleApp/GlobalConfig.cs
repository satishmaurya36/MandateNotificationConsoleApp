using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandateNotificationConsoleApp
{
    public static class GlobalConfig
    {
        public static readonly string ConnectionString = "Data Source=192.168.1.55;Initial Catalog=PDLERP;User ID=BeetaUser;Password=BeetaUser@123;Connection Timeout=120;Trusted_Connection=False;MultipleActiveResultSets=True;Encrypt=false";
        public static readonly string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjE1OSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJTQVRJU0ggTUFVUllBIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoiZG90bmV0ZGV2MUBwYWlzYWxvLmluIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZWlkZW50aWZpZXIiOiIxNTkiLCJDcmVhdG9yIjoiQWdyYSIsIkVtcENvZGUiOiJQRExBMDAwMTAxIiwidG9rZW5WZXJzaW9uIjoiMTgiLCJuYmYiOjE3Njc2MTI4NzgsImV4cCI6MTc2NzYxNDY3OCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzE4OCIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjcxODgifQ.NMVTI6iLb2hF-dq9eoEwUAtfxr5Da8nefOLtQzflDoY";
        
        
        public static readonly string SendSmsUrl = "https://apiuat.paisalo.in:4015/PDLadmin/api/SMS/SendSMS";
        //public static readonly string ExecuteMandate = "https://apiuat.paisalo.in:4015/Collection/api/ICICIMandate/ExecuteMandate";
        public static readonly string ExecuteMandates = "https://localhost:7170/api/ICICIMandate/ExecuteMandate";
    }
}
