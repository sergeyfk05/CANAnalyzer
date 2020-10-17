using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CANAnalyzer.Models.ChannelsProxy.InfluxDB.XML
{
    [Serializable]
    [XmlRoot("Tag")]
    public class InfluxDBTag
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
