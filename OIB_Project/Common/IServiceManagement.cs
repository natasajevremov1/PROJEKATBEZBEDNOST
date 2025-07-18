﻿using System;
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
        string Connect(string userName);


        [OperationContract]
        bool RunService(string ip, string port, string protocol, string userName);
        [OperationContract]
        void AddItemToBlacklist(string type,string value);

        [OperationContract]
        bool StopService(string ip, string port, string protocol);

        [OperationContract]
        string ReadFromBlacklist();
    }
}
