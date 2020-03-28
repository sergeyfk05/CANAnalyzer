using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CANAnalyzer.Models.ChannelsViewData
{
    public class TransmitToViewData : INotifyPropertyChanged
    {
        public bool IsTransmit
        {
            get { return _isTransmit; }
            set
            {
                if (value == _isTransmit)
                    return;

                _isTransmit = value;
                OnPropertyChanged();
            }
        }
        private bool _isTransmit = false;

        public string DescriptionKey
        {
            get { return _descriptionKey; }
            set
            {
                if (value == _descriptionKey)
                    return;

                _descriptionKey = value;
                OnPropertyChanged();
            }
        }
        private string _descriptionKey;


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
