using System;
using System.Collections.Generic;
using System.Text;

namespace CANAnalyzer.Models.ChannelsProxy.InfluxDB.XML
{
    public class Insert
    {
        public string Measurement;
        public InfluxDBTag Tag { get; set; }
        public InfluxDBField Field { get; set; }
    }
}
