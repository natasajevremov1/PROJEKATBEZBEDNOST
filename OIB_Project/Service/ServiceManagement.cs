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

        [PrincipalPermission(SecurityAction.Demand, Role ="ExchangeSessionKey")]
        public void Connect()
        {
            Console.WriteLine("Client successfully connected  !");
            var sessionId = OperationContext.Current.SessionId;
            Console.WriteLine("Session id: "+sessionId);
        }

        [PrincipalPermission(SecurityAction.Demand,Role ="RunService")]
        public void RunService(string ip, string port, string protocol)
        {
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
    }
}
