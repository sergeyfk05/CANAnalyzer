/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzer.Models;
using CANAnalyzer.Models.Databases;
using CANAnalyzer.Models.DataTypesProviders;
using CANAnalyzer.Models.Delegates;
using CANAnalyzer.Models.States;
using CANAnalyzer.Models.TraceFilters;
using CANAnalyzer.Models.ViewData;
using CANAnalyzer.Resources.DynamicResources;
using DynamicResource;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using static CANAnalyzer.Models.TraceTransmiter;

namespace CANAnalyzer.VM
{
    public class TransmitFilePageVM : BaseClosableVM, IDisposable
    {
        public TransmitFilePageVM()
        {

            PropertyChanged += OnSaveFileCommandCanExecuteChanged_PropertyChanged;
            PropertyChanged += OnSaveAsFileCommandCanExecuteChanged_PropertyChanged;
            PropertyChanged += OnOpenFileCommandCanExecuteChanged_PropertyChanged;
            PropertyChanged += ShowedData_PropertyChanged;
            PropertyChanged += TransmitToSelectedChannels_PropertyChanged;

            Settings.Instance.PropertyChanged += Device_PropertyChanged;

            _transmiter = new TraceTransmiter();
            _transmiter.StatusChanged += _transmiter_StatusChanged;
            _transmiter.CurrentIndexChanged += _transmiter_CurrentIndexChanged;

            Device_PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Device"));
        }


        #region properties

        public ObservableCollection<TransmitToViewData> TransmitToItems
        {
            get { return _transmitToItems ?? (_transmitToItems = new ObservableCollection<TransmitToViewData>()); }
            set
            {
                if (_transmitToItems == value)
                    return;

                _transmitToItems = value;
                RaisePropertyChanged();
            }
        }
        private ObservableCollection<TransmitToViewData> _transmitToItems;

        public ObservableCollection<TraceModel> ShowedData
        {
            get { return _showedData; }
            set
            {
                if (value == _showedData)
                    return;

                if (_showedData != null)
                    _showedData.CollectionChanged -= ShowedData_CollectionChanged;

                _showedData = value;
                if (_showedData != null)
                    _showedData.CollectionChanged += ShowedData_CollectionChanged;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<TraceModel> _showedData;

        public List<CanIdTraceFilter> Filters
        {
            get { return _filters ?? (_filters = new List<CanIdTraceFilter>()); }
            set
            {
                if (value == _filters)
                    return;

                _filters = value;
                RaisePropertyChanged();
            }
        }
        private List<CanIdTraceFilter> _filters;

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (value == _isEnabled)
                    return;

                _isEnabled = value;
                RaisePropertyChanged();
            }
        }
        private bool _isEnabled = true;

        public FileState FileIsOpened
        {
            get { return _fileIsOpened; }
            set
            {
                if (value == _fileIsOpened)
                    return;

                _fileIsOpened = value;
                RaisePropertyChanged();
            }
        }
        private FileState _fileIsOpened = FileState.Closed;

        public int SelectedItemIndex
        {
            get { return _selectedItemIndex; }
            set
            {
                if (_selectedItemIndex == value)
                    return;

                _selectedItemIndex = value;
                RaisePropertyChanged();
            }
        }
        private int _selectedItemIndex;

        public TransmitToDelegate TransmitToSelectedChannels
        {
            get { return _transmitToSelectedChannels; }
            private set
            {
                if (value == _transmitToSelectedChannels)
                    return;

                _transmitToSelectedChannels = value;
                RaisePropertyChanged();
            }
        }
        private TransmitToDelegate _transmitToSelectedChannels;

        public TraceTransmiterStatus Status => _transmiter.Status;

        #endregion


        #region commands

