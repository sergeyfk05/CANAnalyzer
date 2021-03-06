﻿/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CANAnalyzerDataModels
{
    public class TracePeriodicModel : BaseModel
    {

        public TracePeriodicModel()
        {
        }

        private bool _isExtId;
        private UInt64 _canId;
        private int _dlc = 8;
        private ObservableCollection<byte> _payload;
        private uint _period;
        private string _comment = "";
        private int _id;

        [Key]
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (value == _id)
                    return;

                _id = value;
                OnPropertyChanged();
            }
        }


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

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, IsExtId, CanId, DLC, Payload, Period, Comment);
        }
    }
}
