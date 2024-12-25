using Service.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service
{
    public class Database
    {
        public static BlacklistManager blacklistManager = new BlacklistManager("blacklist.txt");
        public static List<string> ports = new List<string>();
        public static List<string> ips = new List<string>();
        public static List<string> protocols = new List<string>();
        public static byte[] fileChecksum = null;
        public static Dictionary<string, ServiceHost> openHosts = new Dictionary<string, ServiceHost>();

        static Database() 
        {
            
            blacklistManager.LoadBlacklist();
            fileChecksum = BlacklistManager.Checksum();
            BlacklistManager.CheckBlacklist();
            
        }

       
    }
}
