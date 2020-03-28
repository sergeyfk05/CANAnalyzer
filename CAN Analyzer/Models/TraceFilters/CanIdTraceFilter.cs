using CANAnalyzer.Models.Databases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CANAnalyzer.Models.TraceFilters
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

            PropertyChanged += CanIdTraceFilter_CanIdPropertyChanged;
        }

        private void CanIdTraceFilter_CanIdPropertyChanged(object sender, PropertyChangedEventArgs e)
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

        public override string ToString()
        {
            return CanId.ToString("X3");
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
