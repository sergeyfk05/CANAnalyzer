/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzerDevices.Devices;
using CANAnalyzerDevices.Devices.DeviceChannels;
using CANAnalyzerDevices.Devices.DeviceChannels.Events;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace CANAnalyzer.Models.ChannelsProxy
{
    public class ConsoleChannelProxy : IChannel, IDisposable, IChannelProxy
    {
        public ConsoleChannelProxy(string path)
        {
            Path = path;
            Name = this.ToString();

            if (!File.Exists(path))
                throw new ArgumentException("Invalid file path.");

            try
            {
                _process = new Process
                {
#if DEBUG
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = path,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = true,
                        CreateNoWindow = false
                    }
#else
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = path,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = true,
                        CreateNoWindow = true
                    }
#endif
                };
                _process.Start();


                if (!CheckConnection())
                    throw new ArgumentException("Invalide file\r\n");
            }
            catch (Exception e)
            {
                throw new ArgumentException("Invalide file\r\n" + e.ToString());
            }
        }

        private void OnRealChannel_ReceivedData(object sender, ChannelDataReceivedEventArgs e)
        {
            SendDataToProxy(e.Data);

            string response = _process.StandardOutput.ReadLine();
            var match = Regex.Match(response,
                @"([0-1]{1});" +
                @"([0-9a-fA-F]{8});" +
                @"([0-9a-fA-F]{1});" +
                @"([0-9a-fA-F])+;" +
                @"([0-9a-fA-F]{4});");

            if (!match.Success)
                return;

            ReceivedData result = new ReceivedData();
            try
            {
                result.DLC = Convert.ToInt32(match.Groups[3].Value, 16);
            }
            catch { return; }


            match = Regex.Match(response,
                @"([0-1]{1});" +
                @"([0-9a-fA-F]{8});" +
                @"([0-9a-fA-F]{1});" +
                @"([0-9a-fA-F]{" + (result.DLC * 2) + "});" +
                @"([0-9a-fA-F]{4});");


            if (!match.Success)
                return;

            try
            {
                result.IsExtId = Convert.ToInt32(match.Groups[1].Value, 16) == 0 ? false : true;
                result.CanId = Convert.ToInt32(match.Groups[2].Value, 16);
                result.Time = Convert.ToInt32(match.Groups[5].Value, 16) / 1000.0;
                result.Payload = new byte[result.DLC];

                for (int i = 0; i < result.DLC; i++)
                {
                    result.Payload[i] = Convert.ToByte(match.Groups[4].Value.Substring(i * 2, 2), 16);
                }
            }
            catch { return; }


            RaiseReceivedData(result);

        }

        private void SendDataToProxy(ReceivedData data)
        {

            string payload = "";
            for(int i=0;i<data.DLC;i++)
            {
                payload += data.Payload[i].ToString("X2");
            }

            string response = $"{(data.IsExtId ? "1" : "0")};" +
                $"{data.CanId.ToString("X8")};" +
                $"{data.DLC.ToString("X1")};" +
                $"{payload};" +
                $"{((int)(data.Time * 1000)).ToString("X4")};";

            _process.StandardInput.WriteLine(response);
        }

        private Process _process;
        public IChannel Channel { get; private set; }

        public void SetChannel(IChannel newCH)
        {
            try
            {
                if (Channel != null)
                    Channel.ReceivedData -= OnRealChannel_ReceivedData;
            }
            catch { }

            Channel = newCH;
            if(Channel != null)
                Channel.ReceivedData += OnRealChannel_ReceivedData;
        }

        public string Path { get; private set; }

        private bool CheckConnection()
        {
            string response = "Hello world!";
            _process.StandardInput.WriteLine(response);

            string message = _process.StandardOutput.ReadLine();

            return message.TrimEnd('\0') == response;
        }

        public int Bitrate => Channel.Bitrate;

        public bool IsListenOnly
        {
            get { return _isListenOnly; }
            set
            {
                if (value == _isListenOnly)
                    return;

                _isListenOnly = value;
            }
        }
        private bool _isListenOnly = true;

        public bool IsOpen
        {
            get { return _isOpen; }
            private set
            {
                if (value == _isOpen)
                    return;

                _isOpen = value;
                RaiseIsOpenChanged();
            }
        }
        private bool _isOpen = false;

        public IDevice Owner => Channel.Owner;

        public event ChannelDataReceivedEventHandler ReceivedData;
        private void RaiseReceivedData(ReceivedData data)
        {
            if (!Validator.TryValidateObject(data, new ValidationContext(data), null, true))
                return;

                ReceivedData?.Invoke(this, new ChannelDataReceivedEventArgs(data));
        }

        public event EventHandler IsOpenChanged;
        private void RaiseIsOpenChanged()
        {
            IsOpenChanged?.Invoke(this, null);
        }

        public void Close()
        {
            IsOpen = false;
        }

        public void Open(int bitrate = 500, bool isListenOnly = true)
        {
            IsOpen = true;
            IsListenOnly = isListenOnly;
        }

        public void Transmit(TransmitData data)
        {
            //throw new NotImplementedException();
        }


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

        public event EventHandler NameChanged;
        private void RaiseNameChangedEvent()
        {
            NameChanged?.Invoke(this, new EventArgs());
        }


        private bool isDisposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
                return;

            if (disposing)
            {

            }

            _process?.Kill();
            _process?.Dispose();

            isDisposed = true;
        }

        ~ConsoleChannelProxy()
        {
            Dispose(false);
        }


        public override string ToString()
        {
            return $"ConsoleProxy";
        }
    }
}
