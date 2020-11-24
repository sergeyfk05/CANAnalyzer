/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzer.Models;
using CANAnalyzer.Models.DataTypesProvidersBuilders;
using CANAnalyzer.Models.Delegates;
using CANAnalyzer.Models.States;
using CANAnalyzer.Models.TraceFilters;
using CANAnalyzer.Models.ViewData;
using CANAnalyzer.Resources.DynamicResources;
using CANAnalyzerDataModels;
using CANAnalyzerDataProvidersInterfaces;
using CANAnalyzerDevices.Devices.DeviceChannels;
using CANAnalyzerSQLiteDataProvider;
using DynamicResource;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

namespace CANAnalyzer.VM
{
    public class RecieveChannelPageVM : TransmitableAndClosableBaseVM, IDisposable
    {
        public RecieveChannelPageVM(IChannel ch)
        {
            PropertyChanged += RecieveState_PropertyChanged;

            

            //create main trace data type provider
            currentTraceProvider = new SQLiteTraceDataTypeProvider();
            string tempfile = "";
            while (true)
            {
                tempfile = new Uri(
                    new Uri(Path.GetTempPath(), UriKind.Absolute),
                    new Uri(Path.GetRandomFileName() + ".traceDB", UriKind.Relative)
                    ).AbsolutePath;
                if (!File.Exists(tempfile))
                {
                    currentTraceProvider = currentTraceProvider.SaveAs(tempfile, null, null);
                    break;
                }

            }


            PropertyChanged += Channel_PropertyChanged;

            Channel = ch;
        }

        private void TransmitToViewDataIsTransmit_PropertyChanged(object sender, PropertyChangedEventArgs e)
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

        private void RecieveState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ((sender is RecieveChannelPageVM vm) && (e.PropertyName == "RecieveState"))
            {
                vm.SaveAsFileCommand.RaiseCanExecuteChanged();
                vm.RecieveStartCommand.RaiseCanExecuteChanged();
                vm.PauseRecievingCommand.RaiseCanExecuteChanged();
                vm.ClearTraceCommand.RaiseCanExecuteChanged();
            }
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (sender != Items)
                return;

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (var el in e.NewItems)
                {
                    //filtering
                    if (el is TraceModel model)
                    {
                        bool isFiltered = false;

                        foreach (var filter in Filters)
                        {
                            if (filter.FilterOne(model))
                            {
                                isFiltered = true;
                                break;
                            }
                        }


                        if (!isFiltered)
                            _context.Post((s) =>
                            {
                                ShowedItems.Add(model);
                            }, null);

                        TransmitToSelectedChannels?.Invoke(new TransmitData() { CanId = model.CanHeader.CanId, DLC = model.CanHeader.DLC, IsExtId = model.CanHeader.IsExtId, Payload = model.Payload });
                    }
                }
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {

                lock (currentTraceProvider)
                {
                    currentTraceProvider.RemoveAll();
                    currentTraceProvider.SaveChanges();
                }



                _context.Post((s) =>
                {
                    ShowedItems.Clear();
                }, null);
            }
        }

        private SynchronizationContext _context = SynchronizationContext.Current;

