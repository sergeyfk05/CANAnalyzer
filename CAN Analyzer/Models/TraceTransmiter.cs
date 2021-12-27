/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzer.Models.Delegates;
using CANAnalyzerDataModels;
using CANAnalyzerDevices.Devices.DeviceChannels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CANAnalyzer.Models
{



    public class TraceTransmiter : IDisposable
    {
        public enum TraceTransmiterStatus
        {
            Undefined,
            Working,
            Paused,
            Reseted
        };
        public TraceTransmiter(int timerAccuracy = 1)
        {
            _timeAccuracy = timerAccuracy;
            Status = TraceTransmiterStatus.Reseted;

            _stopWatch = new Stopwatch();

            _timer = new System.Timers.Timer();
            _timer.Interval = _timeAccuracy;
            _timer.AutoReset = true;
            _timer.Stop();

            _timer.Elapsed += Timer_Elapsed;

        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Status == TraceTransmiterStatus.Working)
            {
                Task.Factory.StartNew(() => 
                {
                });
                bool isEnd = false;

                if (_enumerator.Current == null)
                {
                    isEnd = !_enumerator.MoveNext();
                }

                while (true)
                {
                    if (isEnd || _enumerator.Current == null)
                    {
                        Stop();
                        break;
                    }

                    if (_stopWatch.ElapsedMilliseconds + _stopWatchOffset >= (int)(_enumerator.Current.Time * 1000))
                    {
                        TransmitTo?.Invoke(new TransmitData()
                        {
                            CanId = _enumerator.Current.CanHeader.CanId,
                            IsExtId = _enumerator.Current.CanHeader.IsExtId,
                            DLC = _enumerator.Current.CanHeader.DLC,
                            Payload = _enumerator.Current.Payload
                        });
                        CurrentIndex++;
                        isEnd = !_enumerator.MoveNext();
                    }
                    else
                    {
                        break;
                    }
                }

            }
        }

        private int _stopWatchOffset;
        private Stopwatch _stopWatch;

        public TraceTransmiterStatus Status
        {
            get { return _status; }
            private set
            {
                if (_status == value)
                    return;

                _status = value;
                RaiseStatusChanged();
            }
        }
        private TraceTransmiterStatus _status;

        public void UpdateEnumerator()
        {
            if(Source != null)
            {
                _enumerator = Source.GetEnumerator();
                SetCurrentItemIndex(0);
            }
        }
        public IEnumerable<TraceModel> Source
        {
            get { return _source; }
            set
            {

                if (value == _source)
                    return;

                if (Status == TraceTransmiterStatus.Working || Status == TraceTransmiterStatus.Undefined)
                    throw new Exception("TraceTransmiter should be stoped before updating source first");



                _source = value;

                if (_source == null)
                    return;

                _enumerator = _source.GetEnumerator();
                SetCurrentItemIndex(0);
            }
        }
        private IEnumerable<TraceModel> _source;

        public int CurrentIndex
        {
            get { return _currentIndex; }
            private set
            {
                if (_currentIndex == value)
                    return;

                _currentIndex = value;
                RaiseCurrentIndexChanged();
            }
        }
        private int _currentIndex;
        FastSmartWeakEvent<EventHandler> _currentIndexChanged = new FastSmartWeakEvent<EventHandler>();
        public event EventHandler CurrentIndexChanged
        {
            add { _currentIndexChanged.Add(value); }
            remove { _currentIndexChanged.Remove(value); }
        }
        private void RaiseCurrentIndexChanged()
        {
            _currentIndexChanged.Raise(this, new EventArgs());
        }

        private IEnumerator<TraceModel> _enumerator;

        public TransmitToDelegate TransmitTo { get; set; }
        private int _timeAccuracy;


        public void SetCurrentItemIndex(int index)
        {


            if (Status == TraceTransmiterStatus.Working)
                throw new Exception("TraceTransmiter should be stoped before source changing");

            if (_enumerator == null)
                throw new ArgumentException("Need to set the Source propery first");

            _enumerator.Reset();
            for (int i = -1; i < index; i++)
            {
                if (!_enumerator.MoveNext())
                {
                    _enumerator.Reset();
                    CurrentIndex = 0;
                    return;
                }
            }
            if (_enumerator.Current == null)
            {
                CurrentIndex = 0;
                _stopWatchOffset = 0;
                return;
            }

            CurrentIndex = index;
            _stopWatchOffset = (int)(_enumerator.Current.Time * 1000);
        }

        public void Start()
        {
            if (_enumerator == null)
                throw new ArgumentException("Need to set the Source propery first");

            _timer.Start();
            _stopWatch.Start();
            Status = TraceTransmiterStatus.Working;
        }
        public void Stop()
        {
            _stopWatch.Stop();
            _stopWatch.Reset();
            _timer.Stop();
            Status = TraceTransmiterStatus.Reseted;
            try
            {
                SetCurrentItemIndex(0);
            }
            catch { }

        }
        public void Pause()
        {
            _stopWatch.Stop();
            _timer.Stop();
            Status = TraceTransmiterStatus.Paused;
        }



        private System.Timers.Timer _timer;

        FastSmartWeakEvent<EventHandler> _statusChanged = new FastSmartWeakEvent<EventHandler>();
        public event EventHandler StatusChanged
        {
            add { _statusChanged.Add(value); }
            remove { _statusChanged.Remove(value); }
        }
        private void RaiseStatusChanged()
        {
            _statusChanged.Raise(this, new EventArgs());
        }

        public void Dispose()
        {
            if(_timer != null)
            {
                _timer.Elapsed -= Timer_Elapsed;
                _timer.Dispose();
            }
        }

        public override bool Equals(object obj)
        {
            return obj is TraceTransmiter transmiter &&
                   Status == transmiter.Status &&
                   EqualityComparer<IEnumerable<TraceModel>>.Default.Equals(Source, transmiter.Source) &&
                   CurrentIndex == transmiter.CurrentIndex &&
                   EqualityComparer<TransmitToDelegate>.Default.Equals(TransmitTo, transmiter.TransmitTo);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Status, Source, CurrentIndex, TransmitTo);
        }

        ~TraceTransmiter()
        {
            this.Dispose();
        }
    }
}
