using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Manager;

namespace Audit
{
    class DOSDetector
    {
        public static IAudit auditProxy;
        public int allowedNumberOfDOSAttacks = 3;
        public int DOSInterval = 12;
        public static Dictionary<string, int> DOSTracker;

        public DOSDetector()
        {
            DOSTracker = new Dictionary<string, int>();
        }

        public void DOSAttackDetection()
        {
            var thread = new Thread(() =>
            {
                string user;

                while(true)
                {
                    for(int interval = 1; interval <= DOSInterval; interval++)
                    {
                        lock(DOSTracker)
                        {
                            for(int i = 0; i < DOSTracker.Count; i++)
                            {
                                user = (DOSTracker.ElementAt(i)).Key;

                                if((DOSTracker.ElementAt(i)).Value > allowedNumberOfDOSAttacks)
                                {
                                    auditProxy.LogEvent((int)AuditEventTypes.DOSAttackDetected, user);
                                    DOSTracker[user] = 0;
                                    Console.WriteLine($"Denial of Service(DOS) attack attempted by user: {user}");
                                }

                                if(interval == DOSInterval)
                                {
                                    DOSTracker[user] = 0;
                                }
                            }
                        }

                        Thread.Sleep(1000);
                    }
                }
            });

            thread.Start();
        }
    }
}
