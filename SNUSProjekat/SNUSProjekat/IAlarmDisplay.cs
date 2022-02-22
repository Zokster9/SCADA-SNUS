using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SNUSProjekat
{
    [ServiceContract(CallbackContract = typeof(IAlarmDisplayCallback))]
    interface IAlarmDisplay
    {
        [OperationContract]
        void Init();
    }
}
