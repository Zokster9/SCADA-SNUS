using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SNUSProjekat
{
    [ServiceContract]
    public interface IDatabaseManager
    {
        [OperationContract]
        bool ChangeOutputValue(double output, string tagName);
        [OperationContract]
        double GetOutputValue(string tagName);
        [OperationContract]
        bool ChangeScanState(string tagName);
        [OperationContract]
        bool AddDigitalInputTag(string tagName, string description, string driver, string ioAddress, 
            int scanTime, bool onOffScan);
        [OperationContract]
        bool AddDigitalOutputTag(string tagName, string description, string ioAddress, double initialValue);
        [OperationContract]
        bool AddAnalogInputTag(string tagName, string description, string driver, string ioAddress, 
            int scanTime, bool onOffScan, double lowLimit, double highLimit, string units);
        [OperationContract]
        bool AddAnalogOutputTag(string tagName, string description, string ioAddress, double initialValue,
                double lowLimit, double highLimit, string units);
        [OperationContract]
        bool RemoveTag(string tagName);
    }
}
