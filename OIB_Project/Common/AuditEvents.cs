using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum AuditEventTypes
    {
        RunServiceSuccess = 0,
        RunServiceFailed = 1,
        DOSAttackDetected = 2,
        ChangedBlacklistFile = 3,
        ConnectionSuccess = 4
    }
    class AuditEvents
    {
        private static ResourceManager resourceManager = null;
        private static object resourceLock = new object();

        private static ResourceManager ResourceMgr
        {
            get
            {
                lock(resourceLock)
                {
                    if(resourceManager == null)
                    {
                        resourceManager = new ResourceManager
                            (typeof(AuditEventFile).ToString(), Assembly.GetExecutingAssembly());
                    }

                    return resourceManager;
                }
            }
        }

        public static string RunServiceSuccess
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.RunServiceSuccess.ToString());
            }
        }

        public static string RunServiceFailed
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.RunServiceFailed.ToString());
            }
        }

        public static string DOSAttackDetected
        {
            get { return ResourceMgr.GetString(AuditEventTypes.DOSAttackDetected.ToString()); }
        }

        public static string ChangedBlacklistFile
        {
            get { return ResourceMgr.GetString(AuditEventTypes.ChangedBlacklistFile.ToString()); }
        }

        public static string ConnectionSuccess
        {
            get { return ResourceMgr.GetString(AuditEventTypes.ConnectionSuccess.ToString()); }

        }
    }
}
