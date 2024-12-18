using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IServiceManagement
    {

        [OperationContract]
        void Connect();


        [OperationContract]
        void RunService(string ip, string port, string protocol);
        [OperationContract]
        void AddItemToBlacklist(string type,string value);


    }
}
