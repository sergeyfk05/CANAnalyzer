using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CANAnalyzer.Models.Databases
{
    [Table("Traces")]
    public class TraceModel : BaseModel
    {

        private int _id;
        private int _time;
        private int _isExtId;
        private int _canId;
        private int _dlc = 8;
        private string _payload;


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

        [Required]
        public int Time
        {
            get
            {
                return _time;
            }
            set
            {
                if (value == _time)
                    return;

                _time = value;
                OnPropertyChanged();
            }
        }

        [Required]
        [DefaultValue(0)]
        public int IsExtId
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
        public string Payload
        {
            get
            {
                return _payload;
            }
            set
            {
                if (value == _payload)
                    return;

                _payload = value;
                OnPropertyChanged();
            }
        }
    }
}
