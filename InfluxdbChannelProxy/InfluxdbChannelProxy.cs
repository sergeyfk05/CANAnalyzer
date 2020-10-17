using CANAnalyzer.Models.ChannelsProxy;
using CANAnalyzerDevices.Devices;
using CANAnalyzerDevices.Devices.DeviceChannels;
using CANAnalyzerDevices.Devices.DeviceChannels.Events;
using InfluxdbChannelProxy.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace InfluxdbChannelProxy
{
    public class InfluxdbChannelProxy : IChannelProxy
    {

        public InfluxdbChannelProxy(string path)
        {
            if (!File.Exists(path))
                throw new ArgumentException("Invalid file path.");

            if (!TryParseConfigurationXmlDocument(path))
            {
                throw new ArgumentException("Invalid input file.");
            }

            Path = path;


            _udpClient = new UdpClient(_serverInfo.hostname, _serverInfo.port);
        }
       

        public string Path
        {
            get { return _path; }
            private set
            {
                if (_path == value)
                    return;

                _path = value;
            }
        }
        private string _path;

        public IChannel Channel
        {
            get { return _channel; }
            private set
            {
                if (_channel == value)
                    return;

                _channel = value;
            }
        }
        private IChannel _channel;

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value)
                    return;

                _name = value;
                RaiseNameChangedEvent();
            }
        }
        private string _name;

        public int Bitrate => 0;

        public bool IsListenOnly => false;

        public bool IsOpen
        {
            get { return _isOpen; }
            private set
            {
                if (_isOpen == value)
                    return;

                _isOpen = value;
                RaiseIsOpenChanged();
            }
        }
        private bool _isOpen;

        public IDevice Owner => null;



        public void Close()
        {
            IsOpen = false;
        }

        public void Open(int bitrate = 500, bool isListenOnly = true)
        {
            IsOpen = true;
        }

        public void SetChannel(IChannel newCH)
        {
        }

        public void Transmit(TransmitData data)
        {
            bool isVerified = false;
            string udpString = ConvertCANPackageToInfluxdbUDPPackage(data, out isVerified);

            if (!isVerified)
                return;

            RaiseReceivedData(new ReceivedData()
            {
                Time = _watch.ElapsedMilliseconds / 1000.0,
                CanId = data.CanId,
                DLC = data.DLC,
                IsExtId = data.IsExtId,
                Payload = data.Payload
            });


            SendUDPtoInfluxDB(udpString);

        }

        private string ConvertCANPackageToInfluxdbUDPPackage(TransmitData data, out bool isVerified)
        {
            throw new NotImplementedException();
            isVerified = false;
            return "";
        }

        private bool SendUDPtoInfluxDB(string data)
        {
            byte[] sendBytes = Encoding.ASCII.GetBytes(data);
            try
            {
                _udpClient.Send(sendBytes, sendBytes.Length);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private bool TryParseConfigurationXmlDocument(string path)
        {
            throw new NotImplementedException();
        }


        private Stopwatch _watch;
        private UdpClient _udpClient;
        private ServerInfo _serverInfo;

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
        public event EventHandler NameChanged;
        private void RaiseNameChangedEvent()
        {
            NameChanged?.Invoke(this, new EventArgs());
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~InfluxdbChannelProxy()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private bool disposedValue;

        public override string ToString()
        {
            return $"InfluxDB proxy";
        }
    }
}
