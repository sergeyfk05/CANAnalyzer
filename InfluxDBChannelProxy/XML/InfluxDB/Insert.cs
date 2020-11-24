namespace InfluxDBChannelProxy.XML.InfluxDB
{
    public class Insert
    {
        public string Measurement;
        public InfluxDBTag Tag { get; set; }
        public InfluxDBField Field { get; set; }
    }
}
