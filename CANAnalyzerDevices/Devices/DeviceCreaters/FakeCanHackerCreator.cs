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

namespace CANAnalyzerDevices.Devices.DeviceCreaters
{
    internal class FakeCanHackerCreator : IDeviceCreator
    {
        /// <summary>
        /// Check on compatability device.
        /// </summary>
        /// <param name="info">Information needed for check compatability.</param>
        /// <returns>Return true if device is compatability, else return false.</returns>
        bool IDeviceCreator.IsCanWorkWith(HardwareDeviceInfo info)
        {
            if (info.VID != 0483 || info.PID != 5740)
                return false;

            if (String.IsNullOrEmpty(SerialPort.GetPortNames().FirstOrDefault(x => x == info.PortName)))
                return false;

            try
            {
                using (SerialPort port = new SerialPort())
                {
                    port.PortName = info.PortName;
                    port.BaudRate = 115200;
                    port.DataBits = 8;
                    port.Parity = System.IO.Ports.Parity.None;
                    port.StopBits = System.IO.Ports.StopBits.One;

                    port.Open();
                    char[] buf = new char[15];

                    port.Write(new char[] { 'v' }, 0, 1);
                    int size = port.Read(buf, 0, 15);

                    if (size != 7)
                        return false;

                    if (new string(buf, 0, 7) != "vSTM32\r")
                        return false;



                    port.Write(new char[] { 'V' }, 0, 1);

                    size = port.Read(buf, 0, 15);

                    if (size != 6)
                        return false;

                    if (new string(buf, 0, 6) != "V0100\r")
                        return false;


                    port.Close();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Create new IDevice.
        /// </summary>
        /// <param name="info">Information needed for creation IDevice</param>
        /// <param name="returnDefault">If device isn't supported, then return null or exception. If true - return null or IDevice, if false - return IDevice or throw exceprion.</param>
        /// <returns>Return IDevice. Before creation checks compatibility. If not compatible, then throw exception or returned null.</returns>
        IDevice IDeviceCreator.CreateInstance(HardwareDeviceInfo info)
        {
            if (!((IDeviceCreator)this).IsCanWorkWith(info))
            {
                throw new ArgumentException("this device cannot work with this hardware device.");
            }


            return new FakeCanHackerDevice(info.PortName);
        }

        IDevice IDeviceCreator.CreateInstanceDefault(HardwareDeviceInfo info)
        {
            if (!((IDeviceCreator)this).IsCanWorkWith(info))
            {
                return null;
            }


            return new FakeCanHackerDevice(info.PortName);
        }

    }
}
