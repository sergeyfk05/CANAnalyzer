﻿/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using InfluxDBChannelProxy.XML.Server;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace InfluxDBChannelProxy.XML
{
    [Serializable]
    [XmlRoot("Config")]
    public class InfuxDBConfig
    {
        public ServerInfo Server { get; set; }
        public List<InfuxDBFilter> Filters
        {
            get { return _filters ?? (_filters = new List<InfuxDBFilter>()); }
            set { _filters = value; }
        }
        private List<InfuxDBFilter> _filters;
    }
}
