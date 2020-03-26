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
using CANAnalyzerDevices.Devices;

namespace CANAnalyzerDevices.Finder
{
    public static class DevicesFinder
    {
        /// <summary>
        /// Method for find available devices.
        /// </summary>
        /// <returns>Returns list of available to work devices.</returns>
        public static IEnumerable<IDevice> FindAvailableDevices()
        {
            List<IDevice> availableDevices = new List<IDevice>();
            List<IDeviceCreator> creators = DeviceCreatorsBuilder.BuildDeviceCreatorsList();

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_SerialPort");
            foreach (ManagementObject win32Device in searcher.Get())
            {
                HardwareDeviceInfo info = new HardwareDeviceInfo(win32Device["DeviceID"].ToString(), win32Device["PNPDeviceID"].ToString());

                foreach(var creator in creators)
                {
                    IDevice a = creator.CreateInstanceDefault(info);
                    if (a != null)
                        availableDevices.Add(a);
                }
            }

            return availableDevices;
        }
    }
}
