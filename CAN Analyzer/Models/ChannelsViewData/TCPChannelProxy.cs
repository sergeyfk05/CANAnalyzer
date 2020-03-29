using CANAnalyzerDevices.Devices;
using CANAnalyzerDevices.Devices.DeviceChannels;
using CANAnalyzerDevices.Devices.DeviceChannels.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace CANAnalyzer.Models.ChannelsViewData
{
    public class TCPChannelProxy : IChannel, IDisposable
    {
        public TCPChannelProxy(IChannel ch, string path)
        {
            //_ch = ch ?? throw new ArgumentException("IChannel cannot be null.");

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


                if (!CheckConnectionAsync().Result)
                    throw new ArgumentException("Invalide file\r\n");
            }
            catch (Exception e)
            {
                throw new ArgumentException("Invalide file\r\n" + e.ToString());
            }


            //_ch.ReceivedData += RealChannel_ReceivedData;
            RealChannel_ReceivedData(this, new ChannelDataReceivedEventArgs(new ReceivedData() { IsExtId = false, CanId = 0x60D, DLC = 4, Time = 50.2, Payload = new byte[] { 0x10, 0x20, 0x30, 0x40 } }));
        }

        private async void RealChannel_ReceivedData(object sender, ChannelDataReceivedEventArgs e)
        {
            await SendDataToProxyAsync(e.Data);

            string response = _process.StandardOutput.ReadLine();
            var match = Regex.Match(response,
                @"([0-1]{1});" +
                @"([0-9a-fA-F]{8});" +
                @"([0-9a-fA-F]{1})+;" +
                @"([0-9a-fA-F]*);" +
                @"([0-9a-fA-F]{4});");

            if (!match.Success)
                return;

            ReceivedData result = new ReceivedData();
            result.DLC = Convert.ToInt32(match.Groups[3].Value, 16);

            match = Regex.Match(response,
                @"([0-1]{1})+;" +
                @"([0-9a-fA-F]{8})+;" +
                @"([0-9a-fA-F]{1})+;" +
                @"([0-9a-fA-F]{" + (result.DLC * 2) + "})+;" +
                @"([0-9a-fA-F]{4})+;");


            if (!match.Success)
                return;

            throw new NotImplementedException("распарсить регулярку и проверить данные");

            result.Payload = new byte[result.DLC];

        }

        private async Task SendDataToProxyAsync(ReceivedData data)
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
        private IChannel _ch;

        private async Task<bool> CheckConnectionAsync()
        {
            string response = "Hello world!";
            _process.StandardInput.WriteLine(response);

            string message = _process.StandardOutput.ReadLine();

            return message.TrimEnd('\0') == response;
        }

        public int Bitrate => _ch.Bitrate;

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

        public IDevice Owner => _ch.Owner;

        public event ChannelDataReceivedEventHandler ReceivedData;
        private void RaiseReceivedData(ReceivedData data)
        {
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

        public void Dispose()
        {
            _process?.Close();
            _process?.Dispose();
        }
    }
}
