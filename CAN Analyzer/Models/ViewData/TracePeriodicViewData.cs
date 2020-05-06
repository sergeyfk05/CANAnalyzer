/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzer.Models.Databases;
using CANAnalyzer.Models.Extensions;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace CANAnalyzer.Models.ViewData
{
    public class TracePeriodicViewData : INotifyPropertyChanged
    {
        public TracePeriodicViewData()
        {
            PropertyChanged += Model_PropertyChanged;

            Model = new TracePeriodicModel();
            Model.Payload = ObservableCollectionExtension.CreateEmpty((uint)Model.DLC);
        }
        public TracePeriodicViewData(TracePeriodicModel _m)
        {
            if (_m == null)
                throw new ArgumentNullException();

            PropertyChanged += Model_PropertyChanged;
            Model = _m;
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Model")
                return;

            Model.PropertyChanged += DLC_PropertyChanged;
            Model.PropertyChanged += IsExtId_PropertyChanged;
        }

        public TracePeriodicModel Model
        {
            get { return _model; }
            private set
            {
                if (_model == value)
                    return;

                _model = value;
                RaisePropertyChanged();
            }
        }
        private TracePeriodicModel _model;

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


        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

    }
}
