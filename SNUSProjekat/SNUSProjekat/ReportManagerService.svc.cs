using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SNUSProjekat
{
    public class ReportManagerService : IReportManagerService
    {
        public TagValue[] GetAllTagValues(string tagName)
        {
            using (var tagValueDb = new TagValueContext())
            {
                return tagValueDb.TagValues.Where(tagValue => tagValue.TagName == tagName)
                    .OrderBy(tagValue => tagValue.Value).Take(100).ToArray();
            }
        }

        public TagValue[] GetAllTagValuesByTime(DateTime lowLimit, DateTime highLimit)
        {
            using (var tagValueDb = new TagValueContext())
            {
                return tagValueDb.TagValues.Where(tagValue => tagValue.ValueTime >= lowLimit && 
                tagValue.ValueTime <= highLimit).OrderBy(tagValue => tagValue.ValueTime).Take(100).ToArray();
            }
        }

        public TriggeredAlarm[] GetAllTriggeredAlarmsByPriority(int priority)
        {
            using (var triggeredAlarmDb = new TriggeredAlarmContext())
            {
                return triggeredAlarmDb.TriggeredAlarms
                    .Where(triggeredAlarm => triggeredAlarm.Priority == priority)
                    .OrderBy(triggeredAlarm => triggeredAlarm.TriggerTime).Take(100).ToArray();
            }
        }

        public TriggeredAlarm[] GetAllTriggeredAlarmsByTime(DateTime lowLimit, DateTime highLimit)
        {
            using (var triggeredAlarmDb = new TriggeredAlarmContext())
            {
                return triggeredAlarmDb.TriggeredAlarms
                    .Where(triggeredAlarm => triggeredAlarm.TriggerTime >= lowLimit && 
                    triggeredAlarm.TriggerTime <= highLimit)
                    .OrderBy(triggeredAlarm => triggeredAlarm.Priority).ThenBy(triggeredAlarm => 
                    triggeredAlarm.TriggerTime).Take(100).ToArray();
            }
        }

        public TagValue[] GetLastAnalogInputTagValue()
        {
            using (var tagValueDb = new TagValueContext())
            {
                return tagValueDb.TagValues.Where(tagValue => tagValue.Type == 
                "SNUSProjekat.AnalogInput").GroupBy(tagValue => tagValue.TagName)
                .Select(group => group.OrderByDescending(tagValue => tagValue.ValueTime)
                .FirstOrDefault()).OrderBy(tagValue => tagValue.ValueTime).Take(100).ToArray();
            }
        }

        public TagValue[] GetLastDigitalInputTagValue()
        {
            using (var tagValueDb = new TagValueContext())
            {
                return tagValueDb.TagValues.Where(tagValue => tagValue.Type ==
                "SNUSProjekat.DigitalInput").GroupBy(tagValue => tagValue.TagName)
                .Select(group => group.OrderByDescending(tagValue => tagValue.ValueTime)
                .FirstOrDefault()).OrderBy(tagValue => tagValue.ValueTime).Take(100).ToArray();
            }
        }
    }
}
