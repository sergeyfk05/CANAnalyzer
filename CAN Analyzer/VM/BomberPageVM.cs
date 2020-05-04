using CANAnalyzer.Models;
using CANAnalyzer.Models.Delegates;
using CANAnalyzer.Models.States;
using CANAnalyzer.Models.ViewData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Timers;
using CANAnalyzerDevices.Devices.DeviceChannels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CANAnalyzer.VM
{
    public class BomberPageVM : BaseClosableVM
    {

        public BomberPageVM()
        {
            PropertyChanged += TransmitToSelectedChannels_PropertyChanged;
            PropertyChanged += DLC_PropertyChanged;
            PropertyChanged += Status_PropertyChanged;

            Settings.Instance.PropertyChanged += Device_PropertyChanged;
            Device_PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Device"));

            _timer.Stop();
            _timer.AutoReset = true;
            _timer.Elapsed += _timer_Elapsed;
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

        public uint MsgPerStep
        {
            get { return _msgPerStep; }
            set
            {
                if (_msgPerStep == value)
                    return;

                _msgPerStep = value;
                RaisePropertyChanged();
            }
        }
        private uint _msgPerStep = 5;

        public uint Period
        {
            get { return _period; }
            set
            {
                if (_period == value)
                    return;

                _period = value;
                RaisePropertyChanged();
            }
        }
        private uint _period = 200;

        public TransmitState Status
        {
            get { return _status; }
            set
            {
                if (_status == value)
                    return;

                _status = value;
                RaisePropertyChanged();
            }
        }
        private TransmitState _status;



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

        private RelayCommandAsync _stepForwardCommand;
        public RelayCommandAsync StepForwardCommand
        {
            get
            {
                if (_stepForwardCommand == null)
                    _stepForwardCommand = new RelayCommandAsync(this.StepForwardCommand_Execute, () => { return Status != TransmitState.Transmiting; });

                return _stepForwardCommand;
            }
        }
        private void StepForwardCommand_Execute()
        {
            byte[] newPayload = new byte[DLC];
            for (int i = 0; i < Payload.Length; i++)
            {
                newPayload[i] = (byte)((byte)Payload[i] + (byte)Increment[i]);
            }
            Payload = newPayload;
        }

        private RelayCommandAsync _stepBackCommand;
        public RelayCommandAsync StepBackCommand
        {
            get
            {
                if (_stepBackCommand == null)
                    _stepBackCommand = new RelayCommandAsync(this.StepBackCommand_Execute, () => { return Status != TransmitState.Transmiting; });

                return _stepBackCommand;
            }
        }
        private void StepBackCommand_Execute()
        {
            byte[] newPayload = new byte[DLC];
            for (int i = 0; i < Payload.Length; i++)
            {
                newPayload[i] = (byte)((byte)Payload[i] - (byte)Increment[i]);
            }
            Payload = newPayload;
        }

        private RelayCommandAsync _shotCommand;
        public RelayCommandAsync ShotCommand
        {
            get
            {
                if (_shotCommand == null)
                    _shotCommand = new RelayCommandAsync(this.ShotCommand_Execute, () => { return Status != TransmitState.Transmiting; });

                return _shotCommand;
            }
        }
        private void ShotCommand_Execute()
        {
            TransmitData data = new TransmitData()
            {
                CanId = (int)this.CanId,
                IsExtId = this.IsExtId,
                DLC = (int)this.DLC,
                Payload = this.Payload.ToArray()
            };

            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(data, new ValidationContext(data), results, true))
            {
                foreach (var error in results)
                {
                    MessageBox.Show(error.ErrorMessage, "warning", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if (Status == TransmitState.Transmiting)
                    RunCommand.Execute();

                return;
            }


            TransmitToSelectedChannels?.BeginInvoke(data, null, null);
        }

        private RelayCommandAsync _runCommand;
        public RelayCommandAsync RunCommand
        {
            get
            {
                if (_runCommand == null)
                    _runCommand = new RelayCommandAsync(this.RunCommand_Execute);

                return _runCommand;
            }
        }
        private void RunCommand_Execute()
        {
            if (Status == TransmitState.Transmiting)
            {
                Status = TransmitState.Paused;
                _timer.Stop();
            }
            else
            {
                Status = TransmitState.Transmiting;
                _msgPerStepComleted = 0;
                _timer.Interval = Period;
                _timer.Start();
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

            if (Payload.Length >= (int)DLC)
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
        private void Status_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Status")
                return;

            StepForwardCommand.RaiseCanExecuteChanged();
            StepBackCommand.RaiseCanExecuteChanged();
            ShotCommand.RaiseCanExecuteChanged();
            RunCommand.RaiseCanExecuteChanged();
        }
        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_msgPerStepComleted >= MsgPerStep)
            {
                _msgPerStepComleted = 0;
                StepForwardCommand_Execute();
            }

            TransmitData data = new TransmitData()
            {
                CanId = (int)this.CanId,
                IsExtId = this.IsExtId,
                DLC = (int)this.DLC,
                Payload = this.Payload.ToArray()
            };
            _msgPerStepComleted++;

            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(data, new ValidationContext(data), results, true))
            {
                foreach (var error in results)
                {
                    MessageBox.Show(error.ErrorMessage, "warning", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if (Status == TransmitState.Transmiting)
                    RunCommand.Execute();

                return;
            }


            TransmitToSelectedChannels?.BeginInvoke(data, null, null);
        }


        private uint _msgPerStepComleted = 0;
        private Timer _timer = new Timer();
        private System.Threading.SynchronizationContext _context = System.Threading.SynchronizationContext.Current;
    }
}
