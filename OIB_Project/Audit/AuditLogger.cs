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
                        Console.WriteLine(message);
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

            }

            Console.WriteLine(message);
        }
    }
}
