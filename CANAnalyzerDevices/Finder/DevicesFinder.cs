/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzerDevices.Devices;
using CANAnalyzerDevices.Devices.DeviceCreaters;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Management;

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
                HardwareDeviceInfo info;
                try
                {
                    info = new HardwareDeviceInfo(win32Device["DeviceID"].ToString(), win32Device["PNPDeviceID"].ToString());
                }
                catch
                {
                    continue;
                }

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
