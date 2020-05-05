using CANAnalyzer.Models.Databases;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CANAnalyzer.Models.ViewData
{
    public class TracePeriodicViewData : INotifyPropertyChanged
    {
        public TracePeriodicViewData(TracePeriodicModel _m)
        {
            if (_m == null)
                throw new ArgumentNullException();

            Model = _m;

            Model.PropertyChanged += DLC_PropertyChanged;
            Model.PropertyChanged += IsExtId_PropertyChanged;

            PayloadCollection = new ObservableCollection<byte>();
            for (int i = 0; i < _m.DLC; i++)
            { PayloadCollection.Add(0); }
        }

        private void PayloadCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (sender is ObservableCollection<byte> collection && collection == PayloadCollection)
            {
                if (Model.DLC != collection.Count)
                    return;

                byte[] buf = new byte[collection.Count];
                for (int i = 0; i < collection.Count; i++)
                {
                    buf[i] = collection[i];
                }

                Model.Payload = buf;
            }
        }

        public ObservableCollection<byte> PayloadCollection
        {
            get { return _payloadCollection; }
            set
            {
                if (value == _payloadCollection)
                    return;

                _payloadCollection = value;
                _payloadCollection.CollectionChanged += PayloadCollection_CollectionChanged;
                RaisePropertyChanged();
            }
        }
        private ObservableCollection<byte> _payloadCollection;

        public TracePeriodicModel Model
        {
            get { return _model; }
            private set
            {
                if (_model == value)
                    return;

                _model = value;

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

            if (Model.Payload.Length >= (int)Model.DLC)
            {
                byte[] newPayload = new byte[Model.DLC];
                Array.Copy(Model.Payload, Model.Payload.Length - (int)Model.DLC, newPayload, 0, (int)Model.DLC);
                Model.Payload = newPayload;
            }
            else
            {
                byte[] newPayload = new byte[Model.DLC];
                Array.Copy(Model.Payload, 0, newPayload, (int)Model.DLC - Model.Payload.Length, Model.Payload.Length);
                Model.Payload = newPayload;
            }
        }
        private void IsExtId_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsExtId")
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
