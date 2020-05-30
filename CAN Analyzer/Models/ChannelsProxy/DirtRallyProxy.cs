using CANAnalyzerDevices.Devices;
using CANAnalyzerDevices.Devices.DeviceChannels;
using CANAnalyzerDevices.Devices.DeviceChannels.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using CANAnalyzerDevices.Devices;
using System.ComponentModel.DataAnnotations;

namespace CANAnalyzer.Models.ChannelsProxy
{
    public class DirtRallyProxy : IChannel, IChannelProxy, IDisposable
    {
        UdpClient receivingUdpClient;
        IPEndPoint RemoteIpEndPoint = null;
        Thread t;
        public DirtRallyProxy(string path)
        {
            Path = path;
            Name = this.ToString();

            receivingUdpClient = new UdpClient(Convert.ToInt32(File.ReadAllText(path)));
            RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, Convert.ToInt32(File.ReadAllText(path)));
            t = new Thread(() => ReceiveCallback(receivingUdpClient));
        }


        public string Path { get; private set; }

        public IChannel Channel { get; private set; }

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name)
                    return;

                _name = value;
                RaiseNameChangedEvent();
            }
        }
        private string _name = "";

        private void RaiseNameChangedEvent()
        {
            NameChanged?.Invoke(this, new EventArgs());
        }

        public int Bitrate => 500;

        public bool IsListenOnly => false;

        public bool IsOpen => t.IsAlive;

        public IDevice Owner => null;

        public event EventHandler NameChanged;
        public event ChannelDataReceivedEventHandler ReceivedData;
        private void RaiseReceivedData(ReceivedData data)
        {
            if (!Validator.TryValidateObject(data, new ValidationContext(data), null, true))
                return;

            ReceivedData?.Invoke(this, new ChannelDataReceivedEventArgs(data));
        }
        public event EventHandler IsOpenChanged;

        public void Close()
        {
            t.Abort();
        }

        public void Dispose()
        {
            t.Abort();
        }

        public void Open(int bitrate = 500, bool isListenOnly = true)
        {
            t.Start();
        }

        public void ReceiveCallback(UdpClient u)
        {
            IPEndPoint e = null;
            while (true)
            {
                byte[] receiveBytes = u.Receive(ref e);

                byte[] b = { receiveBytes[28], receiveBytes[29], receiveBytes[30], receiveBytes[31] };

                //Преобразуем и отображаем данные
                int speed = (int)(System.BitConverter.ToSingle(b, 0) * 3.6);
                int panel = 0;
                if (speed < 5)
                    panel = 0;
                else
                {
                    panel = 0x190 + 0x62 * (speed - 5);
                }


                ReceivedData data = new ReceivedData()
                {
                    CanId = 0x354,
                    DLC = 8,
                    Time = 0,
                    IsExtId = false,
                    Payload = new byte[8]
                };
                byte[] intBytes = BitConverter.GetBytes(panel);
                data.Payload[1] = intBytes[0];
                data.Payload[0] = intBytes[1];


                RaiseReceivedData(data);


                byte[] b2 = { receiveBytes[148], receiveBytes[149], receiveBytes[150], receiveBytes[151] };
                int rpm = (int)(System.BitConverter.ToSingle(b2, 0) *10);
                byte[] b3 = { receiveBytes[252], receiveBytes[253], receiveBytes[254], receiveBytes[255] };
                int maxrpm = (int)(System.BitConverter.ToSingle(b3, 0) * 10);

                ReceivedData data1 = new ReceivedData()
                {
                    CanId = 0x181,
                    DLC = 8,
                    Time = 0,
                    IsExtId = false,
                    Payload = new byte[8]
                };
                int panelRPM = 0x10FF;
                if (rpm > 6000)
                    panelRPM = 0xE0FF;
                else
                    panelRPM = 0x10FF + (int)(((double)rpm/maxrpm)* (0xE0FF - 0x10FF));




                intBytes = BitConverter.GetBytes(panelRPM);
                data1.Payload[1] = intBytes[0];
                data1.Payload[0] = intBytes[1];

                RaiseReceivedData(data1);
            }



        }

        public void SetChannel(IChannel newCH)
        {

        }

        public void Transmit(TransmitData data)
        {

        }

        public override string ToString()
        {
            return $"DirtRally";
        }
    }
}
