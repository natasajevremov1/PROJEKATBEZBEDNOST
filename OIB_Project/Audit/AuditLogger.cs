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
            Console.WriteLine("Test Log Event");
        }
    }
}
