﻿/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CANAnalyzerDataModels
{
    /// <summary>
    /// EF6 model CanHeaderss table entity
    /// </summary>
    [Table("CanHeaders")]
    public class CanHeaderModel : BaseModel
    {
        private int _id;
        private bool _isExtId;
        private int _canId;
        private int _dlc = 8;
        private string _comment = "";

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
        public int CanId
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
            if(obj is CanHeaderModel model)
            {
                return (model.IsExtId == IsExtId) && (model.CanId == CanId) && (model.DLC == DLC);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, IsExtId, CanId, DLC, Comment);
        }
    }
}