using CANAnalyzer.Models;
using CANAnalyzer.Models.Delegates;
using CANAnalyzer.Models.Extensions;
using CANAnalyzer.Models.ViewData;
using CANAnalyzerDevices.Devices.DeviceChannels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace CANAnalyzer.VM
{
    public abstract class TransmitableAndClosableBaseVM : BaseClosableVM, IDisposable
    {
        public TransmitableAndClosableBaseVM()
        { 

            Settings.Instance.PropertyChanged += Device_PropertyChanged;

            Device_PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Device"));

            Settings.Instance.ProxiesCollectionChanged += Settins_ProxiesCollectionChanged;
            AddTransmitToItems(Settings.Instance.Proxies, false);
        }

        private void Settins_ProxiesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach(var el in e.NewItems)
                {
                    if (el is IChannel ch)
                        AddTransmitToItems(ch, false);
                }
            }
            else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach(var el in e.OldItems)
                {
                    if (el == null)
                        continue;

                     TransmitToItems.RemoveAll(x => x.Channel == (IChannel)el);
                    
                }
            }
            else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                TransmitToItems.Clear();
            }
            else { throw new NotImplementedException(); }
        }

        public ObservableCollection<TransmitToViewData> TransmitToItems
        {
            get { return _transmitToItems ?? (_transmitToItems = new ObservableCollection<TransmitToViewData>()); }
            set
            {
                if (_transmitToItems == value)
                    return;

                _transmitToItems = value;
                RaisePropertyChanged();
            }
        }
        private ObservableCollection<TransmitToViewData> _transmitToItems;
        public TransmitToDelegate TransmitToSelectedChannels
        {
            get { return _transmitToSelectedChannels; }
            protected set
            {
                if (value == _transmitToSelectedChannels)
                    return;

                _transmitToSelectedChannels = value;
                RaisePropertyChanged();
            }
        }
        private TransmitToDelegate _transmitToSelectedChannels;
        private void TransmitToViewDataIsTransmit_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "IsTransmit") && (sender is TransmitToViewData viewData) && (TransmitToItems.Contains(viewData)))
            {
                if (viewData.IsTransmit)
                {
                    if (TransmitToSelectedChannels == null)
                        TransmitToSelectedChannels = viewData.Transmit;
                    else
                        TransmitToSelectedChannels += viewData.Transmit;
                }
                else
                {
                    try
                    {
                        TransmitToSelectedChannels -= viewData.Transmit;
                    }
                    catch { }
                }
            }
        }

        private void Device_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Device")
                return;

            //create ViewData for  transmitable channels
            _context.Send((s) =>
            {
                //remove real channels
                TransmitToItems.RemoveAll(x => x.IsRealChannel);
            }, null);
            TransmitToSelectedChannels = null;
            if (Settings.Instance.Device != null && Settings.Instance.Device.IsConnected && Settings.Instance.Device.Channels != null)
            {
                Settings.Instance.Device.IsConnectedChanged += Device_IsConnectedChanged;
                AddTransmitToItems(Settings.Instance.Device.Channels, true);
            }
        }
        private void AddTransmitToItems(IEnumerable<IChannel> data, bool isRealChannels)
        {
            foreach (var el in data)
            {
                AddTransmitToItems(el, isRealChannels);
            }
        }
        private void AddTransmitToItems(IChannel el, bool isRealChannels)
        {
            var viewData = new TransmitToViewData(isRealChannels) { IsTransmit = false, DescriptionKey = $"#{el.ToString()}NavMenu", Channel = el };
            viewData.PropertyChanged += TransmitToViewDataIsTransmit_PropertyChanged;
            _context.Send((s) =>
            {
                TransmitToItems.Add(viewData);
            }, null);
        }
        private void Device_IsConnectedChanged(object sender, EventArgs e)
        {
            if (Settings.Instance.Device == sender)
            {
                Device_PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Device"));
            }
        }

        public virtual void Dispose()
        {
            TransmitToItems = null;
        }

        public override bool Equals(object obj)
        {
            return obj is TransmitableAndClosableBaseVM vM &&
                   base.Equals(obj) &&
                   EqualityComparer<ObservableCollection<TransmitToViewData>>.Default.Equals(TransmitToItems, vM.TransmitToItems) &&
                   EqualityComparer<TransmitToDelegate>.Default.Equals(TransmitToSelectedChannels, vM.TransmitToSelectedChannels);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TransmitToItems, TransmitToSelectedChannels);
        }

        private System.Threading.SynchronizationContext _context = System.Threading.SynchronizationContext.Current;
    }
}
