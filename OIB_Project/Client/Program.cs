using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9999/ServiceManagement";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            Console.WriteLine("Client process run by user: " + WindowsIdentity.GetCurrent().Name);

            EndpointAddress endpointAddress = new EndpointAddress(new Uri(address), EndpointIdentity.CreateUpnIdentity("Service"));

            // Kreiraj instancu AES enkripcije
            AESAlgorithm aes = new AESAlgorithm();

            // Unos podataka
            Console.Write("IP :\t");
            string ip = Console.ReadLine();
            Console.Write("PORT :\t");
            string port = Console.ReadLine();
            Console.Write("PROTOCOL :\t");
            string protocol = Console.ReadLine();

            
            // Kreiraj proxy i poveži se sa serverom
            using (ClientProxy proxy = new ClientProxy(binding, endpointAddress))
            {
               string sessionId= proxy.Connect();
                // Enkriptuj podatke
                string encryptedIp = aes.Encrypt(ip.Trim(),sessionId);
                string encryptedPort = aes.Encrypt(port.Trim(),sessionId);
                string encryptedProtocol = aes.Encrypt(protocol.Trim(), sessionId);

                Console.WriteLine("\nEncrypted Data:");
                Console.WriteLine("Encrypted IP: " + encryptedIp);
                Console.WriteLine("Encrypted PORT: " + encryptedPort);
                Console.WriteLine("Encrypted PROTOCOL: " + encryptedProtocol);

                proxy.RunService(encryptedIp, encryptedPort, encryptedProtocol);

                proxy.AddItemToBlacklist("port", port);
                string testAddress = $"net.{protocol}://{ip}:{port}/TestService";
                EndpointAddress testEndPointAddress = new EndpointAddress(new Uri(testAddress), EndpointIdentity.CreateUpnIdentity("TestService"));
                ChannelFactory<ITest> testFactory = new ChannelFactory<ITest>(binding);
                ITest testProxy = testFactory.CreateChannel(testEndPointAddress);

                testProxy.TestConnection();

                Console.WriteLine("CLIENT: Service run successfully!");
            }

            Console.ReadLine();
        }
    }
}

