/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using InfluxDBChannelProxy.XML.CanPackage;
using InfluxDBChannelProxy.XML.InfluxDB;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace InfluxDBChannelProxy.XML
{
    [Serializable]
    [XmlRoot("Filter")]
    public class InfuxDBFilter
    {
        public CanHeader Header { get; set; }
        public List<Insert> Inserts
        {
            get { return _inserts ?? (_inserts = new List<Insert>()); }
            set { _inserts = value; }
        }
        private List<Insert> _inserts;
    }
}
