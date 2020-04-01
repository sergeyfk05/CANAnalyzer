/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CANAnalyzer.Models.ViewData
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
                RaisePropertyChanged();
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
                RaisePropertyChanged();
            }
        }
        private string _descriptionKey;


        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
