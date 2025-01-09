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

        public void AddItemToBlacklist(string type, string value)
        {
            try
            {
                factory.AddItemToBlacklist(type,value);
                Console.WriteLine("Item is added!\n");

            }
            catch (Exception e)
            {
                Console.WriteLine("Error : {0}", e.Message);
            }
        }

        public string Connect(string userName)
        {
            try
            {
                string sessionId = factory.Connect(userName);
                Console.WriteLine("Connected!\n");
                return sessionId;

            }
            catch(Exception e)
            {
                Console.WriteLine("Error : {0}",e.Message);
                return null;
            }
        }

        public string ReadFromBlacklist()
        {
            try
            {
               string result = factory.ReadFromBlacklist();
                return result;
                
            }catch(Exception e)
            {
                Console.WriteLine("Error : {0}", e.Message);
                return string.Empty;
            }
        }

        public void RunService(string ip, string port, string protocol, string userName)
        {
            try
            {
                factory.RunService(ip, port, protocol, userName);
            }catch(Exception e)
            {
                Console.WriteLine("Error : {0}", e.Message);
            }
        }

        public bool StopService(string ip, string port, string protocol)
        {
            try
            {
                return factory.StopService(ip, port, protocol);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error : {0}", e.Message);
                return false;
            }
        }
    }
}
