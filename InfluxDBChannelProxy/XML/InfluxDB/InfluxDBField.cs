using System;
using System.Xml.Serialization;

namespace InfluxDBChannelProxy.XML.InfluxDB
{
    [Serializable]
    [XmlRoot("Field")]
    public class InfluxDBField
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
