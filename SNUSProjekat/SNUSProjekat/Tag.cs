using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace SNUSProjekat
{
    [XmlInclude(typeof(DigitalInput))]
    [XmlInclude(typeof(DigitalOutput))]
    [KnownType(typeof(DigitalInput))]
    [KnownType(typeof(DigitalOutput))]
    [DataContract]
    public abstract class Tag
    {
        [DataMember] public string TagName { get; set; }
        [DataMember] public string Description { get; set; }
        [DataMember] public string IOAdress { get; set; }

        public Tag()
        {

        }

        public Tag(string tagName, string description, string ioAdress)
        {
            TagName = tagName;
            Description = description;
            IOAdress = ioAdress;
        }
    }

    [XmlInclude(typeof(AnalogInput))]
    [KnownType(typeof(AnalogInput))]
    [DataContract]
    public class DigitalInput : Tag
    {
        [DataMember] public string Driver { get; set; }
        [DataMember] public int ScanTime { get; set; }
        [DataMember] public bool OnOffScan { get; set; }

        public DigitalInput() : base()
        {
            
        }

        public DigitalInput(string tagName, string description, string ioAdress, string driver, 
            int scanTime, bool onOffScan) : base(tagName, description, ioAdress)
        {
            Driver = driver;
            ScanTime = scanTime;
            OnOffScan = onOffScan;
        }
    }

    [XmlInclude(typeof(AnalogOutput))]
    [KnownType(typeof(AnalogOutput))]
    [DataContract]
    public class DigitalOutput : Tag
    {
        [DataMember] public double InitialValue { get; set; }
        [DataMember] public double OutputValue { get; set; }

        public DigitalOutput() : base()
        {
            OutputValue = InitialValue;
        }

        public DigitalOutput(string tagName, string description, string ioAdress,
            double initialValue) : base(tagName, description, ioAdress)
        {
            InitialValue = initialValue;
            OutputValue = InitialValue;
        }
    }

    [DataContract]
    public class AnalogInput : DigitalInput
    {
        [DataMember] public double LowLimit { get; set; }
        [DataMember] public double HighLimit { get; set; }
        [DataMember] public string Units { get; set; }

        public AnalogInput() : base()
        {

        }

        public AnalogInput(string tagName, string description, string ioAdress, string driver,
            int scanTime, bool onOffScan, double lowLimit, double highLimit, string units) : base(tagName, description, ioAdress, driver, scanTime, onOffScan)
        {
            LowLimit = lowLimit;
            HighLimit = highLimit;
            Units = units;
        }
    }

    [DataContract]
    public class AnalogOutput : DigitalOutput
    {
        [DataMember] public double LowLimit { get; set; }
        [DataMember] public double HighLimit { get; set; }
        [DataMember] public string Units { get; set; }

        public AnalogOutput() : base()
        {

        }

        public AnalogOutput(string tagName, string description, string ioAdress, double initialValue,
            double lowLimit, double highLimit, string units) : base(tagName, description, ioAdress, initialValue)
        {
            LowLimit = lowLimit;
            HighLimit = highLimit;
            Units = units;
        }
    }
}