using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using System.Threading;

namespace SNUSProjekat
{
    public static class TagProcessing
    {
        public static Dictionary<string, Tag> Tags = new Dictionary<string, Tag>();
        public static readonly object Locker = new object();
        public static readonly string SCADA_FILE = "scadaConfig.xml";
        public static readonly string LOG_FILE = "alarmsLog.txt";
        public delegate void InputTagValueChangedDelegate(string tagName, double value);
        public static event InputTagValueChangedDelegate InputTagValueChanged;
        public delegate void AlarmTriggeredDelegate(string tagName, string type, double limit, 
            double value, DateTime triggerTime, int priority);
        public static event AlarmTriggeredDelegate AlarmTriggered;
        public static Dictionary<string, Thread> InputTagThread = new Dictionary<string, Thread>();
        public static void SaveTags() 
        {
            using (var writer = new StreamWriter(SCADA_FILE))
            {
                var serializer = new XmlSerializer(typeof(List<Tag>));
                serializer.Serialize(writer, Tags.Values.ToList());
                Console.WriteLine("TAGS SAVED");
            }
        }

        public static void RunDriver(DigitalInput inputTag)
        {
            double outputValue;
            lock (Locker)
            {
                outputValue = -1000;
            }
            while (true)
            {
                if (inputTag.OnOffScan)
                {
                    double value = GetDriverValue(inputTag);

                    lock (Locker)
                    {
                        if (inputTag is AnalogInput)
                        {
                            AnalogInput analogInput = (AnalogInput)inputTag;
                            if (inputTag.Driver == "SIMULATION")
                            {
                                value = value < analogInput.LowLimit ? analogInput.LowLimit : 
                                    value;
                                value = value > analogInput.HighLimit ? analogInput.HighLimit : 
                                    value;
                            }
                            else
                            {
                                Random r = new Random();
                                if (value < analogInput.LowLimit || value > analogInput.HighLimit)
                                {
                                    value = r.NextDouble() * 
                                        (analogInput.HighLimit - analogInput.LowLimit) + 
                                        analogInput.LowLimit;
                                }
                            }

                            AlarmCheck(analogInput, value);
                        }
                        else
                        {
                            value = value < 51 ? 0 : 1;
                        }
                        if (value != outputValue)
                        {
                            outputValue = value;
                            TagValue tagValue;
                            if (inputTag is AnalogInput)
                            {
                                tagValue = new TagValue(inputTag.TagName, value,
                                typeof(AnalogInput).FullName, DateTime.Now);
                            }
                            else
                            {
                                tagValue = new TagValue(inputTag.TagName, value,
                                typeof(DigitalInput).FullName, DateTime.Now);
                            }
                            using (var tagValueDb = new TagValueContext())
                            {
                                tagValueDb.TagValues.Add(tagValue);
                                tagValueDb.SaveChanges();
                            }
                        }
                    }

                    InputTagValueChanged?.Invoke(inputTag.TagName, outputValue);
                }

                Thread.Sleep(inputTag.ScanTime * 1000);
            }
        }

        private static void AlarmCheck(AnalogInput analogInput, double value)
        {
            using (var alarmDb = new TriggeredAlarmContext())
            {
                foreach (Alarm alarm in analogInput.Alarms)
                {
                    if (alarm.Type == "low" && value <= alarm.Limit)
                    {
                        TriggeredAlarm triggeredAlarm = new TriggeredAlarm(alarm.Type,
                            alarm.Priority, alarm.Limit, alarm.TagName, DateTime.Now, value);
                        alarmDb.TriggeredAlarms.Add(triggeredAlarm);
                        alarmDb.SaveChanges();
                        AlarmTriggered?.Invoke(triggeredAlarm.TagName, triggeredAlarm.Type,
                            triggeredAlarm.Limit, triggeredAlarm.TriggerValue,
                            triggeredAlarm.TriggerTime, triggeredAlarm.Priority);
                        WriteAlarmToLog(triggeredAlarm);
                    }
                    else if (alarm.Type == "high" && value >= alarm.Limit)
                    {
                        TriggeredAlarm triggeredAlarm = new TriggeredAlarm(alarm.Type,
                            alarm.Priority, alarm.Limit, alarm.TagName, DateTime.Now, value);
                        alarmDb.TriggeredAlarms.Add(triggeredAlarm);
                        alarmDb.SaveChanges();
                        AlarmTriggered?.Invoke(triggeredAlarm.TagName, triggeredAlarm.Type,
                            triggeredAlarm.Limit, triggeredAlarm.TriggerValue,
                            triggeredAlarm.TriggerTime, triggeredAlarm.Priority);
                        WriteAlarmToLog(triggeredAlarm);
                    }
                }
            }
        }

