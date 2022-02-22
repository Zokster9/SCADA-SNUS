using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SNUSProjekat
{
    [ServiceContract]
    interface IRealTimeUnit
    {
        [OperationContract]
        bool Init(string id, string address, byte[] signature, string message);
        [OperationContract]
        void UpdateValue(string address, int value);
    }
}
