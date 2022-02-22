using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
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

    [DataContract]
    public class TriggeredAlarm : Alarm
    {
        [Key]
        public int Id { get; set; }
        [DataMember] public DateTime TriggerTime { get; set; }
        [DataMember] public double TriggerValue { get; set; }
        public TriggeredAlarm()
        {

        }

        public TriggeredAlarm(string type, int priority, double limit, string tagName, 
            DateTime triggerTime, double triggerValue) : base(type, priority, limit, tagName)
        {
            TriggerTime = triggerTime;
            TriggerValue = triggerValue;
        }
    }

    public class TriggeredAlarmContext : DbContext
    {
        public DbSet<TriggeredAlarm> TriggeredAlarms { get; set; }
    }
}