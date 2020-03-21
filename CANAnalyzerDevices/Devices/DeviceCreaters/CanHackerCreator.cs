using CANAnalyzerDevices.Finder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace CANAnalyzerDevices.Devices.DeviceCreaters
{
    internal class CanHackerCreator : IDeviceCreator
    {
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

                    port.Write(new char[]{ 'v' }, 0, 1);
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

            return false;
        }



    }
}
