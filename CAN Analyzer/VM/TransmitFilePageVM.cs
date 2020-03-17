using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicResource;
using CANAnalyzer.Models;
using System.Windows.Input;
using System.Windows.Controls;
using Microsoft.Win32;
using CANAnalyzer.Models.DataTypesProviders;
using System.Windows;
using CANAnalyzer.Models.Databases;
using System.Data.Entity;

namespace CANAnalyzer.VM
{
    public class TransmitFilePageVM : BaseVM
    {
        public TransmitFilePageVM()
        {
            TransmitToItems.Add(new TransmitToViewData() { IsTransmit = false, DescriptionKey = "ss" });
            TransmitToItems.Add(new TransmitToViewData() { IsTransmit = false, DescriptionKey = "s" });
            TransmitToItems.Add(new TransmitToViewData() { IsTransmit = true, DescriptionKey = "s" });



            traceProviders = new List<ITraceDataTypeProvider>();
            traceProviders.Add(new SQLiteTraceDataTypeProvider());
        }

        private List<ITraceDataTypeProvider> traceProviders;
        private ITraceDataTypeProvider currentTraceProvider;

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


        public List<TraceModel> ShowedData
        {
            get { return _showedData; }
            set
            {
                if (value == _showedData)
                    return;

                _showedData = value;
                OnPropertyChanged();
            }
        }
        private List<TraceModel> _showedData;


        private ICommand _openFileCommand;
        public ICommand OpenFileCommand
        {
            get
            {
                if (_openFileCommand == null)
                    _openFileCommand = new RelayCommandAsync(this.OpenFileCommand_Execute);

                return _openFileCommand;
            }
        }
        private void OpenFileCommand_Execute()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Картинки(*.db;*.sqlite3)|*.db;*.sqlite3" + "|Все файлы (*.*)|*.* ";
            openFileDialog.CheckFileExists = true;
            openFileDialog.Multiselect = false;
            if (openFileDialog?.ShowDialog() == true)
            {
                bool isOpened = false;
                foreach(var el in traceProviders)
                {
                    if(el.CanWorkWithIt(openFileDialog.FileName))
                    {
                        isOpened = true;

                        currentTraceProvider?.CloseConnection();
                        el.TargetFile = openFileDialog.FileName;
                        currentTraceProvider = el;
                        UpdateData();

                        break;
                    }
                }

                if(!isOpened)
                {
                    MessageBox.Show("Added successfully", "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void UpdateData()
        {
            ShowedData = await currentTraceProvider.Traces.Include("CanHeader").ToListAsync();
        }

    }
}
