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
            fileChecksum = Checksum();
            CheckBlacklist();
            
        }

        public static byte[] Checksum()
        {
            try
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead("blacklist.txt"))
                    {
                        return md5.ComputeHash(stream);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public static void CheckBlacklist()
        {
            var thread = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(5000);
                    lock (fileChecksum)
                    {
                        byte[] help = Checksum();
                        for (int i = 0; i < fileChecksum.Length; i++)
                        {
                            if (fileChecksum[i] != help[i])
                            {
                                Console.WriteLine("Unauthorised blacklist file corrupted, Admin reaction REQUIRED!!!");
                                break;
                            }
                        }
                    }
                }
            });

            thread.Start();
        }
    }
}
