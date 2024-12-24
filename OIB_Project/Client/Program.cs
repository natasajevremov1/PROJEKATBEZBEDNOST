using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
            try
            {
                NetTcpBinding binding = new NetTcpBinding();
                string address = "net.tcp://localhost:9999/ServiceManagement";

                binding.Security.Mode = SecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
                binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

                // Kreiraj instancu AES enkripcije
                AESAlgorithm aes = new AESAlgorithm();

                Console.WriteLine("Client process run by user: " + WindowsIdentity.GetCurrent().Name);

                EndpointAddress endpointAddress = new EndpointAddress(new Uri(address), EndpointIdentity.CreateUpnIdentity("wcfService"));

                using (ClientProxy proxy = new ClientProxy(binding, endpointAddress))
                {

                    while (true)
                    {


                        Console.Write("IP :\t");
                        string ip = Console.ReadLine();
                        Console.WriteLine();
                        Console.Write("PORT :\t");
                        string port = Console.ReadLine();
                        Console.WriteLine();
                        Console.Write("PROTOCOL :\t");
                        string protocol = Console.ReadLine();
                        Console.WriteLine();

                        string userName = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

                        string sessionId = proxy.Connect(userName);

                        Console.WriteLine(sessionId);
                        if (string.IsNullOrEmpty(sessionId))
                        {
                            Console.WriteLine("Failed to establish session. Exiting...");
                            return;
                        }

                        // Enkriptuj podatke
                        string encryptedIp = aes.Encrypt(ip.Trim(), sessionId);
                        string encryptedPort = aes.Encrypt(port.Trim(), sessionId);
                        string encryptedProtocol = aes.Encrypt(protocol.Trim(), sessionId);

                        Console.WriteLine("\nEncrypted Data:");
                        Console.WriteLine("Encrypted IP: " + encryptedIp);
                        Console.WriteLine("Encrypted PORT: " + encryptedPort);
                        Console.WriteLine("Encrypted PROTOCOL: " + encryptedProtocol);

                        proxy.RunService(encryptedIp, encryptedPort, encryptedProtocol, userName);


                        string testAddress = $"net.{protocol}://{ip}:{port}/TestService";
                        EndpointAddress testEndPointAddress = new EndpointAddress(new Uri(testAddress), EndpointIdentity.CreateUpnIdentity("TestService"));
                        ChannelFactory<ITest> testFactory = new ChannelFactory<ITest>(binding);
                        ITest testProxy = testFactory.CreateChannel(testEndPointAddress);

                        // testProxy.TestConnection();

                        Console.WriteLine("CLIENT: Service run successfully!");

                        Console.ReadLine();

                    }

                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();

            }
            finally
            {
                Console.ReadLine();
            }
            
        }
    }
}
