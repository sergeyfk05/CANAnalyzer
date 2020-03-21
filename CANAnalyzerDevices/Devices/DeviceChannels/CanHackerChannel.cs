using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace CANAnalyzerDevices.Devices.DeviceChannels
{
    public class CanHackerChannel : IChannel
    {
        public CanHackerChannel(IDevice owner, SerialPort port)
        {
            this.Owner = owner;
            _port = port;
        }


        public void Open(int bitrate = 500, bool isListenOnly = true)
        {
            if (!Owner.IsConnected)
            {
                IsOpen = false;
                throw new Exception("open device connection first");                
            }



            if (IsOpen)
            {
                if(isListenOnly == this.IsListenOnly && Bitrate == bitrate)
                {
                    return;
                }
                else
                {
                    Close();
                }
            }


            //set bitrate
            string command = "S";
            switch(bitrate)
            {
                case 10:
                    command += "0";
                    break;
                case 20:
                    command += "1";
                    break;
                case 50:
                    command += "2";
                    break;
                case 100:
                    command += "3";
                    break;
                case 125:
                    command += "4";
                    break;
                case 250:
                    command += "5";
                    break;
                case 500:
                    command += "6";
                    break;
                case 800:
                    command += "7";
                    break;
                case 1000:
                    command += "8";
                    break;
                default:
                    throw new ArgumentException("invalid bitrate. Support only 10, 20, 50, 100, 125, 250, 500, 800 and 1000 kbps");
            }
            _port.Write(command);

            //open channel
            if (isListenOnly)
                _port.Write(new char[] { 'L' }, 0, 1);
            else
                _port.Write(new char[] { 'O' }, 0, 1);


            //open
            this.Bitrate = bitrate;
            this.IsListenOnly = isListenOnly;
        }

        public void Close()
        {
            if (!Owner.IsConnected)
            {
                IsOpen = false;
                throw new Exception("open device connection first");
            }

            if (!IsOpen)
                return;

            IsOpen = false;
            _port.Write(new char[] { 'C' }, 0, 1);
        }


        public int Bitrate { get; private set; }

        public bool IsListenOnly { get; private set; } = true;

        public bool IsOpen { get; private set; } = false;

        public IDevice Owner { get; private set; }

        SerialPort _port;
    }
}
