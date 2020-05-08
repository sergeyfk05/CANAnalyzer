/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.ComponentModel.DataAnnotations;
using CANAnalyzerDevices.Devices.DeviceChannels.Events;
using System.Text.RegularExpressions;

namespace CANAnalyzerDevices.Devices.DeviceChannels
{
    /// <summary>
    /// Channel interface for CANAnalyzerDevices.Devices.CanHackerDevice
    /// </summary>
    public class CanHackerChannel : IChannel
    {
        public CanHackerChannel(IDevice owner, SerialPort port)
        {
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
            if (!IsOpen)
                return;

            if (sender is SerialPort port)
            {
                ReceivedData data;
                lock (_port)
                {
                    data = ParseData(port.ReadExisting());
                }

                if (data != null)
                    OnReceivedData(data);
            }
        }

        public static ReceivedData ParseData(string data)
        {
            ReceivedData result = new ReceivedData();

            if (data.Length < 9)
                return null;

            if (data[0] == 't')
                result.IsExtId = false;
            else if (data[0] == 'T')
                result.IsExtId = true;
            else
                return null;

            if (result.IsExtId)
            {
                var match = Regex.Match(data, @"T([0-9a-fA-F]{8})(\d{1})(\w*)\r");
                if (!match.Success)
                    return null;

                int dlc = Convert.ToInt32(match.Groups[2].Value);
                if (dlc < 0 || dlc > 8)
                    return null;

                match = Regex.Match(data, @"T([0-9a-fA-F]{8})+(\d{1})+([0-9a-fA-F]{" + (dlc * 2) + "})+([0-9a-fA-F]{4})+\r");
                if (!match.Success)
                    return null;


                result.CanId = Convert.ToInt32(match.Groups[1].Value, 16);
                result.DLC = Convert.ToInt32(match.Groups[2].Value, 16);
                result.Time = Convert.ToInt32(match.Groups[4].Value, 16) / 1000.0;
                result.Payload = new byte[result.DLC];

                for(int i = 0; i < result.DLC; i++)
                {
                    result.Payload[i] = byte.Parse(match.Groups[3].Value.Substring(2 * i, 2), System.Globalization.NumberStyles.HexNumber);
                }

            }
            else
            {
                var match = Regex.Match(data, @"t([0-9a-fA-F]{3})(\d{1})(\w*)\r");
                if (!match.Success)
                    return null;

                int dlc = Convert.ToInt32(match.Groups[2].Value);
                if (dlc < 0 || dlc > 8)
                    return null;

                match = Regex.Match(data, @"t([0-9a-fA-F]{3})+(\d{1})+([0-9a-fA-F]{" + (dlc * 2) + "})+([0-9a-fA-F]{4})+\r");
                if (!match.Success)
                    return null;


                result.CanId = Convert.ToInt32(match.Groups[1].Value, 16);
                result.DLC = Convert.ToInt32(match.Groups[2].Value, 16);
                result.Time = Convert.ToInt32(match.Groups[4].Value, 16) / 1000.0;
                result.Payload = new byte[result.DLC];

                for (int i = 0; i < result.DLC; i++)
                {
                    result.Payload[i] = byte.Parse(match.Groups[3].Value.Substring(2 * i, 2), System.Globalization.NumberStyles.HexNumber);
                }
            }

            if (!Validator.TryValidateObject(result, new ValidationContext(result), null, true))
                return null;


            return result;
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


            //set bitrate
            string command = "S";
            switch (bitrate)
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

            lock(_port)
            {
                _port.Write(command);

                //open channel
                if (isListenOnly)
                    _port.Write(new char[] { 'L' }, 0, 1);
                else
                    _port.Write(new char[] { 'O' }, 0, 1);
            }


            //open
            this.Bitrate = bitrate;
            this.IsListenOnly = isListenOnly;
            IsOpen = true;

        }

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

            IsOpen = false;
            lock (_port)
            {
                _port.Write(new char[] { 'C' }, 0, 1);
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

            string transmit = "";
            string payloadstr = "";
            for (int i = 0; i < data.DLC; i++)
                payloadstr += data.Payload[i].ToString("X2");

            if (data.IsExtId)
                transmit = $"T{data.CanId.ToString("X8")}{data.DLC.ToString("X1")}{payloadstr}\r";
            else
                transmit = $"t{data.CanId.ToString("X3")}{data.DLC.ToString("X1")}{payloadstr}\r";

            lock (_port)
            {
                _port.Write(transmit);
            }

        }

        /// <summary>
        /// Indicates that data has been received through a port represented by the CANAnalyzerDevices.Devices.DeviceChannels.IChannel object.
        /// </summary>
        public event ChannelDataReceivedEventHandler ReceivedData;
        private void OnReceivedData(ReceivedData data)
        {
            ReceivedData?.Invoke(this, new ChannelDataReceivedEventArgs(data));
        }

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

        public event EventHandler IsOpenChanged;
        private void OnIsOpenChanged()
        {
            IsOpenChanged?.Invoke(this, null);
        }


        public override string ToString()
        {
            return "CanHackerChannel";
        }

        SerialPort _port;
    }
}
