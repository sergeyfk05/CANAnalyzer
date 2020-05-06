﻿/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzer.Models;
using CANAnalyzer.Models.Databases;
using CANAnalyzer.Models.Delegates;
using CANAnalyzer.Models.ViewData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CANAnalyzer.Models.Extensions;
using Microsoft.Win32;
using System.IO;
using DynamicResource;
using CANAnalyzer.Resources.DynamicResources;
using CANAnalyzer.Models.DataTypesProviders;
using CANAnalyzer.Models.DataTypesProviders.Builders;
using System.Collections;

namespace CANAnalyzer.VM
{
    public class TransmitPageVM : BaseClosableVM
    {

        public TransmitPageVM()
        {
            PropertyChanged += TransmitToSelectedChannels_PropertyChanged;
            PropertyChanged += CurrentTraceProvider_PropertyChanged;
            Settings.Instance.PropertyChanged += Device_PropertyChanged;

            Device_PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Device"));
        }

        public ObservableCollection<TracePeriodicViewData> Data
        {
            get { return _data; }
            private set
            {
                if (_data == value)
                    return;

                _data = value;

                if (_data != null)
                    _data.CollectionChanged += Data_CollectionChanged;

                RaisePropertyChanged();
            }
        }


        private ObservableCollection<TracePeriodicViewData> _data = new ObservableCollection<TracePeriodicViewData>();

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

