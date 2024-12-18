using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ClientProxy : ChannelFactory<IServiceManagement>, IServiceManagement, IDisposable
    {

        IServiceManagement factory;

        public ClientProxy(NetTcpBinding binding,EndpointAddress address): base(binding, address)
        {
            factory = this.CreateChannel();
        }

        public string Connect()
        {
            try
            {
                string sessionId= factory.Connect();
                Console.WriteLine(sessionId);

                Console.WriteLine("Connected!\n");
                return sessionId;

            }catch(Exception e)
            {
                Console.WriteLine("Error : {0}",e.Message);
                return null;
            }

            
        }

        public void RunService(string ip, string port, string protocol)
        {
            try
            {
                factory.RunService(ip, port, protocol);
            }catch(Exception e)
            {
                Console.WriteLine("Error : {0}", e.Message);
            }
        }
    }
}
