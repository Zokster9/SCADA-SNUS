using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SNUSProjekat
{
    [ServiceContract]
    public interface IReportManagerService
    {
        [OperationContract]
        TriggeredAlarm[] GetAllTriggeredAlarmsByTime(DateTime lowLimit, DateTime highLimit);

        [OperationContract]
        TriggeredAlarm[] GetAllTriggeredAlarmsByPriority(int priority);

        [OperationContract]
        TagValue[] GetAllTagValuesByTime(DateTime lowLimit, DateTime highLimit);

        [OperationContract]
        TagValue[] GetLastAnalogInputTagValue();

        [OperationContract]
        TagValue[] GetLastDigitalInputTagValue();

        [OperationContract]
        TagValue[] GetAllTagValues(string tagName);
    }
}