        private RelayCommandAsync _openFileCommand;
        public RelayCommandAsync OpenFileCommand
        {
            get
            {
                if (_openFileCommand == null)
                    _openFileCommand = new RelayCommandAsync(this.OpenFileCommand_Execute, () => { return this.FileIsOpened != FileState.Opening; });

                return _openFileCommand;
            }
        }
        private void OpenFileCommand_Execute()
        {

            var prevStatus = FileIsOpened;
            FileIsOpened = FileState.Opening;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = GenerateFilterForDialog(traceProviders);
            openFileDialog.CheckFileExists = true;
            openFileDialog.Multiselect = false;
            
            
            IsEnabled = false;

            if (openFileDialog.ShowDialog() == true)
            {
                if (openFileDialog.FileName == currentTraceProvider?.TargetFile)
                {
                    MessageBox.Show((string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("#FileError"), (string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("#ErrorMsgBoxTitle"), MessageBoxButton.OK, MessageBoxImage.Warning);
                    FileIsOpened = prevStatus;
                    IsEnabled = true;
                    return;
                }

                //reset data and closing connection
                ShowedData = null;
                currentTraceProvider?.CloseConnection();
                FileIsOpened = FileState.Closed;

                //find provider and load data
                bool findedProvider = false;
                foreach (var el in traceProviders)
                {
                    if (el.CanWorkWithIt(openFileDialog.FileName))
                    {
                        
                        findedProvider = true;

                        try
                        {
                            currentTraceProvider?.CloseConnection();
                            el.TargetFile = openFileDialog.FileName;
                            currentTraceProvider = el;
                            UpdateDataAndFiltersCommand.Execute();
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.ToString(), (string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("#ErrorMsgBoxTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
                            FileIsOpened = FileState.Closed;
                        }

                        break;
                    }
                }

                //if provider not founded
                if (!findedProvider)
                {
                    MessageBox.Show((string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("#InvalidFileError"), (string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("#ErrorMsgBoxTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
                    IsEnabled = true;
                }

            }
            else
            {
                IsEnabled = true;
                FileIsOpened = prevStatus;
            }
        }

        private RelayCommandAsync _saveFileCommand;
        public RelayCommandAsync SaveFileCommand
        {
            get
            {
                if (_saveFileCommand == null)
                    _saveFileCommand = new RelayCommandAsync(this.SaveFileCommand_Execute, () => { return this.FileIsOpened == FileState.Opened; });

                return _saveFileCommand;
            }
        }
        private void SaveFileCommand_Execute()
        {
            IsEnabled = false;
            try
            {
                currentTraceProvider.SaveChanges();
            }
            catch (Exception e)
            { MessageBox.Show(e.ToString(), (string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("#ErrorMsgBoxTitle"), MessageBoxButton.OK, MessageBoxImage.Error); }
            IsEnabled = true;
        }

        private RelayCommandAsync _saveAsFileCommand;
        public RelayCommandAsync SaveAsFileCommand
        {
            get
            {
                if (_saveAsFileCommand == null)
                    _saveAsFileCommand = new RelayCommandAsync(this.SaveAsFileCommand_Execute, () => { return this.FileIsOpened == FileState.Opened; });

                return _saveAsFileCommand;
            }
        }
        private async void SaveAsFileCommand_Execute()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = GenerateFilterForDialog(traceProviders);

            if (saveFileDialog.ShowDialog() == true)
            {
                IsEnabled = false;
                try
                {
                    if (File.Exists(saveFileDialog.FileName))
                        File.Delete(saveFileDialog.FileName);
                }
                catch
                {
                    MessageBox.Show((string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("#FileError"),
                        (string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("#ErrorMsgBoxTitle"),
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }

                bool findedProvider = false;
                foreach (var el in traceProviders)
                {
                    if (el.CanWorkWithIt(saveFileDialog.FileName))
                    {
                        findedProvider = true;

                        try
                        {
                            currentTraceProvider = await el.SaveAsAsync(saveFileDialog.FileName, currentTraceProvider.Traces, currentTraceProvider.CanHeaders);
                            UpdateDataAndFiltersCommand.Execute();
                        }
                        catch (Exception e)
                        { MessageBox.Show(e.ToString(), (string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("#ErrorMsgBoxTitle"), MessageBoxButton.OK, MessageBoxImage.Error); }

                        break;
                    }
                }


                if (!findedProvider)
                {
                    MessageBox.Show((string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("#InvalidFileError"), (string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("#ErrorMsgBoxTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
                }
                IsEnabled = true;
            }

        }



        private RelayCommand _closePageCommand;
        public RelayCommand ClosePageCommand
        {
            get
            {
                if (_closePageCommand == null)
                    _closePageCommand = new RelayCommand(this.ClosePageCommand_Execute);

                return _closePageCommand;
            }
        }
        private void ClosePageCommand_Execute()
        {

            if (MessageBox.Show("are you want close the page?", "???", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                RaiseClosedEvent();
                currentTraceProvider?.CloseConnection();
            }

        }



        private RelayCommandAsync _startTraceCommand;
        public RelayCommandAsync StartTraceCommand
        {
            get
            {
                if (_startTraceCommand == null)
                    _startTraceCommand = new RelayCommandAsync(this.StartTraceCommand_Execute, () => { return (_transmiter.Status == TraceTransmiter.TraceTransmiterStatus.Paused) || (_transmiter.Status == TraceTransmiter.TraceTransmiterStatus.Reseted) || (_transmiter.Status == TraceTransmiter.TraceTransmiterStatus.Undefined); });

                return _startTraceCommand;
            }
        }
        private void StartTraceCommand_Execute()
        {
            _transmiter.SetCurrentItemIndex(SelectedItemIndex);
            _transmiter.Start();
        }

        private RelayCommandAsync _pauseTraceCommand;
        public RelayCommandAsync PauseTraceCommand
        {
            get
            {
                if (_pauseTraceCommand == null)
                    _pauseTraceCommand = new RelayCommandAsync(this.PauseTraceCommand_Execute, () => { return _transmiter.Status == TraceTransmiter.TraceTransmiterStatus.Working; });

                return _pauseTraceCommand;
            }
        }
        private void PauseTraceCommand_Execute()
        {
            _transmiter.Pause();
        }

        private RelayCommandAsync _stopTraceCommand;
        public RelayCommandAsync StopTraceCommand
        {
            get
            {
                if (_stopTraceCommand == null)
                    _stopTraceCommand = new RelayCommandAsync(this.StopTraceCommand_Execute, () => { return (_transmiter.Status == TraceTransmiter.TraceTransmiterStatus.Working) || (_transmiter.Status == TraceTransmiter.TraceTransmiterStatus.Paused); });

                return _stopTraceCommand;
            }
        }
        private void StopTraceCommand_Execute()
        {
            _transmiter.Stop();
        }

        #endregion


        #region private local-use methods

        private RelayCommandAsync _updateDataAndFiltersCommand;
        public RelayCommandAsync UpdateDataAndFiltersCommand
        {
            get
            {
                if (_updateDataAndFiltersCommand == null)
                    _updateDataAndFiltersCommand = new RelayCommandAsync(this.UpdateDataAndFilters_Execute);

                return _updateDataAndFiltersCommand;
            }
        }
        private void UpdateDataAndFilters_Execute()
        {
            try
            {
                var newFilters = new List<CanIdTraceFilter>();
                foreach (var el in currentTraceProvider.CanHeaders.ToList())
                {
                    if (newFilters.Count(x => x.CanId == el.CanId && x.IsExtId == el.IsExtId) == 0)
                    {
                        CanIdTraceFilter entity = new CanIdTraceFilter(el.CanId, el.IsExtId);
                        entity.PropertyChanged += FilterIsActive_PropertyChanged;
                        newFilters.Add(entity);
                    }
                }
                Filters = newFilters;

                UpdateDataCommand.Execute();

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), (string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("#ErrorMsgBoxTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private RelayCommandAsync _updateDataCommand;
        public RelayCommandAsync UpdateDataCommand
        {
            get
            {
                if (_updateDataCommand == null)
                    _updateDataCommand = new RelayCommandAsync(this.UpdateData_Execute);

                return _updateDataCommand;
            }
        }
        public void UpdateData_Execute()
        {
            try
            {
                IsEnabled = false;
                var data = currentTraceProvider.Traces;
                foreach (var el in Filters)
                {
                    data = el.Filter(data);
                }


                ShowedData = new ObservableCollection<TraceModel>(data.Include("CanHeader").ToList());
                FileIsOpened = FileState.Opened;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), (string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("#ErrorMsgBoxTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsEnabled = true;
            }

        }

        private RelayCommandWithParameterAsync<IList> _removeDataCommand;
        public RelayCommandWithParameterAsync<IList> RemoveDataCommand
        {
            get
            {
                if (_removeDataCommand == null)
                    _removeDataCommand = new RelayCommandWithParameterAsync<IList>(this.RemoveDataCommand_Execute);

                return _removeDataCommand;
            }
        }
        public void RemoveDataCommand_Execute(IList data)
        {
            IsEnabled = false;

            if (data == null)
                return;

            foreach (var item in data)
            {
                if (item is TraceModel model)
                {
                    try { currentTraceProvider.Remove(model); } catch { }
                }
            }

            IsEnabled = true;

        }

        private string GenerateFilterForDialog(IEnumerable<ITraceDataTypeProvider> source)
        {
            string result = "";

            foreach (var el in source)
            {
                if (result == "")
                    result += $"{Manager<LanguageCultureInfo>.StaticInstance.GetResource("#" + el.GetType().ToString() + "_FileGroup")}({el.SupportedFiles})|{el.SupportedFiles}";
                else
                    result += $"|{Manager<LanguageCultureInfo>.StaticInstance.GetResource("#" + el.GetType().ToString() + "_FileGroup")}({el.SupportedFiles})|{el.SupportedFiles}";
            }

            if (result == "")
                result += $"{Manager<LanguageCultureInfo>.StaticInstance.GetResource("#AllFiles_FileGroup")} (*.*)|*.*";
            else
                result += $"|{Manager<LanguageCultureInfo>.StaticInstance.GetResource("#AllFiles_FileGroup")} (*.*)|*.*";

            return result;
        }

        #endregion


        #region private varibles

        private List<ITraceDataTypeProvider> traceProviders = TraceDataTypeProvidersListBuilder.GenerateTraceDataTypeProviders();
        private ITraceDataTypeProvider currentTraceProvider;
        private TraceTransmiter _transmiter;
        private System.Threading.SynchronizationContext _context = System.Threading.SynchronizationContext.Current;

        #endregion


        #region eventHandlers

        private void _transmiter_CurrentIndexChanged(object sender, EventArgs e)
        {
            SelectedItemIndex = _transmiter.CurrentIndex;
        }
        private void ShowedData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _transmiter.Source = ShowedData;
        }
        private void TransmitToViewDataIsTransmit_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "IsTransmit") && (sender is TransmitToViewData viewData) && (TransmitToItems.Contains(viewData)))
            {
                if (viewData.IsTransmit)
                {
                    if (TransmitToSelectedChannels == null)
                        TransmitToSelectedChannels = viewData.Transmit;
                    else
                        TransmitToSelectedChannels += viewData.Transmit;
                }
                else
                {
                    try
                    {
                        TransmitToSelectedChannels -= viewData.Transmit;
                    }
                    catch { }
                }
            }
        }
        private void TransmitToSelectedChannels_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _transmiter.TransmitTo = TransmitToSelectedChannels;
        }
        private void OnOpenFileCommandCanExecuteChanged_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "FileIsOpened") && (sender is TransmitFilePageVM vm))
                vm.OpenFileCommand.RaiseCanExecuteChanged();
        }
        private void FilterIsActive_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsActive")
                return;

            UpdateDataCommand.Execute();

        }
        private void OnSaveFileCommandCanExecuteChanged_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "FileIsOpened") && (sender is TransmitFilePageVM vm))
                vm.SaveFileCommand.RaiseCanExecuteChanged();
        }
        private void OnSaveAsFileCommandCanExecuteChanged_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "FileIsOpened") && (sender is TransmitFilePageVM vm))
                vm.SaveAsFileCommand.RaiseCanExecuteChanged();
        }
        private void _transmiter_StatusChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("Status");
            StartTraceCommand.RaiseCanExecuteChanged();
            PauseTraceCommand.RaiseCanExecuteChanged();
            StopTraceCommand.RaiseCanExecuteChanged();
        }
        private void Device_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Device")
                return;

            //create ViewData for  transmitable channels
            _context.Send((s) =>
            {
                TransmitToItems.Clear();
            }, null);
            TransmitToSelectedChannels = null;
            if (Settings.Instance.Device != null && Settings.Instance.Device.IsConnected && Settings.Instance.Device.Channels != null)
            {
                Settings.Instance.Device.IsConnectedChanged += Device_IsConnectedChanged;
                foreach (var el in Settings.Instance.Device.Channels)
                {
                    var viewData = new TransmitToViewData() { IsTransmit = false, DescriptionKey = $"#{el.ToString()}NavMenu", Channel = el };
                    viewData.PropertyChanged += TransmitToViewDataIsTransmit_PropertyChanged;
                    _context.Send((s) =>
                    {
                        TransmitToItems.Add(viewData);
                    }, null);
                }
            }
        }
        private void Device_IsConnectedChanged(object sender, EventArgs e)
        {
            if (Settings.Instance.Device == sender)
            {
                Device_PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Device"));
            }
        }
        private void ShowedData_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!((sender is ObservableCollection<TraceModel> data) && (ShowedData == data)))
                return;

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                RemoveDataCommand.Execute(e.OldItems);
            }
        }

        public void Dispose()
        {

            if (Settings.Instance.Device != null)
                Settings.Instance.Device.IsConnectedChanged -= Device_IsConnectedChanged;


            TransmitToItems = null;
            currentTraceProvider?.Dispose();

            if(_transmiter != null)
            {
                _transmiter.StatusChanged -= _transmiter_StatusChanged;
                _transmiter.CurrentIndexChanged -= _transmiter_CurrentIndexChanged;
                _transmiter.Dispose();
            }
        }
        ~TransmitFilePageVM()
        {
            this.Dispose();
        }

        #endregion
    }
}
