using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Management;

namespace CANAnalyzerDevices.Devices
{
    public class CANAnalyzerDevice
    {
        public CANAnalyzerDevice()
        {
            string query = "SELECT * FROM Win32_SerialPort";

            string[] ModemObjects = new string[250];
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            byte k = 0;
            foreach (ManagementObject obj in searcher.Get())
            {


                ModemObjects[k] = obj["Name"].ToString() + "(" + obj["AttachedTo"].ToString() + ")";
                k += 1;

            }

            return; 
        }
    }
}
