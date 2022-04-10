/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
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
