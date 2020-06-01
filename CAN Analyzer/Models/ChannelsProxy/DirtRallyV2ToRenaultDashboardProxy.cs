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
using System.ComponentModel.DataAnnotations;

namespace CANAnalyzer.Models.ChannelsProxy
{
    public class DirtRallyV2ToRenaultDashboardProxy : IChannel, IChannelProxy, IDisposable
    {

        public DirtRallyV2ToRenaultDashboardProxy(string path)
        {
            Path = path;
            Name = this.ToString();

            udpClient = new UdpClient(Convert.ToInt32(File.ReadAllText(path)));
            worker = new Thread(() => ReceiveCallback(udpClient));
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


        public int Bitrate => 500;

        public bool IsListenOnly => false;

        public bool IsOpen
        {
            get { return _isOpen; }
            private set
            {
                if (_isOpen == value)
                    return;

                _isOpen = value;
                RaiseIsOpenChangedEvent();
            }

        }
        private bool _isOpen = false;

        public IDevice Owner => null;        

        public void Close()
        {
            worker.Abort();
            IsOpen = false;
        }

        public void Dispose()
        {
            worker.Abort();
            udpClient.Close();
        }

        public void Open(int bitrate = 500, bool isListenOnly = true)
        {
            worker.Start();
            IsOpen = true;
        }

        private void ReceiveCallback(UdpClient client)
        {
            IPEndPoint e = null;
            while (true)
            {
                byte[] receivedBytes = client.Receive(ref e);
                r(receivedBytes);
                //int i = 0;

                //i++;

                //if(i>10)
                //{
                //    Action a = () => { r(receivedBytes); };
                //    a.Invoke();
                //    i = 0;
                //}

            }
        }

        private void r(byte[] receivedBytes)
        {
            //get speed
            int speed = (int)(System.BitConverter.ToSingle(receivedBytes, 28) * 3.6);
            //convert speed to renault's dashbord
            int dashbordSpeed = speed < 5 ? 0 : 0x190 + 0x62 * (speed - 5);

            //create data for physical dashbord
            byte[] convertedData = BitConverter.GetBytes(dashbordSpeed);
            ReceivedData ABSData = new ReceivedData()
            {
                CanId = 0x354,
                DLC = 8,
                Time = DateTime.Now.Second + DateTime.Now.Millisecond / 1000,
                IsExtId = false,
                Payload = new byte[] { convertedData[1], convertedData[0], 0, 0, 0, 0, 0, 0 }
            };
            RaiseReceivedData(ABSData);


            //get engine rmp and maximum rpm
            int rpm = (int)(System.BitConverter.ToSingle(receivedBytes, 148) * 10);
            int maxrpm = (int)(System.BitConverter.ToSingle(receivedBytes, 252) * 10);

            //convert rpm to renault's dashbord
            int dashbordRPM = 0x10FF + (int)(((double)rpm / maxrpm) * (0xE0FF - 0x10FF));
            dashbordRPM = dashbordRPM > 0xE0FF ? 0xE0FF : dashbordRPM;
            convertedData = BitConverter.GetBytes(dashbordRPM);

            //create data for physical panel
            ReceivedData EngineData = new ReceivedData()
            {
                CanId = 0x181,
                DLC = 8,
                Time = 0,
                IsExtId = false,
                Payload = new byte[] { convertedData[1], convertedData[0], 0, 0, 0, 0, 0, 0 }
            };
            RaiseReceivedData(EngineData);
        }

        public void SetChannel(IChannel newCH)
        {

        }

        public void Transmit(TransmitData data)
        {

        }

        public event EventHandler IsOpenChanged;
        private void RaiseIsOpenChangedEvent()
        {
            IsOpenChanged?.Invoke(this, new EventArgs());
        }

        public event EventHandler NameChanged;
        private void RaiseNameChangedEvent()
        {
            NameChanged?.Invoke(this, new EventArgs());
        }

        public event ChannelDataReceivedEventHandler ReceivedData;
        private void RaiseReceivedData(ReceivedData data)
        {
            if (!Validator.TryValidateObject(data, new ValidationContext(data), null, true))
                return;

            ReceivedData?.Invoke(this, new ChannelDataReceivedEventArgs(data));
        }


        private UdpClient udpClient;
        private Thread worker;


        public override string ToString()
        {
            return $"DirtRallyV2ToRenaultDashboardProxy";
        }
    }
}
