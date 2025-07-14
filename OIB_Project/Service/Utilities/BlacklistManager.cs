using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO; // Dodaj na vrh ako nije već dodato
using System.Threading;
using System.Security.Cryptography;

namespace Service.Utilities
{
   public class BlacklistManager
    {
        private readonly string filePath;
        

        public BlacklistManager(string filePath)
        {
            this.filePath = filePath;
           
        }

        // Učitavanje blacklist-a iz fajla
        public void LoadBlacklist()
        {
            if (!File.Exists(filePath)) return;

            foreach (var line in File.ReadLines(filePath))
            {
                var parts = line.Split(':').Select(p => p.Trim()).ToArray();
                if (parts.Length != 2) continue;

                string type = parts[0].ToLower();
                string value = parts[1];

                if (type == "port") Database.ports.Add(value);
                else if (type == "ip") Database.ips.Add(value);
                else if (type == "protokol") Database.protocols.Add(value);
            }
        }

        // Provera da li je stavka na blacklist-i
        public bool IsBlacklisted(string ip, string port, string protocol)
        {
            return Database.ips.Contains(ip) || Database.ports.Contains(port) || Database.protocols.Contains(protocol);
        }

        // Dodavanje u fajl i listu
        public void AddToBlacklist(string type, string value)
        {
             value = value.Trim();
           CheckBlacklist();


            try
            {
                if (type.ToLower() == "port" && !Database.ports.Contains(value))
                {
                    Database.ports.Add(value);
                    AppendToFile("port", value);
                }
                else if (type.ToLower() == "ip" && !Database.ips.Contains(value))
                {
                    Database.ips.Add(value);
                    AppendToFile("ip", value);
                }
                else if (type.ToLower() == "protokol" && !Database.protocols.Contains(value))
                {
                    Database.protocols.Add(value);
                    AppendToFile("protokol", value);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while adding to blacklist: {ex.Message}");
            }
            finally
            {
                Database.fileChecksum = Checksum();
            }
        }

        // Brisanje iz fajla i liste
        public void RemoveFromBlacklist(string type, string value)
        {
            value = value.Trim();

            if (type.ToLower() == "port" && Database.ports.Remove(value) ||
                type.ToLower() == "ip" && Database.ips.Remove(value) ||
                type.ToLower() == "protokol" && Database.protocols.Remove(value))
            {
                RewriteFile();
            }
        }

        // Privatna funkcija za dodavanje u fajl
        private void AppendToFile(string type, string value)
        {
            try
            {

               
                
                Console.WriteLine(filePath);
                using (var writer = File.AppendText(Path.Combine(Environment.CurrentDirectory,filePath)))
                {
                    writer.WriteLine($"{type}: {value}");
                    Console.WriteLine($"Successfully added {type}: {value} to file.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while writing to file: {ex.Message}");
            }
        }

        // Privatna funkcija za ponovno pisanje fajla nakon brisanja
        private void RewriteFile()
        {
            try
            {
                using (var writer = new StreamWriter(filePath, false))
                {
                    foreach (var port in Database.ports)
                        writer.WriteLine($"port: {port}");
                    foreach (var ip in Database.ips)
                        writer.WriteLine($"ip: {ip}");
                    foreach (var protocol in Database.protocols)
                        writer.WriteLine($"protokol: {protocol}");
                }
                Console.WriteLine("Blacklist file successfully rewritten.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while rewriting file: {ex.Message}");
            }
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
                    lock (Database.fileChecksum)
                    {
                        byte[] help = Checksum();
                        for (int i = 0; i < Database.fileChecksum.Length; i++)
                        {
                            if (Database.fileChecksum[i] != help[i])
                            {
                                Console.WriteLine("Unauthorised blacklist file corrupted, Admin reaction REQUIRED!!!\n----------SERVICE MANAGER IS SHUT DOWN DUE TO SECURITY REASONS!----------");
                                Common.Audit.ChangedBlacklistFile();
                                Service.ServiceManagement.StopServiceBlacklistCorrupted();
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
