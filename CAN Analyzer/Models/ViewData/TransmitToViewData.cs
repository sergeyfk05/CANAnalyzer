/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzerDevices.Devices.DeviceChannels;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CANAnalyzer.Models.ViewData
{
    public class TransmitToViewData : INotifyPropertyChanged
    {
        public TransmitToViewData(bool isRealChannel = false)
        {
            IsRealChannel = isRealChannel;
        }

        public bool IsRealChannel
        {
            get { return _isRealChannel; }
            private set
            {
                if (value == _isRealChannel)
                    return;

                _isRealChannel = value;
                RaisePropertyChanged();
            }
        }
        private bool _isRealChannel = false;
        public bool IsTransmit
        {
            get { return _isTransmit; }
            set
            {
                if (value == _isTransmit)
                    return;

                _isTransmit = value;
                RaisePropertyChanged();
            }
        }
        private bool _isTransmit = false;

        public string DescriptionKey
        {
            get { return _descriptionKey; }
            set
            {
                if (value == _descriptionKey)
                    return;

                _descriptionKey = value;
                RaisePropertyChanged();
            }
        }
        private string _descriptionKey;

        public IChannel Channel
        {
            get { return _channel; }
            set
            {
                if (value == _channel)
                    return;

                _channel = value;
                RaisePropertyChanged();
            }
        }
        private IChannel _channel;

        public void Transmit(TransmitData data)
        {
            Channel?.Transmit(data);
        }

        FastSmartWeakEvent<PropertyChangedEventHandler> _propertyChanged = new FastSmartWeakEvent<PropertyChangedEventHandler>();
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _propertyChanged.Add(value); }
            remove { _propertyChanged.Remove(value); }
        }

        protected void RaisePropertyChanged([CallerMemberName]string prop = "")
        {
            _propertyChanged.Raise(this, new PropertyChangedEventArgs(prop));
        }
    }
}
