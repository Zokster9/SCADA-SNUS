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
        public static readonly string FILE = "scadaConfig.xml";
        public delegate void InputTagValueChangedDelegate(string tagName, double value);
        public static event InputTagValueChangedDelegate InputTagValueChanged;
        public static Dictionary<string, Thread> inputTagThread = new Dictionary<string, Thread>();
        public static void SaveTags() 
        {
            using (var writer = new StreamWriter(FILE))
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
                bool isScanOn = inputTag.OnOffScan;
                if (isScanOn)
                {
                    double value = GetDriverValue(inputTag);

                    lock (Locker)
                    {
                        if (inputTag is AnalogInput)
                        {
                            AnalogInput analogInput = (AnalogInput)inputTag;
                            value = value < analogInput.LowLimit ? analogInput.LowLimit : value;
                            value = value > analogInput.HighLimit ? analogInput.HighLimit : value;
                        }
                        else
                        {
                            value = value < 50 ? 0 : 1;
                        }
                        if (value != outputValue)
                        {
                            outputValue = value;
                        }
                    }

                    InputTagValueChanged?.Invoke(inputTag.TagName, outputValue);
                }

                Thread.Sleep(inputTag.ScanTime * 1000);
            }
        }

        public static double GetDriverValue(DigitalInput inputTag)
        {
            return inputTag.Driver == "SIMULATION" ?
                SimulationDriver.ReturnValue(inputTag.IOAdress) : -1000;
        }

        internal static bool ChangeOutputValue(double output, string tagName)
        {
            if (output < 0) return false;
            Tag tag;
            lock (Locker)
            {
                try
                {
                    tag = Tags[tagName];
                }
                catch (KeyNotFoundException)
                {
                    return false;
                }
                if (tag is DigitalOutput && !(tag is AnalogOutput))
                {
                    if (output != 0 && output != 1) return false;
                    DigitalOutput digitalOutputTag = (DigitalOutput)tag;
                    digitalOutputTag.OutputValue = output;
                }
                else
                {
                    AnalogOutput analogOutputTag = (AnalogOutput)tag;
                    analogOutputTag.OutputValue = output;
                }
                SaveTags();
                return true;
            }
        }

        internal static bool AddDigitalOutputTag(string tagName, string description, string ioAddress, 
            double initialValue)
        {
            Tag tag = new DigitalOutput(tagName, description, ioAddress, initialValue);
            lock (Locker)
            {
                Tags.Add(tagName, tag);
                SaveTags();
                return true;
            }
        }

        internal static bool AddAnalogInputTag(string tagName, string description, string ioAddress, 
            string driver, int scanTime, bool onOffScan, double lowLimit, double highLimit, string units)
        {
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
                inputTagThread.Add(tag.TagName, t);
                t.Start();
                return true;
            }
        }

        internal static bool RemoveTag(string tagName)
        {
            lock (Locker)
            {
                if (Tags.Remove(tagName))
                {
                    SaveTags();
                    return true;
                }
                return false;
            }
        }

        internal static bool AddAnalogOutputTag(string tagName, string description, string ioAddress, 
            double initialValue, double lowLimit, double highLimit, string units)
        {
            Tag tag = new AnalogOutput(tagName, description, ioAddress, initialValue,
                lowLimit, highLimit, units);
            lock (Locker)
            {
                Tags.Add(tagName, tag);
                SaveTags();
                return true;
            }
        }

        internal static bool AddDigitalInputTag(string tagName, string description, string ioAddress, 
            string driver, int scanTime, bool onOffScan)
        {
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
                inputTagThread.Add(tag.TagName, t);
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
                catch (KeyNotFoundException)
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
                catch (KeyNotFoundException)
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
            if (!File.Exists(FILE))
            {
                Console.WriteLine("FILE DOES NOT EXIST");
            }
            else
            {
                using (var reader = new StreamReader(FILE))
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
            foreach(KeyValuePair<string, Tag> keyValuePair in Tags)
            {
                if (keyValuePair.Value is DigitalInput)
                {
                    Thread t = new Thread(() =>
                    {
                        RunDriver((DigitalInput)keyValuePair.Value);
                    });
                    inputTagThread.Add(keyValuePair.Value.TagName, t);
                }
            }

            RunThreads();
        }

        private static void RunThreads()
        {
            foreach (KeyValuePair<string, Thread> keyValuePair in inputTagThread)
            {
                keyValuePair.Value.Start();
            }
        }
    }
}