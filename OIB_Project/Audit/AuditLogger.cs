using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Audit
{
    class AuditLogger : IAudit
    {

        public void LogEvent(int command, string userName)
        {
            string message = null;
            switch(command)
            {
                case 0:

                    try
                    {
                        Common.Audit.RunServiceSuccess(userName);
                        message = $"User {userName} started service successfully.";
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;

                case 1:

                    try
                    {
                        Common.Audit.RunServiceFailed(userName);
                        message = $"User {userName} failed to run service.";
                        lock (DOSDetector.DOSTracker)
                        {
                            DOSDetector.DOSTracker[userName]++;
                            Console.WriteLine(DOSDetector.DOSTracker[userName]);
                        }
                        
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;

                case 2:

                    try
                    {
                        Common.Audit.DOSAttackDetected(userName);
                        message = $"Denial of Service attack attempted by user: {userName}!";
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;

                case 3:

                    try
                    {
                        Common.Audit.ChangedBlacklistFile();
                        message = $"blacklist.txt file changed in an illegal way!";
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;

                case 4:

                    try
                    {
                        Common.Audit.ConnectionSuccess(userName);
                        message = $"{userName} connected successfully.";
                        lock (DOSDetector.DOSTracker)
                        {
                            if (!DOSDetector.DOSTracker.ContainsKey(userName))
                            {
                                DOSDetector.DOSTracker.Add(userName, 0);
                            }
                            else
                            {
                                Console.WriteLine("User already exists in DOSTracker.");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;

            }

            Console.WriteLine(message);
        }
    }
}
