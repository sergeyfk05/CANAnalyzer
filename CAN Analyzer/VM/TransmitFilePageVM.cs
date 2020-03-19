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

            PropertyChanged += SaveFileCommandCanExecuteChanged_PropertyChanged;
        }

        private List<ITraceDataTypeProvider> traceProviders = TraceDataTypeProvidersListBuilder.GenerateTraceDataTypeProviders();
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

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (value == _isEnabled)
                    return;

                _isEnabled = value;
                OnPropertyChanged();
            }
        }
        private bool _isEnabled = true;

        public bool FileIsOpened
        {
            get { return _fileIsOpened; }
            set
            {
                if (value == _fileIsOpened)
                    return;

                _fileIsOpened = value;
                OnPropertyChanged();
            }
        }
        private bool _fileIsOpened = false;

        private RelayCommandWithParameter<ComboBox> _transmitToComboBoxSelected;
        public RelayCommandWithParameter<ComboBox> TransmitToComboBoxSelected
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


        private RelayCommandAsync _openFileCommand;
        public RelayCommandAsync OpenFileCommand
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
                //reset data and closing connection
                ShowedData = null;
                currentTraceProvider?.CloseConnection();
                FileIsOpened = false;

                //find provider and load data
                bool findedProvider = false;
                foreach(var el in traceProviders)
                {
                    if(el.CanWorkWithIt(openFileDialog.FileName))
                    {
                        IsEnabled = false;
                        findedProvider = true;

                        currentTraceProvider?.CloseConnection();
                        el.TargetFile = openFileDialog.FileName;
                        currentTraceProvider = el;
                        UpdateData();

                        break;
                    }
                }

                //if provider not founded
                if(!findedProvider)
                {
                    MessageBox.Show("Added successfully", "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                IsEnabled = true;
            }
        }

        private async void UpdateData()
        {
            try
            {
                ShowedData = await currentTraceProvider.Traces.Include("CanHeader").ToListAsync();
                FileIsOpened = true;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private RelayCommandAsync _saveFileCommand;
        public RelayCommandAsync SaveFileCommand
        {
            get
            {
                if (_saveFileCommand == null)
                    _saveFileCommand = new RelayCommandAsync(this.SaveFileCommand_Execute, () => { return this.FileIsOpened; });

                return _saveFileCommand;
            }
        }
        private void SaveFileCommand_Execute()
        {
            currentTraceProvider.SaveChanges();
        }
        private void SaveFileCommandCanExecuteChanged_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if((e.PropertyName == "FileIsOpened") && (sender is TransmitFilePageVM vm))
                vm.SaveFileCommand.RaiseCanExecuteChanged();
        }
    }
}
