using CANAnalyzer.Models.Databases;
using CANAnalyzer.Models.DataTypesProviders;
using CANAnalyzerDevices.Devices.DeviceChannels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CANAnalyzer.Models;
using System.Threading;
using System.ComponentModel;
using CANAnalyzer.Models.TraceFilters;
using System.Data.Entity;

namespace CANAnalyzer.VM
{
    public class RecieveChannelPageVM : BaseVM
    {
        public RecieveChannelPageVM(IChannel ch)
        {
            currentTraceProvider = new SQLiteTraceDataTypeProvider();

            string tempfile = "";
            while (true)
            {
                tempfile = new Uri(
                    new Uri(Path.GetTempPath(), UriKind.Absolute),
                    new Uri(Path.GetRandomFileName() + ".sqlite3", UriKind.Relative)
                    ).AbsolutePath;
                if (!File.Exists(tempfile))
                {
                    currentTraceProvider.GenerateFile(tempfile);
                    break;
                }

            }
            currentTraceProvider.TargetFile = tempfile;
            Status = FileState.Temporary;


            PropertyChanged += Channel_PropertyChanged;

            Channel = ch;
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
                    }
                }
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
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
            if (sender != Channel || currentTraceProvider == null)
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
            }

            //если появился новый CAN ID
            if(filtersCount == 0)
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
            lock (currentTraceProvider)
            {
                currentTraceProvider.SaveChanges();
            }

            _context.Post((s) =>
            {
                Items.Add(model);
            }, null);

        }

        private void FilterIsActive_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
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

        public FileState Status
        {
            get
            {
                return _status;
            }
            private set
            {
                if (_status == value)
                    return;

                _status = value;
                RaisePropertyChanged();
            }
        }
        private FileState _status = FileState.Closed;

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






        public void Dispose()
        {
            if (currentTraceProvider != null)
            {
                currentTraceProvider.CloseConnection();

                if (Status == FileState.Temporary && File.Exists(currentTraceProvider.TargetFile))
                    File.Delete(currentTraceProvider.TargetFile);
            }
        }
        ~RecieveChannelPageVM()
        {
            this.Dispose();
        }
    }
}
