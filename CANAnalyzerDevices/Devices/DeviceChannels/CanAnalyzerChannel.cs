using CANAnalyzerDevices.Devices.DeviceChannels.Events;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using CANAnalyzerDevices.Devices.DeviceChannels.Protocols.CanAnalyzer;
using System.ComponentModel.DataAnnotations;

namespace CANAnalyzerDevices.Devices.DeviceChannels
{
    public class CanAnalyzerChannel : IChannel
    {

        public CanAnalyzerChannel(IDevice owner, SerialPort port, byte index)
        {
            this.Index = index;
            this.Owner = owner;
            owner.IsConnectedChanged += OnOwner_IsConnectedChanged;
            _port = port;
            _port.DataReceived += OnPort_DataReceived;
        }

        private void OnOwner_IsConnectedChanged(object sender, EventArgs e)
        {
            if (!Owner.IsConnected)
                IsOpen = false;
        }

        private void OnPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (sender is SerialPort port)
            {
                ReceivedData data;
                lock (_port)
                {
                    byte[] buf = new byte[port.BytesToRead];
                    port.Read(buf, 0, buf.Length);
                    data = ParseData(buf);
                }

                if (data != null)
                    OnReceivedData(data);
            }
        }

        private ReceivedData ParseData(byte[] buf)
        {
            if (buf == null || buf.Length < 1)
                return null;

            switch(buf[0])
            {
                case 2:
                    UpdateChannelInfoParse(buf);
                    break;
                case 6:
                    return ReceiveCanADataParse(buf);
                    break;
                case 7:
                    return ReceiveCanBDataParse(buf);
                    break;
            }

            return null;
        }

        private ReceivedData ReceiveCanBDataParse(byte[] buf)
        {
            try
            {
                ReceiveCanBData data = (ReceiveCanBData)buf;
                if (data.ChannelId != this.Index)
                    return null;

                ReceivedData result = new ReceivedData();
                result.CanId = Convert.ToInt32(data.CanId);
                result.DLC = data.DLC;
                result.IsExtId = true;
                result.Time = (double)data.Time / 1000.0;
                result.Payload = data.data;
                return result;
            }
            catch
            {
                return null;
            }
        }

        private ReceivedData ReceiveCanADataParse(byte[] buf)
        {
            try
            {
                ReceiveCanAData data = (ReceiveCanAData)buf;
                if (data.ChannelId != this.Index)
                    return null;

                ReceivedData result = new ReceivedData();
                result.CanId = data.CanId;
                result.DLC = data.DLC;
                result.IsExtId = false;
                result.Time = (double)data.Time / 1000.0;
                result.Payload = data.data;
                return result;
            }
            catch
            {
                return null;
            }
        }

        private void UpdateChannelInfoParse(byte[] buf)
        {
            try
            {
                UpdateChannelInfo info = (UpdateChannelInfo)buf;
                if (info.ChannelId != this.Index)
                    return;

                Bitrate = info.BitrateType.Value();
                switch(info.Status)
                {
                    case CanState.Closed:
                        IsOpen = false;
                        break;
                    case CanState.OpenedListenOnly:
                        IsOpen = true;
                        IsListenOnly = true;
                        break;
                    case CanState.OpenedNormal:
                        IsOpen = true;
                        IsListenOnly = false;
                        break;
                }
            }
            catch
            {

            }
        }

        public byte Index { get; private set; }

        /// <summary>
        /// Current CAN bus bitrate.
        /// </summary>
        public int Bitrate { get; private set; }

        /// <summary>
        /// Current CAN transiever mode.
        /// </summary>
        public bool IsListenOnly { get; private set; } = true;

        /// <summary>
        /// CAN bus status
        /// </summary>
        public bool IsOpen
        {
            get { return _isOpen; }
            private set
            {
                if (value == _isOpen)
                    return;

                _isOpen = value;
                OnIsOpenChanged();
            }
        }
        private bool _isOpen = false;

        /// <summary>
        /// Channel owner.
        /// </summary>
        public IDevice Owner { get; private set; }



        /// <summary>
        /// Close channel. Need first connected to owner. 
        /// </summary>
        public void Close()
        {
            if (!Owner.IsConnected)
            {
                IsOpen = false;
                throw new Exception("open device connection first");
            }

            if (!IsOpen)
                return;

            SetChannelInfo data = new SetChannelInfo();
            data.ChannelId = Index;
            data.Status = CanState.Closed;

            byte[] buf = (byte[])data;
            lock (_port)
            {
                _port.Write(buf, 0, buf.Length);
            }
        }

