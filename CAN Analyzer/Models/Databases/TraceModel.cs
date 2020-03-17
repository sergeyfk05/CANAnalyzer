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
    /// <summary>
    /// EF6 model Traces table entity
    /// </summary>
    [Table("Traces")]
    public class TraceModel : BaseModel
    {

        private int _id;
        private double _time;
        private int _canHeaderId;
        private CanHeaderModel _canHeader;
        private byte[] _payload;


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
        /// Relative time at which the packet was sent.
        /// </summary>
        [Required]
        public double Time
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

        /// <summary>
        /// Foreign key to CanHeaders table
        /// </summary>
        [Required]
        public int CanHeaderId
        {
            get
            {
                return _canHeaderId;
            }
            set
            {
                if (value == _canHeaderId)
                    return;

                _canHeaderId = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// CanHeaderModel entity from CanHeaders table
        /// </summary>
        [ForeignKey("CanHeaderId")]
        public CanHeaderModel CanHeader
        {
            get
            {
                return _canHeader;
            }
            set
            {
                if (value == _canHeader)
                    return;

                _canHeader = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Packet payload. It's bytes converted to string
        /// </summary>
        [Required]
        public byte[] Payload
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
