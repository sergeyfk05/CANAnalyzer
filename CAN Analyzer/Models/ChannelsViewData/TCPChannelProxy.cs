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
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = path,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = false
                    }
                };
                _process.Start();

                char[] buf = new char[256];
                _process.StandardOutput.Read(buf, 0, buf.Length);

                TcpClient _tcpClient = new TcpClient();
                _tcpClient.Connect("127.0.0.1", Convert.ToInt32(new string(buf)));
                _netStream = _tcpClient.GetStream();

                if (!CheckConnection())
                    throw new ArgumentException("Invalide file\r\n");
            }
            catch (Exception e)
            {
                throw new ArgumentException("Invalide file\r\n" + e.ToString());
            }


            //_ch.ReceivedData += RealChannel_ReceivedData;
            RealChannel_ReceivedData(this, new ChannelDataReceivedEventArgs(new ReceivedData() { IsExtId = false, CanId = 0x60D, DLC = 4, Time = 50.2, Payload = new byte[] { 0x10, 0x20, 0x30, 0x40 } }));
        }

        private void RealChannel_ReceivedData(object sender, ChannelDataReceivedEventArgs e)
        {
            SendDataToProxy(e.Data);

            byte[] buf = new byte[2048];
            int count = _netStream.Read(buf, 0, buf.Length);
            var match = Regex.Match(Encoding.Default.GetString(buf, 0, count).Trim('\0'),
                "([0-1])\r\n" +
                "([0-9a-fA-F])\r\n" +
                "([0-9a-fA-F])\r\n" +
                "([0-9a-fA-F])\r\n" +
                "([0-9a-fA-F])\r\n");

            //		string.Trim returned	"0\r\n0000060D\r\n4\r\n10203040\r\nC418\r\n"	string


            if (!match.Success)
                return;

            ReceivedData result = new ReceivedData();
            result.CanId = Convert.ToInt32(match.Groups[1].Value, 16);
            result.DLC = Convert.ToInt32(match.Groups[2].Value, 16);
            result.Time = Convert.ToInt32(match.Groups[4].Value, 16) / 1000.0;
            result.Payload = new byte[result.DLC];

        }

        private void SendDataToProxy(ReceivedData data)
        {

            string payload = "";
            for(int i=0;i<data.DLC;i++)
            {
                payload += data.Payload[i].ToString("X2");
            }

            string response = $"{(data.IsExtId ? "1" : "0")}\r\n{data.CanId.ToString("X8")}\r\n{data.DLC.ToString("X1")}\r\n{payload}\r\n{((int)(data.Time * 1000)).ToString("X4")}\r\n";
            byte[] toSend = System.Text.Encoding.UTF8.GetBytes(response);
            _netStream.Write(toSend, 0, toSend.Length);
        }

        private NetworkStream _netStream;
        private TcpClient _tcpClient;
        private Process _process;
        private IChannel _ch;

        private bool CheckConnection()
        {
            string response = "Hello world!";
            byte[] data = System.Text.Encoding.UTF8.GetBytes(response);
            _netStream.Write(data, 0, data.Length);

            data = new byte[256];
            int bytes = _netStream.Read(data, 0, data.Length); // получаем количество считанных байтов
            string message = Encoding.UTF8.GetString(data, 0, bytes);

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
            _netStream?.Dispose();
            _tcpClient?.Dispose();
            _process?.Close();
            _process?.Dispose();
        }
    }
}
