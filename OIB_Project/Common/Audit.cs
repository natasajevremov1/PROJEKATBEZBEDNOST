﻿using System;
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
        const string SourceName = "Audit";
        const string LogName = "MyOIBLogs";

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

        public static void ConnectionSuccess(string userName)
        {
            if (customLog != null)
            {
                string ConnestionSuccess =
                    AuditEvents.ConnectionSuccess;
                string message = String.Format(ConnestionSuccess, userName);

                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventId = {0}) to event log.", (int)AuditEventTypes.ConnectionSuccess));
            }
        }

        public static void DOSAttackDetected(string username)
        {
            if (customLog != null)
            {
                string DoSAttackDetected = AuditEvents.DOSAttackDetected;
                string message = String.Format(DoSAttackDetected, username);
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.DOSAttackDetected));
            }
        }

        public static void ChangedBlacklistFile()
        {
            if (customLog != null)
            {
                string message = AuditEvents.ChangedBlacklistFile;
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.ChangedBlacklistFile));
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
