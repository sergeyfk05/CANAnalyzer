using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CANAnalyzer.Models.Databases
{
    public class TracePeriodicModel : BaseModel
    {
        private bool _isExtId;
        private UInt64 _canId;
        private int _dlc = 8;
        private ObservableCollection<byte> _payload;
        private uint _period;
        private string _comment = "";


        /// <summary>
        /// If IsExtId == true than package will be send as External Id.
        /// </summary>
        [Required]
        [DefaultValue(false)]
        [Range(0, 1)]
        public bool IsExtId
        {
            get
            {
                return _isExtId;
            }
            set
            {
                if (value == _isExtId)
                    return;

                _isExtId = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// CAN package's identificator.
        /// </summary>
        [Required]
        [Range(0, 0x1FFFFFFF)]
        public UInt64 CanId
        {
            get
            {
                return _canId;
            }
            set
            {
                if (value == _canId)
                    return;

                _canId = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// CAN Package's DLC. It's count of bytes in payload.
        /// </summary>
        [Required]
        [Range(0, 9)]
        [DefaultValue(8)]
        public int DLC
        {
            get
            {
                return _dlc;
            }
            set
            {
                if (value == _dlc)
                    return;

                _dlc = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Package payload. It's bytes converted to string
        /// </summary>
        [Required]
        public ObservableCollection<byte> Payload
        {
            get
            {
                return _payload;
            }
            set
            {
                if (value == _payload)
                    return;

                if (value.Count != DLC)
                    throw new ArgumentException("value.Length != DLC");

                _payload = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Period for transmiting package.
        /// </summary>
        [Required]
        [Range(1, uint.MaxValue)]
        public uint Period
        {
            get { return _period; }
            set
            {
                if (value == _period)
                    return;

                _period = value;
                OnPropertyChanged();
            }
        }


        [MaxLength(100)]
        [DefaultValue("")]
        public string Comment
        {
            get
            {
                return _comment;
            }
            set
            {
                if (value == _comment)
                    return;

                _comment = value;
                OnPropertyChanged();
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is TracePeriodicModel model)
            {
                return (model.IsExtId == IsExtId) && (model.CanId == CanId) && (model.DLC == DLC) && (model.Period == Period) && (model.Payload != null) && (Payload != null) && (model.Payload.Equals(Payload));
            }

            return false;
        }
    }
}
