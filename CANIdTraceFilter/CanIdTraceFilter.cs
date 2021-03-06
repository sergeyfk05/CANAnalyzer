﻿/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzerDataModels;
using CANTraceFilterInterfaces;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CANIdTraceFilter
{
    public class CanIdTraceFilter : ITraceFilter
    {
        public CanIdTraceFilter(int canId, bool isExtId = false, bool isActive = false)
        {
            this.CanId = canId;
            this.IsExtId = isExtId;
            this.IsActive = isActive;
            this.DisplayName = ToString();

            var context = new ValidationContext(this);

            if (!Validator.TryValidateObject(this, context, null, true))
            {
                throw new ArgumentException();
            }

            PropertyChanged += OnCanIdTraceFilter_CanIdPropertyChanged;
        }

        private void OnCanIdTraceFilter_CanIdPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "CanId")
                return;

            if (sender is CanIdTraceFilter obj)
                obj.DisplayName = obj.ToString();
        }

        [Range(0, 0x1FFFFFFF)]
        public int CanId
        {
            get { return _canId; }
            private set
            {
                if (value == _canId)
                    return;

                _canId = value;
                RaisePropertyChanged();
            }
        }
        private int _canId;

        public bool IsExtId
        {
            get { return _isExtId; }
            private set
            {
                if (value == _isExtId)
                    return;

                _isExtId = value;
                RaisePropertyChanged();
            }
        }
        private bool _isExtId;

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (value == _isActive)
                    return;

                _isActive = value;
                RaisePropertyChanged();
            }
        }
        private bool _isActive;

        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                if (value == _displayName)
                    return;

                _displayName = value;
                RaisePropertyChanged();
            }
        }
        private string _displayName;


        public IQueryable<TraceModel> Filter(IQueryable<TraceModel> source)
        {
            if (!IsActive)
                return source;

            return source.Where(x => !(x.CanHeader.CanId == CanId && x.CanHeader.IsExtId == IsExtId));
        }

        public bool FilterOne(TraceModel source)
        {
            if (!IsActive)
                return false;

            return (source.CanHeader.CanId == CanId && source.CanHeader.IsExtId == IsExtId);
        }

        public override string ToString()
        {
            return IsExtId ? CanId.ToString("X8") : CanId.ToString("X3");
        }


        FastSmartWeakEvent<PropertyChangedEventHandler> _propertyChanged = new FastSmartWeakEvent<PropertyChangedEventHandler>();
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _propertyChanged.Add(value); }
            remove { _propertyChanged.Remove(value); }
        }

        protected void RaisePropertyChanged([CallerMemberName]string prop = "")
        {
            _propertyChanged.Raise(this, new PropertyChangedEventArgs(prop));
        }

        public override bool Equals(object obj)
        {
            return obj is CanIdTraceFilter filter &&
                   CanId == filter.CanId &&
                   IsExtId == filter.IsExtId &&
                   IsActive == filter.IsActive &&
                   DisplayName == filter.DisplayName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CanId, IsExtId, IsActive, DisplayName);
        }
    }
}
