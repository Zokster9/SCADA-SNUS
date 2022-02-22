using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SNUSProjekat
{
    interface IAlarmDisplayCallback
    {
        [OperationContract(IsOneWay = true)]
        void AlarmTriggered(string tagName, string type, double limit, double value, 
            DateTime triggerTime, int priority);
    }
}
