/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CANAnalyzer.Models.Databases;
using CANAnalyzer.Models.Delegates;
using CANAnalyzerDevices.Devices.DeviceChannels;

namespace CANAnalyzer.Models
{
    public class TraceTransmiter : IDisposable
    {
        public TraceTransmiter(int timerAccuracy = 10)
        {
            _timeAccuracy = timerAccuracy;
            Status = TraceTransmiterStatus.Reseted;


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
                ElapsedMilliseconds += _timeAccuracy;

                bool isEnd = false;

                do
                {
                    if (_enumerator.Current == null)
                    {
                        isEnd =  !_enumerator.MoveNext();
                        continue;
                    }

                    if (ElapsedMilliseconds >= (int)(_enumerator.Current.Time * 1000))
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


                } while (!isEnd);

                if(isEnd)
                {
                    Stop();
                }

            }
        }


        public int ElapsedMilliseconds { get; private set; } = 0;

        public enum TraceTransmiterStatus
        {
            Undefined,
            Working,
            Paused,
            Reseted
        };
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
        public event EventHandler CurrentIndexChanged;
        private void RaiseCurrentIndexChanged()
        {
            CurrentIndexChanged?.Invoke(this, new EventArgs());
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

            CurrentIndex = index;
            ElapsedMilliseconds = (int)(_enumerator.Current.Time * 1000);
        }

        public void Start()
        {
            if (_enumerator == null)
                throw new ArgumentException("Need to set the Source propery first");

            _timer.Start();
            Status = TraceTransmiterStatus.Working;
        }
        public void Stop()
        {
            _timer.Stop();
            Status = TraceTransmiterStatus.Reseted;
            SetCurrentItemIndex(0);
        }
        public void Pause()
        {
            _timer.Stop();
            Status = TraceTransmiterStatus.Paused;
        }



        private System.Timers.Timer _timer;

        public event EventHandler StatusChanged;
        private void RaiseStatusChanged()
        {
            StatusChanged?.Invoke(this, new EventArgs());
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        ~TraceTransmiter()
        {
            this.Dispose();
        }
    }
}
