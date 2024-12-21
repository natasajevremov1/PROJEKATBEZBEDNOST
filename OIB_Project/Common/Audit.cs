using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Audit : IDisposable
    {
        private static EventLog customLog = null;
        const string SourceName = "Common.Audit";
        const string LogName = "MySecTest";

        static Audit()
        {
            try
            {
                if(!EventLog.SourceExists(SourceName))
                {
                    EventLog.CreateEventSource(SourceName, LogName);
                }

                customLog = new EventLog(LogName, Environment.MachineName, SourceName);
            }
            catch(Exception e)
            {
                customLog = null;
                Console.WriteLine("Error while trying to create log handle. Error = {0}", e.Message);
            }
        }

        public static void RunServiceSuccess(string userName)
        {
            if(customLog != null)
            {
                string RunServiceSuccess =
                    AuditEvents.RunServiceSuccess;
                string message = String.Format(RunServiceSuccess, userName);

                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventId = {0}) to event log.", (int)AuditEventTypes.RunServiceSuccess));
            }
        }

        public static void RunServiceFailed(string userName)
        {
            if (customLog != null)
            {
                string RunServiceSuccess =
                    AuditEvents.RunServiceFailed;
                string message = String.Format(RunServiceSuccess, userName);

                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventId = {0}) to event log.", (int)AuditEventTypes.RunServiceFailed));
            }
        }
        public void Dispose()
        {
            if(customLog != null)
            {
                customLog.Dispose();
                customLog = null;
            }
        }
    }
}
