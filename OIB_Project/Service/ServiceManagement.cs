using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ServiceManagement : IServiceManagement
    {
        private readonly AESAlgorithm aes = new AESAlgorithm(); // Instanca AES algoritma
        private string sessionId;

        [PrincipalPermission(SecurityAction.Demand, Role ="ExchangeSessionKey")]
        public string Connect()
        {
            Console.WriteLine("Client successfully connected  !");
            sessionId = OperationContext.Current.SessionId;
            Console.WriteLine("Session id: "+sessionId);
            return sessionId;
        }

        [PrincipalPermission(SecurityAction.Demand,Role ="RunService")]
        public void RunService(string encryptedIp, string encryptedPort, string encryptedProtocol)
        {
            try
            {
                // Dekriptuji podatke
                string ip = aes.Decrypt(encryptedIp,sessionId);
                string port = aes.Decrypt(encryptedPort,sessionId);
                string protocol = aes.Decrypt(encryptedProtocol,sessionId);

                Console.WriteLine($"Decrypted IP: {ip}");
                Console.WriteLine($"Decrypted Port: {port}");
                Console.WriteLine($"Decrypted Protocol: {protocol}");

                if (protocol.ToLower().Equals("tcp"))
                {
                    protocol = "net.tcp";
                }
                else
                {
                    return;
                }

                NetTcpBinding binding = new NetTcpBinding();
                string address = $"{protocol}://{ip}:{port}/TestService";

                binding.Security.Mode = SecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
                binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

                ServiceHost host = new ServiceHost(typeof(TestService));

                host.AddServiceEndpoint(typeof(ITest), binding, address);

                host.Open();

                Console.WriteLine("Port running");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during decryption or service run: " + ex.Message);
            }
        }
    }
}