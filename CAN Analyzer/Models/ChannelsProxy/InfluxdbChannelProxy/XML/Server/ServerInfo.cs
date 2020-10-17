using System;
using System.Collections.Generic;
using System.Text;

namespace CANAnalyzer.Models.ChannelsProxy.InfluxDB.XML
{
    public struct ServerInfo
    {
        public string Hostname { get; set; }
        public short Port { get; set; }
        public string Token { get; set; }
        public string Organization { get; set; }
        public string Bucket { get; set; }
    }
}
