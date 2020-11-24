using InfluxDBChannelProxy.Extensions;
using System.Xml.Serialization;

namespace InfluxDBChannelProxy.XML.CanPackage
{
    public class CanHeader
    {
        public int Id { get; set; }
        public bool IsExtId { get; set; }
        public byte DLC { get; set; }

        public string Mask 
        {
            get { return _mask; }
            set
            {
                if (value == _mask)
                    return;

                _mask = value;
                MaskBytes = _mask.StringToByteArray();
            }
        }
        private string _mask;

        [XmlIgnore]
        public byte[] MaskBytes { get; private set; }

        public string Value
        {
            get { return _value; }
            set
            {
                if (value == _value)
                    return;

                _value = value;
                ValueBytes = _value.StringToByteArray();
            }
        }
        private string _value;

        [XmlIgnore]
        public byte[] ValueBytes { get; private set; }


    }
}
