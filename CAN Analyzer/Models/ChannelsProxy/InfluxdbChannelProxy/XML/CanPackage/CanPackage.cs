﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CANAnalyzer.Models.ChannelsProxy.InfluxDB.XML
{
    public class CanHeader
    {
        public int Id { get; set; }
        public bool IsExtId { get; set; }
        public byte DLC { get; set; }
    }
}