using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CANAnalyzer.Models.ViewData
{
    public class MonitorChannelPageData : INotifyPropertyChanged
    {
        public MonitorChannelPageData(int canId, bool isExtId, int dlc)
        {
            this.CanId = canId;
            this.IsExtId = isExtId;
            this.DLC = dlc;

            data = new MonitorByteViewData[dlc];
            for (int i = 0; i < dlc;  i++)
                data[i] = new MonitorByteViewData();

            RaisePropertyChanged("Data");

        }

        public bool IsExtId
        {
            get
            {
                return _isExtId;
            }
            private set
            {
                if (value == _isExtId)
                    return;

                _isExtId = value;
                RaisePropertyChanged();
            }
        }
        private bool _isExtId;

        public int CanId
        {
            get
            {
                return _canId;
            }
            private set
            {
                if (value == _canId)
                    return;

                _canId = value;
                RaisePropertyChanged();
            }
        }
        private int _canId;

        public int DLC
        {
            get
            {
                return _dlc;
            }
            private set
            {
                if (value == _dlc)
                    return;

                _dlc = value;
                RaisePropertyChanged();
            }
        }
        private int _dlc;

        public int Period
        {
            get
            {
                return _period;
            }
            private set
            {
                if (value == _dlc)
                    return;

                _period = value;
                RaisePropertyChanged();
            }
        }
        private int _period;
        private double _lastSet;

        public ReadOnlyMonitorByteViewData[] Data
        {
            get { return data; }
        }
        private MonitorByteViewData[] data;


        public void SetPayload(byte[] payload, double time)
        {
            if (payload.Length != DLC)
                throw new ArgumentException("payload.Length should be equals with DLC");

            for (int i = 0; i < DLC; i++)
                data[i].Data = payload[i];

            //convert to ms
            Period = Convert.ToInt32(Math.Abs(time - _lastSet) * 1000);
            _lastSet = time;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
