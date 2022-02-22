using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlarmDisplay.ServiceReference;
using System.ServiceModel;

namespace AlarmDisplay
{
    class Program
    {
        static AlarmDisplayClient alarmDisplayClient = new AlarmDisplayClient(
            new InstanceContext(new AlarmDisplayCallback()));
        static void Main(string[] args)
        {
            Console.WriteLine("Alarm display started...");
            alarmDisplayClient.Init();
            Console.ReadKey();
        }
    }
}