        private static void WriteAlarmToLog(TriggeredAlarm triggeredAlarm)
        {
            lock (Locker)
            {
                using (StreamWriter sw = File.AppendText(LOG_FILE))
                {
                    sw.WriteLine($"Tag: {triggeredAlarm.TagName}; Type: {triggeredAlarm.Type}; " +
                        $"Limit: {triggeredAlarm.Limit}; " +
                        $"Trigger value: {triggeredAlarm.TriggerValue}; " +
                        $"Trigger time: {triggeredAlarm.TriggerTime}");
                }
            }
        }

        public static double GetDriverValue(DigitalInput inputTag)
        {
            return inputTag.Driver == "SIMULATION" ?
                SimulationDriver.ReturnValue(inputTag.IOAdress) : 
                RealTimeDriver.ReturnValue(inputTag.IOAdress);
        }

        internal static bool ChangeOutputValue(double output, string tagName)
        {
            if (output < 0) return false;
            Tag tag;
            TagValue tagValue;
            lock (Locker)
            {
                try
                {
                    tag = Tags[tagName];
                }
                catch
                {
                    return false;
                }
                if (tag is DigitalOutput && !(tag is AnalogOutput))
                {
                    if (output != 0 && output != 1) return false;
                    DigitalOutput digitalOutputTag = (DigitalOutput)tag;
                    digitalOutputTag.OutputValue = output;
                    tagValue = new TagValue(tagName, output, typeof(DigitalOutput).FullName, 
                        DateTime.Now);
                }
                else
                {
                    AnalogOutput analogOutputTag = (AnalogOutput)tag;
                    analogOutputTag.OutputValue = output;
                    tagValue = new TagValue(tagName, output, typeof(AnalogOutput).FullName,
                        DateTime.Now);
                }
                SaveTags();
                using (var tagValueDb = new TagValueContext())
                {
                    tagValueDb.TagValues.Add(tagValue);
                    tagValueDb.SaveChanges();
                }
                return true;
            }
        }

        public static bool AddAlarm(string tagName, string type, int priority, double limit)
        {
            Tag tag;
            lock (Locker)
            {
                try
                {
                    tag = Tags[tagName];
                }
                catch
                {
                    return false;
                }

                if (tag is AnalogInput)
                {
                    AnalogInput analogInput = (AnalogInput)tag;
                    analogInput.Alarms.Add(new Alarm(type.ToLower(), priority, limit, tagName));
                    SaveTags();
                    return true;
                }
            }            

            return false;
        }

        public static bool RemoveAlarm(string tagName, string type, int priority, double limit)
        {
            Tag tag;
            lock (Locker)
            {
                try
                {
                    tag = Tags[tagName];
                }
                catch
                {
                    return false;
                }

                if (tag is AnalogInput)
                {
                    AnalogInput analogInput = (AnalogInput)tag;
                    if (analogInput.Alarms.Count != 0)
                    {
                        foreach(Alarm alarm in analogInput.Alarms)
                        {
                            if (alarm.Limit == limit && alarm.Priority == priority && 
                                alarm.Type == type)
                            {
                                analogInput.Alarms.Remove(alarm);
                                SaveTags();
                                break;
                            }
                        }
                    }
                    return true;
                }
            }

            return false;
        }
        internal static bool AddDigitalOutputTag(string tagName, string description, string ioAddress, 
            double initialValue)
        {
            if (Tags.ContainsKey(tagName)) return false;
            Tag tag = new DigitalOutput(tagName, description, ioAddress, initialValue);
            lock (Locker)
            {
                Tags.Add(tagName, tag);
                SaveTags();
                TagValue tagValue = new TagValue(tagName, initialValue, typeof(DigitalOutput).FullName,
                        DateTime.Now);
                using (var tagValueDb = new TagValueContext())
                {
                    tagValueDb.TagValues.Add(tagValue);
                    tagValueDb.SaveChanges();
                }
                return true;
            }
        }

