using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SNUSProjekat
{
    [DataContract]
    public class Alarm
    {
        [DataMember] public string Type { get; set; }
        [DataMember] public int Priority { get; set; }
        [DataMember] public double Limit { get; set; }
        [DataMember] public string TagName { get; set; }

        public Alarm()
        {

        }

        public Alarm(string type, int priority, double limit, string tagName)
        {
            Type = type;
            Priority = priority;
            Limit = limit;
            TagName = tagName;
        }
    }
}