using CANAnalyzer.Models;
using CANAnalyzer.Models.Delegates;
using CANAnalyzer.Models.ViewData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CANAnalyzer.VM
{
    public class BomberPageVM : BaseClosableVM
    {

        public BomberPageVM()
        {
            PropertyChanged += TransmitToSelectedChannels_PropertyChanged;
            PropertyChanged += DLC_PropertyChanged;
            Settings.Instance.PropertyChanged += Device_PropertyChanged;
            Device_PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Device"));
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
            private set
            {
                if (value == _transmitToSelectedChannels)
                    return;

                _transmitToSelectedChannels = value;
                RaisePropertyChanged();
            }
        }
        private TransmitToDelegate _transmitToSelectedChannels;

        

        public bool IsExtId
        {
            get { return _isExtId; }
            set
            {
                if (value == _isExtId)
                    return;

                _isExtId = value;
                RaisePropertyChanged();
            }
        }
        private bool _isExtId;

        public UInt64 DLC
        {
            get { return _dlc; }
            set
            {
                if (value == _dlc)
                    return;

                _dlc = value;
                RaisePropertyChanged();
            }
        }
        private UInt64 _dlc = 8;


        public UInt64 CanId
        {
            get { return _canId; }
            set
            {
                if (_canId == value)
                    return;

                _canId = value;
                RaisePropertyChanged();
            }
        }
        private UInt64 _canId = UInt64.MaxValue;


        public byte[] Payload
        {
            get { return _payload; }
            set
            {
                if (_payload == value)
                    return;

                _payload = value;
                RaisePropertyChanged();
            }
        }
        private byte[] _payload = new byte[8];

        public byte[] Increment
        {
            get { return _increment; }
            set
            {
                if (_increment == value)
                    return;

                _increment = value;
                RaisePropertyChanged();
            }
        }
        private byte[] _increment = new byte[8];

        private RelayCommand _closePageCommand;
        public RelayCommand ClosePageCommand
        {
            get
            {
                if (_closePageCommand == null)
                    _closePageCommand = new RelayCommand(this.ClosePageCommand_Execute);

                return _closePageCommand;
            }
        }
        private void ClosePageCommand_Execute()
        {

            if (MessageBox.Show("are you want close the page?", "???", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                RaiseClosedEvent();
            }

        }


        private void TransmitToSelectedChannels_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //_transmiter.TransmitTo = TransmitToSelectedChannels;
        }
        private void Device_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Device")
                return;

            //create ViewData for  transmitable channels
            _context.Send((s) =>
            {
                TransmitToItems.Clear();
            }, null);
            TransmitToSelectedChannels = null;
            if (Settings.Instance.Device != null && Settings.Instance.Device.IsConnected && Settings.Instance.Device.Channels != null)
            {
                Settings.Instance.Device.IsConnectedChanged += Device_IsConnectedChanged;
                foreach (var el in Settings.Instance.Device.Channels)
                {
                    var viewData = new TransmitToViewData() { IsTransmit = false, DescriptionKey = $"#{el.ToString()}NavMenu", Channel = el };
                    viewData.PropertyChanged += TransmitToViewDataIsTransmit_PropertyChanged;
                    _context.Send((s) =>
                    {
                        TransmitToItems.Add(viewData);
                    }, null);
                }
            }
        }
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
        private void Device_IsConnectedChanged(object sender, EventArgs e)
        {
            if (Settings.Instance.Device == sender)
            {
                Device_PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Device"));
            }
        }
        private void DLC_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "DLC")
                return;

            if (DLC == 0)
                return;
            
            if(Payload.Length >= (int)DLC)
            {
                byte[] newPayload = new byte[DLC];
                Array.Copy(Payload, Payload.Length - (int)DLC, newPayload, 0, (int)DLC);
                Payload = newPayload;
            }
            else
            {
                byte[] newPayload = new byte[DLC];
                Array.Copy(Payload, 0, newPayload, (int)DLC - Payload.Length, Payload.Length);
                Payload = newPayload;
            }

            if (Increment.Length >= (int)DLC)
            {
                byte[] newIncrement = new byte[DLC];
                Array.Copy(Increment, Increment.Length - (int)DLC, newIncrement, 0, (int)DLC);
                Increment = newIncrement;
            }
            else
            {
                byte[] newIncrement = new byte[DLC];
                Array.Copy(Increment, 0, newIncrement, (int)DLC - Increment.Length, Increment.Length);
                Increment = newIncrement;
            }
        }


        private System.Threading.SynchronizationContext _context = System.Threading.SynchronizationContext.Current;
    }
}
