/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzerDevices.Devices.DeviceChannels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CANAnalyzer.Models.ViewData;
using CANAnalyzer.Models;
using System.Threading;
using System.Collections.ObjectModel;

namespace CANAnalyzer.VM
{
    public class MonitorChannelPageVM : BaseVM
    {
        public MonitorChannelPageVM(IChannel ch)
        {
            Channel = ch;
            Channel.ReceivedData += Channel_ReceivedData;

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
        private RecieveState _status = RecieveState.Recieving;

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
    }
}
