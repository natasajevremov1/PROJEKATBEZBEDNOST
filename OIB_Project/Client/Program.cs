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

            binding.Security.Mode =  SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;


            Console.WriteLine("Client process run by user: " + WindowsIdentity.GetCurrent().Name);

            EndpointAddress endpointAddress = new EndpointAddress(new Uri(address), EndpointIdentity.CreateUpnIdentity("Service"));

            using(ClientProxy proxy = new ClientProxy(binding, endpointAddress))
            {
                proxy.Connect();

                Console.Write("IP :\t");
                string ip = Console.ReadLine();
                Console.WriteLine();
                Console.Write("PORT :\t");
                string port = Console.ReadLine();
                Console.WriteLine();
                Console.Write("PROTOCOL :\t");
                string protocol = Console.ReadLine();
                Console.WriteLine();

                proxy.RunService(ip.Trim(), port.Trim(), protocol.Trim());

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
