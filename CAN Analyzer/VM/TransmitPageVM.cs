/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzer.Models;
using CANAnalyzer.Models.Databases;
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
    public class TransmitPageVM : BaseClosableVM
    {

        public TransmitPageVM()
        {
            PropertyChanged += TransmitToSelectedChannels_PropertyChanged;
            Settings.Instance.PropertyChanged += Device_PropertyChanged;

            Device_PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Device"));

            Data.Add(new TracePeriodicViewData(new TracePeriodicModel()
            {
                CanId = 0x123,
                DLC = 8,
                IsExtId = false,
                Period = 100,
                Payload = CreateEmpty(8)
            }));
        }
        private static ObservableCollection<byte> CreateEmpty(uint c)
        {
            ObservableCollection<byte> result = new ObservableCollection<byte>();
            for (uint i = 0; i < c; i++)
            {
                result.Add(0);
            }

            return result;
        }

        public ObservableCollection<TracePeriodicViewData> Data
        {
            get { return _data; }
            private set
            {
                if (_data == value)
                    return;

                _data = value;
                RaisePropertyChanged();
            }
        }
        private ObservableCollection<TracePeriodicViewData> _data = new ObservableCollection<TracePeriodicViewData>();

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





        private System.Threading.SynchronizationContext _context = System.Threading.SynchronizationContext.Current;





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
        private void Device_IsConnectedChanged(object sender, EventArgs e)
        {
            if (Settings.Instance.Device == sender)
            {
                Device_PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Device"));
            }
        }
    }
}