        public List<TracePeriodicViewData> SelectedItems
        {
            get { return _selectedItems; }
            private set
            {
                if (_selectedItems == value)
                    return;

                _selectedItems = value;
                RaisePropertyChanged();
            }
        }
        private List<TracePeriodicViewData> _selectedItems = new List<TracePeriodicViewData>();

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
            }

        }

        private RelayCommandWithParameterAsync<IList> _selectedItemsChangedCommand;
        public RelayCommandWithParameterAsync<IList> SelectedItemsChangedCommand
        {
            get
            {
                if (_selectedItemsChangedCommand == null)
                    _selectedItemsChangedCommand = new RelayCommandWithParameterAsync<IList>(this.SelectedItemsChangedCommand_Execute);

                return _selectedItemsChangedCommand;
            }
        }
        private void SelectedItemsChangedCommand_Execute(IList ts)
        {
            _selectedItems.Clear();

            foreach (var el in ts)
            {
                if (el is TracePeriodicViewData viewData)
                {
                    _selectedItems.Add(viewData);
                }
            }

            StartTransmitingCommand.RaiseCanExecuteChanged();
            StopTransmitingCommand.RaiseCanExecuteChanged();
            ShotCommand.RaiseCanExecuteChanged();
        }

        private RelayCommandAsync _startTransmitingCommand;
        public RelayCommandAsync StartTransmitingCommand
        {
            get
            {
                if (_startTransmitingCommand == null)
                    _startTransmitingCommand = new RelayCommandAsync(this.StartTransmitingCommand_Execute, () =>
                    {
                        if (SelectedItems == null)
                            return false;

                        return SelectedItems.Count((x) => !x.IsTrasmiting) > 0;
                    });

                return _startTransmitingCommand;
            }
        }
        private void StartTransmitingCommand_Execute()
        {

            SelectedItems.ForEach((el) =>
            {
                el.StartTransmiting();
                el.TransmitToSelectedChannels = TransmitToSelectedChannels;
            });
        }

        private RelayCommandAsync _shotCommand;
        public RelayCommandAsync ShotCommand
        {
            get
            {
                if (_shotCommand == null)
                    _shotCommand = new RelayCommandAsync(this.ShotCommand_Execute, () => { return SelectedItems != null && SelectedItems.Count > 0; });

                return _shotCommand;
            }
        }
        private void ShotCommand_Execute()
        {
            SelectedItems.ForEach((el) => el.Shot());
        }

        private RelayCommandAsync _stopTransmitingCommand;
        public RelayCommandAsync StopTransmitingCommand
        {
            get
            {
                if (_stopTransmitingCommand == null)
                    _stopTransmitingCommand = new RelayCommandAsync(this.StopTransmitingCommand_Execute, () =>
                    {
                        if (SelectedItems == null)
                            return false;

                        return SelectedItems.Count((x) => x.IsTrasmiting) > 0;
                    });

                return _stopTransmitingCommand;
            }
        }
        private void StopTransmitingCommand_Execute()
        {
            SelectedItems.ForEach((el) => el.StopTransmiting());
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
            IsEnabled = false;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = GenerateFilterForDialog(traceProviders);
            openFileDialog.CheckFileExists = true;
            openFileDialog.Multiselect = false;


            if (openFileDialog.ShowDialog() == true)
            {

                if (openFileDialog.FileName == CurrentTraceProvider?.TargetFile)
                {
                    MessageBox.Show("Файл уже открыт", (string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("ErrorMsgBoxTitle"), MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                //reset data and closing connection
                CurrentTraceProvider?.CloseConnection();
                CurrentTraceProvider = null;
                _context.Post((s) => { Data.Clear(); }, null);


                //find provider and load data
                bool findedProvider = false;
                foreach (var el in traceProviders)
                {
                    if (el.CanWorkWithIt(openFileDialog.FileName))
                    {
                        IsEnabled = false;
                        findedProvider = true;

                        try
                        {
                            CurrentTraceProvider?.CloseConnection();
                            el.TargetFile = openFileDialog.FileName;
                            CurrentTraceProvider = el;
                            UpdateDataCommand_Execute();
                        }
                        catch (Exception e)
                        { MessageBox.Show(e.ToString(), (string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("ErrorMsgBoxTitle"), MessageBoxButton.OK, MessageBoxImage.Error); }

                        break;
                    }
                }

                //if provider not founded
                if (!findedProvider)
                {
                    MessageBox.Show("Added successfully", (string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("ErrorMsgBoxTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            IsEnabled = true;
        }

        private RelayCommandAsync _saveFileCommand;
        public RelayCommandAsync SaveFileCommand
        {
            get
            {
                if (_saveFileCommand == null)
                    _saveFileCommand = new RelayCommandAsync(this.SaveFileCommand_Execute, () => { return CurrentTraceProvider != null; });

                return _saveFileCommand;
            }
        }
        private void SaveFileCommand_Execute()
        {
            IsEnabled = false;
            try
            {
                CurrentTraceProvider?.SaveChanges();
            }
            catch (Exception e)
            { MessageBox.Show(e.ToString(), (string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("ErrorMsgBoxTitle"), MessageBoxButton.OK, MessageBoxImage.Error); }
            IsEnabled = true;
        }

        private RelayCommandAsync _saveAsFileCommand;
        public RelayCommandAsync SaveAsFileCommand
        {
            get
            {
                if (_saveAsFileCommand == null)
                    _saveAsFileCommand = new RelayCommandAsync(this.SaveAsFileCommand_Execute);

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
                    MessageBox.Show((string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("FileError"),
                        (string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("ErrorMsgBoxTitle"),
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
                            CurrentTraceProvider = await el.SaveAsAsync(saveFileDialog.FileName, Data.Select(x => x.Model));
                            UpdateDataCommand_Execute();
                        }
                        catch (Exception e)
                        { MessageBox.Show(e.ToString(), (string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("ErrorMsgBoxTitle"), MessageBoxButton.OK, MessageBoxImage.Error); }

                        break;
                    }
                }


                if (!findedProvider)
                {
                    MessageBox.Show("Added successfully", (string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("ErrorMsgBoxTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
                }
                IsEnabled = true;
            }

        }

        private RelayCommand _clearDataCommand;
        public RelayCommand ClearDataCommand
        {
            get
            {
                if (_clearDataCommand == null)
                    _clearDataCommand = new RelayCommand(this.ClearDataCommand_Execute);

                return _clearDataCommand;
            }
        }
        private void ClearDataCommand_Execute()
        {
            Data.Clear();
        }

        private RelayCommandAsync _updateDataCommand;
        public RelayCommandAsync UpdateDataCommand
        {
            get
            {
                if (_updateDataCommand == null)
                    _updateDataCommand = new RelayCommandAsync(this.UpdateDataCommand_Execute);

                return _updateDataCommand;
            }
        }
        private void UpdateDataCommand_Execute()
        {
            if (CurrentTraceProvider == null)
                return;

            Data = new ObservableCollection<TracePeriodicViewData>(CurrentTraceProvider.TransmitModels.ToList().Select(x => new TracePeriodicViewData(x)));
        }






        private List<ITransmitPeriodicDataTypeProvider> traceProviders = TransmitPeriodicDataTypeProvidedsBuilder.GenerateTransmitPeriodicDataTypeProviders();
        private ITransmitPeriodicDataTypeProvider CurrentTraceProvider
        {
            get { return _currentTraceProvider; }
            set
            {
                if (_currentTraceProvider == value)
                    return;

                _currentTraceProvider = value;
                RaisePropertyChanged();
            }
        }
        private ITransmitPeriodicDataTypeProvider _currentTraceProvider;
        private System.Threading.SynchronizationContext _context = System.Threading.SynchronizationContext.Current;




        private void CurrentTraceProvider_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentTraceProvider")
            {
                SaveFileCommand.RaiseCanExecuteChanged();
            }
        }
        private string GenerateFilterForDialog(IEnumerable<ITransmitPeriodicDataTypeProvider> source)
        {
            string result = "";

            foreach (var el in source)
            {
                if (result == "")
                    result += $"{Manager<LanguageCultureInfo>.StaticInstance.GetResource(el.GetType().ToString() + "_FileGroup")}({el.SupportedFiles})|{el.SupportedFiles}";
                else
                    result += $"|{Manager<LanguageCultureInfo>.StaticInstance.GetResource(el.GetType().ToString() + "_FileGroup")}({el.SupportedFiles})|{el.SupportedFiles}";
            }

            if (result == "")
                result += $"{Manager<LanguageCultureInfo>.StaticInstance.GetResource("AllFiles_FileGroup")} (*.*)|*.* ";
            else
                result += $"|{Manager<LanguageCultureInfo>.StaticInstance.GetResource("AllFiles_FileGroup")} (*.*)|*.* ";

            return result;
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
            foreach (var el in Data)
            {
                if (el != null)
                    el.TransmitToSelectedChannels = TransmitToSelectedChannels;
            }
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
        private void Data_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (sender is ObservableCollection<TracePeriodicViewData> collection && collection == Data)
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    foreach (var el in e.NewItems)
                    {
                        if (el is TracePeriodicViewData viewData)
                            CurrentTraceProvider?.Add(viewData.Model);
                    }
                }

                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                {
                    foreach (var el in e.OldItems)
                    {
                        if (el is TracePeriodicViewData viewData)
                            CurrentTraceProvider?.Remove(viewData.Model);
                    }
                }

                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                {
                    CurrentTraceProvider?.RemoveAll();
                }

            }
        }
    }
}
