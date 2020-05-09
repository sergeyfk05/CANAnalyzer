/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzer.Models.Databases;
using CANAnalyzer.Models.Delegates;
using CANAnalyzer.Models.Extensions;
using CANAnalyzerDevices.Devices.DeviceChannels;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace CANAnalyzer.Models.ViewData
{
    public class TracePeriodicViewData : INotifyPropertyChanged, IDisposable
    {
        public TracePeriodicViewData()
        {
            _timer.AutoReset = true;
            _timer.Elapsed += _timer_Elapsed;

            PropertyChanged += Model_PropertyChanged;

            Model = new TracePeriodicModel();
            Model.Payload = ObservableCollectionExtension.CreateEmpty((uint)Model.DLC);
        }

        public TracePeriodicViewData(TracePeriodicModel _m)
        {
            if (_m == null)
                throw new ArgumentNullException();

            _timer.AutoReset = true;
            _timer.Elapsed += _timer_Elapsed;

            PropertyChanged += Model_PropertyChanged;
            Model = _m;
        }




        public TracePeriodicModel Model
        {
            get { return _model; }
            private set
            {
                if (value == null)
                    throw new ArgumentNullException();

                if (_model == value)
                    return;

                _model = value;
                RaisePropertyChanged();
            }
        }
        private TracePeriodicModel _model;

        public TransmitToDelegate TransmitToSelectedChannels
        {
            get { return _transmitToSelectedChannels; }
            set
            {
                if (value == _transmitToSelectedChannels)
                    return;

                _transmitToSelectedChannels = value;
                RaisePropertyChanged();
            }
        }
        private TransmitToDelegate _transmitToSelectedChannels;

        public bool IsTrasmiting
        {
            get { return _isTrasmiting; }
            private set
            {
                if (value == _isTrasmiting)
                    return;

                _isTrasmiting = value;
                RaisePropertyChanged();
            }
        }
        private bool _isTrasmiting = false;

        public int Count
        {
            get { return _count; }
            set
            {
                if (value == _count)
                    return;

                _count = value;
                RaisePropertyChanged();
            }
        }
        private int _count;

        private void DLC_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "DLC")
                return;

            if (Model.DLC == 0)
                return;

            if (!(sender is TracePeriodicModel model && model == this.Model))
                return;

            if (Model.Payload.Count >= (int)Model.DLC)
            {
                while (Model.Payload.Count > (int)Model.DLC)
                {
                    Application.Current.Dispatcher.Invoke((Action)(() => { Model.Payload.RemoveAt(0); }));
                }
            }
            else
            {
                while (Model.Payload.Count < (int)Model.DLC)
                {
                    Application.Current.Dispatcher.Invoke((Action)(() => { Model.Payload.Insert(0, 0); }));
                }
            }
            RaisePropertyChanged("Model");
        }
        private void IsExtId_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsExtId")
                return;


            if (!(sender is TracePeriodicModel model && model == this.Model))
                return;


            if (Model.IsExtId)
            {
                if (Model.CanId > 0x1fffffff)
                    Model.CanId = 0x1fffffff;
            }
            else
            {
                if (Model.CanId > 0x7ff)
                    Model.CanId = 0x7ff;
            }
        }
        private void Period_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "Period") && (sender is TracePeriodicModel model) && (model == Model))
            {
                if (Model.Period < 10)
                    Model.Period = 10;

                _timer.Interval = Model.Period;
            }
        }
        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Model == null)
                StopTransmiting();

            Shot();

        }
        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Model")
                return;

            StopTransmiting();

            Model.PropertyChanged += DLC_PropertyChanged;
            Model.PropertyChanged += IsExtId_PropertyChanged;
            Model.PropertyChanged += Period_PropertyChanged;
        }


        public void Shot()
        {
            Task.Run(() =>
            {
                if (TransmitToSelectedChannels == null)
                    return;

                var dataToTrasmit = new TransmitData() { CanId = (int)Model.CanId, DLC = Model.DLC, IsExtId = Model.IsExtId, Payload = Model.Payload.ToByteArray() };
                if (!Validator.TryValidateObject(dataToTrasmit, new ValidationContext(dataToTrasmit), null, true))
                {
                    StopTransmiting();
                    return;
                }

                TransmitToSelectedChannels.Invoke(dataToTrasmit);

                Count++;
            });
        }
        public void StartTransmiting()
        {
            if (Model.Period < 10)
                Model.Period = 10;

            _timer.Interval = Model.Period;
            _timer.Start();

            IsTrasmiting = true;
        }
        public void StopTransmiting()
        {
            _timer?.Stop();
            IsTrasmiting = false;
        }
        private System.Timers.Timer _timer = new System.Timers.Timer();



        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
        ~TracePeriodicViewData()
        {
            this.Dispose();
        }
    }
}