        private void Channel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Channel")
            {
                Channel.ReceivedData += Channel_ReceivedData;
            }
        }

        private void Channel_ReceivedData(object sender, CANAnalyzerDevices.Devices.DeviceChannels.Events.ChannelDataReceivedEventArgs e)
        {
            if (RecieveState != RecieveState.Recieving || sender != Channel || currentTraceProvider == null)
                return;

            TraceModel model = new TraceModel()
            {
                Time = e.Data.Time,
                Payload = e.Data.Payload
            };


            //проверка на существование такого же CanHeader'a
            List<CanHeaderModel> canHeaders;
            int filtersCount;
            lock (currentTraceProvider)
            {
                canHeaders = currentTraceProvider.CanHeaders.Where(x => (x.IsExtId == e.Data.IsExtId) && (x.CanId == e.Data.CanId) && (x.DLC == e.Data.DLC)).Take(1).ToList();
                filtersCount = currentTraceProvider.CanHeaders.Where(x => (x.IsExtId == e.Data.IsExtId) && (x.CanId == e.Data.CanId)).Count();
            }

            //если такого CanHeader'a нет, то создаем, добавляем в БД
            if (canHeaders.Count == 0)
            {
                canHeaders.Add(new CanHeaderModel()
                {
                    IsExtId = e.Data.IsExtId,
                    CanId = e.Data.CanId,
                    DLC = e.Data.DLC
                });
                currentTraceProvider.Add(canHeaders[0]);
                lock (currentTraceProvider)
                {
                    currentTraceProvider.SaveChanges();
                }
            }

            //если появился новый CAN ID
            if (filtersCount == 0)
            {
                CanIdTraceFilter filter = new CanIdTraceFilter(e.Data.CanId, e.Data.IsExtId);
                filter.PropertyChanged += FilterIsActive_PropertyChanged;
                _context.Post((s) =>
                {
                    Filters.Add(filter);
                }, null);
            }


            model.CanHeader = canHeaders[0];

            currentTraceProvider.Add(model);


            _context.Post((s) =>
            {
                Items.Add(model);
            }, null);

        }

        private void FilterIsActive_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            lock(currentTraceProvider)
            {
                currentTraceProvider.SaveChanges();
            }

            var data = currentTraceProvider.Traces;
            foreach (var el in Filters)
            {
                data = el.Filter(data);
            }

            List<TraceModel> buffer;
            lock (currentTraceProvider)
            {
                buffer = data.Include("CanHeader").ToList();
            }
            _context.Post((s) =>
            {
                ShowedItems = new ObservableCollection<TraceModel>(buffer);
            }, null);


            //Items.Clear();
            //foreach(var el in await data.Include("CanHeader").ToListAsync())
            //{
            //    Items.Add(el);
            //}
        }

       



        private List<ITraceDataTypeProvider> traceProviders = TraceDataTypeProvidersListBuilder.GenerateTraceDataTypeProviders();
        private ITraceDataTypeProvider currentTraceProvider;




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
        public RecieveState RecieveState
        {
            get { return _recieveState; }
            set
            {
                if (value == _recieveState)
                    return;

                _recieveState = value;
                RaisePropertyChanged();
            }
        }
        private RecieveState _recieveState = RecieveState.Blocked;

        public BindingList<ITraceFilter> Filters
        {
            get { return _filters ?? (_filters = new BindingList<ITraceFilter>()); }
            set
            {
                if (value == _filters)
                    return;

                _filters = value;
                RaisePropertyChanged();
            }
        }
        private BindingList<ITraceFilter> _filters;

        public ObservableCollection<TraceModel> ShowedItems
        {
            get { return _showedItems ?? (_showedItems = new ObservableCollection<TraceModel>()); }
            private set
            {
                if (value == _showedItems)
                    return;

                _showedItems = value;
                RaisePropertyChanged();
            }
        }
        private ObservableCollection<TraceModel> _showedItems;

        public ObservableCollection<TraceModel> Items
        {
            get
            {
                if (_items != null)
                    return _items;

                _items = new ObservableCollection<TraceModel>();
                _items.CollectionChanged += Items_CollectionChanged;
                return _items;
            }
            private set
            {
                if (value == _items)
                    return;

                if (_items != null)
                    _items.CollectionChanged -= Items_CollectionChanged;

                _items = value;
                _items.CollectionChanged += Items_CollectionChanged;
                RaisePropertyChanged();
            }
        }
        private ObservableCollection<TraceModel> _items;

        public IChannel Channel
        {
            get { return _channel; }
            protected set
            {
                if (_channel == value)
                    return;

                _channel = value;
                RaisePropertyChanged();
            }
        }
        private IChannel _channel;


        private RelayCommandAsync _saveAsFileCommand;
        public RelayCommandAsync SaveAsFileCommand
        {
            get
            {
                if (_saveAsFileCommand == null)
                    _saveAsFileCommand = new RelayCommandAsync(this.SaveAsFileCommand_Execute, () => { return this.RecieveState == RecieveState.Blocked; });

                return _saveAsFileCommand;
            }
        }
        private async void SaveAsFileCommand_Execute()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = GenerateFilterForDialog(traceProviders);

            IsEnabled = false;

            if (saveFileDialog.ShowDialog() == true)
            {
                
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
                            currentTraceProvider.SaveChanges();
                            await el.SaveAsAsync(saveFileDialog.FileName, currentTraceProvider.Traces, currentTraceProvider.CanHeaders);
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

        private RelayCommandAsync _recieveStartCommand;
        public RelayCommandAsync RecieveStartCommand
        {
            get
            {
                if (_recieveStartCommand == null)
                    _recieveStartCommand = new RelayCommandAsync(this.RecieveStartCommand_Execute, () => { return this.RecieveState == RecieveState.Blocked; });

                return _recieveStartCommand;
            }
        }
        private void RecieveStartCommand_Execute()
        {
            this.RecieveState = RecieveState.Recieving;
        }

        private RelayCommandAsync _pauseRecievingCommand;
        public RelayCommandAsync PauseRecievingCommand
        {
            get
            {
                if (_pauseRecievingCommand == null)
                    _pauseRecievingCommand = new RelayCommandAsync(this.PauseRecievingCommand_Execute, () => { return this.RecieveState == RecieveState.Recieving; });

                return _pauseRecievingCommand;
            }
        }
        private void PauseRecievingCommand_Execute()
        {
            this.RecieveState = RecieveState.Blocked;
        }

        private RelayCommand _clearTraceCommand;
        public RelayCommand ClearTraceCommand
        {
            get
            {
                if (_clearTraceCommand == null)
                    _clearTraceCommand = new RelayCommand(this.ClearTraceCommand_Execute, () => { return this.RecieveState == RecieveState.Blocked; });

                return _clearTraceCommand;
            }
        }
        private void ClearTraceCommand_Execute()
        {
            Filters.Clear();
            this.Items.Clear();
        }

        private string GenerateFilterForDialog(IEnumerable<ITraceDataTypeProvider> source)
        {
            string result = "";

            foreach (var el in traceProviders)
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


        public override void Dispose()
        {
            base.Dispose();

            TransmitToSelectedChannels = null;
            
            if(_items != null)
                _items.CollectionChanged -= Items_CollectionChanged;


            if (currentTraceProvider != null)
            {
                currentTraceProvider.CloseConnection();

                if (File.Exists(currentTraceProvider.TargetFile))
                    File.Delete(currentTraceProvider.TargetFile);
            }
        }

        public override bool Equals(object obj)
        {
            return obj is RecieveChannelPageVM vM &&
                   base.Equals(obj) &&
                   EqualityComparer<ObservableCollection<TransmitToViewData>>.Default.Equals(TransmitToItems, vM.TransmitToItems) &&
                   EqualityComparer<TransmitToDelegate>.Default.Equals(TransmitToSelectedChannels, vM.TransmitToSelectedChannels) &&
                   IsEnabled == vM.IsEnabled &&
                   RecieveState == vM.RecieveState &&
                   EqualityComparer<BindingList<ITraceFilter>>.Default.Equals(Filters, vM.Filters) &&
                   EqualityComparer<ObservableCollection<TraceModel>>.Default.Equals(ShowedItems, vM.ShowedItems) &&
                   EqualityComparer<ObservableCollection<TraceModel>>.Default.Equals(Items, vM.Items) &&
                   EqualityComparer<IChannel>.Default.Equals(Channel, vM.Channel) &&
                   EqualityComparer<RelayCommandAsync>.Default.Equals(SaveAsFileCommand, vM.SaveAsFileCommand) &&
                   EqualityComparer<RelayCommandAsync>.Default.Equals(RecieveStartCommand, vM.RecieveStartCommand) &&
                   EqualityComparer<RelayCommandAsync>.Default.Equals(PauseRecievingCommand, vM.PauseRecievingCommand) &&
                   EqualityComparer<RelayCommand>.Default.Equals(ClearTraceCommand, vM.ClearTraceCommand);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(TransmitToItems);
            hash.Add(TransmitToSelectedChannels);
            hash.Add(IsEnabled);
            hash.Add(RecieveState);
            hash.Add(Filters);
            hash.Add(ShowedItems);
            hash.Add(Items);
            hash.Add(Channel);
            hash.Add(SaveAsFileCommand);
            hash.Add(RecieveStartCommand);
            hash.Add(PauseRecievingCommand);
            hash.Add(ClearTraceCommand);
            return hash.ToHashCode();
        }

        ~RecieveChannelPageVM()
        {
            this.Dispose();
        }
    }
}
