using Audit;
using Common;
using Service.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Service
{
    public class ServiceManagement : IServiceManagement
    {
        private readonly AESAlgorithm aes = new AESAlgorithm(); // Instanca AES algoritma
        private string sessionId;

        [PrincipalPermission(SecurityAction.Demand,Role = "ModifyBlacklist")]
        public void AddItemToBlacklist(string type, string value)
        {
            Database.blacklistManager.AddToBlacklist(type, value);
        }

        [PrincipalPermission(SecurityAction.Demand, Role ="ExchangeSessionKey")]
        public string Connect(string userName)
        {
            Console.WriteLine("Client successfully connected!");
             sessionId = OperationContext.Current.SessionId;
            Console.WriteLine("Session id: "+sessionId);
            // string userName = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            Program.auditProxy.LogEvent((int)AuditEventTypes.ConnectionSuccess, userName);
         
            return sessionId;
        }

        [PrincipalPermission(SecurityAction.Demand,Role ="RunService")]
        public void RunService(string encryptedIp, string encryptedPort, string encryptedProtocol, string userName)
        {

            try
            {
                 // Dekriptuji podatke
                string ip = aes.Decrypt(encryptedIp, sessionId);
                string port = aes.Decrypt(encryptedPort, sessionId);
                string protocol = aes.Decrypt(encryptedProtocol, sessionId);

                Console.WriteLine($"Decrypted IP: {ip}");
                Console.WriteLine($"Decrypted Port: {port}");
                Console.WriteLine($"Decrypted Protocol: {protocol}");

                CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
                // string userName = Formatter.ParseName(principal.Identity.Name);
               

                if (Database.blacklistManager.IsBlacklisted(ip, port, protocol))
                {
                    try
                    {
                        
                        Program.auditProxy.LogEvent((int)AuditEventTypes.RunServiceFailed, userName);
                        Console.WriteLine("Access denied: One or more parameters are blacklisted.");
                        return;
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    // Console.WriteLine("Access denied: One or more parameters are blacklisted.");
                    // return;
                }
           
                

                NetTcpBinding binding = new NetTcpBinding();
                string address = $"net.{protocol}://{ip}:{port}/TestService";

                if (Database.openHosts.ContainsKey(address))
                {
                    Console.WriteLine("Host with that address already exists\n\n");
                    return;
                }

                binding.Security.Mode = SecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
                binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

                ServiceHost host = new ServiceHost(typeof(TestService));

                host.AddServiceEndpoint(typeof(ITest), binding, address);

                host.Open();
                Database.openHosts.Add(address, host);

                Program.auditProxy.LogEvent((int)AuditEventTypes.RunServiceSuccess, userName);

                Console.WriteLine("Port running");

               
            }
            catch (Exception ex)
            {
                CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
                // string userName = Formatter.ParseName(principal.Identity.Name);

                try
                {
                    Program.auditProxy.LogEvent((int)AuditEventTypes.RunServiceFailed, userName);
                    Console.WriteLine("Error during decryption or service run: " + ex.Message);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                // Console.WriteLine("Error during decryption or service run: " + ex.Message);
            }

        }

        public void StopService(string encryptedIp,string encryptedPort, string encryptedProtocol)
        {

            try
            {
                // Dekriptuji podatke
                string ip = aes.Decrypt(encryptedIp, sessionId);
                string port = aes.Decrypt(encryptedPort, sessionId);
                string protocol = aes.Decrypt(encryptedProtocol, sessionId);

                string address = $"net.{protocol}://{ip}:{port}/TestService";

                ServiceHost host = null;
                if (Database.openHosts.TryGetValue(address, out host))
                {
                    host.Close();
                    Database.openHosts.Remove(address);
                    Console.WriteLine("Host with address " + address + " is closed!\n\n");
                }
                else
                {
                    Console.WriteLine("Host with address " + address + " is not existing!\n\n");
                }



            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during decryption or service close: " + ex.Message);
            }


            
        }
    }
}