        internal static bool AddAnalogInputTag(string tagName, string description, string ioAddress, 
            string driver, int scanTime, bool onOffScan, double lowLimit, double highLimit, string units)
        {
            if (Tags.ContainsKey(tagName)) return false;
            Tag tag = new AnalogInput(tagName, description, ioAddress, driver.ToUpper(), scanTime,
                onOffScan, lowLimit, highLimit, units);
            lock (Locker)
            {
                Tags.Add(tagName, tag);
                SaveTags();
                Thread t = new Thread(() =>
                {
                    RunDriver((AnalogInput)tag);
                });
                InputTagThread.Add(tag.TagName, t);
                t.Start();
                return true;
            }
        }

        internal static bool RemoveTag(string tagName)
        {
            if (tagName == "") return false;
            lock (Locker)
            {
                if (Tags.Remove(tagName))
                {
                    try
                    {
                        Thread t = InputTagThread[tagName];
                        t.Abort();
                        InputTagThread.Remove(tagName);
                    }
                    catch
                    {

                    }
                    SaveTags();
                    return true;
                }
                return false;
            }
        }

        internal static bool AddAnalogOutputTag(string tagName, string description, string ioAddress, 
            double initialValue, double lowLimit, double highLimit, string units)
        {
            if (Tags.ContainsKey(tagName)) return false;
            Tag tag = new AnalogOutput(tagName, description, ioAddress, initialValue,
                lowLimit, highLimit, units);
            lock (Locker)
            {
                Tags.Add(tagName, tag);
                SaveTags();
                TagValue tagValue = new TagValue(tagName, initialValue, typeof(AnalogOutput).FullName,
                        DateTime.Now);
                using (var tagValueDb = new TagValueContext())
                {
                    tagValueDb.TagValues.Add(tagValue);
                    tagValueDb.SaveChanges();
                }
                return true;
            }
        }

        internal static bool AddDigitalInputTag(string tagName, string description, string ioAddress, 
            string driver, int scanTime, bool onOffScan)
        {
            if (Tags.ContainsKey(tagName)) return false;
            Tag tag = new DigitalInput(tagName, description, ioAddress, driver.ToUpper(), 
                scanTime, onOffScan);
            lock (Locker)
            {
                Tags.Add(tagName, tag);
                SaveTags();
                Thread t = new Thread(() =>
                {
                    RunDriver((DigitalInput)tag);
                });
                InputTagThread.Add(tag.TagName, t);
                t.Start();
                return true;
            }
        }

        internal static bool ChangeScanState(string tagName)
        {
            Tag tag;
            lock (Locker)
            {
                try
                {
                    tag = Tags[tagName];
                }
                catch
                {
                    return false;
                }
                if (tag is DigitalInput)
                {
                    DigitalInput digitalInputTag = (DigitalInput)tag;
                    digitalInputTag.OnOffScan = !digitalInputTag.OnOffScan;
                    SaveTags();
                    return true;
                }
                return false;
            }
        }

        internal static double GetOutputValue(string tagName)
        {
            Tag tag;
            lock (Locker)
            {
                try
                {
                    tag = Tags[tagName];
                }
                catch
                {
                    return -1000;
                }
                if (tag is DigitalOutput)
                {
                    DigitalOutput digitalOutputTag = (DigitalOutput)tag;
                    return digitalOutputTag.OutputValue;
                }
                return -1000;
            }
        }

        public static void LoadTags()
        {
            if (Tags.Count != 0) return;
            if (!File.Exists(SCADA_FILE))
            {
                Console.WriteLine("FILE DOES NOT EXIST");
            }
            else
            {
                using (var reader = new StreamReader(SCADA_FILE))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Tag>));
                    var tagsList = (List<Tag>)serializer.Deserialize(reader);
                    if (tagsList != null)
                    {
                        Tags = tagsList.ToDictionary(tag => tag.TagName);
                    }
                }
                AddThreads();
            }
        }

        private static void AddThreads()
        {
            if (InputTagThread.Count != 0) return;
            foreach(KeyValuePair<string, Tag> keyValuePair in Tags)
            {
                if (keyValuePair.Value is DigitalInput)
                {
                    Thread t = new Thread(() =>
                    {
                        RunDriver((DigitalInput)keyValuePair.Value);
                    });
                    InputTagThread.Add(keyValuePair.Value.TagName, t);
                }
            }

            RunThreads();
        }

        private static void RunThreads()
        {
            foreach (KeyValuePair<string, Thread> keyValuePair in InputTagThread)
            {
                keyValuePair.Value.Start();
            }
        }
    }
}