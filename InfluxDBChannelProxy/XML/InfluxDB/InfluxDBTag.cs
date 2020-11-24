using System;
using System.Xml.Serialization;

namespace InfluxDBChannelProxy.XML.InfluxDB
{
    [Serializable]
    [XmlRoot("Tag")]
    public class InfluxDBTag
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
