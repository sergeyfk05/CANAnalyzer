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
