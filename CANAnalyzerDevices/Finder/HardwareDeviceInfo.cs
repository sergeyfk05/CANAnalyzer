using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CANAnalyzerDevices.Finder
{
    internal class HardwareDeviceInfo
    {
        internal HardwareDeviceInfo(string portName, int vid, int pid)
        {
            this.PortName = portName;
            this.VID = vid;
            this.PID = pid;
        }

        internal HardwareDeviceInfo(string portName, string PNPDeviceID)
        {
            //"USB\\VID_0483&PID_5740\\3180327F3138"
            var match = Regex.Match(PNPDeviceID, @"USB\\VID_(\d{4})+&PID_(\d{4})+\\\w");
            if (!match.Success)
                throw new ArgumentException("PNPDeviceID is invalid");


            this.PortName = portName;
            this.VID = Convert.ToInt32(match.Groups[1].Value);
            this.PID = Convert.ToInt32(match.Groups[2].Value);
        }

        internal string PortName { get; private set; }

        internal int PID { get; private set; }
        internal int VID { get; private set; }
    }
}
