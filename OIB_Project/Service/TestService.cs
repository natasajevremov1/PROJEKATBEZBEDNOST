using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    class TestService : ITest
    {
        public void TestConnection()
        {
            Console.WriteLine("\n\nCONNECTION SUCCESS!\n\n");
        }
    }
}
