/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzer.Models;
using CANAnalyzer.Models.Delegates;
using CANAnalyzer.Models.States;
using CANAnalyzer.Models.ViewData;
using CANAnalyzer.Resources.DynamicResources;
using CANAnalyzerDevices.Devices.DeviceChannels;
using DynamicResource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace CANAnalyzer.VM
{
    public class BomberPageVM : BaseClosableVM, IDisposable
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


        public ObservableCollection<byte> Payload
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
        private ObservableCollection<byte> _payload = CreateEmptyBindingList(8);

        public ObservableCollection<byte> Increment
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
        private ObservableCollection<byte> _increment = CreateEmptyBindingList(8);

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

            if (MessageBox.Show((string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("#ClosePageMsgBoxContent"), (string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("#QuestionMsgBoxTitle"), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
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
            for (int i = 0; i < Payload.Count; i++)
            {
                Application.Current.Dispatcher.Invoke((Action)(() => { Payload[i] += Increment[i]; }));
            }
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
            for (int i = 0; i < Payload.Count; i++)
            {
                Application.Current.Dispatcher.Invoke((Action)(() => { Payload[i] -= Increment[i]; }));
            }
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
                    MessageBox.Show(error.ErrorMessage, (string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("#WarningMsgBoxTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
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

                if (Period < 10)
                    Period = 10;

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

            if (Payload.Count >= (int)DLC)
            {
                while (Payload.Count > (int)DLC)
                {
                    Application.Current.Dispatcher.Invoke((Action)(() => { Payload.RemoveAt(0); }));
                }
            }
            else
            {
                while (Payload.Count < (int)DLC)
                {
                    Application.Current.Dispatcher.Invoke((Action)(() => { Payload.Insert(0, 0); }));
                }
            }

            if (Increment.Count >= (int)DLC)
            {
                while (Increment.Count > (int)DLC)
                {
                    Application.Current.Dispatcher.Invoke((Action)(() => { Increment.RemoveAt(0); }));
                }
            }
            else
            {
                while (Increment.Count < (int)DLC)
                {
                    Application.Current.Dispatcher.Invoke((Action)(() => { Increment.Insert(0, 0); }));
                }
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
                    MessageBox.Show(error.ErrorMessage, (string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("#WarningMsgBoxTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if (Status == TransmitState.Transmiting)
                    RunCommand.Execute();

                return;
            }

            Task.Run(() => { TransmitToSelectedChannels?.Invoke(data); });
        }


        private uint _msgPerStepComleted = 0;
        private Timer _timer = new Timer();
        private System.Threading.SynchronizationContext _context = System.Threading.SynchronizationContext.Current;


        private static ObservableCollection<byte> CreateEmptyBindingList(int c)
        {
            ObservableCollection<byte> result = new ObservableCollection<byte>();
            for (uint i = 0; i < c; i++)
            {
                result.Add(0);
            }

            return result;
        }

        public void Dispose()
        {
            TransmitToSelectedChannels = null;

            if(Settings.Instance.Device != null)
                Settings.Instance.Device.IsConnectedChanged -= Device_IsConnectedChanged;

            if(_timer != null)
            {
                _timer.Elapsed -= _timer_Elapsed;
                _timer.Dispose();
            }
        }
        ~BomberPageVM()
        {
            this.Dispose();
        }
    }
}
