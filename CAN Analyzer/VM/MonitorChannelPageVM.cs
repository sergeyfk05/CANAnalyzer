/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzer.Models;
using CANAnalyzer.Models.States;
using CANAnalyzer.Models.ViewData;
using CANAnalyzerDevices.Devices.DeviceChannels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace CANAnalyzer.VM
{
    public class MonitorChannelPageVM : BaseVM
    {
        public MonitorChannelPageVM(IChannel ch)
        {
            Channel = ch;
            Channel.ReceivedData += Channel_ReceivedData;
            PropertyChanged += MonitorChannelPageVM_PropertyChanged;
            Status = RecieveState.Blocked;
        }
        ~MonitorChannelPageVM()
        {
            if(Channel != null)
                Channel.ReceivedData -= Channel_ReceivedData;
            PropertyChanged -= MonitorChannelPageVM_PropertyChanged;
        }

        private void MonitorChannelPageVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MonitorStartCommand.RaiseCanExecuteChanged();
            PauseRecievingCommand.RaiseCanExecuteChanged();
        }

        private void Channel_ReceivedData(object sender, CANAnalyzerDevices.Devices.DeviceChannels.Events.ChannelDataReceivedEventArgs e)
        {
            if((Status == RecieveState.Recieving) && (sender is IChannel ch) && (ch == Channel))
            {
                MonitorChannelPageData data = Items.FirstOrDefault(x => (x.CanId == e.Data.CanId) && (x.IsExtId == e.Data.IsExtId) && (x.DLC == e.Data.DLC));
                if (data == null)
                {
                    data = new MonitorChannelPageData(e.Data.CanId, e.Data.IsExtId, e.Data.DLC);

                    _context.Post((s) =>
                    {
                        Items.Add(data);
                    }, null);
                }
                data.SetPayload(e.Data.Payload, e.Data.Time);
            }
        }

        private SynchronizationContext _context = SynchronizationContext.Current;

        public RecieveState Status
        {
            get { return _status; }
            private set
            {
                if (value == _status)
                    return;

                _status = value;
                RaisePropertyChanged();
            }
        }
        private RecieveState _status;

        public IChannel Channel
        {
            get { return _channel; }
            protected set
            {
                if (_channel == value)
                    return;

                _channel = value;
                RaisePropertyChanged();
            }
        }
        private IChannel _channel;

        public ObservableCollection<MonitorChannelPageData> Items
        {
            get { return _items ?? (_items = new ObservableCollection<MonitorChannelPageData>()); }
            private set
            {
                if (value == _items)
                    return;

                _items = value;
                RaisePropertyChanged();
            }
        }
        private ObservableCollection<MonitorChannelPageData> _items;


        private RelayCommand _monitorStartCommand;
        public RelayCommand MonitorStartCommand
        {
            get
            {
                if (_monitorStartCommand == null)
                    _monitorStartCommand = new RelayCommand(this.MonitorStartCommand_Execute, () => { return this.Status == RecieveState.Blocked; });

                return _monitorStartCommand;
            }
        }
        private void MonitorStartCommand_Execute()
        {
            Status = RecieveState.Recieving;
        }

        private RelayCommand _pauseRecievingCommand;
        public RelayCommand PauseRecievingCommand
        {
            get
            {
                if (_pauseRecievingCommand == null)
                    _pauseRecievingCommand = new RelayCommand(this.PauseRecievingCommand_Execute, () => { return this.Status == RecieveState.Recieving; });

                return _pauseRecievingCommand;
            }
        }
        private void PauseRecievingCommand_Execute()
        {
            Status = RecieveState.Blocked;
        }

        private RelayCommand _clearMonitorCommand;
        public RelayCommand ClearMonitorCommand
        {
            get
            {
                if (_clearMonitorCommand == null)
                    _clearMonitorCommand = new RelayCommand(this.ClearMonitorCommand_Execute);

                return _clearMonitorCommand;
            }
        }
        private void ClearMonitorCommand_Execute()
        {
            Items.Clear();
        }
    }
}
