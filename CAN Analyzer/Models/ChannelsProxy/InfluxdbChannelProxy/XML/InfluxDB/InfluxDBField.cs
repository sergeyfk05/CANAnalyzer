using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CANAnalyzer.Models.ChannelsProxy.InfluxDB.XML
{
    [Serializable]
    [XmlRoot("Field")]
    public class InfluxDBField
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
