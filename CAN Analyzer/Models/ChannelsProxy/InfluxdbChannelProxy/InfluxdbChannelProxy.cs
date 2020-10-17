using CANAnalyzer.Models.ChannelsProxy;
using CANAnalyzer.Models.ChannelsProxy.InfluxDB.XML;
using CANAnalyzerDevices.Devices;
using CANAnalyzerDevices.Devices.DeviceChannels;
using CANAnalyzerDevices.Devices.DeviceChannels.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Windows.Media.Animation;
using System.Xml.Serialization;
using CANAnalyzer.Models.Extensions;
using System.Threading.Tasks;
using System.Data;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using System.Linq.Expressions;

namespace CANAnalyzer.Models.ChannelsProxy
{
    public class InfluxdbChannelProxy : IChannelProxy
    {

        public InfluxdbChannelProxy(string path)
        {
            Name = this.ToString();

            if (!System.IO.File.Exists(path))
                throw new ArgumentException("Invalid file path.");

            if (!TryParseConfigurationXmlDocument(path))
            {
                throw new ArgumentException("Invalid input file.");
            }

            Path = path;

            _watch.Start();

            InfluxDBClient client = InfluxDBClientFactory.Create($"http://{_config.Server.Hostname}:{_config.Server.Port}", _config.Server.Token.ToCharArray());
            _writeApi = client.GetWriteApi();
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
            if (!IsOpen)
                return;

            if (!DoesItPassFilter(data))
                return;

            List<string> udpStrings = ConvertCANPackageToInfluxdbUDPPackage(data);


            RaiseReceivedData(new ReceivedData()
            {
                Time = _watch.ElapsedMilliseconds / 1000.0,
                CanId = data.CanId,
                DLC = data.DLC,
                IsExtId = data.IsExtId,
                Payload = data.Payload
            });


            foreach (var el in udpStrings)
            {
                SendToInfluxDB(el);
            }

        }


        private bool DoesItPassFilter(TransmitData data)
        {
            InfuxDBFilter a = _config.Filters.FirstOrDefault(x =>
                (x.Header.IsExtId == data.IsExtId) &&
                (x.Header.DLC == data.DLC) &&
                (x.Header.Id == data.CanId) &&
                (data.Payload.BinaryAnd(x.Header.MaskBytes).IsEquals(x.Header.ValueBytes)));
            return a == null ? false : true;
        }



        private List<string> ConvertCANPackageToInfluxdbUDPPackage(TransmitData data)
        {
            List<string> result = new List<string>();

            foreach (var filter in _config.Filters.Where(x =>
                 (x.Header.IsExtId == data.IsExtId) &&
                 (x.Header.DLC == data.DLC) &&
                 (x.Header.Id == data.CanId) &&
                 (x.Header.ValueBytes.BinaryAnd(x.Header.MaskBytes).IsEquals(x.Header.ValueBytes))))
            {



                foreach (var insert in filter.Inserts)
                {
                    string exp = insert.Field.Value;
                    for (int i = 0; i < data.Payload.Length; i++)
                    {
                        exp = exp.Replace($"data{i}", data.Payload[i].ToString());
                    }



                    try
                    {
                        result.Add($"{insert.Measurement},{insert.Tag.Name}={insert.Tag.Value} {insert.Field.Name}={new DataTable().Compute(exp, null).ToString().Replace(",", ".")}");
                    }
                    catch { }

                }
            }


            return result;
        }

        private bool SendToInfluxDB(string data)
        {
            _writeApi.WriteRecord(_config.Server.Bucket, _config.Server.Organization, WritePrecision.Ns, data);
            return true;
        }

        private bool TryParseConfigurationXmlDocument(string path)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    _config = (InfuxDBConfig)formatter.Deserialize(fs);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }


        private Stopwatch _watch = new Stopwatch();
        private WriteApi _writeApi;
        private InfuxDBConfig _config;
        private static XmlSerializer formatter = new XmlSerializer(typeof(InfuxDBConfig));

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
