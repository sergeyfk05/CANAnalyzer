/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzerDevices.Finder;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CANAnalyzerDevices.Devices;
using CANAnalyzerDevices;
using CANAnalyzerDevice.Protocol;

namespace CANAnalyzerDevice
{
    public class CanAnalyzerCreator : IDeviceCreator
    {

        IDevice IDeviceCreator.CreateInstance(HardwareDeviceInfo info)
        {
            if (!((IDeviceCreator)this).IsCanWorkWith(info))
            {
                throw new ArgumentException("this device cannot work with this hardware device.");
            }


            return new CANAnalyzerDevice(info.PortName);
        }

        IDevice IDeviceCreator.CreateInstanceDefault(HardwareDeviceInfo info)
        {
            if (!((IDeviceCreator)this).IsCanWorkWith(info))
            {
                return null;
            }

            return new CANAnalyzerDevice(info.PortName);
        }


        bool IDeviceCreator.IsCanWorkWith(HardwareDeviceInfo info)
        {
            if (info.VID != 0483 || info.PID != 5740)
                return false;

            if (String.IsNullOrEmpty(SerialPort.GetPortNames().FirstOrDefault(x => x == info.PortName)))
                return false;

            using (SerialPort port = new SerialPort())
            {
                try
                {
                    port.PortName = info.PortName;
                    port.BaudRate = 115200;
                    port.DataBits = 8;
                    port.Parity = System.IO.Ports.Parity.None;
                    port.StopBits = System.IO.Ports.StopBits.One;

                    port.Open();
                    port.ReadTimeout = 500;
                    byte[] buf = new byte[15];

                    port.Write(new byte[] { 1 }, 0, 1);

                    //timeout read
                    int size = port.SafeRead(50, ref buf);

                    //check size and command id
                    if ((size != 5) || (buf[0] != 1))
                    {
                        port.Close();
                        return false;
                    }


                    DeviceInfo deviceInfo = (DeviceInfo)buf;

                    //check UID
                    if ((deviceInfo.UID[0] != 1) || (deviceInfo.UID[1] != 2) || (deviceInfo.UID[2] != 3))
                    {
                        port.Close();
                        return false;
                    }

                    port.Close();
                    return true;
                }
                catch
                {
                    port.Close();
                    return false;
                }
            }

        }

    }
}