        /// <summary>
        /// Open channel. Need first connected to owner. 
        /// If Channel already opened and params isn't equals then channel will be close and reopen.
        /// </summary>
        /// <param name="bitrate">Bitrate CAN bus in kbps.</param>
        /// <param name="isListenOnly">Flag which indicate CAN transiever mode.</param>
        public void Open(int bitrate = 500, bool isListenOnly = true)
        {
            if (!Owner.IsConnected)
            {
                IsOpen = false;
                throw new Exception("open device connection first");
            }



            if (IsOpen)
            {
                if (isListenOnly == this.IsListenOnly && Bitrate == bitrate)
                {
                    return;
                }
                else
                {
                    Close();
                }
            }


            SetChannelInfo data = new SetChannelInfo();
            data.ChannelId = Index;
            data.Status = isListenOnly ? CanState.OpenedListenOnly : CanState.OpenedNormal;

            //set bitrate
            switch (bitrate)
            {
                case 10:
                    data.BitrateType = BitrateType.kpbs10;
                    break;
                case 20:
                    data.BitrateType = BitrateType.kpbs20;
                    break;
                case 50:
                    data.BitrateType = BitrateType.kpbs50;
                    break;
                case 83:
                case 84:
                    data.BitrateType = BitrateType.kpbs83;
                    break;
                case 100:
                    data.BitrateType = BitrateType.kpbs100;
                    break;
                case 125:
                    data.BitrateType = BitrateType.kpbs125;
                    break;
                case 250:
                    data.BitrateType = BitrateType.kpbs250;
                    break;
                case 500:
                    data.BitrateType = BitrateType.kpbs500;
                    break;
                case 800:
                    data.BitrateType = BitrateType.kpbs800;
                    break;
                case 1000:
                    data.BitrateType = BitrateType.kpbs1000;
                    break;
                default:
                    throw new ArgumentException("invalid bitrate. Support only 10, 20, 50, 100, 125, 250, 500, 800 and 1000 kbps");
            }

            byte[] buf = (byte[])data;
            lock(_port)
            {
                _port.Write(buf, 0, buf.Length);
            }

        }

        /// <summary>
        /// Trasmit data to CAN channel.
        /// </summary>
        /// <param name="data">Data to be transmited.</param>
        public void Transmit(TransmitData data)
        {
            if (_port == null)
                return;

            if (!_port.IsOpen)
                Owner.Disconnect();


            var validatorResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(data, new ValidationContext(data), validatorResults, true))
            {
                string errors = "";
                foreach (var el in validatorResults)
                {
                    errors += $"{el.ErrorMessage}\r\n";
                }
                throw new ArgumentException($"Transmit data is invalid:\r\n{errors}");
            }

            if (!IsOpen || IsListenOnly)
                return;



            byte[] buf;
            object toTransmit = null;

            if(data.IsExtId)
            {
                TransmitCanBData package = new TransmitCanBData();
                package.ChannelId = Index;
                package.CanId = (UInt32)data.CanId;
                package.DLC = (byte)data.DLC;
                package.data = data.Payload;

                buf = (byte[])package;
                toTransmit = package;
            }
            else
            {
                TransmitCanAData package = new TransmitCanAData();
                package.ChannelId = Index;
                package.CanId = (UInt16)data.CanId;
                package.DLC = (byte)data.DLC;
                package.data = data.Payload;

                buf = (byte[])package;
                toTransmit = package;
            }

            if (!Validator.TryValidateObject(toTransmit, new ValidationContext(toTransmit), null, true))
            {
                return;
            }

            lock (_port)
            {
                _port.Write(buf, 0, buf.Length);
            }

        }

        public override string ToString()
        {
            return $"CanAnalyzerChannel{Index}";
        }

        SerialPort _port;

        FastSmartWeakEvent<ChannelDataReceivedEventHandler> _receivedData = new FastSmartWeakEvent<ChannelDataReceivedEventHandler>();
        public event ChannelDataReceivedEventHandler ReceivedData
        {
            add { _receivedData.Add(value); }
            remove { _receivedData.Remove(value); }
        }
        private void OnReceivedData(ReceivedData data)
        {
            _receivedData.Raise(this, new ChannelDataReceivedEventArgs(data));
        }

        FastSmartWeakEvent<EventHandler> _isOpenChanged = new FastSmartWeakEvent<EventHandler>();
        public event EventHandler IsOpenChanged
        {
            add { _isOpenChanged.Add(value); }
            remove { _isOpenChanged.Remove(value); }
        }
        private void OnIsOpenChanged()
        {
            _isOpenChanged.Raise(this, EventArgs.Empty);
        }
    }
}
