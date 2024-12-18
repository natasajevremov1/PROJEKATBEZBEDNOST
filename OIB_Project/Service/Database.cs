using Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class Database
    {
        public static BlacklistManager blacklistManager = new BlacklistManager("blacklist.txt");
        public static List<string> ports = new List<string>();
        public static List<string> ips = new List<string>();
        public static List<string> protocols = new List<string>();

        static Database() 
        {
            
            blacklistManager.LoadBlacklist();
            
        }
    }
}
