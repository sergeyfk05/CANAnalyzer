/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CANAnalyzer.Models.ViewData
{
    public interface ReadOnlyMonitorByteViewData: INotifyPropertyChanged
    {
        bool IsChanged { get; }
        byte Data { get; }
    }
    public class MonitorByteViewData : INotifyPropertyChanged, ReadOnlyMonitorByteViewData
    {

        public bool IsChanged
        {
            get { return _isChanged; }
            private set
            {
                if (value == IsChanged)
                    return;

                _isChanged = value;
                RaisePropertyChanged();
            }
        }
        private bool _isChanged = false;

        public byte Data
        {
            get { return _data; }
            set
            {
                if (_data == value)
                {
                    IsChanged = false;
                    return;
                }

                _data = value;
                RaisePropertyChanged();
                IsChanged = true;
            }
        }
        private byte _data = 0;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
