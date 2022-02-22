using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmDisplay
{
    class AlarmDisplayCallback : ServiceReference.IAlarmDisplayCallback
    {
        public void AlarmTriggered(string tagName, string type, double limit, double value, 
            DateTime triggerTime, int priority)
        {
            for (int i = 0; i < priority; i++)
            {
                Console.WriteLine($"Tag: {tagName} with type {type} and limit of {limit} has been " +
                $"triggered by value of {value} at {triggerTime}");
            }
        }
    }
}
