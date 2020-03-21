using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using Microsoft.Win32;
using System.Management;
using System.Text.RegularExpressions;
using CANAnalyzerDevices.Devices.DeviceCreaters;

namespace CANAnalyzerDevices.Finder
{
    public class DevicesFinder
    {

        public static void find()
        {
            List<IDeviceCreator> creators = DeviceCreatorsBuilder.BuildDeviceCreatorsList();

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_SerialPort");
            foreach (ManagementObject obj in searcher.Get())
            {
                HardwareDeviceInfo info = new HardwareDeviceInfo(obj["DeviceID"].ToString(), obj["PNPDeviceID"].ToString());

                foreach(var creator in creators)
                {
                    if (!creator.IsCanWorkWith(info))
                        continue;
                }


            }
        }
    }
}
