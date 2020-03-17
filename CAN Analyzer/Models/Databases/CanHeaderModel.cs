using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;

namespace CANAnalyzer.Models.Databases
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

        [Required]
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
    }
}