using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface IServiceManagement
    {

        [OperationContract]
        string Connect();


        [OperationContract]
        void RunService(string ip, string port, string protocol);


    }
}
