using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicResource;
using CANAnalyzer.Models;
using System.Windows.Input;
using System.Windows.Controls;

namespace CANAnalyzer.VM
{
    public class TransmitPageVM : BaseVM
    {
        public TransmitPageVM()
        {
            TransmitToItems.Add(new TransmitToViewData() { IsTransmit = false, DescriptionKey = "ss" });
            TransmitToItems.Add(new TransmitToViewData() { IsTransmit = false, DescriptionKey = "s" });
            TransmitToItems.Add(new TransmitToViewData() { IsTransmit = true, DescriptionKey = "s" });
        }

        public List<TransmitToViewData> TransmitToItems
        {
            get { return _transmitToItems ?? (_transmitToItems = new List<TransmitToViewData>()); }
            set
            {
                if (_transmitToItems == value)
                    return;

                _transmitToItems = value;
                OnPropertyChanged();
            }
        }
        private List<TransmitToViewData> _transmitToItems;

        private ICommand _transmitToComboBoxSelected;
        public ICommand TransmitToComboBoxSelected
        {
            get
            {
                if (_transmitToComboBoxSelected == null)
                    _transmitToComboBoxSelected = new RelayCommandWithParameter<ComboBox>(this.TransmitToComboBoxSelected_Execute);

                return _transmitToComboBoxSelected;
            }
        }
        private void TransmitToComboBoxSelected_Execute(ComboBox arg)
        {
            arg.SelectedIndex = 0;
        }
    }
}
